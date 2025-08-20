using System;
using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib
{
    public class InvokableNotFoundException : TirLibException
    {
        public InvokableNotFoundException()
        {
        }

        public InvokableNotFoundException(Type type, MemberType memberType = default, InvokableSignature signature = default)
            :
            base(signature.ToString())
        {
        }
    }
}
