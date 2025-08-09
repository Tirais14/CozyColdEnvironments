#nullable enable
using System;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class EnumFieldFactory
    {
        public static EnumFieldEntry CreateNone(AttributeEntry[]? attributes = null,
                                                int tabulationsCount = 2)
        {
            return new EnumFieldEntry() {
                FieldName = "None",
                FieldValue = 0,
                Attributes = attributes ?? Array.Empty<AttributeEntry>(),
                TabulationsCount = tabulationsCount
            };
        }

        public static EnumFieldEntry[] CreateRange(int startValue,
                                                   int tabulationsCount,
                                                   AttributeEntry[] attributes,
                                                   params string[] names)
        {
            var fields = new EnumFieldEntry[names.Length];


            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new EnumFieldEntry() {
                    TabulationsCount = tabulationsCount,
                    FieldName = names[i],
                    FieldValue = startValue++,
                    Attributes = attributes,
                };
            }

            return fields;
        }
        public static EnumFieldEntry[] CreateRange(int startValue,
                                                   int tabulationsCount,
                                                   params string[] names)
        {
            return CreateRange(startValue,
                               tabulationsCount,
                               attributes: Array.Empty<AttributeEntry>(),
                               names);
        }
        public static EnumFieldEntry[] CreateRange(int tabulationsCount,
                                                   AttributeEntry[] attributes,
                                                   params string[] names)
        {
            return CreateRange(startValue: 0, tabulationsCount, attributes, names);
        }
        public static EnumFieldEntry[] CreateRange(int tabulationsCount, params string[] names)
        {
            return CreateRange(startValue: 0,
                               tabulationsCount,
                               attributes: Array.Empty<AttributeEntry>(),
                               names);
        }
        public static EnumFieldEntry[] CreateRange(AttributeEntry[] attributes, params string[] names)
        {
            return CreateRange(startValue: 0, tabulationsCount: 2, attributes, names);
        }
        public static EnumFieldEntry[] CreateRange(params string[] names)
        {
            return CreateRange(startValue: 0,
                               tabulationsCount: 2,
                               attributes: Array.Empty<AttributeEntry>(),
                               names);
        }

        public static EnumFieldEntry[] CreateFlags(int startValue,
                                                   int tabulationsCount,
                                                   AttributeEntry[] attributes,
                                                   EnumType inheritType,
                                                   params string[] names)
        {
            EnumFieldEntry[] fields = CreateRange(startValue, tabulationsCount, attributes, names);

            EnumFieldHelper.SetFlagsValues(fields, inheritType);

            return fields;
        }
        public static EnumFieldEntry[] CreateFlags(int startValue,
                                                   int tabulationsCount,
                                                   EnumType inheritType,
                                                   params string[] names)
        {
            return CreateFlags(startValue,
                               tabulationsCount,
                               attributes: Array.Empty<AttributeEntry>(),
                               inheritType,
                               names);
        }
        public static EnumFieldEntry[] CreateFlags(int tabulationsCount,
                                                   EnumType inheritType,
                                                   params string[] names)
        {
            return CreateFlags(startValue: 0,
                               tabulationsCount,
                               attributes: Array.Empty<AttributeEntry>(),
                               inheritType,
                               names);
        }
        public static EnumFieldEntry[] CreateFlags(int tabulationsCount,
                                                   AttributeEntry[] attributes,
                                                   EnumType inheritType,
                                                   params string[] names)
        {
            return CreateFlags(startValue: 0, tabulationsCount, attributes, inheritType, names);
        }
        public static EnumFieldEntry[] CreateFlags(int tabulationsCount,
                                                   AttributeEntry[] attributes,
                                                   params string[] names)
        {
            return CreateFlags(startValue: 0,
                               tabulationsCount,
                               attributes,
                               inheritType: EnumType.Int,
                               names);
        }
        public static EnumFieldEntry[] CreateFlags(int tabulationsCount, params string[] names)
        {
            return CreateFlags(startValue: 0,
                               tabulationsCount,
                               attributes: Array.Empty<AttributeEntry>(),
                               inheritType: EnumType.Int,
                               names);
        }
        public static EnumFieldEntry[] CreateFlags(EnumType inheritType, params string[] names)
        {
            return CreateFlags(startValue: 0,
                               tabulationsCount: 0,
                               attributes: Array.Empty<AttributeEntry>(),
                               inheritType,
                               names);
        }
        public static EnumFieldEntry[] CreateFlags(params string[] names)
        {
            return CreateFlags(startValue: 0,
                               tabulationsCount: 0,
                               attributes: Array.Empty<AttributeEntry>(),
                               inheritType: EnumType.Int,
                               names);
        }
    }
}
