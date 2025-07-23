using System;
using System.Reflection;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class EqualityHelper
    {

        public static bool EqualsByFields(object a, object b)
        {
            if (Equals(a, b))
                return true;

            Type aType = a.GetType();
            Type bType = b.GetType();

            if (aType.IsNotType(bType))
                return false;

            FieldInfo[] aFields = aType.GetFields();
            FieldInfo[] bFields = bType.GetFields();

            if (aFields.Length != bFields.Length)
                return false;

            foreach (var aField in aFields)
            {
                foreach (var bField in bFields)
                {
                    if (aField.FieldType == bField.FieldType)
                    {
                        object aFieldValue = aField.GetValue(a);
                        object bFieldValue = bField.GetValue(b);

                        if (!Equals(aFieldValue, bFieldValue))
                            return false;
                    }
                }
            }

            return false;
        }
    }
}
