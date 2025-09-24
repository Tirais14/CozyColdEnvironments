using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public abstract class Serialized<TSerializable, TOutput> 
        : IEditorSerialized<TOutput>,
        ITransformable<TOutput>,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        protected TSerializable input = default!;

        //private readonly Converter<TSerializable, TOutput> inputToOutput;
        //private readonly Converter<TOutput, TSerializable> outputToInput;

        private bool inputErased;

        public TOutput Output { [Converter] get; private set; } = default!;

        public static implicit operator TOutput(Serialized<TSerializable, TOutput> source)
        {
            return source.Output;
        }

        public override string ToString()
        {
            return $"{nameof(input)}: {input} |{nameof(Output)}: {Output}";
        }

        protected abstract TOutput GetOutput();

        protected abstract TSerializable GetInput();

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }

        TOutput ITransformable<TOutput>.DoTransform() => Output;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            try
            {
                if (inputErased && Output is not null)
                    input = GetInput();

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
                Output = GetOutput();

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
