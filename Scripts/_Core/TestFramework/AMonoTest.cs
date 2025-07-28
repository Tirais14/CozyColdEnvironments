#nullable enable
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Reflection;

namespace UTIRLib.TestFramework
{
    public abstract class AMonoTest : MonoX
    {
        protected Type thisType;

        public abstract bool IsEnabled { get; }

        protected override void OnAwake()
        {
            base.OnAwake();

            thisType = GetType();
        }

        public MethodInfo[] GetTestMethods()
        {
            return thisType.GetMethods(BindingFlagsDefault.InstanceAll).Where(x => x.IsDefined<MonoTestAttribute>()).ToArray();
        }

        protected void Complete(string? testName = null)
        {
            Debug.Log($"{thisType.GetName()}.{testName ?? "Test"} => completed.");
        }

        protected void Fail(string? testName = null)
        {
            Debug.LogError($"{thisType.GetName()}.{testName ?? "Test"} => failed.");
        }
    }
}
