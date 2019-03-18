using FluffySpoon.AspNet.Templates.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates
{
    public interface IViewValidator
    {
        Task<ViewValidationException> ValidateAsync(string viewPath);
    }
}
