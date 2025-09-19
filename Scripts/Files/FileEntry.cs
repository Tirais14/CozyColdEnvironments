#nullable enable
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Extensions;
using CCEnvs.Reflection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Files
{
    public class FileEntry : FileSystemEntry
    {
        protected object? customContent;

        public virtual string Extension {
            get => Path.Extension;
            set => SetPath(path.WithExtension(value));
        }

        public FileEntry(params string[] pathParts) : base(pathParts)
        {
        }

        public FileEntry(Path path) : base(path)
        {
        }

        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static FileEntry CreateEntry(bool overwrite, params string[] pathParts)
        {
            var entry = new FileEntry(pathParts);
            entry.Create(overwrite);

            return entry;
        }
        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static FileEntry CreateEntry(params string[] pathParts)
        {
            return CreateEntry(overwrite: false, pathParts);
        }
        /// <param name="overwrite">if false - throws exception if file exists</param>
        public static FileEntry CreateEntry(Path path, bool overwrite = false)
        {
            return CreateEntry(overwrite, path.value);
        }

        public override void Save(bool overwrite = false)
        {
            if (Exists && !overwrite)
                throw new FileOverwriteNotAllowedException(path);
            if (customContent is null)
            {
                CCDebug.PrintWarning($"{nameof(FileEntry)}: Nothing to save.");
                return;
            }
            CC.Validate.StringArgument(Name, nameof(Name));

            using FileStream fileStream = OpenOrCreate();

            Write(fileStream);

            ApplyChanges();
        }

        /// <exception cref="FileOverwriteNotAllowedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> TrySaveAsync(bool overwrite = false)
        {
            try
            {
                await SaveAsync(overwrite);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task SaveAsync(bool overwrite = false)
        {
            if (Exists && !overwrite)
                throw new FileOverwriteNotAllowedException(path);
            if (customContent is null)
            {
                CCDebug.PrintWarning($"{nameof(FileEntry)}: Nothing to save.");
                return;
            }
            CC.Validate.StringArgument(Name, nameof(Name));

            try
            {
                using FileStream fileStream = OpenOrCreate();

                await WriteAsync(fileStream);

                ApplyChanges();

                await fileStream.DisposeAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <exception cref="ArgumentNullException"></exception>
        public virtual void SetContent(byte[] bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            customContent = bytes;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void SetContent(params string[] lines)
        {
            if (lines is null)
                throw new ArgumentNullException(nameof(lines));

            customContent = lines;
        }

        public byte[] GetBytesContent()
        {
            if (customContent is byte[] bytes)
                return bytes;

            return ReadBytes();
        }

        public string GetTextContent()
        {
            if (customContent is string text)
                return text;

            return ReadText();
        }

        public string[] GetLinesContent()
        {
            if (customContent is string[] lines)
                return lines;

            return ReadLines();
        }

        #region Default
        public byte[] ReadBytes() => File.ReadAllBytes(path);

        public string ReadText() => File.ReadAllText(path);

        public string[] ReadLines() => File.ReadAllLines(path);

        public void WriteBytes(byte[] bytes) => File.WriteAllBytes(Path, bytes);

        public async Task WriteBytesAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            await File.WriteAllBytesAsync(Path, bytes, cancellationToken);
        }

        public void WriteText(string text) => File.WriteAllText(Path, text);

        public async Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
        {
            await File.WriteAllTextAsync(Path, text, cancellationToken);
        }

        public void WriteLines(string[] textLines) => File.WriteAllLines(Path, textLines);

        public async Task WriteTextLinesAsync(string[] textLines)
        {
            await File.WriteAllLinesAsync(Path, textLines);
        }

        public void Move(string destFileName) => File.Move(Path, destFileName);

        public FileStream Open(FileMode fileMode) => File.Open(Path, fileMode);
        public FileStream Open(FileMode fileMode, FileAccess fileAccess)
        {
            return File.Open(Path, fileMode, fileAccess);
        }
        public FileStream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return File.Open(Path, fileMode, fileAccess, fileShare);
        }

        public FileStream OpenOrCreate() => Open(FileMode.OpenOrCreate);
        public FileStream OpenOrCreate(FileAccess fileAccess)
        {
            return Open(FileMode.OpenOrCreate, fileAccess);
        }
        public FileStream OpenOrCreate(FileAccess fileAccess, FileShare fileShare)
        {
            return Open(FileMode.OpenOrCreate, fileAccess, fileShare);
        }

        public FileStream OpendWrite() => File.OpenWrite(Path);

        public FileStream OpenRead() => File.OpenRead(Path);

        public StreamWriter OpenWriteText(FileMode fileMode)
        {
            using FileStream fileStream = Open(fileMode);

            return new StreamWriter(fileStream);
        }
        public StreamWriter OpenWriteText(FileMode fileMode, FileAccess fileAccess)
        {
            using FileStream fileStream = Open(fileMode, fileAccess);

            return new StreamWriter(fileStream);
        }
        public StreamWriter OpenWriteText(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            using FileStream fileStream = Open(fileMode, fileAccess, fileShare);

            return new StreamWriter(fileStream);
        }

        public StreamReader OpenReadText(FileMode fileMode)
        {
            using FileStream fileStream = Open(fileMode);

            return new StreamReader(fileStream);
        }
        public StreamReader OpenReadText(FileMode fileMode, FileAccess fileAccess)
        {
            using FileStream fileStream = Open(fileMode, fileAccess);

            return new StreamReader(fileStream);
        }
        public StreamReader OpenReadText(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            using FileStream fileStream = Open(fileMode, fileAccess, fileShare);

            return new StreamReader(fileStream);
        }

        public void Replace(string destFileName, string backupFileName)
        {
            File.Replace(Path, destFileName, backupFileName);
        }
        public void Replace(string destFileName, string backupFileName, bool ignoreMetadataErrors)
        {
            File.Replace(Path, destFileName, backupFileName, ignoreMetadataErrors);
        }

        public override void Create(bool overwrite = false)
        {
            if (Exists && !overwrite)
                throw new FileNotFoundException(path);
            CC.Validate.StringArgument(Name, nameof(Name));

            File.Create(Path);
            CCDebug.PrintLog($"File created: \"{Path}\"", this);
        }
        #endregion Default

        /// <exception cref="InvalidOperationException"></exception>
        protected void Write(FileStream fileStream)
        {
            using StreamWriter writer = new(fileStream);

            switch (customContent)
            {
                case byte[] bytes:
                    fileStream.Write(bytes, 0, bytes.Length);
                    break;
                case string str:
                    writer.Write(str);
                    break;
                case string[] lines:
                    writer.Write(lines.JoinStrings(Environment.NewLine));
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected content type: {customContent?.GetType()}.");
            }
        }

        protected async Task WriteAsync(FileStream fileStream,
                                        CancellationToken cancellationToken = default)
        {
            using StreamWriter writer = new(fileStream);

            switch (customContent)
            {
                case byte[] bytes:
                    await fileStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                    break;
                case string str:
                    await writer.WriteAsync(str);
                    break;
                case string[] lines:
                    await writer.WriteAsync(lines.JoinStrings(Environment.NewLine));
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected content type: {customContent?.GetType()}.");
            }
        }

        protected string[] GetContentAsLines()
        {
            switch (customContent)
            {
                case string text:
                    return text.SplitByLines();
                case string[] lines:
                    return lines;
                default:
                    CCDebug.PrintError($"{customContent?.GetTypeName()} type doesn't supported.");
                    return Array.Empty<string>();
            }
        }

        public static implicit operator FileInfo(FileEntry fileEntry)
        {
            return new FileInfo(fileEntry.Path);
        }
    }
}
