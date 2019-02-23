using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace GABDemo
{
    /// <summary>
    /// This filter allows for displaying more explicit options validation exception.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
    public class OptionsValidationExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// This method is called when an exception occurs and wrap <see cref="Microsoft.Extensions.Options.OptionsValidationException"/> 
        /// into another <see cref="Exception"/> to make the message easier to understand.
        /// </summary>
        /// <param name="context">The provided exception context.</param>
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OptionsValidationException validationEx)
            {
                context.Exception = new Exception(validationEx.Failures.First(), validationEx);
            }
        }
    }
}
