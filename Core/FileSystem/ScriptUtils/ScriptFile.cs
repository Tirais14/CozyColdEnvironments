using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed class ScriptFile : FileEntry
    {
        [Flags]
        public enum SaveOptions
        {
            None,
            /// <summary>
            /// Saves existing file usings and adds to current content
            /// </summary>
            AddUsings,
        }

        /// <summary>
        /// Add usings to file, or empty if already setted by <see cref="ScriptFileBuilder"/> or manually
        /// </summary>
        public UsingEntry[] ToAddUsings { get; set; } = Array.Empty<UsingEntry>();
        public SaveOptions SaveOptionsValue { get; set; }
        public override string Extension {
            get => ".cs";
            set => _ = value;
        }

        public ScriptFile(params string[] pathParts) : base(pathParts)
        {
        }

        public override bool TrySave(bool overwrite = false)
        {
            if (Exists && !overwrite)
                return false;
            if (customContent is null)
            {
                TirLibDebug.Error($"{nameof(FileEntry)}: content is null.");
                return false;
            }
            if (Name.IsNullOrEmpty())
            {
                TirLibDebug.Error(new FileNameException(Name), this);
                return false;
            }

            ProccessUsings();

            using FileStream fileStream = OpenOrCreate();

            Write(fileStream);

            return true;
        }

        public override async Task<bool> TrySaveAsync(bool overwrite = false)
        {
            if (Exists && !overwrite)
                return false;
            if (customContent is null)
            {
                TirLibDebug.Error($"{nameof(FileEntry)}: content is null.");
                return false;
            }
            if (Name.IsNullOrEmpty())
            {
                TirLibDebug.Error(new FileNameException(Name), this);
                return false;
            }

            ProccessUsings();

            using FileStream fileStream = OpenOrCreate();

            await WriteAsync(fileStream);

            return true;
        }

        public string[] ReadUsingLines()
        {
            string[] lines = ReadLines();

            return UsingHelper.GetUsingLines(lines);
        }

        public UsingEntry[] ReadUsings()
        {
            string[] lines = ReadLines();

            return UsingHelper.GetUsings(lines);
        }

        public override void SetPath(FSPath path)
        {
            if (path.Extension != Extension)
            {
                this.path = path.WithExtension(Extension);
                return;
            }

            this.path = path;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void SetContent(ScriptContentList contentLines)
        {
            if (contentLines is null)
                throw new ArgumentNullException(nameof(contentLines));

            SetContent(contentLines.ToStringArray().JoinStringsByLine());
        }
        public void SetContent(params IScriptContent[] content)
        {
            SetContent(content.ToStringArray().JoinStringsByLine());
        }

        private void ProccessUsings()
        {
            if (ToAddUsings.IsNullOrEmpty()
                &&
                !SaveOptionsValue.HasFlag(SaveOptions.AddUsings)
                )
                return;

            string[] contentLines = GetContentAsLines();

            if (SaveOptionsValue.HasFlag(SaveOptions.AddUsings))
            {
                UsingEntry[] fileUsings = ReadUsings();

                if (fileUsings.IsNotEmpty())
                {
                    if (ToAddUsings.IsNullOrEmpty())
                    {
                        SetContent(fileUsings.ToStringArray().Concat(contentLines).ToArray());

                        return;
                    }

                    UsingEntry[] usings = ToAddUsings.Concat(fileUsings).ToArray();
                    usings = UsingHelper.Distinct(usings);

                    SetContent(usings.ToStringArray().Concat(contentLines).ToArray());
                    return;
                }
            }

            SetContent(ToAddUsings.ToStringArray().Concat(contentLines).ToArray());
        }
    }
}