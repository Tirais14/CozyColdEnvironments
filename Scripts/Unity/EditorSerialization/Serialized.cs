using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    /// <summary>
    /// Anonymous serialized type with intermediate value as serializable.
    /// Erase in runtime <see cref="input"/> value and restores it before serializing from <see cref="Output"/>
    /// </summary>
    [Serializable]
    public class Serialized<TSerializable, TOutput> 
        : IEditorSerialized<TOutput>,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        protected TSerializable input = default!;

        private readonly Converter<TSerializable, TOutput> inputToOutput;
        private readonly Converter<TOutput, TSerializable> outputToInput;

        private bool inputErased;

        public TOutput Output { get; private set; } = default!;

        public Serialized(Converter<TSerializable, TOutput> inputToOutput,
            Converter<TOutput, TSerializable> outputToInput)
        {
            CC.Validate.ArgumentNull(inputToOutput, nameof(inputToOutput));
            CC.Validate.ArgumentNull(outputToInput, nameof(outputToInput));

            this.inputToOutput = inputToOutput;
            this.outputToInput = outputToInput;
        }

        public static implicit operator TOutput(Serialized<TSerializable, TOutput> source)
        {
            return source.Output;
        }

        public override string ToString()
        {
            return $"{nameof(input)}: {input} |{nameof(Output)}: {Output}";
        }

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (inputErased)
                input = outputToInput(Output);

            OnBeforeSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Output = inputToOutput(input);

            OnAfterDeserialize();
#if !UNITY_EDITOR
            EditorSerializing.SetDefault(ref input!);
            inputErased = true;
#endif
        }
    }
}
