using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
#pragma warning disable S2346
namespace UTIRLib.Reflection
{
    public readonly struct InvokableArguments : IEquatable<InvokableArguments>
    {
        [Flags]
        public enum CreationSettings
        {
            Default,
            AllowSignatureTypesInheritance,
            CastTypesToItType = 2,
            CastArraysToElementType = 4,
        }

        public static InvokableArguments Empty => new(Array.Empty<object>());

        private readonly InvokableSignature signature;
        private readonly object?[] argumentValues;

        public InvokableSignature Signature => signature;
        public IReadOnlyList<object?> ArgumentValues => argumentValues;


        public InvokableArguments(InvokableSignature signature,
                                  object?[] argumentValues,
                                  bool allowSignatureTypesInheritance = false)
        {
            this.signature = signature;

            if (signature.Count != argumentValues.Length)
            {
                this.argumentValues = new object[signature.Count];

                argumentValues.CopyTo(this.argumentValues, 0);
            }
            else
                this.argumentValues = argumentValues;
        }

        public InvokableArguments(InvokableSignature signature)
            :
            this(signature, new object[signature.Count])
        {
        }

        public InvokableArguments(object[] argumentValues,
            CreationSettings creationSettings = CreationSettings.Default)
        {
            var castedArgs = new object[argumentValues.Length];
            for (int i = 0; i < argumentValues.Length; i++)
            {
                if (creationSettings.IsFlagSetted(CreationSettings.CastArraysToElementType)
                    &&
                    argumentValues[i] is Array arr
                    )
                    castedArgs[i] = ArrayHelper.CastToElementType(arr);
                else if (creationSettings.IsFlagSetted(CreationSettings.CastTypesToItType))
                    castedArgs[i] = Convert.ChangeType(argumentValues[i],
                                                       argumentValues[i].GetType());
                else
                    castedArgs[i] = argumentValues[i];
            }

            signature = InvokableSignature.Create(castedArgs,
                                                  creationSettings.IsFlagSetted(CreationSettings.AllowSignatureTypesInheritance));
            this.argumentValues = castedArgs;
        }

        public static InvokableArguments Create(object value,
            CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] { value };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] { 
                value0,
                value1 
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] {
                value0,
                value1,
                value2 
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] {
                value0,
                value1,
                value2,
                value3 
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] 
            {
                value0,
                value1,
                value2,
                value3,
                value4 
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] 
            {
                value0, 
                value1,
                value2,
                value3,
                value4,
                value5
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                object value6,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[] 
            {
                value0, 
                value1,
                value2,
                value3,
                value4,
                value5,
                value6 
            };

            return new InvokableArguments(args, creationSettings);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                object value6,
                                                object value7,
                                                CreationSettings creationSettings = CreationSettings.Default)
        {
            var args = new object[]
            {
                value0,
                value1,
                value2,
                value3,
                value4,
                value5,
                value6,
                value7
            };

            return new InvokableArguments(args, creationSettings);
        }

        public bool Equals(InvokableArguments other)
        {
            if (argumentValues.Length != other.argumentValues.Length)
                return false;

            if (signature != other.signature)
                return false;

            object?[] otherArgumentValues = other.argumentValues;
            for (int i = 0; i < argumentValues.Length; i++)
            {
                if (!Equals(argumentValues[i], otherArgumentValues[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is InvokableArguments other && Equals(other);
        }

        public override int GetHashCode()
        {
            var code = new HashCode();

            code.Add(signature.GetHashCode());
            for (int i = 0; i < argumentValues.Length; i++)
                code.Add(argumentValues[i]);

            return code.ToHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder(signature.ToString());

            sb.Append($". {nameof(InvokableArguments)}: ");
            if (argumentValues.IsEmpty())
                sb.Append("empty");
            else
                sb.AppendJoin(Environment.NewLine, argumentValues.Select(x => x?.ToString()));

            return sb.ToString();
        }

        public static bool operator ==(InvokableArguments left, InvokableArguments right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InvokableArguments left, InvokableArguments right)
        {
            return !left.Equals(right);
        }
    }
}
