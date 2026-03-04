using System;
using System.IO;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Saves
{
    public struct WriteSaveDataToFileParameters : IEquatable<WriteSaveDataToFileParameters>
    {
        private string? fileContent;

        private bool? compressed;

        private bool? backuped;

        public string FilePath { get; }

        public string FileContent {
            get
            {
                fileContent ??= string.Empty;

                return fileContent;
            }
            set => fileContent = value;
        }

        public string FileExtension { get; }

        public bool Compressed {
            get
            {
                compressed ??= true;

                return compressed.Value;
            }
            set => compressed = value;
        }

        public bool Backuped {
            get
            {
                backuped ??= true;

                return backuped.Value;
            }
            set => backuped = value;
        }

        public WriteSaveDataToFileParameters(
            string filePath,
            string? fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION
            )
            :
            this()
        {
            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));

            if (fileExtension.IsNullOrWhiteSpace())
                fileExtension = SaveWrite.DEFAULT_SAVE_EXTENSION;

            FilePath = Path.ChangeExtension(filePath, fileExtension);
            FileExtension = fileExtension;
        }

        public static bool operator ==(WriteSaveDataToFileParameters left, WriteSaveDataToFileParameters right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WriteSaveDataToFileParameters left, WriteSaveDataToFileParameters right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is WriteSaveDataToFileParameters parameters && Equals(parameters);
        }

        public readonly bool Equals(WriteSaveDataToFileParameters other)
        {
            return fileContent == other.fileContent
                   &&
                   FileExtension == other.FileExtension
                   &&
                   compressed == other.compressed
                   &&
                   backuped == other.backuped
                   &&
                   FilePath == other.FilePath;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                fileContent,
                FileExtension,
                compressed,
                backuped,
                FilePath
                );
        }

        public readonly override string ToString()
        {
            if (this == default)
                return GetType().Name;

            return $"({nameof(FilePath)}: {FilePath})";
        }
    }
}
