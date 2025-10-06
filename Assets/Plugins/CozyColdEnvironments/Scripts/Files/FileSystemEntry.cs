using System;
using System.IO;
using System.Threading;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Files
{
    public abstract class FileSystemEntry
    {
        private FileAttributes? attributes;
        private string? customName;
        protected Path path;

        public Path Path {
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

        protected FileSystemEntry(Path path, string? name = null)
        {
            SetPath(path);
            customName = name;
        }

        public bool TrySave(bool overwrite = false)
        {
            try
            {
                Save(overwrite);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public abstract void Save(bool overwrite = false);

        public virtual void SetPath(Path path) => this.path = path;
        public void SetPath(string path)
        {
            SetPath(new Path(path));
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

            SetPath(new Path(pathParts));
        }

        /// <param name="name"></param>
        public void SetName(string name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new EmptyStringArgumentException(nameof(name), name);
            }

            customName = name;
        }

        public string GetDefaultName() => path.FileName;

        public void RestoreDefaultName() => customName = null;

        public virtual bool Delete()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);

                return true;
            }

            return false;
        }

        public bool TryCreate(bool overwrite = false)
        {
            try
            {
                Create(overwrite);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public abstract void Create(bool overwrite = false);

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
