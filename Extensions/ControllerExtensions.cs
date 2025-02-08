using Microsoft.AspNetCore.Mvc;

    public static class ControllerExtensions
    {
        public static IActionResult WithStatusCode(this IActionResult result, int statusCode)
        {
            if (result is ViewResult viewResult)
            {
                viewResult.StatusCode = statusCode;
            }
            return result;
        }
    }
