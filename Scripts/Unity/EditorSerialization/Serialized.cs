using CCEnvs.Diagnostics;
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
            CCDebug.PrintException(new ArgumentNullException(nameof(inputToOutput)));
            CCDebug.PrintException(new ArgumentNullException(nameof(outputToInput)));

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
            try
            {
                if (inputErased && Output is not null)
                    input = outputToInput(Output);

                OnBeforeSerialize();
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            try
            {
                Output = inputToOutput(input);

                OnAfterDeserialize();
#if !UNITY_EDITOR
                EditorSerializing.SetDefault(ref input!);
                inputErased = true;
#endif
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
