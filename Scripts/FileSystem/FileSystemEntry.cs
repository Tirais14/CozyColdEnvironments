using System;
using System.IO;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.FileSystem
{
    public abstract class FileSystemEntry
    {
        private FileAttributes? attributes;
        private string? customName;
        protected FSPath path;

        public FSPath Path {
            get => path;
            set => SetPath(value);
        }

        public string Name {
            get => customName ?? GetDefaultName();
            set => SetName(value);
        }
        public virtual bool Exists => File.Exists(Path);
        public FileAttributes? Attributes {
            get {
                if (attributes.HasValue)
                    return attributes.Value;

                if (!Exists)
                    return null;

                return File.GetAttributes(Path);
            }
            set => attributes = value;
        }
        public DateTime LastWriteTime => File.GetLastWriteTime(Path);
        public DateTime LastWriteTimeUtc => File.GetLastWriteTimeUtc(Path);
        public DateTime CreationTime => File.GetCreationTime(Path);
        public DateTime CreationTimeUtc => File.GetCreationTimeUtc(Path);
        public DateTime LastAccessTime => File.GetLastAccessTime(Path);
        public DateTime LastAccessTimeUtc => File.GetLastAccessTimeUtc(Path);

        protected FileSystemEntry(params string[] pathParts)
        {
            SetPath(pathParts);
        }

        protected FileSystemEntry(FSPath path, string? name = null)
        {
            SetPath(path);
            customName = name;
        }

        public abstract bool TrySave(bool overwrite = false);

        /// <exception cref="FileOverwriteNotAllowedException"></exception>
        public virtual void Save(bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new PathNotValidException(path);

            bool saveResult = TrySave(overwrite);

            if (!saveResult && Exists && !overwrite)
                throw new FileOverwriteNotAllowedException(path);
        }

        public virtual void SetPath(FSPath path) => this.path = path;
        public void SetPath(string path)
        {
            SetPath(new FSPath(path));
        }
        public void SetPath(params string[] pathParts)
        {
            if (pathParts is null)
            {
                throw new ArgumentNullException(nameof(pathParts));
            }
            if (pathParts.IsEmpty())
            {
                SetPath(string.Empty);
                return;
            }

            SetPath(new FSPath(pathParts));
        }

        /// <param name="name"></param>
        public void SetName(string name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(name), name);
            }

            customName = name;
        }

        public string GetDefaultName() => path.FileName;

        public void RestoreDefaultName() => customName = null;

        public bool Delete()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);

                return true;
            }

            return false;
        }

        public abstract bool TryCreate(bool overwrite = false);

        public virtual void Create(bool overwrite = false)
        {
            if (!TryCreate())
            {
                throw new FileOverwriteNotAllowedException(path);
            }
        }

        public override string ToString() => Path.value;

        protected void ApplyChanges()
        {
            ApplyName();
            ApplyAttributes();
        }

        private void ApplyName()
        {
            if (customName.IsNullOrEmpty())
            {
                return;
            }

            path = path.WithFileName(customName);
        }

        private void ApplyAttributes()
        {
            if (!attributes.HasValue)
            {
                return;
            }

            File.SetAttributes(path, attributes.Value);
        }
    }
}
