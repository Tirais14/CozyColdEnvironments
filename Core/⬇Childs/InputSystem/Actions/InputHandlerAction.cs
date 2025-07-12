using System;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public class InputHandlerAction<TInputValue> : IInputHandlerAction<TInputValue>
        where TInputValue : struct
    {
        [Flags]
        protected enum Actions
        {
            OnPerformedRaw = 1,
            OnPerformedWithValue = 2,
            OnPerformed = 4,
            OnStartedRaw = 8,
            OnStartedWithValue = 16,
            OnStarted = 32,
            OnCancelledRaw = 64,
            OnCancelledWithValue = 128,
            OnCancelled = 256
        }

        protected readonly InputAction inputAction;
        protected Actions enabledActions;
        private Action<InputAction.CallbackContext>? onPerformedRaw;
        private Action<InputAction.CallbackContext>? onStartedRaw;
        private Action<InputAction.CallbackContext>? onCancelledRaw;
        private Action<TInputValue>? onPerformedWithValue;
        private Action<TInputValue>? onStartedWithValue;
        private Action<TInputValue>? onCancelledWithValue;
        private Action? onPerformed;
        private Action? onStarted;
        private Action? onCancelled;

        public event Action<InputAction.CallbackContext> OnPerformedRaw {
            add {
                onPerformedRaw += value;
                ActionSwitcher(Actions.OnPerformedRaw);
            }
            remove {
                onPerformedRaw -= value;
                ActionSwitcher(Actions.OnPerformedRaw);
            }
        }

        public event Action<InputAction.CallbackContext> OnStartedRaw {
            add {
                onStartedRaw += value;
                ActionSwitcher(Actions.OnStartedRaw);
            }
            remove {
                onStartedRaw -= value;
                ActionSwitcher(Actions.OnStartedRaw);
            }
        }

        public event Action<InputAction.CallbackContext> OnCancelledRaw {
            add {
                onCancelledRaw += value;
                ActionSwitcher(Actions.OnCancelledRaw);
            }
            remove {
                onCancelledRaw -= value;
                ActionSwitcher(Actions.OnCancelledRaw);
            }
        }

        public event Action<TInputValue> OnPerformedWithValue {
            add {
                onPerformedWithValue += value;
                ActionSwitcher(Actions.OnPerformedWithValue);
            }
            remove {
                onPerformedWithValue -= value;
                ActionSwitcher(Actions.OnPerformedWithValue);
            }
        }

        public event Action<TInputValue> OnStartedWithValue {
            add {
                onStartedWithValue += value;
                ActionSwitcher(Actions.OnStartedWithValue);
            }
            remove {
                onStartedWithValue -= value;
                ActionSwitcher(Actions.OnStartedWithValue);
            }
        }

        public event Action<TInputValue> OnCancelledWithValue {
            add {
                onCancelledWithValue += value;
                ActionSwitcher(Actions.OnCancelledWithValue);
            }
            remove {
                onCancelledWithValue -= value;
                ActionSwitcher(Actions.OnCancelledWithValue);
            }
        }

        public event Action OnPerformed {
            add {
                onPerformed += value;
                ActionSwitcher(Actions.OnPerformed);
            }
            remove {
                onPerformed -= value;
                ActionSwitcher(Actions.OnPerformed);
            }
        }

        public event Action OnStarted {
            add {
                onStarted += value;
                ActionSwitcher(Actions.OnStarted);
            }
            remove {
                onStarted -= value;
                ActionSwitcher(Actions.OnStarted);
            }
        }

        public event Action OnCancelled {
            add {
                onCancelled += value;
                ActionSwitcher(Actions.OnCancelled);
            }
            remove {
                onCancelled -= value;
                ActionSwitcher(Actions.OnCancelled);
            }
        }

        public InputAction InputAction => inputAction;
        public string InputActionName => inputAction.name;
        public virtual TInputValue Value => inputAction.ReadValue<TInputValue>();

        public InputHandlerAction(InputAction inputAction) => this.inputAction = inputAction
            ?? throw new ArgumentNullException(nameof(inputAction));

        public void OnPerformedAction(InputAction.CallbackContext context)
        {
            if (IsEnabled(Actions.OnPerformedRaw))
            {
                onPerformedRaw!(context);
            }
            if (IsEnabled(Actions.OnPerformedWithValue))
            {
                onPerformedWithValue!(ReadValue(context));
            }
            if (IsEnabled(Actions.OnPerformed))
            {
                onPerformed!();
            }
        }

        public void OnStartedAction(InputAction.CallbackContext context)
        {
            if (IsEnabled(Actions.OnStartedRaw))
            {
                onStartedRaw!(context);
            }
            if (IsEnabled(Actions.OnStartedWithValue))
            {
                onStartedWithValue!(ReadValue(context));
            }
            if (IsEnabled(Actions.OnStarted))
            {
                onStarted!();
            }
        }

        public void OnCancelledAction(InputAction.CallbackContext context)
        {
            if (IsEnabled(Actions.OnCancelledRaw))
            {
                onCancelledRaw!(context);
            }
            if (IsEnabled(Actions.OnCancelledWithValue))
            {
                onCancelledWithValue!(ReadValue(context));
            }
            if (IsEnabled(Actions.OnCancelled))
            {
                onCancelled!();
            }
        }

        public void Subscribe(Delegate action, InputActionEventType eventType)
        {
            switch (action)
            {
                case Action<InputAction.CallbackContext> rawAction:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformedRaw += rawAction;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStartedRaw += rawAction;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelledRaw += rawAction;
                            break;

                        default:
                            break;
                    }
                    break;

                case Action<TInputValue> actionWithValue:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformedWithValue += actionWithValue;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStartedWithValue += actionWithValue;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelledWithValue += actionWithValue;
                            break;

                        default:
                            break;
                    }
                    break;

                case Action basicAction:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformed += basicAction;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStarted += basicAction;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelled += basicAction;
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        public void Unsubscribe(Delegate action, InputActionEventType eventType)
        {
            switch (action)
            {
                case Action<InputAction.CallbackContext> rawAction:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformedRaw -= rawAction;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStartedRaw -= rawAction;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelledRaw -= rawAction;
                            break;

                        default:
                            break;
                    }
                    break;

                case Action<TInputValue> actionWithValue:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformedWithValue -= actionWithValue;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStartedWithValue -= actionWithValue;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelledWithValue -= actionWithValue;
                            break;

                        default:
                            break;
                    }
                    break;

                case Action basicAction:
                    switch (eventType)
                    {
                        case InputActionEventType.OnPerformed:
                            OnPerformed -= basicAction;
                            break;

                        case InputActionEventType.OnStarted:
                            OnStarted -= basicAction;
                            break;

                        case InputActionEventType.OnCancelled:
                            OnCancelled -= basicAction;
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        protected virtual TInputValue ReadValue(InputAction.CallbackContext context) => context.ReadValue<TInputValue>();

        protected bool IsEnabled(Actions actionType) => (enabledActions & actionType) == actionType;

        protected void ActionSwitcher(Actions actionType)
        {
            switch (actionType)
            {
                case Actions.OnPerformedRaw:
                    ActionStateSwitcher(onPerformedRaw, actionType);
                    break;

                case Actions.OnPerformedWithValue:
                    ActionStateSwitcher(onPerformedWithValue, actionType);
                    break;

                case Actions.OnPerformed:
                    ActionStateSwitcher(onPerformed, actionType);
                    break;

                case Actions.OnStartedRaw:
                    ActionStateSwitcher(onStartedRaw, actionType);
                    break;

                case Actions.OnStartedWithValue:
                    ActionStateSwitcher(onStartedWithValue, actionType);
                    break;

                case Actions.OnStarted:
                    ActionStateSwitcher(onStarted, actionType);
                    break;

                case Actions.OnCancelledRaw:
                    ActionStateSwitcher(onCancelledRaw, actionType);
                    break;

                case Actions.OnCancelledWithValue:
                    ActionStateSwitcher(onCancelledWithValue, actionType);
                    break;

                case Actions.OnCancelled:
                    ActionStateSwitcher(onCancelled, actionType);
                    break;

                default:
                    break;
            }
        }

        protected void ActionStateSwitcher(Delegate? action, Actions actionType)
        {
            if (action is not null && !IsEnabled(actionType))
            {
                EnableAction(actionType);
            }
            else if (action is null && IsEnabled(actionType))
            {
                DisableAction(actionType);
            }
        }

        protected void EnableAction(Actions actionType) => enabledActions |= actionType;

        protected void DisableAction(Actions actionType) => enabledActions &= ~actionType;
    }
}