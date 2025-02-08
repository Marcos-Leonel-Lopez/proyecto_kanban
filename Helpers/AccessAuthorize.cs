// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Microsoft.Extensions.DependencyInjection;

// [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
// public class AccessAuthorizeAttribute : TypeFilterAttribute
// {
//     public AccessAuthorizeAttribute(params string[] requiredAccessLevels)
//         : base(typeof(AccessLevelAuthorizeFilter))
//     {
//         Arguments = new object[] { requiredAccessLevels };
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AccessAuthorizeAttribute : TypeFilterAttribute
{
    public AccessAuthorizeAttribute(params string[] requiredAccessLevels)
        : base(typeof(AccessLevelAuthorizeFilter))
    {
        Arguments = new object[] { requiredAccessLevels.Length > 0 ? requiredAccessLevels : null };
    }
}
