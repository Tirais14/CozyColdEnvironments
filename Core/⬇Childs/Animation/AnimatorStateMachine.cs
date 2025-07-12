using System;
using System.Reflection;
using UnityEngine;
using UTIRLib.Extensions;
using UTIRLib.Injector;
using UTIRLib.Patterns.Composite;
using UTIRLib.Utils;

#nullable enable
#pragma warning disable S2292 // Trivial properties should be auto-implemented

namespace UTIRLib.Animation
{
    [RequireComponent(typeof(Animator))]
    public abstract class AnimatorStateMachine : MonoX, IAnimatorStateMachine<AnimatorState>
    {
        [GetComponent]
        protected Animator animator = null!;

        protected string?[] playingStateNames = null!;
        protected bool isEnabled;
        protected bool isExecuting;

        public Animator Animator => animator;

        public bool IsEnabled {
            get => isEnabled;
            set => isEnabled = value;
        }

        public bool IsExecuting => isExecuting;

        protected override void OnAwake()
        {
            base.OnAwake();
            playingStateNames = new string[animator.layerCount];
        }

        protected abstract void OnUpdate();

        public bool IsStateExecuting(string? stateName, int layerIndex)
        {
            if (stateName.IsNullOrEmpty())
            {
                return false;
            }

            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        public bool IsStateExecuting(IAnimatorState animatorState) =>
            IsStateExecuting(animatorState.StateName, animatorState.Layer);

        public bool IsStateExecuting(AnimatorState animatorState) =>
            IsStateExecuting(animatorState.StateName, animatorState.Layer);

        protected void SwitchStateTo(string? stateName, int layerIndex)
        {
            if (IsStateExecuting(stateName, layerIndex))
            {
                return;
            }

            playingStateNames[layerIndex] = stateName;
            animator.Play(stateName, layerIndex);
        }

        protected void SwitchStateTo(IAnimatorState animatorState) => SwitchStateTo(animatorState.StateName,
            animatorState.Layer);

        protected void SwitchStateTo(AnimatorState animatorState) => SwitchStateTo(animatorState.StateName,
            animatorState.Layer);

        protected void SwitchStateTo(IComposite<IAnimatorState> compositeAnimatorState)
        {
            for (int i = 0; i < compositeAnimatorState.Count; i++)
            {
                SwitchStateTo(compositeAnimatorState[i]);
            }
        }

        protected virtual void SwitchToIdle() => SwitchStateTo("Idle", 0);

        protected void ConstructParametersByReflection()
        {
            FieldInfo[] fields = TypeHelper.GetAllMembers<FieldInfo>(GetType(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            object constructedParameter;
            ConstructorInfo parameterConstructor;
            Type[] constructorParameters = new Type[] { typeof(string) };
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Is(typeof(AnimatorParameter)))
                {
                    parameterConstructor = field.FieldType.GetConstructor(bindingFlags, binder: null,
                        constructorParameters, Array.Empty<ParameterModifier>());
                    if (parameterConstructor == null)
                    {
                        Debug.LogError("Cannot find constructor.");
                        continue;
                    }

                    constructedParameter = parameterConstructor.Invoke(new object[] { field.Name });
                    field.SetValue(this, constructedParameter);
                }
            }
        }

        protected void Update() => OnUpdate();
    }
}