using System;
using UnityEngine;

namespace UTIRLib.Animation
{
    [Serializable]
    public class AnimatorParameterVector2 : AnimatorParameter<Vector2>
    {
        public AnimatorParameterVector2(string name) : base(name)
        { }

        public AnimatorParameterVector2(string name, Vector2 defaultValue) : base(name, defaultValue)
        { }

        public AnimatorParameterVector2(string name, Predicate<Vector2> valueSetRulePredicate,
            Vector2 defaultValue = default) : base(name, defaultValue, valueSetRulePredicate)
        { }
    }
}