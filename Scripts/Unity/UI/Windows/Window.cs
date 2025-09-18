#nullable enable
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public class Window : MonoCC, IOpenable
    {
        public bool IsOpened { get; private set; }

        public Rx.IObservable OnOpen { get; private set; } = new Rx.Observable();
        public Rx.IObservable OnClose { get; private set; } = new Rx.Observable();

        public virtual void Close()
        {
            OnClose.As<Rx.Observable>().Publish();
            gameObject.SetActive(false);
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            OnOpen.As<Rx.Observable>().Publish();
        }

        public bool SwitchOpenableState()
        {
            gameObject.SetActive(!IsOpened);

            return gameObject.activeSelf;
        }
    }
}
