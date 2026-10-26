#if UNITY_EDITOR
using CCEnvs.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Editr.AITools
{
    public class ScriptContextGenerator
    {
        // Быстрый регекс для поиска namespace (для построения карты)
        private static readonly Regex NamespaceRegex = new Regex(@"^\s*namespace\s+([\w.]+)", RegexOptions.Multiline | RegexOptions.Compiled);

        // Быстрый регекс для поиска using (для зависимостей)
        private static readonly Regex UsingRegex = new Regex(@"^\s*using\s+(?:static\s+)?([\w.]+)(?:\s*=\s*[\w.]+)?\s*;", RegexOptions.Multiline | RegexOptions.Compiled);

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
            var allFilePaths = scriptsDirs.SelectMany(dir => Directory.GetFiles(dir, "*.cs", new EnumerationOptions { RecurseSubdirectories = true})).ToList();

            int maxDegreeOfParallelism = Math.Max(4, Environment.ProcessorCount);

            // ШАГ 1: Быстрое построение карты Namespace -> List<FilePath> (синхронно или параллельно, это быстро)
            var namespaceToFileMap = new ConcurrentDictionary<string, List<string>>();

            Parallel.ForEach(allFilePaths, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, filePath =>
            {
                try
                {
                    // ✅ ИСПРАВЛЕНИЕ: Используем простой и гарантированно работающий во всех версиях Unity метод.
                    // Синхронное чтение здесь предпочтительнее, так как мы уже внутри Parallel.ForEach
                    string content = File.ReadAllText(filePath);

                    var matches = NamespaceRegex.Matches(content);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            string ns = match.Groups[1].Value;
                            namespaceToFileMap.AddOrUpdate(ns, _ => new List<string> { filePath }, (_, list) => { list.Add(filePath); return list; });
                        }
                    }
                    else if (includeFilesWithoutNamespace)
                    {
                        namespaceToFileMap.AddOrUpdate(string.Empty, _ => new List<string> { filePath }, (_, list) => { list.Add(filePath); return list; });
                    }
                }
                catch
                {
                    // Игнорируем ошибки чтения (например, если файл заблокирован или удален)
                }
            });

            // ШАГ 2: Асинхронная обработка с ограничением параллелизма через SemaphoreSlim
            var processedFiles = new ConcurrentDictionary<string, byte>(); // Работает как HashSet для отслеживания добавленных файлов
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
            var tasks = new List<Task>(allFilePaths.Count);

            foreach (var filePath in allFilePaths)
            {
                var task = ProcessFileWithDependenciesAsync(filePath, namespaces, namespaceToFileMap, results, processedFiles, semaphore, cancellationToken);
                tasks.Add(task);
            }

            // Ждем завершения всех задач
            await Task.WhenAll(tasks);

            return results;
        }

        private static async Task ProcessFileWithDependenciesAsync(
            string filePath,
            ICollection<ScriptContextNamespace> namespaces,
            ConcurrentDictionary<string, List<string>> namespaceToFileMap,
            Scripts results,
            ConcurrentDictionary<string, byte> processedFiles,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var content = await File.ReadAllTextAsync(filePath, cancellationToken);
                var namespaceMatches = NamespaceRegex.Matches(content);

                string matchedNs = string.Empty;
                bool isTargetMatch = false;
                ScriptContextNamespace? targetRule = null;

                if (namespaceMatches.Count == 0)
                {
                    isTargetMatch = namespaces.Any(ns => string.IsNullOrEmpty(ns.Name));
                    if (isTargetMatch) targetRule = namespaces.FirstOrDefault(ns => string.IsNullOrEmpty(ns.Name));
                }
                else
                {
                    foreach (Match match in namespaceMatches)
                    {
                        string ns = match.Groups[1].Value;
                        targetRule = namespaces.FirstOrDefault(nsRule => nsRule.Name.Match(ns, nsRule.MatchSettings));
                        if (targetRule != null)
                        {
                            isTargetMatch = true;
                            matchedNs = ns;
                            break;
                        }
                    }
                }

                if (isTargetMatch && targetRule != null)
                {
                    // 1. Обрабатываем целевой файл
                    ProcessAndAddFile(filePath, matchedNs, content, results, processedFiles, isDependency: false, rule: targetRule);

                    // 2. Обрабатываем зависимости, если флаг включен
                    if (targetRule.IncludeDependecies)
                    {
                        var usings = ExtractUsings(content);
                        foreach (var usingNs in usings)
                        {
                            if (namespaceToFileMap.TryGetValue(usingNs, out var depFiles))
                            {
                                foreach (var depFilePath in depFiles)
                                {
                                    // TryAdd гарантирует, что файл будет обработан только один раз
                                    if (processedFiles.TryAdd(depFilePath, 0))
                                    {
                                        try
                                        {
                                            var depContent = await File.ReadAllTextAsync(depFilePath, cancellationToken);
                                            ProcessAndAddFile(depFilePath, usingNs, depContent, results, processedFiles, isDependency: true, rule: targetRule);
                                        }
                                        catch (Exception ex)
                                        {
                                            typeof(ScriptContextGenerator).PrintException(ex);
                                        }
                                    }
                                }
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

        private static void ProcessAndAddFile(
            string filePath,
            string ns,
            string content,
            Scripts results,
            ConcurrentDictionary<string, byte> processedFiles,
            bool isDependency,
            ScriptContextNamespace rule)
        {
            // Гарантируем, что файл помечен как обработанный (на случай, если он пришел как зависимость)
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

        private static IEnumerable<string> ExtractUsings(string content)
        {
            // Используем Regex для скорости. Он игнорирует закомментированные using и корректно извлекает пространство имен.
            var matches = UsingRegex.Matches(content);
            var usings = new HashSet<string>();

            foreach (Match match in matches)
            {
                usings.Add(match.Groups[1].Value);
            }

            return usings;
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
            var rewriter = new CSharpAPIExtractorRewriter(); // Ваш класс из предыдущего ответа
            var newRoot = rewriter.Visit(root);
            return newRoot.ToFullString();
        }

        //public static string ExtractAPI(string script)
        //{
        //    // 1. Удаляем тела методов, оставляя сигнатуру до первой '{'
        //    // Ищет: модификаторы + возвращаемый тип + имя + параметры + пробелы + { ... }
        //    // Заменяет на: всё до '{' + ";"
        //    string noMethodBodies = Regex.Replace(script,
        //        @"(\b(?:public|private|protected|internal|virtual|override|abstract|sealed|static|async|new|extern)\s+)+[\w\s\<\>\,\.\[\]\?\:]+\s+\w+\s*\([^)]*\)\s*(?:where\s+[\w\s\,\:]+)?\s*\{[^}]*\}",
        //        match =>
        //        {
        //            string signature = match.Value.Substring(0, match.Value.IndexOf('{')).Trim();
        //            return signature + ";";
        //        },
        //        RegexOptions.Multiline | RegexOptions.Singleline);

        //    // 2. Удаляем тела свойств с get/set (превращаем { get { ... } set { ... } } в { get; set; })
        //    string noPropertyBodies = Regex.Replace(noMethodBodies,
        //        @"\{\s*get\s*\{[^}]*\}\s*(?:set\s*\{[^}]*\})?\s*\}",
        //        "{ get; set; }",
        //        RegexOptions.Multiline);

        //    noPropertyBodies = Regex.Replace(noPropertyBodies,
        //        @"\{\s*set\s*\{[^}]*\}\s*(?:get\s*\{[^}]*\})?\s*\}",
        //        "{ get; set; }",
        //        RegexOptions.Multiline);

        //    // 3. Удаляем инициализаторы полей (public int a = 5; -> public int a;)
        //    string noInitializers = Regex.Replace(noPropertyBodies,
        //        @"(\w+\s+\w+)\s*=\s*[^;]+;",
        //        "$1;",
        //        RegexOptions.Multiline);

        //    // 4. Удаляем пустые строки для экономии токенов LLM
        //    string[] lines = noInitializers.Split('\n');
        //    string result = string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l)));

        //    return result;
        //}
    }
}
#endif