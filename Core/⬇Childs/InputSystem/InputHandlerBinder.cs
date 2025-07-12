using System;

#nullable enable

namespace UTIRLib.InputSystem
{
    [Obsolete]
    public class InputHandlerBinder
    //IInputHandlerBinder<bool>, IInputHandlerBinder<Vector2>, IInputHandlerBinder<Vector3>, IInputHandlerBinder<Quaternion>
    {
        //    protected readonly IInputHandler inputHandler;
        //    protected readonly IInputHandler<bool>? buttonInputHandler;
        //    protected readonly List<InputActionBindInfo<bool>>? bindedButtonInputs;
        //    protected readonly IInputHandler<Vector2>? vector2InputHandler;
        //    protected readonly List<InputActionBindInfo<Vector2>>? bindedVector2Inputs;
        //    protected readonly IInputHandler<Vector3>? vector3InputHandler;
        //    protected readonly List<InputActionBindInfo<Vector3>>? bindedVector3Inputs;
        //    protected readonly IInputHandler<Quaternion>? quaternionInputHandler;
        //    protected readonly List<InputActionBindInfo<Quaternion>>? bindedQuaternionInputs;

        //    public IInputHandler InputHandler => inputHandler;

        //    /// <exception cref="ArgumentNullException"></exception>
        //    public InputHandlerBinder(IInputHandler inputHandler)
        //    {
        //        this.inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));

        //        buttonInputHandler = inputHandler as IInputHandler<bool>;
        //        bindedButtonInputs = buttonInputHandler != null ? new List<InputActionBindInfo<bool>>() : null;

        //        vector2InputHandler = inputHandler as IInputHandler<Vector2>;
        //        bindedVector2Inputs = vector2InputHandler != null ? new List<InputActionBindInfo<Vector2>>() : null;

        //        vector3InputHandler = inputHandler as IInputHandler<Vector3>;
        //        bindedVector3Inputs = vector3InputHandler != null ? new List<InputActionBindInfo<Vector3>>() : null;

        //        quaternionInputHandler = inputHandler as IInputHandler<Quaternion>;
        //        bindedQuaternionInputs = quaternionInputHandler != null ? new List<InputActionBindInfo<Quaternion>>() : null;
        //    }

        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void BindInput(string inputActionName, Action<bool> action, InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (buttonInputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support button bindings.");
        //            return;
        //        }

        //        buttonInputHandler.BindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void BindInput(string inputActionName, Action<Vector2> action, InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (vector2InputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Vector2).Name} bindings.");
        //            return;
        //        }

        //        vector2InputHandler.BindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void BindInput(string inputActionName, Action<Vector3> action,
        //        InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (vector3InputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Vector3).Name} bindings.");
        //            return;
        //        }

        //        vector3InputHandler.BindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void BindInput(string inputActionName, Action<Quaternion> action,
        //        InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (quaternionInputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Quaternion).Name} bindings.");
        //            return;
        //        }

        //        quaternionInputHandler.BindAction(inputActionName, action, eventType);
        //    }

        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void UnbindInput(string inputActionName, Action<bool> action, InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (buttonInputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support button bindings.");
        //            return;
        //        }

        //        buttonInputHandler.UnbindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void UnbindInput(string inputActionName, Action<Vector2> action, InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (vector2InputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Vector2).Name} bindings.");
        //            return;
        //        }

        //        vector2InputHandler.UnbindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void UnbindInput(string inputActionName, Action<Vector3> action,
        //        InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (vector3InputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Vector3).Name} bindings.");
        //            return;
        //        }

        //        vector3InputHandler.UnbindAction(inputActionName, action, eventType);
        //    }
        //    /// <remarks>Debug:
        //    /// <br/><see cref="NullOrEmptyStringMessage"/>
        //    /// <br/><see cref="ArgumentNullMessage"/>
        //    /// </remarks>
        //    public virtual void UnbindInput(string inputActionName, Action<Quaternion> action,
        //        InputActionEventType eventType)
        //    {
        //        if (string.IsNullOrEmpty(inputActionName)) {
        //            Debug.LogError(new NullOrEmptyStringMessage(inputActionName, nameof(inputActionName)));
        //            return;
        //        }
        //        if (action == null) {
        //            Debug.LogError(new ArgumentNullMessage(nameof(action)));
        //            return;
        //        }
        //        if (quaternionInputHandler == null) {
        //            Debug.LogError($"{inputHandler.GetType().Name} doesn't support {typeof(Quaternion).Name} bindings.");
        //            return;
        //        }

        //        quaternionInputHandler.UnbindAction(inputActionName, action, eventType);
        //    }

        //    public void UnbindAllInputs()
        //    {
        //        ((IInputHandlerBinder<bool>)this).UnbindAllInputs();
        //        ((IInputHandlerBinder<Vector2>)this).UnbindAllInputs();
        //        ((IInputHandlerBinder<Vector3>)this).UnbindAllInputs();
        //        ((IInputHandlerBinder<Quaternion>)this).UnbindAllInputs();
        //    }

        //    void IInputHandlerBinder<bool>.UnbindAllInputs()
        //    {
        //        if (bindedButtonInputs == null || bindedButtonInputs.Count == 0) {
        //            return;
        //        }

        //        foreach (InputActionBindInfo<bool> bindedInput in bindedButtonInputs) {
        //            buttonInputHandler!.UnbindAction(bindedInput.InputActionName, bindedInput.Action,
        //                bindedInput.EventType);
        //        }
        //    }
        //    void IInputHandlerBinder<Vector2>.UnbindAllInputs()
        //    {
        //        if (bindedVector2Inputs == null || bindedVector2Inputs.Count == 0) {
        //            return;
        //        }

        //        foreach (InputActionBindInfo<Vector2> bindedInput in bindedVector2Inputs) {
        //            vector2InputHandler!.UnbindAction(bindedInput.InputActionName, bindedInput.Action,
        //                bindedInput.EventType);
        //        }
        //    }
        //    void IInputHandlerBinder<Vector3>.UnbindAllInputs()
        //    {
        //        if (bindedVector3Inputs == null || bindedVector3Inputs.Count == 0) {
        //            return;
        //        }

        //        foreach (InputActionBindInfo<Vector3> bindedInput in bindedVector3Inputs) {
        //            vector3InputHandler!.UnbindAction(bindedInput.InputActionName, bindedInput.Action,
        //                bindedInput.EventType);
        //        }
        //    }
        //    void IInputHandlerBinder<Quaternion>.UnbindAllInputs()
        //    {
        //        if (bindedQuaternionInputs == null || bindedQuaternionInputs.Count == 0) {
        //            return;
        //        }

        //        foreach (InputActionBindInfo<Quaternion> bindedInput in bindedQuaternionInputs) {
        //            quaternionInputHandler!.UnbindAction(bindedInput.InputActionName, bindedInput.Action,
        //                bindedInput.EventType);
        //        }
        //    }
        //}
        //public class InputHandlerBinder<TEnum> : InputHandlerBinder, IInputHandlerBinder<bool, TEnum>,
        //    IInputHandlerBinder<Vector2, TEnum>, IInputHandlerBinder<Vector3, TEnum>, IInputHandlerBinder<Quaternion, TEnum>
        //    where TEnum : Enum
        //{
        //    public InputHandlerBinder(IInputHandler inputHandler) : base(inputHandler)
        //    { }

        //    public void BindInput(TEnum inputActionName, Action<bool> action,
        //        InputActionEventType eventType) => BindInput(inputActionName.ToString(), action, eventType);
        //    public void BindInput(TEnum inputActionName, Action<Vector2> action,
        //        InputActionEventType eventType) => BindInput(inputActionName.ToString(), action, eventType);
        //    public void BindInput(TEnum inputActionName, Action<Vector3> action,
        //        InputActionEventType eventType) => BindInput(inputActionName.ToString(), action, eventType);
        //    public void BindInput(TEnum inputActionName, Action<Quaternion> action,
        //        InputActionEventType eventType) => BindInput(inputActionName.ToString(), action, eventType);

        //    public void UnbindInput(TEnum inputActionName, Action<bool> action,
        //        InputActionEventType eventType) =>
        //        UnbindInput(inputActionName.ToString(), action, eventType);
        //    public void UnbindInput(TEnum inputActionName, Action<Vector2> action,
        //        InputActionEventType eventType) =>
        //        UnbindInput(inputActionName.ToString(), action, eventType);
        //    public void UnbindInput(TEnum inputActionName, Action<Vector3> action,
        //        InputActionEventType eventType) =>
        //        UnbindInput(inputActionName.ToString(), action, eventType);
        //    public void UnbindInput(TEnum inputActionName, Action<Quaternion> action,
        //        InputActionEventType eventType) =>
        //        UnbindInput(inputActionName.ToString(), action, eventType);
    }
}