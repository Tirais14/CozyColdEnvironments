using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public static class RequireComponentExtensions
    {
        public static IEnumerable<Type> AsEnumerable(this RequireComponent value)
        {
            if (value.m_Type0 is null)
                yield break;

            yield return value.m_Type0;

            if (value.m_Type1 is null)
                yield break;

            yield return value.m_Type1;

            if (value.m_Type2 is null)
                yield break;

            yield return value.m_Type2;
        }
    }
}
