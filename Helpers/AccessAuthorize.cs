using Microsoft.AspNetCore.Mvc;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AccessAuthorizeAttribute : TypeFilterAttribute
{
    public AccessAuthorizeAttribute(params string[] requiredAccessLevels)
        : base(typeof(AccessLevelAuthorizeFilter))
    {
        Arguments = new object[] { requiredAccessLevels.Length > 0 ? requiredAccessLevels : null };
    }
}
