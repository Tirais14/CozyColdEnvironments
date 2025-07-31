#nullable enable
using System;

namespace UTIRLib
{
    public static class ArrayHelper
    {
        /// <exception cref="ArgumentException"></exception>
        public static Array CastToElementType(Array array)
        {
            if (array.Length == 0)
                throw new ArgumentException("Array cannot be empty.");

            foreach (var item in array)
            {
                if (item is not null)
                {
                    Type elementType = item.GetType();
                    Type newArrayType = elementType.MakeArrayType();

                    if (array.GetType() == newArrayType)
                        return array;

                    Array result = Array.CreateInstance(elementType,
                                                        array.Length);

                    object valueToCast;
                    object castedValue;
                    for (int i = 0; i < array.Length; i++)
                    {
                        valueToCast = array.GetValue(i);

                        if (valueToCast is null)
                        {
                            result.SetValue(valueToCast, i);
                            continue;
                        }

                        castedValue = Convert.ChangeType(valueToCast, elementType);

                        result.SetValue(castedValue, i);
                    }

                    return result;
                }
            }

            throw new ArgumentException("Array must be contain any not null element.");
        }
    }
}
