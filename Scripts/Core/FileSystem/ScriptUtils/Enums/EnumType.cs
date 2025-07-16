using UTIRLib.Attributes.Metadata;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public enum EnumType
    {
        [MetaType(typeof(int))]
        [MetaString("")]
        Int,

        [MetaType(typeof(byte))]
        [MetaString("byte")]
        Byte,

        [MetaType(typeof(sbyte))]
        [MetaString("sbyte")]
        Sbyte,

        [MetaType(typeof(short))]
        [MetaString("short")]
        Short,

        [MetaType(typeof(ushort))]
        [MetaString("ushort")]
        Ushort,

        [MetaType(typeof(uint))]
        [MetaString("uint")]
        Uint,

        [MetaType(typeof(long))]
        [MetaString("long")]
        Long,

        [MetaType(typeof(ulong))]
        [MetaString("ulong")]
        Ulong
    }
}
