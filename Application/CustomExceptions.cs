using System;

namespace Application
{
    public abstract class CustomException : Exception
    {

        public CustomException() : base("Illegal operation for " + "An error has occured") { }

        public virtual string ErrorMessage()
        {
            var error = "Error: ";
            return error;
        }
    }
}

