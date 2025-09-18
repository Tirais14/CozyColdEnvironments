using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Rx;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    //public sealed class ObservableEvent<T> : Observable<T>, IObservableEvent<T>, IEquatable<ObservableEvent<T>?>
    //    where T : Delegate
    //{
    //    public ObservableEvent()
    //    {
    //    }

    //    public ObservableEvent(T function)
    //    {
    //        Function = function;
    //    }

    //    public static bool operator ==(ObservableEvent<T>? left, ObservableEvent<T>? right)
    //    {
    //        if (left is null && right is null)
    //            return true;

    //        return left is not null && left.Equals(right);
    //    }

    //    public static bool operator !=(ObservableEvent<T>? left, ObservableEvent<T>? right)
    //    {
    //        return !(left == right);
    //    }

    //    public static ObservableEvent<T> operator +(ObservableEvent<T>? left,
    //                                                T right)
    //    {
    //        if (left is null)
    //            return new ObservableEvent<T>(right);

    //        left.AddHandler(right);

    //        return left;
    //    }

    //    public static ObservableEvent<T>? operator -(ObservableEvent<T>? left,
    //                                                T right)
    //    {
    //        if (left is null)
    //            return null;

    //        left.RemoveHandler(right);

    //        return left;
    //    }

    //    /// <summary>
    //    /// Doesn't call <see cref="Observable{T}.Publish"/>
    //    /// </summary>
    //    /// <param name="function"></param>
    //    /// <returns></returns>
    //    public IDisposable AddHandler(T function)
    //    {
    //        observers.Add(new Observer<T>(x => x.DynamicInvoke()));

    //        return Subscription.Create(function, (x) => RemoveHandler(x));
    //    }

    //    public void RemoveHandler(T function)
    //    {
    //        actions.Remove(function);
    //    }

    //    public bool Equals(ObservableEvent<T>? other)
    //    {
    //        if (other is null)
    //            return false;

    //        return actions.SequenceEqual(other.actions);
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        return Equals(obj as ObservableEvent<T>);
    //    }

    //    public override int GetHashCode() => actions.ToHashCode();

    //    public override string ToString()
    //    {
    //        return typeof(ObservableEvent<T>).GetFullName();
    //    }
    //}
}
