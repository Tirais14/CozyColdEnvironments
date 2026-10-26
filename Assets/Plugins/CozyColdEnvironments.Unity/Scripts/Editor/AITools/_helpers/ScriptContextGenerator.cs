#if UNITY_EDITOR
using CCEnvs.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Editr.AITools
{
    public class ScriptContextGenerator
    {
        public static async Task<Scripts> GetScriptsAsync(
            ICollection<ScriptContextScriptInfo> scriptInfos,
            IEnumerable<string>? directories = null,
            int dependencyMaxDepth = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            CC.Guard.IsNotNull(scriptInfos, nameof(scriptInfos));

            var results = new Scripts();
            var scriptsDirs = new List<string>(1024);

            if (directories.IsNullOrEmpty() || directories.Count(dir => Directory.Exists(dir)) == 0)
                scriptsDirs.AddRange(Directory.GetDirectories(Application.dataPath, "Scripts", SearchOption.AllDirectories));
            else
                foreach (var dir in directories.Distinct())
                    scriptsDirs.AddRange(Directory.GetDirectories(Application.dataPath, dir, SearchOption.AllDirectories));

            var allFilePaths = scriptsDirs.SelectMany(dir => Directory.GetFiles(dir, "*.cs", new EnumerationOptions { RecurseSubdirectories = true })).ToList();
            int maxDegreeOfParallelism = Math.Max(4, Environment.ProcessorCount);

            // ШАГ 1: Построение карты "ИмяТипа -> СписокФайлов"
            var typeToFileMap = new ConcurrentDictionary<string, List<string>>();

            Parallel.ForEach(allFilePaths, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, filePath =>
            {
                try
                {
                    string content = File.ReadAllText(filePath);
                    var tree = CSharpSyntaxTree.ParseText(content, path: filePath);
                    var root = tree.GetRoot();

                    string namespaceName = root.DescendantNodes()
                        .OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString() ?? "";

                    var typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

                    foreach (var typeDecl in typeDeclarations)
                    {
                        string typeName = typeDecl.Identifier.Text;
                        string fullName = string.IsNullOrEmpty(namespaceName) ? typeName : $"{namespaceName}.{typeName}";

                        AddToTypeMap(typeToFileMap, fullName, filePath);
                        AddToTypeMap(typeToFileMap, typeName, filePath);
                    }
                }
                catch { }
            });

            // ШАГ 2: Обработка файлов, чьи типы совпадают с правилами
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
            var tasks = new List<Task>(allFilePaths.Count);

            foreach (var filePath in allFilePaths)
            {
                var task = ProcessTargetFileAsync(
                    filePath,
                    scriptInfos,
                    typeToFileMap,
                    results,
                    dependencyMaxDepth,
                    semaphore,
                    cancellationToken);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        private static async Task ProcessTargetFileAsync(
            string filePath,
            ICollection<ScriptContextScriptInfo> scriptInfos,
            ConcurrentDictionary<string, List<string>> typeToFileMap,
            Scripts results,
            int dependencyMaxDepth,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var content = await File.ReadAllTextAsync(filePath, cancellationToken);
                var tree = CSharpSyntaxTree.ParseText(content, path: filePath);
                var root = tree.GetRoot();

                string namespaceName = root.DescendantNodes()
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .FirstOrDefault()?.Name.ToString() ?? "";

                var declaredTypes = root.DescendantNodes().OfType<TypeDeclarationSyntax>()
                    .Select(t => new
                    {
                        ShortName = t.Identifier.Text,
                        FullName = string.IsNullOrEmpty(namespaceName) ? t.Identifier.Text : $"{namespaceName}.{t.Identifier.Text}"
                    })
                    .ToList();

                ScriptContextScriptInfo? matchedRule = null;
                bool isTargetMatch = false;

                foreach (var rule in scriptInfos)
                {
                    foreach (var type in declaredTypes)
                    {
                        if (type.FullName.Match(rule.Name, rule.MatchSettings) ||
                            type.ShortName.Match(rule.Name, rule.MatchSettings))
                        {
                            isTargetMatch = true;
                            matchedRule = rule;
                            break;
                        }
                    }
                    if (isTargetMatch) break;
                }

                if (isTargetMatch && matchedRule != null)
                {
                    // 1. Добавляем сам целевой файл
                    ProcessAndAddFile(filePath, namespaceName, content, results, isDependency: false, rule: matchedRule);

                    // 2. Если нужны зависимости и глубина > 0, запускаем рекурсивный обход графа
                    if (matchedRule.IncludeDependecies && dependencyMaxDepth > 0)
                    {
                        var dependencyFiles = ResolveDependenciesRecursive(
                            filePath,
                            typeToFileMap,
                            dependencyMaxDepth,
                            cancellationToken);

                        foreach (var depFilePath in dependencyFiles)
                        {
                            try
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var depContent = await File.ReadAllTextAsync(depFilePath, cancellationToken);

                                var depTree = CSharpSyntaxTree.ParseText(depContent, path: depFilePath);
                                var depNs = depTree.GetRoot().DescendantNodes()
                                    .OfType<BaseNamespaceDeclarationSyntax>()
                                    .FirstOrDefault()?.Name.ToString() ?? "";

                                ProcessAndAddFile(depFilePath, depNs, depContent, results, isDependency: true, rule: matchedRule);
                            }
                            catch (Exception ex)
                            {
                                typeof(ScriptContextGenerator).PrintException(ex);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                typeof(ScriptContextGenerator).PrintException(ex);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static HashSet<string> ResolveDependenciesRecursive(
            string startFilePath,
            ConcurrentDictionary<string, List<string>> typeToFileMap,
            int maxDepth,
            CancellationToken ct)
        {
            var discoveredDependencies = new HashSet<string>();
            var visited = new HashSet<string>();
            var queue = new Queue<(string filePath, int depth)>();

            queue.Enqueue((startFilePath, 0));
            visited.Add(startFilePath);

            while (queue.Count > 0)
            {
                var (currentFile, currentDepth) = queue.Dequeue();

                // Добавляем в результат только если это не стартовый файл
                if (currentFile != startFilePath)
                {
                    discoveredDependencies.Add(currentFile);
                }

                // Если достигли максимальной глубины, не идем дальше
                if (currentDepth >= maxDepth)
                {
                    continue;
                }

                ct.ThrowIfCancellationRequested();

                try
                {
                    var content = File.ReadAllText(currentFile);
                    var tree = CSharpSyntaxTree.ParseText(content, path: currentFile);
                    var root = tree.GetRoot();

                    // А. Находим все типы, на которые есть ссылки в этом файле
                    var referencedTypes = root.DescendantNodes()
                        .OfType<TypeSyntax>()
                        .Select(GetCleanTypeName)
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Distinct();

                    foreach (var refType in referencedTypes)
                    {
                        if (typeToFileMap.TryGetValue(refType, out var depFiles))
                        {
                            foreach (var depFile in depFiles)
                            {
                                if (visited.Add(depFile))
                                {
                                    queue.Enqueue((depFile, currentDepth + 1));
                                }
                            }
                        }
                    }

                    // Б. Находим базовые классы и интерфейсы типов, ОБЪЯВЛЕННЫХ в этом файле
                    var hierarchyTypes = root.DescendantNodes()
                        .OfType<TypeDeclarationSyntax>()
                        .Where(t => t.BaseList != null)
                        .SelectMany(t => t.BaseList.Types)
                        .Select(bt => GetCleanTypeName(bt.Type))
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Distinct();

                    foreach (var hierType in hierarchyTypes)
                    {
                        if (typeToFileMap.TryGetValue(hierType, out var baseFiles))
                        {
                            foreach (var baseFile in baseFiles)
                            {
                                if (visited.Add(baseFile))
                                {
                                    queue.Enqueue((baseFile, currentDepth + 1));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Игнорируем ошибки парсинга зависимостей
                }
            }

            return discoveredDependencies;
        }

        private static void AddToTypeMap(ConcurrentDictionary<string, List<string>> map, string typeName, string filePath)
        {
            map.AddOrUpdate(typeName, _ => new List<string> { filePath }, (_, list) =>
            {
                if (!list.Contains(filePath)) list.Add(filePath);
                return list;
            });
        }

        private static string GetCleanTypeName(TypeSyntax typeSyntax)
        {
            if (typeSyntax is GenericNameSyntax generic) return generic.Identifier.Text;
            if (typeSyntax is IdentifierNameSyntax id) return id.Identifier.Text;
            if (typeSyntax is QualifiedNameSyntax qualified) return qualified.ToString();
            if (typeSyntax is AliasQualifiedNameSyntax alias) return alias.ToString();
            if (typeSyntax is PredefinedTypeSyntax predefined) return predefined.Keyword.Text;

            return typeSyntax.ToString();
        }

        private static void ProcessAndAddFile(string filePath, string ns, string content, Scripts results, bool isDependency, ScriptContextScriptInfo rule)
        {
            bool onlyApi = isDependency ? rule.DependeciesOnlyAPI : rule.OnlyAPI;
            bool removeEmpty = isDependency ? rule.DependeciesRemoveEmptyLines : rule.RemoveEmptyLines;
            string finalContent = content;

            if (onlyApi) finalContent = ExtractAPI(finalContent);
            if (removeEmpty) finalContent = RemoveEmptyLines(finalContent);

            results.Add(filePath, ns, finalContent);
        }

        private static string RemoveEmptyLines(string content)
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrWhiteSpace(l)));
        }

        public static async Task<ScriptContext> GenerateAsync(
            ICollection<ScriptContextScriptInfo> scriptInfos,
            IEnumerable<string>? directories = null,
            int dependencyMaxDepth = int.MaxValue,
            CancellationToken cancellationToken = default)
        {
            var context = new ScriptContext();
            var scripts = await GetScriptsAsync(scriptInfos, directories, dependencyMaxDepth, cancellationToken);

            foreach (var kvp in scripts.GetAll())
            {
                context.AddScriptContent(kvp.Key, kvp.Value);
            }

            return context;
        }

        public static string ExtractAPI(string script)
        {
            var tree = CSharpSyntaxTree.ParseText(script);
            var root = tree.GetRoot();
            var rewriter = new CSharpAPIExtractorRewriter();
            var newRoot = rewriter.Visit(root);
            return newRoot.ToFullString();
        }
    }
}
#endif