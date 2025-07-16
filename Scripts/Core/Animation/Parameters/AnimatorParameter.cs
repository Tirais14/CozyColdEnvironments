using System;
using UnityEngine;

namespace UTIRLib.Animation
{
    public abstract class AnimatorParameter
    {
        protected readonly string name;

        protected AnimatorParameter(string name) => this.name = name;
    }

    [Serializable]
    public class AnimatorParameter<T> : AnimatorParameter
    {
        protected readonly Predicate<T> valueSetRulePredicate;
        protected readonly T defaultValue;
        [SerializeField] protected T value;

        public T Value => value;

        public AnimatorParameter(string name) : base(name)
        { }

        public AnimatorParameter(string name, T defaultValue) : base(name) => this.defaultValue = defaultValue;

        public AnimatorParameter(string name, T defaultValue,
            Predicate<T> valueSetRulePredicate) : this(name, defaultValue)
        {
            this.valueSetRulePredicate = valueSetRulePredicate;

            if (RuleIsSetted() && !InRange(defaultValue))
            {
                throw new ArgumentException("The default value does not meet the requirements of the conditions for assigning a new value! Check predicate for wrong conditions!");
            }
        }

        /// <exception cref="ArgumentException"></exception>
        public void Set(T value)
        {
            if (RuleIsSetted() && !InRange(value))
            {
                throw new ArgumentException($"Argument: {value} outside of range.");
            }

            this.value = value;
        }

        public void Reset() => value = defaultValue;

        public bool InRange(T value) =>
            valueSetRulePredicate == null || valueSetRulePredicate.Method == null ||
            valueSetRulePredicate(value);

        private bool RuleIsSetted() =>
            valueSetRulePredicate != null && valueSetRulePredicate.Method != null;
    }
}