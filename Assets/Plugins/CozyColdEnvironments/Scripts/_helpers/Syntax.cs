#nullable enable
using CCEnvs.Attributes.Metadata;
using CCEnvs.Extensions;
using Cysharp.Text;
using System;

namespace CCEnvs
{
    public static class Syntax
    {
        [Flags]
        public enum OtherModifiers
        {
            [MetaString("")]
            None,

            [MetaString("static")]
            Static,

            [MetaString("readonly")]
            Readonly = 2,

            [MetaString("const")]
            Const = 4
        }

        public enum AccessModifier
        {
            [MetaString("")]
            None,

            [MetaString("public")]
            Public,

            [MetaString("protected")]
            Protected,

            [MetaString("private")]
            Private,

            [MetaString("internal")]
            Internal,
        }

        public enum DataType
        {
            [MetaString("")]
            None,

            [MetaString("class")]
            Class,

            [MetaString("interfaces")]
            Interface,

            [MetaString("struct")]
            Struct,

            [MetaString("record")]
            Record,

            [MetaString("enum")]
            Enum
        }

        public static string ConvertToConstFieldName(string str)
        {
            return str.InsertWhitespacesByCase().Replace(' ', '_').ToUpper();
        }

        public static string Chain(params string[] values)
        {
            var sb = ZString.CreateStringBuilder();

            sb.AppendJoin('.', values);

            return sb.ToString();
        }
    }
}
