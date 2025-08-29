using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace UTIRLib.Unity
{
    public static class RequireComponentExtensions
    {
        public static IEnumerable<Type> AsEnumerable(this RequireComponent value)
        {
            yield return value.m_Type0;
            yield return value.m_Type1;
            yield return value.m_Type2;
        }
    }
}
