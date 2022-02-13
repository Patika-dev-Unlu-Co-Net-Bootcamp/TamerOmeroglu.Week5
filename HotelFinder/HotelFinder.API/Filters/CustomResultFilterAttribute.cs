using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace HotelFinder.API.Filters
{
    public class CustomResultFilterAttribute : Attribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("Time", DateTime.UtcNow.ToString());

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("Time", DateTime.UtcNow.ToString());
        }
    }
}
