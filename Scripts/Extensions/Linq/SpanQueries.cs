using System;
using System.Collections.Generic;
using static UTIRLib.SpanEnumerator;

#nullable enable
namespace UTIRLib.Linq
{
    public static class SpanQueries
    {
        public static int Count<T>(this Span<T> span, Predicate<T> match)
        {
            int spanLength = span.Length;
            int count = 0;
            for (int i = 0; i < spanLength; i++)
            {
                if (match(span[i]))
                    count++;
            }

            return count;
        }
        public static bool Any<T>(this Span<T> span) => span.Length > 0;

        /// <exception cref="ArgumentNullException"></exception>
        public static bool Any<T>(this Span<T> span, Predicate<T> match)
        {
            if (match is null)
                throw new ArgumentNullException(nameof(match));

            int spanLength = span.Length;
            for (int i = 0; i < spanLength; i++)
            {
                if (match(span[i]))
                    return true;
            }

            return false;
        }

        public static T? FirstOrDefault<T>(this Span<T> span)
        {
            if (!span.Any())
                return default;

            return span[0];
        }

        public static T? LastOrDefault<T>(this Span<T> span)
        {
            if (!span.Any())
                return default;

            return span[^1];
        }

        /// <exception cref="IndexOutOfRangeException"></exception>
        public static Span<T> Take<T>(this Span<T> span, int count)
        {
            if (count > span.Length)
                throw new IndexOutOfRangeException();

            return span[..count];
        }

        public static Span<T> TakeWhile<T>(this Span<T> span, Predicate<T> match)
        {
            int spanLength = span.Length;
            int count = 0;
            foreach (var item in span)
            {
                if (match(item))
                    count++;
                else
                    break;
            }

            if (count > 0)
                return span[..(count - 1)];
            else
                return default;
        }

        public static HashSet<T> ToHashSet<T>(this Span<T> span)
        {
            var hashSet = new HashSet<T>(span.Length);
            int spanLength = span.Length;
            for (int i = 0; i < spanLength; i++)
                hashSet.Add(span[i]);

            return hashSet;
        }

        public static List<T> ToList<T>(this Span<T> span)
        {
            var list = new List<T>(span.Length);
            int spanLength = span.Length;
            for (int i = 0; i < spanLength; i++)
                list.Add(span[i]);

            return list;
        }
    }
}

namespace UTIRLib.Linq.Extended
{
    public static class SpanQueries
    {
        #region Select
        public static Select<T, TOut> Select<T, TOut>(this Span<T> span, Func<T, TOut> selector)
        {
            return new Select<T, TOut>(span, selector);
        }
        public static SelectSelect<T, TOut, TNewOut> Select<T, TOut, TNewOut>(
            this Select<T, TOut> source, Func<TOut, TNewOut> selector)
        {
            return new SelectSelect<T, TOut, TNewOut>(source, selector);
        }

        public static WhereSelect<T, TOut> Select<T, TOut>(
            this Where<T> source, Func<T, TOut> selector)
        {
            return new WhereSelect<T, TOut>(source, selector);
        }

        public static SelectSelectSelect<T0, T1, T2, TNewOut> Select<T0, T1, T2, TNewOut>(
            this SelectSelect<T0, T1, T2> source, Func<T2, TNewOut> selector)
        {
            return new SelectSelectSelect<T0, T1, T2, TNewOut>(source, selector);
        }

        public static WhereSelectSelect<T, TMid, TNewOut> Select<T, TMid, TNewOut>(
            this WhereSelect<T, TMid> source, Func<TMid, TNewOut> selector)
        {
            return new WhereSelectSelect<T, TMid, TNewOut>(source, selector);
        }

        public static WhereWhereSelect<T, TOut> Select<T, TOut>(
            this WhereWhere<T> source, Func<T, TOut> selector)
        {
            return new WhereWhereSelect<T, TOut>(source, selector);
        }

        public static SelectWhereSelect<T, TMid, TOut> Select<T, TMid, TOut>(
            this SelectWhere<T, TMid> source, Func<TMid, TOut> selector)
        {
            return new SelectWhereSelect<T, TMid, TOut>(source, selector);
        }

