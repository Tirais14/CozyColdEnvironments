using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class RequireComponentAttributeHelper
    {
        public static Type[] TypesToArray(this RequireComponent source)
        {
            CC.Guard.IsNotNullSource(source);

            int typeCount = 0;

            if (source.m_Type0 is not null)
                typeCount++;

            if (source.m_Type1 is not null)
                typeCount++;

            if (source.m_Type2 is not null)
                typeCount++;

            if (typeCount == 1)
                return Range.From(source.m_Type0!);

            if (typeCount == 2)
                return Range.From(source.m_Type0!, source.m_Type1!);

            if (typeCount == 3)
                return Range.From(source.m_Type0!, source.m_Type1!, source.m_Type2!);

            return Type.EmptyTypes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerator<Type> GetEnumerator(this RequireComponent source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.TypesToArray().GetEnumeratorT();
        }
    }
}
