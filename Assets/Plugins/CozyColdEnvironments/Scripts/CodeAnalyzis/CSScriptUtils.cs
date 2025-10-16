#nullable enable
using CCEnvs.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCEnvs.CodeAnalyzis
{
    public static class CSScriptUtils
    {
        //public static async ValueTask<Type[]> GetTypes(string filePath)
        //{
        //    CC.Validate.ArgumentNull(filePath, nameof(filePath));
        //    if (filePath.IsEmpty())
        //        return Type.EmptyTypes;
        //}

        public static async ValueTask AddMissingUsingsAsync(string filePath, params Type[] forTypes)
        {
            CC.Guard.StringArgument(filePath, nameof(filePath));
            CC.Guard.NullArgument(forTypes, nameof(forTypes));
            if (forTypes.IsEmpty())
                return;

            IEnumerable<string> toAddUsings = forTypes.Select(type => $"using {type.Namespace};");

            IEnumerable<string> lines = await ReadFileAsync(filePath);

            IEnumerable<string> containedUsings = lines.Where(IsUsingDefine);

            IEnumerable<string> resultUsings = containedUsings.Intersect(toAddUsings);

            lines = lines.Intersect(containedUsings);
            string content = resultUsings.Concat(lines).JoinStringsByLine();

            await File.WriteAllTextAsync(filePath, content);
        }


        public static async ValueTask<string[]> GetUsingsFromFileAsync(string filePath)
        {
            CC.Guard.StringArgument(filePath, nameof(filePath));

            using var reader = new StreamReader(filePath);

            string[] lines = await ReadFileAsync(filePath);

            return lines.Where(IsUsingDefine).ToArray();
        }

        public static async ValueTask<string[]> ReadFileAsync(string filePath)
        {
            CC.Guard.StringArgument(filePath, nameof(filePath));

            using var reader = new StreamReader(filePath);
            var lines = new List<string>();
            string? line;
            var loopPredicate = new LoopFuse(() => !reader.EndOfStream);
            while (loopPredicate)
            {
                line = await reader.ReadLineAsync();

                if (line is null)
                    continue;

                lines.Add(line);
            }

            return lines.ToArray();
        }

        public static bool IsUsingDefine(string line)
        {
            CC.Guard.NullArgument(line, nameof(line));

            return Regex.IsMatch(line, @"(^s*)(\w*)(s*$)");
        }
    }
}
