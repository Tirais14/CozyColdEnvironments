using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledObject : IDisposable, IEquatable<PooledObject>
    {
        public static PooledObject Default { get; } = new();

        private readonly Delegate? returnAction;

        private readonly bool isActionTyped;

        public readonly IObjectPoolBase? Pool { get; }
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
            IObjectPoolBase state, 
            Action<object, IObjectPoolBase> disposeAction
            )
            :
            this(value, state, disposeAction, isActionTyped: false)
        {

        }

        internal PooledObject(
            object value,
            IObjectPoolBase state,
            Delegate disposeAction,
            bool isActionTyped
            )
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));
            CC.Guard.IsNotNullState(state);
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            Pool = state;
            Value = value;
            this.returnAction = disposeAction;
            this.isActionTyped = isActionTyped;
        }

        public static PooledObject<T> Create<T>(T value)
            where T : class
        {
            return new PooledObject<T>(value);
        }

        public static PooledObject<T> Create<T>(
            T value,
            IObjectPoolBase<T> pool,
            Action<T, IObjectPoolBase<T>> disposeAction
            )
            where T : class
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            return new PooledObject<T>(value, pool!, (value, state) => disposeAction.Invoke(value, (TPool)state));
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

            if (returnAction is null || Pool.IsNull())
                return new PooledObject<T>((T)Value);

            if (isActionTyped)
            {
                return new PooledObject<T>(
                    (T)Value,
                    (IObjectPoolBase<T>)Pool,
                    (Action<T, IObjectPoolBase<T>>)returnAction
                    );
            }

            return new PooledObject<T>(
                (T)Value,
                (IObjectPoolBase<T>)Pool,
                (Action<object, IObjectPoolBase<T>>)returnAction
                );
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            if (this != Default)
            {
                try
                {
                    if (Pool.IsNotNull()
                        &&
                        returnAction is not null
                        &&
                        Pool.IsActiveObject(Value)
                        )
                    {
                        returnAction.DynamicInvoke(Value, Pool);
                    }
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
            return returnAction == other.returnAction
                   &&
                   EqualityComparer<IObjectPoolBase?>.Default.Equals(Pool, other.Pool)
                   &&
                   EqualityComparer<object?>.Default.Equals(Value, other.Value);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is PooledObject typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(returnAction, Pool, Value);
        }

        public readonly override string ToString()
        {
            if (this == Default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Pool)}: {Pool}; {nameof(Value)}: {Value})";
        }
    }

    public struct PooledObject<T> : IEquatable<PooledObject<T>>, IDisposable
        where T : class
    {
        public static PooledObject<T> Default { get; } = new();

        private readonly Action<T, IObjectPoolBase<T>>? returnAction;

        public readonly IObjectPoolBase<T>? Pool { get; }

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

        public PooledObject(T value, IObjectPoolBase<T> pool, Action<T, IObjectPoolBase<T>> returnAction)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));
            CC.Guard.IsNotNullState(pool);
            Guard.IsNotNull(returnAction, nameof(returnAction));

            Pool = pool;
            Value = value;
            this.returnAction = returnAction;
        }

        public static implicit operator PooledObject(PooledObject<T> instance)
        {
            if (instance == Default)
                return PooledObject.Default;

            if (instance.returnAction is null || instance.Pool.IsNull())
                return new PooledObject(instance.Value);

            return new PooledObject(
                instance.Value,
                instance.Pool,
                instance.returnAction,
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
                    if (Pool.IsNotNull()
                        &&
                        returnAction is not null
                        &&
                        Pool.IsActiveObject(Value)
                        )
                    {
                        returnAction(Value, Pool);
                    }
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
            return returnAction == other.returnAction
                   &&
                   EqualityComparer<IObjectPoolBase<T>?>.Default.Equals(Pool, other.Pool)
                   &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is PooledObject<T> typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(returnAction, Pool, Value);
        }

        public readonly override string ToString()
        {
            if (this == Default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Pool)}: {Pool}; {nameof(Value)}: {Value})";
        }
    }
}
