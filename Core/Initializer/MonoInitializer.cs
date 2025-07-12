using System.Collections;
using UnityEngine;

#nullable enable

namespace UTIRLib.Initializer
{
    public abstract class MonoInitializer : MonoBehaviour
    {
        public void Launch() => StartCoroutine(IntializeCoroutine());

        protected abstract void IntiializeObjects();

        protected abstract bool IntiializePredicate();

        protected IEnumerator IntializeCoroutine()
        {
            yield return new WaitUntil(IntiializePredicate);
            IntiializeObjects();
            Destroy(this);
        }
    }
}