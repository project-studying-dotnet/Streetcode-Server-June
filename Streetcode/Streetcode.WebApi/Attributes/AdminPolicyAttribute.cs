using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

public class AdminPolicyAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly IAuthorizationService _authorizationService;

    public AdminPolicyAttribute(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        if (actionDescriptor != null)
        {
            var methodName = actionDescriptor.ActionName;

            if (methodName.StartsWith("Create") || methodName.StartsWith("Delete") || methodName.StartsWith("Update"))
            {
                var authorized = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, "AdminPolicy");

                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult(); 
                }
            }
        }
    }
}
