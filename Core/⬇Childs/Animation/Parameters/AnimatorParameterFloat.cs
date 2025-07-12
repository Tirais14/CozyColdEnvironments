using System;

namespace UTIRLib.Animation
{
    [Serializable]
    public class AnimatorParameterFloat : AnimatorParameter<float>
    {
        public AnimatorParameterFloat(string name) : base(name)
        { }

        public AnimatorParameterFloat(string name, float defaultValue) : base(name, defaultValue)
        { }

        public AnimatorParameterFloat(string name, Predicate<float> valueSetRulePredicate,
            float defaultValue = default) : base(name, defaultValue, valueSetRulePredicate)
        { }
    }
}