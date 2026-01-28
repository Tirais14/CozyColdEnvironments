using System;
using TMPro;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public class TMP_DropdownOptionDataLocalized : TMP_Dropdown.OptionData
    {
        [SerializeField]
        protected string localizedStringKey;

        public string LocalizedStringKey {
            get => localizedStringKey;
            set => localizedStringKey = value;
        }

        public TMP_DropdownOptionDataLocalized(string localizedStringKey)
            :
            base()
        {
            LocalizedStringKey = localizedStringKey;
        }

        public TMP_DropdownOptionDataLocalized(string text, string localizedStringKey)
            :
            base(text)
        {
            LocalizedStringKey = localizedStringKey;
        }

        public TMP_DropdownOptionDataLocalized(Sprite sprite, string localizedStringKey)
            :
            base(sprite)
        {
            LocalizedStringKey = localizedStringKey;
        }

        public TMP_DropdownOptionDataLocalized(TMP_Dropdown.OptionData optionData, string localizedStringKey)
            :
            base()
        {
            text = optionData.text;
            image = optionData.image;
            color = optionData.color;

            LocalizedStringKey = localizedStringKey;
        }
    }
}
