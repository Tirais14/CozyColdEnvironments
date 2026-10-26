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
            ICollection<ScriptContextNamespace> namespaces,
            IEnumerable<string>? directories = null,
            CancellationToken cancellationToken = default)
        {
            CC.Guard.IsNotNull(namespaces, nameof(namespaces));

            var results = new Scripts();
            var scriptsDirs = new List<string>(1024);

            if (directories.IsNullOrEmpty() || directories.Count(dir => Directory.Exists(dir)) == 0)
                scriptsDirs.AddRange(Directory.GetDirectories(Application.dataPath, "Scripts", SearchOption.AllDirectories));
            else
                foreach (var dir in directories.Distinct())
                    scriptsDirs.AddRange(Directory.GetDirectories(Application.dataPath, dir, SearchOption.AllDirectories));

            bool includeFilesWithoutNamespace = namespaces.Any(ns => string.IsNullOrEmpty(ns.Name));
            var allFilePaths = scriptsDirs.SelectMany(dir => Directory.GetFiles(dir, "*.cs", new EnumerationOptions { RecurseSubdirectories = true })).ToList();

            int maxDegreeOfParallelism = Math.Max(4, Environment.ProcessorCount);

            // ШАГ 1: Построение точной карты "ИмяТипа -> СписокФайлов" с помощью Roslyn Syntax Tree
            var typeToFileMap = new ConcurrentDictionary<string, List<string>>();

            Parallel.ForEach(allFilePaths, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, filePath =>
            {
                try
                {
                    string content = File.ReadAllText(filePath);
                    var tree = CSharpSyntaxTree.ParseText(content, path: filePath);
                    var root = tree.GetRoot();

                    // Определяем текущий неймспейс файла
                    string namespaceName = root.DescendantNodes()
                        .OfType<BaseNamespaceDeclarationSyntax>()
                        .FirstOrDefault()?.Name.ToString() ?? "";

                    // Находим все объявления типов (class, struct, interface, enum, record)
                    var typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();

                    foreach (var typeDecl in typeDeclarations)
                    {
                        string typeName = typeDecl.Identifier.Text;
                        string fullName = string.IsNullOrEmpty(namespaceName) ? typeName : $"{namespaceName}.{typeName}";

                        // Добавляем в карту и полное имя, и короткое (для удобства поиска)
                        AddToTypeMap(typeToFileMap, fullName, filePath);
                        AddToTypeMap(typeToFileMap, typeName, filePath);
                    }
                }
                catch
                {
                    // Игнорируем файлы с синтаксическими ошибками или недоступные
                }
            });

            // ШАГ 2: Обработка целевых файлов и рекурсивное разрешение зависимостей
            var globallyProcessedFiles = new ConcurrentDictionary<string, byte>(); // Защита от дубликатов и циклов
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
            var tasks = new List<Task>(allFilePaths.Count);

            foreach (var filePath in allFilePaths)
            {
                var task = ProcessTargetFileAsync(
                    filePath,
                    namespaces,
                    includeFilesWithoutNamespace,
                    typeToFileMap,
                    results,
                    globallyProcessedFiles,
                    semaphore,
                    cancellationToken);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            return results;
        }

        private static async Task ProcessTargetFileAsync(
            string filePath,
            ICollection<ScriptContextNamespace> namespaces,
            bool includeFilesWithoutNamespace,
            ConcurrentDictionary<string, List<string>> typeToFileMap,
            Scripts results,
            ConcurrentDictionary<string, byte> globallyProcessedFiles,
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

                bool isTargetMatch = false;
                ScriptContextNamespace? targetRule = null;

                if (string.IsNullOrEmpty(namespaceName))
                {
                    isTargetMatch = includeFilesWithoutNamespace;
                    if (isTargetMatch) targetRule = namespaces.FirstOrDefault(ns => string.IsNullOrEmpty(ns.Name));
                }
                else
                {
                    targetRule = namespaces.FirstOrDefault(nsRule => nsRule.Name.Match(namespaceName, nsRule.MatchSettings));
                    if (targetRule != null)
                        isTargetMatch = true;
                }

                if (isTargetMatch && targetRule != null)
                {
                    // 1. Обрабатываем сам целевой файл
                    ProcessAndAddFile(filePath, namespaceName, content, results, globallyProcessedFiles, isDependency: false, rule: targetRule);

                    // 2. Если нужны зависимости, запускаем рекурсивный обход графа
                    if (targetRule.IncludeDependecies)
                    {
                        var dependencyFiles = ResolveDependenciesRecursive(
                            filePath,
                            typeToFileMap,
                            globallyProcessedFiles,
                            cancellationToken);

                        foreach (var depFilePath in dependencyFiles)
                        {
                            try
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var depContent = await File.ReadAllTextAsync(depFilePath, cancellationToken);

                                // Определяем неймспейс зависимости для корректной обработки флагов
                                var depTree = CSharpSyntaxTree.ParseText(depContent, path: depFilePath);
                                var depNs = depTree.GetRoot().DescendantNodes()
                                    .OfType<BaseNamespaceDeclarationSyntax>()
                                    .FirstOrDefault()?.Name.ToString() ?? "";

                                ProcessAndAddFile(depFilePath, depNs, depContent, results, globallyProcessedFiles, isDependency: true, rule: targetRule);
                            }
                            catch (Exception ex)
                            {
                                typeof(ScriptContextGenerator).PrintException(ex);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальное поведение при отмене
            }
            catch (Exception ex)
            {
                typeof(ScriptContextGenerator).PrintException(ex);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Рекурсивно находит все файлы, содержащие типы, на которые есть ссылки, 
        /// а также их базовые классы и интерфейсы.
        /// </summary>
        private static HashSet<string> ResolveDependenciesRecursive(
            string startFilePath,
            ConcurrentDictionary<string, List<string>> typeToFileMap,
            ConcurrentDictionary<string, byte> globallyProcessedFiles,
            CancellationToken ct)
        {
            var discoveredDependencies = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(startFilePath);

            while (queue.Count > 0)
            {
                string currentFile = queue.Dequeue();

                // TryAdd возвращает false, если файл уже был обработан (защита от циклических зависимостей)
                if (!globallyProcessedFiles.TryAdd(currentFile, 0))
                    continue;

                if (currentFile != startFilePath)
                    discoveredDependencies.Add(currentFile);

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
                                queue.Enqueue(depFile);
                        }
                    }

                    // Б. Находим базовые классы и интерфейсы типов, ОБЪЯВЛЕННЫХ в этом файле
                    // Это гарантирует, что если мы тянем MyClass, мы также вытянем его IMyInterface и MyBaseClass
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
                                queue.Enqueue(baseFile);
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
            map.AddOrUpdate(
                typeName,
                _ => new List<string> { filePath },
                (_, list) =>
                {
                    if (!list.Contains(filePath))
                        list.Add(filePath);
                    return list;
                });
        }

        private static string GetCleanTypeName(TypeSyntax typeSyntax)
        {
            if (typeSyntax is GenericNameSyntax generic)
                return generic.Identifier.Text; // "List" из "List<T>"
            if (typeSyntax is IdentifierNameSyntax id)
                return id.Identifier.Text; // "IVendor"
            if (typeSyntax is QualifiedNameSyntax qualified)
                return qualified.ToString(); // "System.Collections.Generic.List"
            if (typeSyntax is AliasQualifiedNameSyntax alias)
                return alias.ToString();

            return typeSyntax.ToString();
        }

        private static void ProcessAndAddFile(
            string filePath,
            string ns,
            string content,
            Scripts results,
            ConcurrentDictionary<string, byte> processedFiles,
            bool isDependency,
            ScriptContextNamespace rule)
        {
            processedFiles.TryAdd(filePath, 0);

            bool onlyApi = isDependency ? rule.DependeciesOnlyAPI : rule.OnlyAPI;
            bool removeEmpty = isDependency ? rule.DependeciesRemoveEmptyLines : rule.RemoveEmptyLines;

            string finalContent = content;

            if (onlyApi)
            {
                finalContent = ExtractAPI(finalContent);
            }

            if (removeEmpty)
            {
                finalContent = RemoveEmptyLines(finalContent);
            }

            results.Add(filePath, ns, finalContent);
        }

        private static string RemoveEmptyLines(string content)
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrWhiteSpace(l)));
        }

        public static async Task<ScriptContext> GenerateAsync(
            ICollection<ScriptContextNamespace> namespaces,
            IEnumerable<string>? directories = null,
            CancellationToken cancellationToken = default)
        {
            var context = new ScriptContext();
            var scripts = await GetScriptsAsync(namespaces, directories, cancellationToken);

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