        public static SelectSelectSelectSelect<T0, T1, T2, T3, TNewOut> Select<T0, T1, T2, T3, TNewOut>(
            this SelectSelectSelect<T0, T1, T2, T3> source, Func<T3, TNewOut> selector)
        {
            return new SelectSelectSelectSelect<T0, T1, T2, T3, TNewOut>(source, selector);
        }

        public static SelectWhereSelectSelect<T0, T1, T2, TNewOut> Select<T0, T1, T2, TNewOut>(
            this SelectWhereSelect<T0, T1, T2> source, Func<T2, TNewOut> selector)
        {
            return new SelectWhereSelectSelect<T0, T1, T2, TNewOut>(source, selector);
        }

        public static WhereWhereSelectSelect<T, TMid, TNewOut> Select<T, TMid, TNewOut>(
            this WhereWhereSelect<T, TMid> source, Func<TMid, TNewOut> selector)
        {
            return new WhereWhereSelectSelect<T, TMid, TNewOut>(source, selector);
        }

        public static SelectWhereWhereSelect<T0, T1, TNewOut> Select<T0, T1, TNewOut>(
            this SelectWhereWhere<T0, T1> source, Func<T1, TNewOut> selector)
        {
            return new SelectWhereWhereSelect<T0, T1, TNewOut>(source, selector);
        }

        public static WhereSelectSelectSelect<T0, T1, T2, TNewOut> Select<T0, T1, T2, TNewOut>(
            this WhereSelectSelect<T0, T1, T2> source, Func<T2, TNewOut> selector)
        {
            return new WhereSelectSelectSelect<T0, T1, T2, TNewOut>(source, selector);
        }

        public static WhereWhereWhereSelect<T, TOut> Select<T, TOut>(
            this WhereWhereWhere<T> source, Func<T, TOut> selector)
        {
            return new WhereWhereWhereSelect<T, TOut>(source, selector);
        }

        public static SelectSelectWhereSelect<T0, T1, T2, TOut> Select<T0, T1, T2, TOut>(
            this SelectSelectWhere<T0, T1, T2> source, Func<T2, TOut> selector)
        {
            return new SelectSelectWhereSelect<T0, T1, T2, TOut>(source, selector);
        }

        public static WhereSelectWhereSelect<T0, T1, TOut> Select<T0, T1, TOut>(
            this WhereSelectWhere<T0, T1> source, Func<T1, TOut> selector)
        {
            return new WhereSelectWhereSelect<T0, T1, TOut>(source, selector);
        }
        #endregion Select

        #region Where
        public static Where<T> Where<T>(this Span<T> span, Predicate<T> match)
        {
            return new Where<T>(span, match);
        }

        public static SelectWhere<T, TOut> Where<T, TOut>(
            this Select<T, TOut> source, Predicate<TOut> match)
        {
            return new SelectWhere<T, TOut>(source, match);
        }

        public static WhereWhere<T> Where<T>(
            this Where<T> source, Predicate<T> match)
        {
            return new WhereWhere<T>(source, match);
        }

        public static SelectSelectWhere<T0, T1, T2> Where<T0, T1, T2>(
            this SelectSelect<T0, T1, T2> source, Predicate<T2> match)
        {
            return new SelectSelectWhere<T0, T1, T2>(source, match);
        }

        public static WhereSelectWhere<T, TMid> Where<T, TMid>(
            this WhereSelect<T, TMid> source, Predicate<TMid> match)
        {
            return new WhereSelectWhere<T, TMid>(source, match);
        }

        public static WhereWhereWhere<T> Where<T>(
            this WhereWhere<T> source, Predicate<T> match)
        {
            return new WhereWhereWhere<T>(source, match);
        }

        public static SelectWhereWhere<T, TMid> Where<T, TMid>(
            this SelectWhere<T, TMid> source, Predicate<TMid> match)
        {
            return new SelectWhereWhere<T, TMid>(source, match);
        }

        public static WhereSelectWhereWhere<T0, T1> Where<T0, T1>(
            this WhereSelectWhere<T0, T1> source, Predicate<T1> match)
        {
            return new WhereSelectWhereWhere<T0, T1>(source, match);
        }

