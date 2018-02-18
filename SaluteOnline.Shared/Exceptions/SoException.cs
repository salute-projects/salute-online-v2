using System;
using System.Net;

namespace SaluteOnline.Shared.Exceptions
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
