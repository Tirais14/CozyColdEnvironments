#nullable enable
using System;

namespace UTIRLib
{
    public readonly ref struct SpanEnumerator
    {
        #region Select
        public unsafe readonly ref struct Select<T, TOut>
        {
            private readonly Span<T>.Enumerator sourceEnumerator;
            private readonly Func<T, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(sourceEnumerator.Current);

            public Select(Span<T> source, Func<T, TOut> selector)
            {
                this.selector = selector;
                sourceEnumerator = source.GetEnumerator();

                SourceLength = source.Length;
            }

            public bool MoveNext() => sourceEnumerator.MoveNext();
        }

        #region 2
        public readonly ref struct SelectSelect<T0, T1, TOut>
        {
            private readonly Select<T0, T1> source;
            private readonly Func<T1, TOut> secondSelector;

            public readonly int SourceLength;

            public readonly TOut Current => secondSelector(source.Current);

            public SelectSelect(Select<T0, T1> source, Func<T1, TOut> secondSelector)
            {
                this.source = source;
                this.secondSelector = secondSelector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereSelect<T, TOut>
        {
            private readonly Where<T> source;
            private readonly Func<T, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereSelect(Where<T> source, Func<T, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }
        #endregion 2

        #region 3
        public readonly ref struct SelectSelectSelect<T0, T1, T2, TOut>
        {
            private readonly SelectSelect<T0, T1, T2> source;
            private readonly Func<T2, TOut> thirdSelector;

            public readonly int SourceLength;

            public readonly TOut Current => thirdSelector(source.Current);

            public SelectSelectSelect(SelectSelect<T0, T1, T2> source,
                                      Func<T2, TOut> thirdSelector)
            {
                this.source = source;
                this.thirdSelector = thirdSelector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct SelectWhereSelect<T0, T1, TOut>
        {
            private readonly SelectWhere<T0, T1> source;
            private readonly Func<T1, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public SelectWhereSelect(SelectWhere<T0, T1> source, Func<T1, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereWhereSelect<T, TOut>
        {
            private readonly WhereWhere<T> source;
            private readonly Func<T, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereWhereSelect(WhereWhere<T> source, Func<T, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereSelectSelect<T0, T1, TOut>
        {
            private readonly WhereSelect<T0, T1> source;
            private readonly Func<T1, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereSelectSelect(WhereSelect<T0, T1> source, Func<T1, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }
        #endregion 3

        #region 4
        public readonly ref struct SelectWhereWhereSelect<T0, T1, TOut>
        {
            private readonly SelectWhereWhere<T0, T1> source;
            private readonly Func<T1, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public SelectWhereWhereSelect(SelectWhereWhere<T0, T1> source,
                                          Func<T1, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct SelectWhereSelectSelect<T0, T1, T2, TOut>
        {
            private readonly SelectWhereSelect<T0, T1, T2> source;
            private readonly Func<T2, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public SelectWhereSelectSelect(SelectWhereSelect<T0, T1, T2> source,
                                           Func<T2, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereSelectSelectSelect<T0, T1, T2, TOut>
        {
            private readonly WhereSelectSelect<T0, T1, T2> source;
            private readonly Func<T2, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereSelectSelectSelect(WhereSelectSelect<T0, T1, T2> source,
                                           Func<T2, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereWhereSelectSelect<T0, T1, TOut>
        {
            private readonly WhereWhereSelect<T0, T1> source;
            private readonly Func<T1, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereWhereSelectSelect(WhereWhereSelect<T0, T1> source,
                                          Func<T1, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereWhereWhereSelect<T, TOut>
        {
            private readonly WhereWhereWhere<T> source;
            private readonly Func<T, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereWhereWhereSelect(WhereWhereWhere<T> source, Func<T, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct SelectSelectWhereSelect<T0, T1, T2, TOut>
        {
            private readonly SelectSelectWhere<T0, T1, T2> source;
            private readonly Func<T2, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public SelectSelectWhereSelect(SelectSelectWhere<T0, T1, T2> source,
                                           Func<T2, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct WhereSelectWhereSelect<T0, T1, TOut>
        {
            private readonly WhereSelectWhere<T0, T1> source;
            private readonly Func<T1, TOut> selector;

            public readonly int SourceLength;

            public readonly TOut Current => selector(source.Current);

            public WhereSelectWhereSelect(WhereSelectWhere<T0, T1> source,
                                          Func<T1, TOut> selector)
            {
                this.source = source;
                this.selector = selector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }

        public readonly ref struct SelectSelectSelectSelect<T0, T1, T2, T3, TOut>
        {
            private readonly SelectSelectSelect<T0, T1, T2, T3> source;
            private readonly Func<T3, TOut> fourthSelector;

            public readonly int SourceLength;

            public readonly TOut Current => fourthSelector(source.Current);

            public SelectSelectSelectSelect(SelectSelectSelect<T0, T1, T2, T3> source, Func<T3, TOut> fourthSelector)
            {
                this.source = source;
                this.fourthSelector = fourthSelector;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext() => source.MoveNext();
        }
        #endregion 4
        #endregion Select

        #region Where
        public readonly ref struct Where<T>
        {
            private readonly Span<T>.Enumerator sourceEnumerator;
            private readonly Predicate<T> match;

            public readonly int SourceLength;

            public readonly T Current => sourceEnumerator.Current;

            public Where(Span<T> source, Predicate<T> match)
            {
                this.match = match;
                sourceEnumerator = source.GetEnumerator();

                SourceLength = source.Length;
            }

            public bool MoveNext()
            {
                while (sourceEnumerator.MoveNext())
                {
                    if (match(sourceEnumerator.Current))
                        return true;
                }

                return false;
            }
        }

        #region 2
        public readonly ref struct WhereWhere<T>
        {
            private readonly Where<T> source;
            private readonly Predicate<T> secondMatch;

            public readonly int SourceLength;

            public readonly T Current => source.Current;

            public WhereWhere(Where<T> source, Predicate<T> secondMatch)
            {
                this.source = source;
                this.secondMatch = secondMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (secondMatch(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct SelectWhere<T, TOut>
        {
            private readonly Select<T, TOut> source;
            private readonly Predicate<TOut> match;

            public readonly int SourceLength;

            public readonly TOut Current => source.Current;

            public SelectWhere(Select<T, TOut> source, Predicate<TOut> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }

                return false;
            }
        }
        #endregion 2

        #region 3
        public readonly ref struct SelectWhereWhere<T, TOut>
        {
            private readonly SelectWhere<T, TOut> source;
            private readonly Predicate<TOut> match;

            public readonly int SourceLength;

            public readonly TOut Current => source.Current;

            public SelectWhereWhere(SelectWhere<T, TOut> source, Predicate<TOut> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }

                return false;
            }
        }

        public readonly ref struct SelectSelectWhere<T0, T1, T2>
        {
            private readonly SelectSelect<T0, T1, T2> source;
            private readonly Predicate<T2> match;

            public readonly int SourceLength;

            public readonly T2 Current => source.Current;

            public SelectSelectWhere(SelectSelect<T0, T1, T2> source, Predicate<T2> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereSelectWhere<T0, T1>
        {
            private readonly WhereSelect<T0, T1> source;
            private readonly Predicate<T1> match;

            public readonly int SourceLength;

            public readonly T1 Current => source.Current;

            public WhereSelectWhere(WhereSelect<T0, T1> source, Predicate<T1> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereWhereWhere<T>
        {
            private readonly WhereWhere<T> source;
            private readonly Predicate<T> thirdMatch;

            public readonly int SourceLength;

            public readonly T Current => source.Current;

            public WhereWhereWhere(WhereWhere<T> source, Predicate<T> thirdMatch)
            {
                this.source = source;
                this.thirdMatch = thirdMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (thirdMatch(source.Current))
                        return true;
                }
                return false;
            }
        }
        #endregion 3

        #region 4
        public readonly ref struct SelectWhereSelectWhere<T0, T1, T2>
        {
            private readonly SelectWhereSelect<T0, T1, T2> source;
            private readonly Predicate<T2> match;

            public readonly int SourceLength;

            public readonly T2 Current => source.Current;

            public SelectWhereSelectWhere(SelectWhereSelect<T0, T1, T2> source,
                                          Predicate<T2> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct SelectWhereWhereWhere<T0, T1>
        {
            private readonly SelectWhereWhere<T0, T1> source;
            private readonly Predicate<T1> thirdMatch;

            public readonly int SourceLength;

            public readonly T1 Current => source.Current;

            public SelectWhereWhereWhere(SelectWhereWhere<T0, T1> source,
                                         Predicate<T1> thirdMatch)
            {
                this.source = source;
                this.thirdMatch = thirdMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (thirdMatch(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereSelectWhereWhere<T0, T1>
        {
            private readonly WhereSelectWhere<T0, T1> source;
            private readonly Predicate<T1> secondMatch;

            public readonly int SourceLength;

            public readonly T1 Current => source.Current;

            public WhereSelectWhereWhere(WhereSelectWhere<T0, T1> source, Predicate<T1> secondMatch)
            {
                this.source = source;
                this.secondMatch = secondMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (secondMatch(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereWhereSelectWhere<T0, T1>
        {
            private readonly WhereWhereSelect<T0, T1> source;
            private readonly Predicate<T1> match;

            public readonly int SourceLength;

            public readonly T1 Current => source.Current;

            public WhereWhereSelectWhere(WhereWhereSelect<T0, T1> source, Predicate<T1> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct SelectSelectSelectWhere<T0, T1, T2, T3>
        {
            private readonly SelectSelectSelect<T0, T1, T2, T3> source;
            private readonly Predicate<T3> match;

            public readonly int SourceLength;

            public readonly T3 Current => source.Current;

            public SelectSelectSelectWhere(SelectSelectSelect<T0, T1, T2, T3> source, Predicate<T3> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct SelectSelectWhereWhere<T0, T1, T2>
        {
            private readonly SelectSelectWhere<T0, T1, T2> source;
            private readonly Predicate<T2> secondMatch;

            public readonly int SourceLength;

            public readonly T2 Current => source.Current;

            public SelectSelectWhereWhere(SelectSelectWhere<T0, T1, T2> source, Predicate<T2> secondMatch)
            {
                this.source = source;
                this.secondMatch = secondMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (secondMatch(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereSelectSelectWhere<T0, T1, T2>
        {
            private readonly WhereSelectSelect<T0, T1, T2> source;
            private readonly Predicate<T2> match;

            public readonly int SourceLength;

            public readonly T2 Current => source.Current;

            public WhereSelectSelectWhere(WhereSelectSelect<T0, T1, T2> source, Predicate<T2> match)
            {
                this.source = source;
                this.match = match;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (match(source.Current))
                        return true;
                }
                return false;
            }
        }

        public readonly ref struct WhereWhereWhereWhere<T>
        {
            private readonly WhereWhereWhere<T> source;
            private readonly Predicate<T> fourthMatch;

            public readonly int SourceLength;

            public readonly T Current => source.Current;

            public WhereWhereWhereWhere(WhereWhereWhere<T> source, Predicate<T> fourthMatch)
            {
                this.source = source;
                this.fourthMatch = fourthMatch;

                SourceLength = source.SourceLength;
            }

            public bool MoveNext()
            {
                while (source.MoveNext())
                {
                    if (fourthMatch(source.Current))
                        return true;
                }
                return false;
            }
        }
        #endregion 4
        #endregion Where
    }
}
