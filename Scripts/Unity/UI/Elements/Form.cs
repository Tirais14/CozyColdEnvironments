using CCEnvs.Rx;
using System;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    [Obsolete("In develop")]
    public class Form : Window
    {
        public override void Close()
        {
            OnClose.As<Observable>().Publish();
            enabled = false;
        }

        public override void Open()
        {
            enabled = true;
            OnOpen.As<Observable>().Publish();
        }
    }
}
