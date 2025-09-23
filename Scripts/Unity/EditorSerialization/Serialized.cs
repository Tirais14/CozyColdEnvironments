using System;
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
        : ITransformable<TOutput>,
        ISerializationCallbackReceiver
    {
        private readonly Converter<TSerializable, TOutput> inputToOutput;
        private readonly Converter<TOutput, TSerializable> outputToInput;

        [SerializeField]
        private TSerializable input = default!;

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

        TOutput ITransformable<TOutput>.DoTransform() => Output;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (inputErased)
                input = outputToInput(Output);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Output = inputToOutput(input);

#if UNITY_EDITOR
            EditorSerializing.SetDefault(ref input!);
            inputErased = true;
#endif
        }
    }
}
