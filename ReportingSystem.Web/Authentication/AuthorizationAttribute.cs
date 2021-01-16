using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Authentication
{
    public class AuthorizationAttribute : TypeFilterAttribute
    {
        public AuthorizationAttribute() : base(typeof(AuthorizationFilter))
        {
        }
    }

    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (HasAllowAnonymous(context.Filters))
            {
                return;
            }

            try
            {
                var user = context.HttpContext?.User;
                if (!user.Identity.IsAuthenticated)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var controller = context.HttpContext.Request.RouteValues.GetValueOrDefault("controller").ToString();
                var action = context.HttpContext.Request.RouteValues.GetValueOrDefault("action").ToString();

                var hasAccess = await _authorizationService.UserHasAccess(GetEmailFromClaims(user.Claims), $"{controller}/{action}");
                if (!hasAccess)
                {
                    context.Result = new ForbidResult();
                }
            }
            catch (Exception ex)
            {
                //tod: log exception
                context.Result = new ForbidResult();
            }

            return;
        }

        protected bool HasAllowAnonymous(IList<IFilterMetadata> filters)
        {
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is IAllowAnonymousFilter)
                {
                    return true;
                }
            }

            return false;
        }

        private string GetEmailFromClaims(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
            {
                throw new Exception("No claims found");
            }

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == JwtRegisteredClaimNames.Email);

            if (email == null)
            {
                throw new Exception("Email claim not found");
            }

            return email.Value;
        }
    }
}
