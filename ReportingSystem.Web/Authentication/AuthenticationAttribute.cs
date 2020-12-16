using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ReportingSystem.Web.Authentication
{
    public class AuthenticationAttribute : TypeFilterAttribute
    {
        public AuthenticationAttribute() : base(typeof(AuthenticationFilter))
        {
        }
    }

    public class AuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly IJwtTokenService _tokenService;
        private readonly IAuthorizationService _authorizationService;

        public AuthenticationFilter(IJwtTokenService tokenService, IAuthorizationService authorizationService)
        {
            _tokenService = tokenService;
            _authorizationService = authorizationService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (HasAllowAnonymous(context.Filters))
            {
                return;
            }

            if (!AuthenticationHeaderValue.TryParse(context.HttpContext.Request.Headers["Authorization"], out AuthenticationHeaderValue authHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (string.IsNullOrEmpty(authHeader?.Parameter) || !authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var jwtTokenString = authHeader.Parameter;
                var token = _tokenService.Read(jwtTokenString);

                var controller = context.HttpContext.Request.RouteValues.GetValueOrDefault("controller").ToString();
                var action = context.HttpContext.Request.RouteValues.GetValueOrDefault("action").ToString();

                var hasAccess = await _authorizationService.UserHasAccess(token.Subject, $"{controller}/{action}");
                if (!hasAccess)
                {
                    context.Result = new ForbidResult();
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
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

    }
}
