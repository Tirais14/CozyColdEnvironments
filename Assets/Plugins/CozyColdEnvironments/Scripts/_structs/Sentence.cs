using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using ZLinq;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs
{
    public struct Sentence : IEquatable<Sentence>
    {
        public const string CONTINUATION_SIGN = "...";
        public static Sentence Empty => new();

        private readonly int capacity;
        private List<(Func<string?> valueFactory, Predicate<string?>? condition)>? parts;

        private List<(Func<string?> valueFactory, Predicate<string?>? condition)> Parts {
            get
            {
                parts ??= new List<(Func<string?> partFactory, Predicate<string?>? condition)>(
                    capacity <= 0 ? 4 : capacity);

                return parts;
            }
        }

        public Sentence(int capacity)
            : 
            this()
        {
            this.capacity = capacity;
        }

        public static bool operator ==(Sentence left, Sentence right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Sentence left, Sentence right)
        {
            return !(left == right);
        }

        private static string PartToString(object? part)
        {
            return part?.ToString() ?? string.Empty;
        }

        public Sentence Add(object? part)
        {
            Parts.Add((() => PartToString(part), null));

            return this;
        }
        public Sentence Add<T>(T part) where T : struct
        {
            return Add(part.ToString());
        }
        public Sentence Add(Func<string?> partFactory)
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));

            Parts.Add((partFactory, null));

            return this;
        }
        public Sentence Add(object? part, bool condition)
        {
            Parts.Add((() => PartToString(part), _ => condition));

            return this;
        }
        public Sentence Add<T>(T obj, bool condition) where T : struct
        {
            return Add(obj.ToString(), condition);
        }
        public Sentence Add(object? part, Predicate<string?> condition)
        {
            Guard.IsNotNull(condition, nameof(condition));

            Parts.Add((() => PartToString(part), condition));

            return this;
        }
        public Sentence Add<T>(T part, Predicate<string?> condition) where T : struct
        {
            return Add(part.ToString(), condition);
        }
        public Sentence Add(Func<string?> partFactory, Predicate<string?>? condition)
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));
            Guard.IsNotNull(condition, nameof(condition));

            Parts.Add((partFactory, condition));

            return this;
        }

        public Sentence AddIfNotDefault(object? part)
        {
            Parts.Add((() => PartToString(part), _ => part.IsNotNull()));

            return this;
        }
        public Sentence AddIfNotDefault(object? part, object? condition)
        {
            Parts.Add((() => PartToString(part), _ => condition.IsNotNull()));

            return this;
        }
        public Sentence AddIfNotDefault<T>(object? part, T? condition)
            where T : struct
        {
            Parts.Add((() => PartToString(part), _ => condition.IsNotDefault()));

            return this;
        }
        public Sentence AddIfNotDefault(Func<string> partFactory, object? condition)
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));

            Parts.Add((partFactory, _ => condition.IsNotNull()));

            return this;
        }
        public Sentence AddIfNotDefault<T>(Func<string> partFactory, T? condition)
            where T : struct
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));

            Parts.Add((partFactory, _ => condition.IsNotDefault()));

            return this;
        }
        public Sentence AddIfNotDefault<T>(T part)
            where T : struct
        {
            Parts.Add((() => part.ToString(), _ => part.IsNotDefault()));

            return this;
        }
        public Sentence AddIfNotDefault<T>(T part, object? condition)
            where T : struct
        {
            Parts.Add((() => part.ToString(), _ => condition.IsNotNull()));

            return this;
        }
        public Sentence AddIfNotDefault<T, TCondition>(T part, TCondition condition)
            where T : struct
        {
            Parts.Add((() => part.ToString(), _ => condition.IsNotDefault()));

            return this;
        }

        public readonly Sentence RemoveAt(int index)
        {
            if (parts.IsNullOrEmpty() || index >= parts.Count)
                return this;

            parts.RemoveAt(index);

            return this;
        }

        public readonly Sentence RemoveLast()
        {
            return RemoveAt((parts?.Count ?? 0) - 1);
        }

        public readonly Sentence RemoveFirst()
        {
            return RemoveAt(0);
        }

        public readonly Sentence Clear()
        {
            if (parts.IsNullOrEmpty())
                return this;

            parts.Clear();

            return this;
        }

        public readonly bool Equals(Sentence other)
        {
            return parts == other.parts;
        }
        public readonly override bool Equals(object obj)
        {
            return obj is Sentence typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(parts);
        }

        public readonly override string ToString()
        {
            if (parts.IsNullOrEmpty())
                return string.Empty;

            var sb = ZString.CreateStringBuilder();
            string? value;
            bool isContinuation;
            bool isLast;
            int i = 0;
            foreach (var (valueFactory, condition) in parts)
            {
                value = valueFactory();
                i++;

                try
                {
                    if ((condition?.Invoke(value) ?? true) && value.IsNotNullOrEmpty())
                    {
                        isContinuation = value.EndsWith(CONTINUATION_SIGN);
                        isLast = i == parts.Count - 1;

                        sb.Append(value);

                        if (isLast)
                        {
                            if (isContinuation)
                                sb.Append(value.ZL().Take(value.Length - CONTINUATION_SIGN.Length));

                            sb.Append('.');
                        }
                        else
                        {
                            if (!isContinuation)
                                sb.Append(". ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            return sb.ToString();
        }
    }
}
