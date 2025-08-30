#nullable enable
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CCEnvs.Reflection;

namespace CCEnvs.Unity.Tests
{
    public abstract class MonoCCTest : MonoCC
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
            return thisType.GetMethods(BindingFlagsDefault.InstanceAll).Where(x => x.IsDefined<CCMonoTestAttribute>()).ToArray();
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
