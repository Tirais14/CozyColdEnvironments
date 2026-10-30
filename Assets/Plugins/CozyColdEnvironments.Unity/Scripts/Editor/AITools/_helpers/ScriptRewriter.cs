#if UNITY_EDITOR
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

            if (aiOutput.IsNullOrWhiteSpace())
                return;

            var filesToProcess = ExtractFiles(aiOutput);
            using var semaphore = new SemaphoreSlim(Math.Max(4, Environment.ProcessorCount));
            var tasks = new List<Task>(filesToProcess.Length);

            foreach (var fileInfo in filesToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await semaphore.WaitAsync(cancellationToken);

                var task = Task.Run(async () =>
                {
                    try
                    {
                        if (fileInfo.ToDelete)
                        {
                            // Если файл помечен на удаление, удаляем его (если он существует)
                            if (File.Exists(fileInfo.FilePath))
                            {
                                File.Delete(fileInfo.FilePath);
                            }
                        }
                        else
                        {
                            // Если файл нужно создать или обновить, гарантируем существование директории
                            var directory = Path.GetDirectoryName(fileInfo.FilePath);
                            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }

                            await File.WriteAllTextAsync(fileInfo.FilePath, fileInfo.Content, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        typeof(ScriptRewriter).PrintException(ex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            // Логирование результатов
            var logMessages = filesToProcess.Select(x => DebugMessageBuilder.CreatePooled()
                .AddMessage(x.ToDelete ? "File deleted" : "File created/updated")
                .AddProperty("FilePath", x.FilePath)
                .ToStringAndDispose()).ToArray();

            if (logMessages.Length > 0)
            {
                typeof(ScriptRewriter).PrintLog(string.Join("\n", logMessages));
            }
        }

        private static ScriptRewriteInfo[] ExtractFiles(string aiOutput)
        {
            Guard.IsNotNull(aiOutput, nameof(aiOutput));

            var results = new List<ScriptRewriteInfo>();

            // ✅ ИСПРАВЛЕННОЕ РЕГУЛЯРНОЕ ВЫРАЖЕНИЕ:
            // 1. (?:----)? делает завершающие тире опциональными, что соответствует реальному выводу ИИ.
            // 2. [^\r\n""]+ надежно захватывает путь до конца строки или кавычки.
            const string pattern = @"//----File Start----\s*//----FilePath:\s*""?(?<path>[^\r\n""]+)""?(?:----)?\s*(?<deleted>//----Deleted----)?\s*(?<content>[\s\S]*?)//----File End----";

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
#endif