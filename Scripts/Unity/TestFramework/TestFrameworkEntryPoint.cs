using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.Tests
{
    [DefaultExecutionOrder(-2)]
    public class TestFrameworkEntryPoint : MonoCC
    {
        private Assembly testsAssembly = null!;
        private MonoCCTest[] monoTests = null!;

        [SerializeField]
        private string testsAssemblyName;

        [field: SerializeField]
        public bool IsEnabled { get; set; } = true;

        protected override void OnAwake()
        {
            if (!IsEnabled)
                return;

            base.OnAwake();

            testsAssembly = AssemblyHelper.GetAssembly(testsAssemblyName, throwIfNotFound: true);

            CreateTestClasses();

            monoTests = GetComponents<MonoCCTest>().Where(x => x.IsEnabled).ToArray();
        }

        protected override void OnStart()
        {
            if (!IsEnabled)
                return;

            base.OnStart();

            StartCoroutine(RunTest(RunMonoTests));
        }

        private static IEnumerator RunTest(Action action)
        {
            yield return new WaitForEndOfFrame();

            action();

            yield return null;
        }

        private void CreateTestClasses()
        {
            Type[] testsTypes = testsAssembly.GetTypes()
                                 .Where(x => x.IsType<MonoCCTest>()
                                        &&
                                        !x.IsAbstract)
                                 .ToArray();

            int count = testsTypes.Length;
            for (int i = 0; i < count; i++)
                gameObject.AddComponent(testsTypes[i]);
        }

        private void RunMonoTests()
        {
            Dictionary<MonoCCTest, MethodInfo[]> testMethods = monoTests.Aggregate(new Dictionary<MonoCCTest, MethodInfo[]>(), (collection, x) =>
            {
                MethodInfo[] methods = x.GetType()
                                        .GetMethods(BindingFlagsDefault.InstanceAll)
                                        .Aggregate(new List<MethodInfo>(), (list, y) =>
                                        {
                                            if (y.IsDefined<CCMonoTestAttribute>()
                                                &&
                                                y.ReturnType.IsAnyType(typeof(void), typeof(IEnumerator))
                                                )
                                                list.Add(y);

                                            return list;
                                        }).ToArray();

                collection.Add(x, methods);

                return collection;
            });

            for (int i = 0; i < monoTests.Length; i++)
            {
                MethodInfo[] methods = testMethods[monoTests[i]];
                object? routine;
                foreach (var method in methods)
                {
                    routine = method.Invoke(null, CC.C.Array(monoTests[i]));

                    if (routine.IsNotNull())
                        StartCoroutine((IEnumerator)routine);

                    Debug.Log($"{monoTests[i].GetType().GetName()}.{method.Name}({method.GetParameters().Select(x => x.ParameterType.GetName()).JoinStrings(", ")}) started.");
                }
            }
        }
    }
}
