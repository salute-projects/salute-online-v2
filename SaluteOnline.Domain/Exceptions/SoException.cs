using System;
using System.Net;

namespace SaluteOnline.Domain.Exceptions
{
    public class SoException : Exception
    {
        public HttpStatusCode HttpCode;

        public SoException(string message, HttpStatusCode httpCode) : base(message)
        {
            HttpCode = httpCode;
            
        }
    }
}
