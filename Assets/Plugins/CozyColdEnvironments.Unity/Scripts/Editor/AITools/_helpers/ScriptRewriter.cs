using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.UnityX.Editr.AITools
{
    public static class ScriptRewriter
    {
        public static async Task ApplyAsync(string aiOutput, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(aiOutput, nameof(aiOutput));

            // ✅ ИСПРАВЛЕНИЕ: Возвращаем управление, если строка пустая, а не наоборот
            if (aiOutput.IsNullOrWhiteSpace())
                return;

            var filesToProcess = ExtractFiles(aiOutput);
            using var semaphore = new SemaphoreSlim(Math.Max(4, Environment.ProcessorCount));
            var tasks = new List<Task>(256);

            foreach (var fileInfo in filesToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await semaphore.WaitAsync(cancellationToken);

                if (!File.Exists(fileInfo.FilePath))
                {
                    typeof(ScriptRewriter).PrintLog(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Invalid file path")
                        .AddProperty("FilePath", fileInfo.FilePath)
                        .ToStringAndDispose()
                        );
                    semaphore.Release();
                    continue;
                }

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await File.ReadAllTextAsync(fileInfo.FilePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        typeof(ScriptRewriter).PrintException(ex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            typeof(ScriptRewriter).PrintLog(filesToProcess.Select(
                x => DebugMessageBuilder.CreatePooled()
                    .AddMessage("File changed")
                    .AddProperty("File", x.FilePath)
                    .AddProperty("IsDeleted", x.ToDelete)
                    .ToStringAndDispose()
                ).SequenceToString());
        }

        private static ScriptRewriteInfo[] ExtractFiles(string aiOutput)
        {
            Guard.IsNotNull(aiOutput, nameof(aiOutput));

            var results = new List<ScriptRewriteInfo>();

            // Регулярное выражение для парсинга блока:
            // 1. //----File Start----
            // 2. //----FilePath: "путь"---- (кавычки опциональны)
            // 3. //----Deleted---- (опционально)
            // 4. Содержимое файла (любые символы, включая переносы строк)
            // 5. //----File End----
            const string pattern = @"//----File Start----\s*//----FilePath:\s*""?(?<path>[^""\r\n]+)""?----\s*(?<deleted>//----Deleted----)?\s*(?<content>[\s\S]*?)//----File End----";

            var matches = Regex.Matches(aiOutput, pattern);

            foreach (Match match in matches)
            {
                // Извлекаем и очищаем путь от возможных кавычек и пробелов
                string path = match.Groups["path"].Value.Trim('"', ' ');

                // Флаг удаления присутствует, если группа совпала
                bool toDelete = match.Groups["deleted"].Success;

                // Очищаем содержимое от лишних переносов строк в начале и конце блока
                string content = match.Groups["content"].Value.Trim();

                results.Add(new ScriptRewriteInfo(content, path)
                {
                    ToDelete = toDelete
                });
            }

            return results.ToArray();
        }
    }
}