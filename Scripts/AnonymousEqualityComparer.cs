using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public static class AnonymousEqualityComparer
    {
        public delegate bool Comparison<in T>(T left, T righ);
        public delegate int HashCodeGenerator<in T>(T obj);
    }
    public class AnonymousEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly AnonymousEqualityComparer.Comparison<T> comparison;
        private readonly AnonymousEqualityComparer.HashCodeGenerator<T> hashCodeGenerator;

        public AnonymousEqualityComparer(AnonymousEqualityComparer.Comparison<T> comparison,
            AnonymousEqualityComparer.HashCodeGenerator<T> hashCodeGenerator)
        {
            CC.Validate.ArgumentNull(comparison, nameof(comparison));
            CC.Validate.ArgumentNull(hashCodeGenerator, nameof(hashCodeGenerator));

            this.comparison = comparison;
            this.hashCodeGenerator = hashCodeGenerator;
        }

        public bool Equals(T x, T y) => comparison(x, y);

        public int GetHashCode(T obj) => hashCodeGenerator(obj);
    }
}
