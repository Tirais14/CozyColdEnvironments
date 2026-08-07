using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public abstract class ContextMenuItem : IContextMenuItem
    {
        public abstract string Name { get; }

        public event Action? OnInvoke;

        public void Invoke()
        {
            InvokeCore();
            if (OnInvoke is not null)
            {
                try
                {
                    OnInvoke();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            if (CCDebug<ContextMenuItem>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Context menu item invoked")
                    .AddProperty("Item", this)
                    .ToStringAndDispose()
                    );
            }
        }

        protected abstract void InvokeCore();
    }
}