        public static WhereWhereWhereWhere<T> Where<T>(
            this WhereWhereWhere<T> source, Predicate<T> match)
        {
            return new WhereWhereWhereWhere<T>(source, match);
        }

        public static SelectSelectSelectWhere<T0, T1, T2, T3> Where<T0, T1, T2, T3>(
            this SelectSelectSelect<T0, T1, T2, T3> source, Predicate<T3> match)
        {
            return new SelectSelectSelectWhere<T0, T1, T2, T3>(source, match);
        }

        public static SelectSelectWhereWhere<T0, T1, T2> Where<T0, T1, T2>(
            this SelectSelectWhere<T0, T1, T2> source, Predicate<T2> match)
        {
            return new SelectSelectWhereWhere<T0, T1, T2>(source, match);
        }

        public static WhereWhereSelectWhere<T0, T1> Where<T0, T1>(
            this WhereWhereSelect<T0, T1> source, Predicate<T1> match)
        {
            return new WhereWhereSelectWhere<T0, T1>(source, match);
        }

        public static WhereSelectSelectWhere<T0, T1, T2> Where<T0, T1, T2>(
            this WhereSelectSelect<T0, T1, T2> source, Predicate<T2> match)
        {
            return new WhereSelectSelectWhere<T0, T1, T2>(source, match);
        }

        public static SelectWhereWhereWhere<T0, T1> Where<T0, T1>(
            this SelectWhereWhere<T0, T1> source, Predicate<T1> match)
        {
            return new SelectWhereWhereWhere<T0, T1>(source, match);
        }

        public static SelectWhereSelectWhere<T0, T1, T2> Where<T0, T1, T2>(
            this SelectWhereSelect<T0, T1, T2> source, Predicate<T2> match)
        {
            return new SelectWhereSelectWhere<T0, T1, T2>(source, match);
        }
        #endregion Where

