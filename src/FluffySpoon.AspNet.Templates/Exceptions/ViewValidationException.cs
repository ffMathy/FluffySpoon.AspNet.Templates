using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Templates.Exceptions
{
    public class ViewValidationException: Exception
    {
        public ViewValidationException(string message) : base(message)
        {

        }
    }
}
