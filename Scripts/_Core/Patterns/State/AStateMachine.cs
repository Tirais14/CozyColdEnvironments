#nullable enable
using System;
using System.Linq;
using UnityEditor;
using UTIRLib.Disposables;
using UTIRLib.Patterns.Factory;
using UTIRLib.Reflection;

namespace UTIRLib.Patterns.States
{
    public abstract class AStateMachine : MonoX, IDisposableContainer
    {
        private bool disposedValue;

        protected readonly DisposableCollection disposables = new();
        protected IState defaultState;
        protected IState playingState;

        public bool IsDefaultState => IsPlaying(defaultState);

        protected override void OnStart()
        {
            base.OnStart();

            playingState = defaultState;
            playingState.Enter();
        }

        public abstract IState GetNextState();

        public bool IsPlaying(IState state) => playingState.Equals(state);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    disposables.Dispose();

                disposedValue = true;
            }
        }

        private void Update()
        {
            SetState(GetNextState());

            playingState.OnUpdate();
        }

        private void FixedUpdate() => playingState.OnFixedUpdate();

        private void LateUpdate() => playingState.OnLateUpdate();

        private void OnDestroy() => Dispose();
    }
}
