using System;
using System.Reflection;
using UnityEngine;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.Unity
{
    public interface IHasGameObjectBody
    {
        Transform Body { get; }

        ///// <summary>
        ///// Don't invoke this, if you don't know for why
        ///// </summary>
        //void Tech_Init()
        //{
        //    Type thisType = GetType();

        //    if (this.IsNot<Component>(out var thisComponent))
        //        throw new InvalidOperationException("Type is not component.");

        //    PropertyInfo bodyProp = thisType.GetProperty(nameof(Body));

        //    if (bodyProp.SetMethod is null)
        //        throw new InvalidOperationException("Cannot find set method.");

        //    thisComponent.transform.Find("Body");
        //}
    }
}
