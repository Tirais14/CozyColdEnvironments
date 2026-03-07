using System;
using System.Collections.Generic;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Pools
{
    public struct PooledObject : IDisposable, IEquatable<PooledObject>
    {
        public static PooledObject Default { get; } = new();

        private readonly Delegate? returnAction;

        private readonly bool isActionTyped;

        public readonly object? Pool { get; }
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
            Action<object, object> returnAction
            )
            :
            this(value, state, returnAction, isActionTyped: false)
        {

        }

        internal PooledObject(
            object value,
            object pool,
            Delegate returnAction,
            bool isActionTyped
            )
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));
            CC.Guard.IsNotNullState(pool);
            Guard.IsNotNull(returnAction, nameof(returnAction));

            Pool = pool;
            Value = value;
            this.returnAction = returnAction;
            this.isActionTyped = isActionTyped;
        }

        public static PooledObject<T> Create<T>(T value)
            where T : class
        {
            return new PooledObject<T>(value);
        }

        public static PooledObject<T> Create<T>(
            T value,
            object pool,
            Action<T, object> returnAction
            )
            where T : class
        {
            Guard.IsNotNull(returnAction, nameof(returnAction));

            return new PooledObject<T>(value, pool!, returnAction);
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
                    Pool,
                    (Action<T, object>)returnAction
                    );
            }

            return new PooledObject<T>(
                (T)Value,
                Pool,
                (Action<object, object>)returnAction
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
                        (Pool.IsNot<IObjectPoolBase>(out var poolBase)
                        ||
                        poolBase.IsActiveObject(Value)
                        ))
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
                   EqualityComparer<object?>.Default.Equals(Pool, other.Pool)
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

        private readonly Action<T, object>? returnAction;

        public readonly object? Pool { get; }

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

        public PooledObject(T value, object pool, Action<T, object> returnAction)
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
                        (Pool.IsNot<IObjectPoolBase>(out var poolBase)
                        ||
                        poolBase.IsActiveObject(Value)
                        ))
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
                   EqualityComparer<object?>.Default.Equals(Pool, other.Pool)
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