        #region HashSet
        #region Select
        public static HashSet<TOut> ToHashSet<T, TOut>(this Select<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);

            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this SelectSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, T2, TOut>(this SelectSelectSelect<T0, T1, T2, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, T2, T3, TOut>(this SelectSelectSelectSelect<T0, T1, T2, T3, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T, TOut>(this WhereSelect<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this SelectWhereSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T, TOut>(this WhereWhereSelect<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this WhereSelectSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this SelectWhereWhereSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, T2, TOut>(this SelectWhereSelectSelect<T0, T1, T2, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, T2, TOut>(this WhereSelectSelectSelect<T0, T1, T2, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this WhereWhereSelectSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T, TOut>(this WhereWhereWhereSelect<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, T2, TOut>(this SelectSelectWhereSelect<T0, T1, T2, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T0, T1, TOut>(this WhereSelectWhereSelect<T0, T1, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }
        #endregion Select

        #region Where
        public static HashSet<T> ToHashSet<T>(this Where<T> source)
        {
            var hashSet = new HashSet<T>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);

            return hashSet;
        }
        public static HashSet<T> ToHashSet<T>(this WhereWhere<T> source)
        {
            var hashSet = new HashSet<T>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }
        public static HashSet<TOut> ToHashSet<T, TOut>(this SelectWhere<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }
        public static HashSet<T> ToHashSet<T>(this WhereWhereWhere<T> source)
        {
            var hashSet = new HashSet<T>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<TOut> ToHashSet<T, TOut>(this SelectWhereWhere<T, TOut> source)
        {
            var hashSet = new HashSet<TOut>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T2> ToHashSet<T0, T1, T2>(this SelectSelectWhere<T0, T1, T2> source)
        {
            var hashSet = new HashSet<T2>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T1> ToHashSet<T0, T1>(this WhereSelectWhere<T0, T1> source)
        {
            var hashSet = new HashSet<T1>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T> ToHashSet<T>(this WhereWhereWhereWhere<T> source)
        {
            var hashSet = new HashSet<T>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T2> ToHashSet<T0, T1, T2>(this SelectWhereSelectWhere<T0, T1, T2> source)
        {
            var hashSet = new HashSet<T2>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T1> ToHashSet<T0, T1>(this SelectWhereWhereWhere<T0, T1> source)
        {
            var hashSet = new HashSet<T1>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T1> ToHashSet<T0, T1>(this WhereSelectWhereWhere<T0, T1> source)
        {
            var hashSet = new HashSet<T1>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T1> ToHashSet<T0, T1>(this WhereWhereSelectWhere<T0, T1> source)
        {
            var hashSet = new HashSet<T1>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T3> ToHashSet<T0, T1, T2, T3>(this SelectSelectSelectWhere<T0, T1, T2, T3> source)
        {
            var hashSet = new HashSet<T3>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T2> ToHashSet<T0, T1, T2>(this SelectSelectWhereWhere<T0, T1, T2> source)
        {
            var hashSet = new HashSet<T2>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }

        public static HashSet<T2> ToHashSet<T0, T1, T2>(this WhereSelectSelectWhere<T0, T1, T2> source)
        {
            var hashSet = new HashSet<T2>(source.SourceLength);
            while (source.MoveNext())
                hashSet.Add(source.Current);
            return hashSet;
        }
        #endregion Where
        #endregion HashSet

        #region List
        #region Select
        public static List<TOut> ToList<T, TOut>(this Select<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);

            return list;
        }
        public static List<TOut> ToList<T0, T1, TOut>(this SelectSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }
        public static List<TOut> ToList<T0, T1, T2, TOut>(this SelectSelectSelect<T0, T1, T2, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, T2, T3, TOut>(this SelectSelectSelectSelect<T0, T1, T2, T3, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T, TOut>(this WhereSelect<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, TOut>(this SelectWhereSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T, TOut>(this WhereWhereSelect<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, TOut>(this WhereSelectSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, TOut>(this SelectWhereWhereSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, T2, TOut>(this SelectWhereSelectSelect<T0, T1, T2, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, T2, TOut>(this WhereSelectSelectSelect<T0, T1, T2, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, TOut>(this WhereWhereSelectSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T, TOut>(this WhereWhereWhereSelect<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, T2, TOut>(
            this SelectSelectWhereSelect<T0, T1, T2, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T0, T1, TOut>(
            this WhereSelectWhereSelect<T0, T1, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }
        #endregion Select

        #region Where
        public static List<T> ToList<T>(this Where<T> source)
        {
            var list = new List<T>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);

            return list;
        }
        public static List<T> ToList<T>(this WhereWhere<T> source)
        {
            var list = new List<T>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }
        public static List<TOut> ToList<T, TOut>(this SelectWhere<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }
        public static List<T> ToList<T>(this WhereWhereWhere<T> source)
        {
            var list = new List<T>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<TOut> ToList<T, TOut>(this SelectWhereWhere<T, TOut> source)
        {
            var list = new List<TOut>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T2> ToList<T0, T1, T2>(this SelectSelectWhere<T0, T1, T2> source)
        {
            var list = new List<T2>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T1> ToList<T0, T1>(this WhereSelectWhere<T0, T1> source)
        {
            var list = new List<T1>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T> ToList<T>(this WhereWhereWhereWhere<T> source)
        {
            var list = new List<T>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T2> ToList<T0, T1, T2>(this SelectWhereSelectWhere<T0, T1, T2> source)
        {
            var list = new List<T2>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T1> ToList<T0, T1>(this SelectWhereWhereWhere<T0, T1> source)
        {
            var list = new List<T1>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T1> ToList<T0, T1>(this WhereSelectWhereWhere<T0, T1> source)
        {
            var list = new List<T1>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T1> ToList<T0, T1>(this WhereWhereSelectWhere<T0, T1> source)
        {
            var list = new List<T1>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T3> ToList<T0, T1, T2, T3>(this SelectSelectSelectWhere<T0, T1, T2, T3> source)
        {
            var list = new List<T3>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T2> ToList<T0, T1, T2>(this SelectSelectWhereWhere<T0, T1, T2> source)
        {
            var list = new List<T2>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }

        public static List<T2> ToList<T0, T1, T2>(this WhereSelectSelectWhere<T0, T1, T2> source)
        {
            var list = new List<T2>(source.SourceLength);
            while (source.MoveNext())
                list.Add(source.Current);
            return list;
        }
        #endregion Where
        #endregion List

        #region Array
        #region Select
        public static TOut[] ToArray<T, TOut>(this Select<T, TOut> source)
        {
            var array = new TOut[source.SourceLength];
            int i = 0;
            while (source.MoveNext())
                array[i++] = source.Current;

            return array;
        }
        public static TOut[] ToArray<T0, T1, TOut>(this SelectSelect<T0, T1, TOut> source)
        {
            var array = new TOut[source.SourceLength];
            int i = 0;
            while (source.MoveNext())
                array[i++] = source.Current;
            return array;
        }

        public static TOut[] ToArray<T, TOut>(this WhereSelect<T, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, TOut>(this SelectWhereSelect<T0, T1, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T, TOut>(this WhereWhereSelect<T, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, TOut>(this WhereSelectSelect<T0, T1, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, T2, TOut>(this SelectSelectSelect<T0, T1, T2, TOut> source)
        {
            var array = new TOut[source.SourceLength];
            int i = 0;
            while (source.MoveNext())
                array[i++] = source.Current;
            return array;
        }

        public static TOut[] ToArray<T0, T1, TOut>(this SelectWhereWhereSelect<T0, T1, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, T2, TOut>(this SelectWhereSelectSelect<T0, T1, T2, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, T2, TOut>(this WhereSelectSelectSelect<T0, T1, T2, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, TOut>(this WhereWhereSelectSelect<T0, T1, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T, TOut>(this WhereWhereWhereSelect<T, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, T2, TOut>(this SelectSelectWhereSelect<T0, T1, T2, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, TOut>(this WhereSelectWhereSelect<T0, T1, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T0, T1, T2, T3, TOut>(this SelectSelectSelectSelect<T0, T1, T2, T3, TOut> source)
        {
            var array = new TOut[source.SourceLength];
            int i = 0;
            while (source.MoveNext())
                array[i++] = source.Current;
            return array;
        }
        #endregion Select

        #region Where
        public static T[] ToArray<T>(this Where<T> source)
        {
            return source.ToList().ToArray();
        }
        public static T[] ToArray<T>(this WhereWhere<T> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T, TOut>(this SelectWhere<T, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static T[] ToArray<T>(this WhereWhereWhere<T> source)
        {
            return source.ToList().ToArray();
        }

        public static TOut[] ToArray<T, TOut>(this SelectWhereWhere<T, TOut> source)
        {
            return source.ToList().ToArray();
        }

        public static T2[] ToArray<T0, T1, T2>(this SelectSelectWhere<T0, T1, T2> source)
        {
            return source.ToList().ToArray();
        }

        public static T1[] ToArray<T0, T1>(this WhereSelectWhere<T0, T1> source)
        {
            return source.ToList().ToArray();
        }

        public static T[] ToArray<T>(this WhereWhereWhereWhere<T> source)
        {
            return source.ToList().ToArray();
        }

        public static T2[] ToArray<T0, T1, T2>(this SelectWhereSelectWhere<T0, T1, T2> source)
        {
            return source.ToList().ToArray();
        }

        public static T1[] ToArray<T0, T1>(this SelectWhereWhereWhere<T0, T1> source)
        {
            return source.ToList().ToArray();
        }

        public static T1[] ToArray<T0, T1>(this WhereSelectWhereWhere<T0, T1> source)
        {
            return source.ToList().ToArray();
        }

        public static T1[] ToArray<T0, T1>(this WhereWhereSelectWhere<T0, T1> source)
        {
            return source.ToList().ToArray();
        }

        public static T3[] ToArray<T0, T1, T2, T3>(this SelectSelectSelectWhere<T0, T1, T2, T3> source)
        {
            return source.ToList().ToArray();
        }

        public static T2[] ToArray<T0, T1, T2>(this SelectSelectWhereWhere<T0, T1, T2> source)
        {
            return source.ToList().ToArray();
        }

        public static T2[] ToArray<T0, T1, T2>(this WhereSelectSelectWhere<T0, T1, T2> source)
        {
            return source.ToList().ToArray();
        }

        #endregion Where
        #endregion Array
    }
}
