using System;

namespace UTIRLib.Animation
{
    [Serializable]
    public class AnimatorParameterFlag : AnimatorParameter<bool>
    {
        public AnimatorParameterFlag(string name) : base(name)
        { }

        public AnimatorParameterFlag(string name, bool defaultValue) : base(name, defaultValue)
        { }

        public AnimatorParameterFlag(string name, Predicate<bool> valueSetRulePredicate,
            bool defaultValue = default) : base(name, defaultValue, valueSetRulePredicate)
        { }

        public void Switch() => value = !value;

        /// <summary>Shortcut to turn on flag</summary>
        public void TurnOn() => Set(true);

        /// <summary>Shortcut to turn off flag</summary>
        public void TurnOff() => Set(false);
    }
}