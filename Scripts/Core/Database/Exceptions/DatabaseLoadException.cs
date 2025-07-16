using System;

namespace UTIRLib.DB
{
    public class DatabaseLoadException : Exception
    {
        public DatabaseLoadException(string message) :
           base($"Critical error while loading database! {message}")
        { }
    }
}