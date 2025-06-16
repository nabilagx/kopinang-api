namespace kopinang_api.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FirebaseAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var uidExists = context.HttpContext.Items.TryGetValue("uid", out var uidObj);
        var uid = uidObj?.ToString();

        if (!uidExists || string.IsNullOrEmpty(uid))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                status = 401,
                message = "Token Firebase tidak valid atau tidak ditemukan"
            });
        }
    }
}
