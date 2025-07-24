#nullable enable
namespace UTIRLib
{
    public readonly ref struct LoopIteration
    {
        public static LoopIteration<T> Complete<T>(T value)
        {
            return new LoopIteration<T>(value, LoopKeyword.None);
        }
        
        public static LoopIteration<T> Continue<T>(T value)
        {
            return new LoopIteration<T>(value, LoopKeyword.Continue);
        }

        public static LoopIteration<T> Break<T>(T value)
        {
            return new LoopIteration<T>(value, LoopKeyword.Break);
        }
    }
    public readonly struct LoopIteration<T>
    {
        public readonly T Value { get; }
        public readonly LoopKeyword Keyword { get; }

        public LoopIteration(T value, LoopKeyword keyword = LoopKeyword.None)
        {
            Value = value;
            Keyword = keyword;
        }

        public static LoopIteration<T> Void()
        {
            return new LoopIteration<T>(default!, LoopKeyword.Continue);
        }
    }
}
