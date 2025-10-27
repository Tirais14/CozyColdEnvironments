#nullable enable
namespace CCEnvs.FuncLanguage
{
    public static class BoxConverter
    {
        public static Box<T> AsBox<T>(this T source) => new(source);
        public static Box<T> AsBox<T>(this T item1, T item2) => new(item1, item2);
        public static Box<T> AsBox<T>(this T item1, T item2, T item3)
        {
            return new Box<T>(item1, item2, item3);
        }
        public static Box<T> AsBox<T>(this T item1,
                                      T item2,
                                      T item3,
                                      T item4)
        {
            return new Box<T>(item1,
                              item2,
                              item3,
                              item4);
        }
        public static Box<T> AsBox<T>(this T item1,
                                      T item2,
                                      T item3,
                                      T item4,
                                      T item5)
        {
            return new Box<T>(item1,
                              item2,
                              item3,
                              item4,
                              item5);
        }
        public static Box<T> AsBox<T>(this T item1,
                                      T item2,
                                      T item3,
                                      T item4,
                                      T item5,
                                      T item6)
        {
            return new Box<T>(item1,
                              item2,
                              item3,
                              item4,
                              item5,
                              item6);
        }
        public static Box<T> AsBox<T>(this (T, T) source)
        {
            return new Box<T>(source.Item1, source.Item2);
        }
        public static Box<T> AsBox<T>(this (T, T, T) source)
        {
            return new Box<T>(source.Item1, source.Item2, source.Item3);
        }
        public static Box<T> AsBox<T>(this (T, T, T, T) source)
        {
            return new Box<T>(source.Item1,
                              source.Item2,
                              source.Item3,
                              source.Item4);
        }
        public static Box<T> AsBox<T>(this (T, T, T, T, T) source)
        {
            return new Box<T>(source.Item1,
                              source.Item2,
                              source.Item3,
                              source.Item4,
                              source.Item5);
        }
        public static Box<T> AsBox<T>(this (T, T, T, T, T, T) source)
        {
            return new Box<T>(source.Item1,
                              source.Item2,
                              source.Item3,
                              source.Item4,
                              source.Item5,
                              source.Item6);
        }
    }
}
