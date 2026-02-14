using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledObject : IDisposable, IEquatable<PooledObject>
    {
        public static PooledObject Default { get; } = new();

        private readonly Delegate? disposeAction;

        private readonly bool isActionTyped;

        public readonly object? State { get; }
        public readonly object Value { get; }

        public readonly bool IsValid {
            get => this != Default && !disposed;
        }

        public PooledObject(object value)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));

            Value = value;
        }

        public PooledObject(
            object value,
            object state, 
            Action<object, object> disposeAction
            )
            :
            this(value, state, disposeAction, isActionTyped: false)
        {

        }

        internal PooledObject(
            object value,
            object state,
            Delegate disposeAction,
            bool isActionTyped
            )
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));
            CC.Guard.IsNotNullState(state);
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            State = state;
            Value = value;
            this.disposeAction = disposeAction;
            this.isActionTyped = isActionTyped;
        }

        public static PooledObject<T> Create<T>(T value)
            where T : class
        {
            return new PooledObject<T>(value);
        }

        public static PooledObject<T> Create<T, TState>(
            T value,
            TState state,
            Action<T, TState> disposeAction
            )
            where T : class
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            return new PooledObject<T>(value, state!, (value, state) => disposeAction.Invoke(value, (TState)state));
        }

        public static bool operator ==(PooledObject left, PooledObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PooledObject left, PooledObject right)
        {
            return !(left == right);
        }

        public readonly PooledObject<T> Convert<T>()
            where T : class
        {
            if (this == Default)
                return PooledObject<T>.Default;

            if (disposeAction is null || State.IsNull())
                return new PooledObject<T>((T)Value);

            if (isActionTyped)
            {
                return new PooledObject<T>(
                    (T)Value,
                    State,
                    (Action<T, object>)disposeAction
                    );
            }

            return new PooledObject<T>(
                (T)Value,
                State,
                (Action<object, object>)disposeAction
                );
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            if (this != default)
            {
                try
                {
                    if (State.IsNotNull() && disposeAction is not null)
                        disposeAction.DynamicInvoke(Value, State);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            disposed = true;
        }

        public readonly bool Equals(PooledObject other)
        {
            return disposeAction == other.disposeAction
                   &&
                   EqualityComparer<object?>.Default.Equals(State, other.State)
                   &&
                   EqualityComparer<object?>.Default.Equals(Value, other.Value);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is PooledObject typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(disposeAction, State, Value);
        }

        public readonly override string ToString()
        {
            if (this == Default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(State)}: {State}; {nameof(Value)}: {Value})";
        }
    }

    public struct PooledObject<T> : IEquatable<PooledObject<T>>, IDisposable
        where T : class
    {
        public static PooledObject<T> Default { get; } = new();

        private readonly Action<T, object>? disposeAction;

        public readonly object? State { get; }

        public readonly T Value { get; }

        public readonly bool IsValid {
            get => this != Default && !disposed;
        }

        public PooledObject(T value)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));

            Value = value;
        }

        public PooledObject(T value, object state, Action<T, object> disposeAction)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));
            CC.Guard.IsNotNullState(state);
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            State = state;
            Value = value;
            this.disposeAction = disposeAction;
        }

        public static implicit operator PooledObject(PooledObject<T> instance)
        {
            if (instance == Default)
                return PooledObject.Default;

            if (instance.disposeAction is null || instance.State.IsNull())
                return new PooledObject(instance.Value);

            return new PooledObject(
                instance.Value,
                instance.State,
                instance.disposeAction,
                isActionTyped: true
                );
        }

        public static bool operator ==(PooledObject<T> left, PooledObject<T> right)
        {
            return left.Equals(right); 
        }

        public static bool operator !=(PooledObject<T> left, PooledObject<T> right)
        {
            return !(left == right);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            if (this != default)
            {
                try
                {
                    if (State.IsNotNull() && disposeAction is not null)
                        disposeAction(Value, State);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            disposed = true;
        }

        public readonly bool Equals(PooledObject<T> other)
        {
            return disposeAction == other.disposeAction
                   &&
                   EqualityComparer<object?>.Default.Equals(State, other.State)
                   &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is PooledObject<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(disposeAction, State, Value);
        }

        public readonly override string ToString()
        {
            if (this == Default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(State)}: {State}; {nameof(Value)}: {Value})";
        }
    }
}
