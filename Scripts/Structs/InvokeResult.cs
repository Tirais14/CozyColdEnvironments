#nullable enable

using System;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using Object = UnityEngine.Object;

namespace UTIRLib
{
    /// <summary>
    /// If value less thand zero - error, otherwise success.
    /// Can be converted to bool (<see cref="IsSuccess"/>) and int <see cref="resultCode"/>
    /// </summary>
    public readonly struct InvokeResult
    {
        public readonly int resultCode;

        public readonly bool IsError => resultCode < 0;
        public readonly bool IsSuccess => !IsError;

        public InvokeResult(int resultCode) => this.resultCode = resultCode;

        public static InvokeResult<Object> Create(Object obj) => new(obj, (value) => value == null);

        public static InvokeResult<string> Create(string str) => new(str, (value) => value.IsNullOrEmpty());

        public static InvokeResult<T[]> Create<T>(T[] array, bool canContainNull = true) =>
            new(array, canContainNull ? (value) => value.IsNullOrEmpty()
            : (value) => value.IsNullOrEmpty() || value.HasNullElement());

        //public static InvokeResult<IEnumerable> Create(IEnumerable enumerable, bool canContainNull = true) =>
        //    new(enumerable, canContainNull ? (value) => value.IsNullOrEmpty()
        //    : (value) => value.IsNullOrEmpty() || value.HasNullElement());
        //public static InvokeResult<IEnumerable<T>> Create<T>(IEnumerable<T> enumerable, bool canContainNull = true) =>
        //    new(enumerable, canContainNull ? (value) => value.IsNullOrEmpty()
        //    : (value) => value.IsNullOrEmpty() || value.HasNullElement());
        //public static InvokeResult<ICollection> Create(ICollection collection, bool canContainNull = true) =>
        //    new(collection, canContainNull ? (value) => value.IsNullOrEmpty()
        //    : (value) => value.IsNullOrEmpty() || value.HasNullElement());
        //public static InvokeResult<ICollection<T>> Create<T>(ICollection<T> collection, bool canContainNull = true) =>
        //    new(collection, canContainNull ? (value) => value.IsNullOrEmpty()
        //    : (value) => value.IsNullOrEmpty() || value.HasNullElement());

        public static implicit operator int(InvokeResult invokeResult) => invokeResult.resultCode;

        public static implicit operator bool(InvokeResult invokeResult) => invokeResult.IsSuccess;
    }

    /// <summary>
    /// If value less thand zero - error, otherwise success.
    /// Can be converted to bool (<see cref="IsSuccess"/>)
    /// </summary>
    public readonly struct InvokeResult<T>
    {
        public readonly T? value;
        public readonly Predicate<T?>? errorPredicate;

        public readonly bool IsError {
            get {
                if (errorPredicate is not null)
                {
                    return errorPredicate(value);
                }
                else return value.IsDefault();
            }
        }

        public readonly bool IsSuccess => !IsError;

        public InvokeResult(T? value, Predicate<T?>? errorPredicate = null)
        {
            this.value = value;
            this.errorPredicate = errorPredicate;
        }

        public static implicit operator bool(InvokeResult<T> invokeResult) => invokeResult.IsSuccess;
    }
}