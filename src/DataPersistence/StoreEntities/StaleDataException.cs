using System;

namespace StoreEntities
{
    public class StaleDataException : Exception
    {
        public StaleDataException()
        {
        }

        public StaleDataException(string message)
            : base(message)
        {
        }

        public StaleDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
