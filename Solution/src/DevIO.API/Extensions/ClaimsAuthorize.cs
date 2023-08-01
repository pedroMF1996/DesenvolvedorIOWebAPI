using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DevIO.API.Extensions
{
    public static class CustomAuthorization
    {
        public static bool ValidarClaimUsuario(HttpContext context, string claimName, string claimValue)
        {
            return context.User.Identity.IsAuthenticated && 
                context.User.Claims.Any(c => c.Type == claimName && c.Value == claimValue);
        }
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new Claim[] { new Claim(claimName, claimValue) };
        }
    }

    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public bool GetIsAuthenticated(AuthorizationFilterContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!GetIsAuthenticated(context))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            if(!CustomAuthorization.ValidarClaimUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new StatusCodeResult(401);
            }
        }
    }
}
