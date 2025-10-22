using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using Cysharp.Text;
using System;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs
{
    public sealed class Sentence
    {
        public const string CONTINUATION_SIGN = "...";

        public static Sentence Empty => new();

        private readonly int capacity;
        private List<(Func<string?> valueFactory, Predicate<string?>? condition)>? parts;

        public int Count => parts?.Count ?? 0;

        private List<(Func<string?> valueFactory, Predicate<string?>? condition)> Parts {
            get
            {
                parts ??= new List<(Func<string?> partFactory, Predicate<string?>? condition)>(
                    capacity <= 0 ? 4 : capacity);

                return parts;
            }
        }

        public Sentence()
        {
        }

        public Sentence(int capacity)
        {
            this.capacity = capacity;
        }

        private static string PartToString<T>(T? part)
        {
            return part?.ToString() ?? string.Empty;
        }

        public Sentence Add(Func<string?> partFactory, Predicate<string?>? condition = null)
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));

            Parts.Add((partFactory, condition));

            return this;
        }
        public Sentence Add<TPart>(TPart? part, Predicate<string?>? condition = null)
        {
            return Add(() => PartToString(part), condition);
        }
        public Sentence Add<TPart>(TPart? part, bool condition)
        {
            return Add(part, condition ? null : static _ => false);
        }
        public Sentence Add<TPart, TCondition>(TPart? part, TCondition? condition)
        {
            return Add(part, condition.IsNull() ? null : static _ => false);
        }

        public Sentence AddIfNotDefault<TCondition>(Func<string> partFactory,
                                                    TCondition? condition)
        {
            Guard.IsNotNull(partFactory, nameof(partFactory));

            if (condition.IsNotDefault())
                Parts.Add((partFactory, null));

            return this;
        }
        public Sentence AddIfNotDefault<TPart, TCondition>(TPart? part,
                                                           TCondition? condition)
        {
            Parts.Add((() => PartToString(part), _ => condition.IsNotDefault()));

            return this;
        }
        public Sentence AddIfNotDefault<TPart>(TPart? part)
        {
            Parts.Add((() => PartToString(part), _ => part.IsNotDefault()));

            return this;
        }

        public Sentence Continue()
        {
            if (parts.IsNullOrEmpty())
                return this;

            var (valueFactory, condition) = parts[^1];

            parts[^1] = (() => valueFactory() + CONTINUATION_SIGN, condition);

            return this;
        }

        public Sentence RemoveAt(int index)
        {
            if (parts.IsNullOrEmpty() || index >= parts.Count)
                return this;

            parts.RemoveAt(index);

            return this;
        }

        public Sentence RemoveLast()
        {
            return RemoveAt((parts?.Count ?? 0) - 1);
        }

        public Sentence RemoveFirst()
        {
            return RemoveAt(0);
        }

        public Sentence Clear()
        {
            if (parts.IsNullOrEmpty())
                return this;

            parts.Clear();

            return this;
        }

        public override string ToString()
        {
            if (parts.IsNullOrEmpty())
                return string.Empty;

            using var sb = ZString.CreateStringBuilder();
            string? value;
            bool isContinuation;
            bool isLast;
            int i = 0;
            foreach (var (valueFactory, condition) in parts)
            {
                value = valueFactory();
                isLast = i == parts.Count - 1;

                try
                {
                    if ((condition?.Invoke(value) ?? true) && value.IsNotNullOrWhiteSpace())
                    {
                        if (value == CONTINUATION_SIGN)
                        {
                            if (isLast)
                                sb.Append('.');
                            else
                                sb.Append(' ');
                        }
                        else
                        {

                            isContinuation = value.EndsWith(CONTINUATION_SIGN);

                            if (isContinuation)
                            {
                                sb.Append(value.Delete(CONTINUATION_SIGN));
                                sb.Append(' ');
                            }
                            else
                            {
                                sb.Append(value);
                                sb.Append(". ");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                i++;
            }

            return sb.ToString();
        }
    }
}
