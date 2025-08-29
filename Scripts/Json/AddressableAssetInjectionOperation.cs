using Cysharp.Threading.Tasks;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments.Json
{
    public class AddressableAssetInjectionOperation
    {
        private readonly Func<object> getTarget;
        private readonly UnityEngine.Object injection;
        private readonly Type injectionMemberValueType;
        private readonly string injectionMemberName;
        private readonly MemberType injectionMemberType;

        public UniTask Task { get; }

        /// <summary>
        /// Starts async task
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TypeCastException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public AddressableAssetInjectionOperation(Func<object> getTarget,
                                                  UnityEngine.Object injection,
                                                  Type injectionMemberValueType,
                                                  string injectionMemberName,
                                                  MemberType injectionMemberType)
        {
            if (!Application.isPlaying)
                throw new NotSupportedException("Cannot deserialize addressable without DTO class in editor for now.");

            this.getTarget = getTarget;
            this.injection = injection;
            this.injectionMemberValueType = injectionMemberValueType;
            this.injectionMemberName = injectionMemberName;
            this.injectionMemberType = injectionMemberType;

            if (getTarget is null)
                throw new ArgumentNullException(nameof(getTarget));
            if (injection == null)
                throw new ArgumentNullException(nameof(injection));
            if (injectionMemberValueType is null)
                throw new ArgumentNullException(nameof(injectionMemberValueType));
            if (injection.GetType().IsNotType(injectionMemberValueType))
                throw new TypeCastException(injection.GetType(), injectionMemberValueType);
            if (injectionMemberName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(injectionMemberName), injectionMemberName);
            if (injectionMemberType != MemberType.Field
                ||
                injectionMemberType != MemberType.Property
                )
                throw new InvalidOperationException("Allowed only fields or properties.");

            Task = AwaitInjection();
            Task.Forget(ex => TirLibDebug.PrintException(ex, this));
        }

        private async UniTask AwaitInjection()
        {


            using var safetyDelayer = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            await UniTask.WaitUntil(() => getTarget().IsNull(),
                                    cancellationToken: safetyDelayer.Token);

            Inject();
        }

        private void Inject()
        {
            object target = getTarget();

            switch (injectionMemberType)
            {
                case MemberType.Field:
                    FieldInfo injectionField = target.GetType()
                        .GetField(injectionMemberName,
                                  BindingFlagsDefault.InstanceAll)
                                  ?? 
                        throw new MemberNotFoundException(target.GetType(),
                                                          MemberType.Field);

                    if (injectionField.FieldType.IsNotType(injectionMemberValueType))
                        throw new InvalidOperationException($"Invalid type = {injectionMemberValueType.GetName()}");

                    injectionField.SetValue(target, injection);
                    break;
                case MemberType.Property:
                    PropertyInfo injectionProp = target.GetType()
                        .GetProperty(injectionMemberName,
                                     BindingFlagsDefault.InstanceAll)
                                     ??
                        throw new MemberNotFoundException(target.GetType(),
                                                          MemberType.Field);

                    if (injectionProp.PropertyType.IsNotType(injectionMemberValueType))
                        throw new InvalidOperationException($"Invalid type = {injectionMemberValueType.GetName()}");

                    if (injectionProp.SetMethod is null)
                        throw new InvalidOperationException($"Property {injectionProp.Name} in {injectionProp.ReflectedType.GetName()} hasn't setter.");

                    injectionProp.SetValue(target, injection);
                    break;
                default:
                    throw new InvalidOperationException(injectionMemberType.ToString());
            }
        }
    }
}
