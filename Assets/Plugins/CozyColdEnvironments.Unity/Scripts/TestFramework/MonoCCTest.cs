#nullable enable
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCEnvs.Unity.Tests
{
    public abstract class MonoCCTest : CCBehaviour
    {
        protected Type thisType;

        public abstract bool IsEnabled { get; }

        protected override void Awake()
        {
            base.Awake();

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
