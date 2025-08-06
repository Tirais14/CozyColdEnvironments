using UnityEngine;
using UTIRLib;
using UTIRLib.Unity;

#nullable enable
namespace UTIRLib
{
    public class WorldObject : MonoX, IHasBody
    {
        public Transform Body { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Body = transform.Find("Body");
        }
    }
}
