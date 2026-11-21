
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GHARSA.API
{
    public class AdminOnlyWritesFilter : IAsyncAuthorizationFilter
    {
        // Controllers that only Admin can manage
        private static readonly HashSet<string>
            AdminControllers = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            )
            {
                "Plants",
                "Categories",
                "PlantCareGuides",
                "CareTasks",
                "PlantCareTasks"
            };

        // Operations that change database data
        private static readonly HashSet<string>
            WriteMethods = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            )
            {
                "POST",
                "PUT",
                "PATCH",
                "DELETE"
            };

        public Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            // Get controller name
            string controllerName =
                context.ActionDescriptor
                    .RouteValues
                    .TryGetValue(
                        "controller",
                        out string? name
                    )
                    ? name ?? ""
                    : "";

            // Get HTTP method
            string method =
                context.HttpContext.Request.Method;

            // Other controllers keep their normal rules
            if (!AdminControllers.Contains(
                controllerName))
            {
                return Task.CompletedTask;
            }

            // GET requests remain available
            if (!WriteMethods.Contains(method))
            {
                return Task.CompletedTask;
            }

            // Check whether the user is logged in
            bool authenticated =
                context.HttpContext.User
                    .Identity?.IsAuthenticated == true;

            if (!authenticated)
            {
                context.Result = new ChallengeResult();

                return Task.CompletedTask;
            }

            // Check Admin role from validated JWT
            bool isAdmin =
                context.HttpContext.User.IsInRole(
                    "Admin"
                );

            if (!isAdmin)
            {
                context.Result = new ForbidResult();

                return Task.CompletedTask;
            }

            // Admin can continue executing the request
            return Task.CompletedTask;
        }
    }
}
