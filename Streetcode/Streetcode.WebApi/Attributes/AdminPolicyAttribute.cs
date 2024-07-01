using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

public class AdminPolicyAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public AdminPolicyAttribute()
    {
        Policy = "AdminPolicy";
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        if (actionDescriptor != null)
        {
            var methodName = actionDescriptor.ActionName;
            var methodInfo = actionDescriptor.MethodInfo;

            var createDeleteUpdateMethods = methodInfo.DeclaringType
                .GetMethods()
                .Where(m => m.Name.StartsWith("Create") || m.Name.StartsWith("Delete") || m.Name.StartsWith("Update"))
                .Select(m => m.Name);

            if (createDeleteUpdateMethods.Contains(methodName))
            {
                var authorizeFilter = new AuthorizeFilter(Policy);
                await authorizeFilter.OnAuthorizationAsync(context);
            }
        }
    }
}
