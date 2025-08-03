using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.TestFramework
{
    [DefaultExecutionOrder(-2)]
    public class TestFrameworkEntryPoint : MonoX
    {
        private Assembly testsAssembly = null!;
        private MonoTest[] monoTests = null!;

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

            monoTests = GetComponents<MonoTest>().Where(x => x.IsEnabled).ToArray();
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
                                 .Where(x => x.IsType<MonoTest>()
                                        &&
                                        !x.IsAbstract)
                                 .ToArray();

            int count = testsTypes.Length;
            for (int i = 0; i < count; i++)
                gameObject.AddComponent(testsTypes[i]);
        }

        private void RunMonoTests()
        {
            Dictionary<MonoTest, MethodInfo[]> testMethods = monoTests.Aggregate(new Dictionary<MonoTest, MethodInfo[]>(), (collection, x) =>
            {
                MethodInfo[] methods = x.GetType()
                                        .GetMethods(BindingFlagsDefault.InstanceAll)
                                        .Aggregate(new List<MethodInfo>(), (list, y) =>
                                        {
                                            if (y.IsDefined<MonoTestAttribute>()
                                                &&
                                                y.ReturnType == typeof(IEnumerator)
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
                foreach (var method in methods)
                {
                    StartCoroutine((IEnumerator)method.Invoke(monoTests[i]));
                    Debug.Log($"{monoTests[i].GetType().GetName()}.{method.Name}({method.GetParameters().Select(x => x.ParameterType.GetName()).JoinStrings(", ")}) started.");
                }
            }
        }
    }
}
