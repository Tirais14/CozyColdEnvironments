using TMPro;
using UnityEngine;

#pragma warning disable IDE0044
#pragma warning disable S101
#nullable enable
namespace UTIRLib.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTextView : ATextView
    {
        [GetBySelf]
        private TextMeshProUGUI textComponent = null!;

        public override string Text {
            get => textComponent.text;
            set => textComponent.text = value;
        }
    }
}
