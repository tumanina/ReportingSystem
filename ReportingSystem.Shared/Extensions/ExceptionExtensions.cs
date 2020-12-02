using System;

namespace ReportingSystem.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetMessage(this Exception ex)
        {
            var message = ex.Message;
            var exception = ex.InnerException;
            while (exception != null)
            {
                message = exception.Message;
                exception = exception.InnerException;
            }

            return message;
        }
    }
}
