using UnityEngine;

#nullable enable
namespace UTIRLib.Unity
{
    public class WorldObject : MonoX, IHasGameObjectBody
    {
        public static bool WarningsEnabled;

        public Transform Body { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Body = transform.Find("Body");

            if (Body == null)
            {
                Body = transform;

                if (WarningsEnabled)
                    TirLibDebug.PrintWarning($"Not found {nameof(Body)}, use self value.");
            }
        }
    }
}
