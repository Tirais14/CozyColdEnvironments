using TMPro;

#pragma warning disable IDE0044
#pragma warning disable S101
#nullable enable
namespace UTIRLib.UI
{
    public class TMPTextView : TextView
    {
        [GetBySelf]
        private TextMeshProUGUI textComponent = null!;

        public override string Text {
            get => textComponent.text ?? string.Empty;
            set => textComponent.text = value ?? string.Empty;
        }
    }
}
