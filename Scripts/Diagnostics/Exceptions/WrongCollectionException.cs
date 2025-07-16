using System.Collections;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public class WrongCollectionException : TirLibException
    {
        public WrongCollectionException()
        {
        }

        public WrongCollectionException(string message) : base(message)
        {
        }

        public WrongCollectionException(IEnumerable? collection)
            : base($"Collection: {Resolve(collection)}.")
        {
        }

        public WrongCollectionException(IEnumerable? collection, string message)
            : base($"Collection: {Resolve(collection)}. {message}")
        {
        }

        internal static string Resolve(IEnumerable? collection)
        {
            if (collection.IsNull())
            {
                return "null";
            }
            else if (IsEmptyCollection(collection))
            {
                return "empty";
            }
            else return collection.ToString();
        }

        private static bool IsEmptyCollection(IEnumerable collection)
        {
            return !collection.GetEnumerator().MoveNext();
        }
    }
}