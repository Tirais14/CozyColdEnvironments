#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Patterns.Factory;
using UTIRLib.Reflection;

#pragma warning disable S3881
namespace UTIRLib.Patterns.States
{
    public abstract class AStateMachine : MonoX, IDisposableContainer
    {
        public static bool HelperWarningsEnabled = true;

        private readonly DisposableCollection disposables = new();

        private bool disposedValue;
        private Dictionary<Type, Action>? forceStopableStates;
        private IState playingState = null!;

        protected bool autoCreateStatesByFactory = true;
        protected IState defaultState;

        public bool IsDefaultState => IsExecuting(defaultState);

        protected override void OnStart()
        {
            if (autoCreateStatesByFactory 
                &&
                !IsStatesExists()
                &&
                TryGetSingleFactory(out IFactory<Type, IState>? stateFactory)
                )
                CreateStatesByFactory(stateFactory);

            base.OnStart();

            playingState = defaultState;
            playingState.Enter();
        }

        public abstract IState GetNextState();

        public bool IsExecuting(IState state) => playingState.Equals(state);

        /// <exception cref="ArgumentNullException"></exception>
        public void ForceStopState(IState state)
        {
            if (state.IsNull())
                throw new ArgumentNullException(nameof(state));
            if (!IsForceStopable(state))
                throw new ArgumentException($"{state.GetTypeName()} is not force stopable state.");

            forceStopableStates![state.GetType()].Invoke();
        }

        public bool IsForceStopable(IState? state)
        {
            if (state.IsNull() || forceStopableStates is null)
                return false;

            return forceStopableStates.ContainsKey(state.GetType());
        }

        /// <exception cref="ArgumentNullException"></exception>
        protected void BindForceStopable(IState state, Action action)
        {
            if (state.IsNull())
                throw new ArgumentNullException(nameof(state));
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            forceStopableStates ??= new Dictionary<Type, Action>(1);

            Type stateType = state.GetType();
            if (forceStopableStates.ContainsKey(stateType))
            {
                forceStopableStates[stateType] += action;
                return;
            }

            forceStopableStates.Add(stateType, action);
        }

        protected void CreateStatesByFactory(IFactory<Type, IState>? factory = null)
        {
            StateMachineHelper.CreateStatesByFactory(this, factory);

            GetType().ForceGetFields(BindingFlagsDefault.InstanceAll)
                      .Where(x => x.FieldType.IsType<IState>()).Select(x =>
                      {
                          if (x.GetValue(this) is IDisposable disposable)
                              disposables.Add(disposable);

                          return x;
                      });
        }

        /// <exception cref="ArgumentNullException"></exception>
        protected void SetState(IState state)
        {
            if (playingState == state)
                return;

            playingState.Exit();
            playingState = state;
            playingState.Enter();
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    DisposeManaged();

                DisposeUnmanaged();
                disposedValue = true;
            }
        }

        protected virtual void DisposeManaged()
        {
            disposables.Dispose();
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        private bool IsStatesExists()
        {
            FieldInfo[] stateFields = GetType().GetFields(BindingFlagsDefault.InstanceAll)
                                          .Where(x => x.FieldType.IsType<IState>())
                                          .ToArray();

            foreach (var field in stateFields)
            {
                if (field.GetValue(this).IsNull())
                    return false;
            }

            return true;
        }

        private bool TryGetSingleFactory(
            [NotNullWhen(true)] out IFactory<Type, IState>? stateFactory)
        {
            FieldInfo[] factoryFields = GetType().GetFields(BindingFlagsDefault.InstanceAll)
                                                 .Where(x => x.FieldType.IsType<IFactory<Type, IState>>())
                                                 .ToArray();

            if (factoryFields.IsEmpty())
            {
                autoCreateStatesByFactory = false;
                stateFactory = null;
                return false;
            }

            if (factoryFields.Length > 1)
            {
                if (HelperWarningsEnabled)
                    TirLibDebug.PrintWarning("Cannot resolve factory to create states.");

                autoCreateStatesByFactory = false;
                stateFactory = null;
                return false;
            }

            stateFactory = (IFactory<Type, IState>)factoryFields[0].GetValue(this);
            autoCreateStatesByFactory = stateFactory.IsNotNull();

            return autoCreateStatesByFactory;
        }

        private void Update()
        {
            SetState(GetNextState());

            playingState.Tick();
        }

        private void FixedUpdate() => playingState.FixedTick();

        private void LateUpdate() => playingState.LateTick();

        private void OnDestroy() => ((IDisposable)this).Dispose();

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
