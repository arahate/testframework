using System;
using System.Runtime.Serialization;

namespace CafeNextFramework
{
    public class CafeNextFrameworkException : Exception
    {
        public CafeNextFrameworkException()
        {
        }

        public CafeNextFrameworkException(string message) : base(message)
        {
        }

        public CafeNextFrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CafeNextFrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}