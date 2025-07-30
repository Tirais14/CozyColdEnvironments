using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public readonly struct InvokableArguments : IEquatable<InvokableArguments>
    {
        public static InvokableArguments Empty => new(Array.Empty<object>());

        private readonly InvokableSignature signature;
        private readonly object?[] argumentValues;

        public InvokableSignature Signature => signature;
        public IReadOnlyList<object?> ArgumentValues => argumentValues;


        public InvokableArguments(InvokableSignature signature, object?[] argumentValues)
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
                                  bool allowTypeInheritance = false)
            :
            this(new InvokableSignature(argumentValues.Select(x =>
            {
                if (x is null)
                    throw new CollectionArgumentException();

                return x.GetType();
            }),
                allowTypeInheritance),
                    argumentValues)
        {
        }

        public static InvokableArguments Create(object value,
                                                bool allowTypeInheritance = false)
        {
            var args = new object[] { value };

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                bool allowTypeInheritance = false)
        {
            var args = new object[] { 
                value0,
                value1 
            };

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                bool allowTypeInheritance = false)
        {
            var args = new object[] {
                value0,
                value1,
                value2 
            };

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                bool allowTypeInheritance = false)
        {
            var args = new object[] {
                value0,
                value1,
                value2,
                value3 
            };

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                bool allowTypeInheritance = false)
        {
            var args = new object[] 
            {
                value0,
                value1,
                value2,
                value3,
                value4 
            };

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                bool allowTypeInheritance = false)
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

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                object value6,
                                                bool allowTypeInheritance = false)
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

            return new InvokableArguments(args, allowTypeInheritance);
        }
        public static InvokableArguments Create(object value0,
                                                object value1,
                                                object value2,
                                                object value3,
                                                object value4,
                                                object value5,
                                                object value6,
                                                object value7,
                                                bool allowTypeInheritance = false)
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

            return new InvokableArguments(args, allowTypeInheritance);
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
                sb.AppendJoin(", ", signature.Select(x => x.GetName()));

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
