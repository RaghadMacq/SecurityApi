using Microsoft.AspNetCore.Mvc.Filters;

namespace SecurityApi.Filters
{
    public class MyLogging : Attribute, IActionFilter
    {
        private readonly string _callerName;
        public MyLogging(string callerName)
        {
            _callerName = callerName;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"filter executed after {_callerName}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"filter executed before {_callerName}");
        }
    }
}
