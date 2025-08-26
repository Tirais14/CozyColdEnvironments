#nullable enable
using System;
using System.Collections.Generic;
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
    /// <typeparam name="TDefault">Default state</typeparam>
    public abstract class AStateMachine<TDefault> 
        :
        MonoX,
        IStateMachine,
        IDisposableContainer

        where TDefault : IState
    {
        private readonly DisposableCollection disposables = new();

        private bool disposedValue;
        private Dictionary<Type, Action>? forceStopableStates;
        private IState playingState = default!;

        protected bool autoCreateStatesByFactory = true;
        public TDefault DefaultState { get; private set; } = default!;

        public bool IsDefaultState => IsExecuting(DefaultState);

        protected override void OnStart()
        {
            base.OnStart();

            if (autoCreateStatesByFactory)
                CreateStatesByFactory();

            DefaultState = GetDefaultState();

            if (DefaultState.IsNull())
                throw new NullReferenceException("Default state is not setted.");

            playingState = DefaultState;
            playingState.Enter();
        }

        public bool IsExecuting(Type? stateType)
        {
            return playingState.GetType() == stateType;
        }
        public bool IsExecuting(IState state)
        {
            return IsExecuting(state.GetType());
        }

        public void ForceStopState(Type stateType)
        {
            if (stateType is null)
                throw new ArgumentNullException(nameof(stateType));
            if (!IsForceStopable(stateType))
                throw new ArgumentException(stateType.GetName());

            if (playingState.GetType() == stateType)
                forceStopableStates![stateType]();
        }
        public void ForceStopState(IState state)
        {
            ForceStopState(state.GetType());
        }

        public bool IsForceStopable(Type? stateType)
        {
            if (stateType is null
                ||
                forceStopableStates is null
                ||
                forceStopableStates.Count == 0
                )
                return false;

            return forceStopableStates.ContainsKey(stateType);
        }
        public bool IsForceStopable(IState? state)
        {
            return IsForceStopable(state?.GetType());
        }

        protected abstract IState GetNextState();

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

        protected void CreateStatesByFactory()
        {
            FieldInfo[] stateFields = StateMachineHelper.GetStateFields(this);
            IFactory<Type, IState>? stateFactory = StateMachineHelper.FindFactoryInFields(this);
            if (stateFactory.IsNull())
            {
                TirLibDebug.PrintError("Failed to find state factory in fields.");
                return;
            }

            IState[] states = StateMachineHelper.CreateStates(
                stateFactory,
                stateFields.Select(x => x.FieldType).ToArray());

            StateMachineHelper.InjectStates(this, stateFields, states);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    disposables.Dispose();

                disposedValue = true;
            }
        }

        protected virtual TDefault GetDefaultState() => DefaultState;

        private void Update()
        {
            if (playingState.IsNull())
                return;

            SetState(GetNextState());

            playingState.Tick();
        }

        private void FixedUpdate()
        {
            if (playingState.IsNull())
                return;

            playingState.FixedTick();
        }

        private void LateUpdate()
        {
            if (playingState.IsNull())
                return;

            playingState.LateTick();
        }

        private void OnDestroy() => ((IDisposable)this).Dispose();

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public abstract class AStateMachine : AStateMachine<IState>
    {
        public static bool HelperWarningsEnabled = true;
    }
}
