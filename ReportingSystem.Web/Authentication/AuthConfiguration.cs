using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ReportingSystem.Shared.Interfaces.Authentification;
using System;
using System.Linq;

namespace ReportingSystem.Web.Authentication
{
    public static class AuthConfiguration
    {
        public const string DefaultScheme = "Smart";

        public static IServiceCollection ConfigureAuth(this IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            {
                var  tokenService = provider.GetRequiredService<IJwtTokenService>();
                services.AddScoped<IAsyncAuthorizationFilter, AuthorizationFilter>();
                services.AddScoped<IAuthenticationHandler, JwtTokenAuthenticationHandler>();
                services.AddScoped<IAuthenticationHandler, BasicAuthenticationHandler>();

                services.AddAuthentication(DefaultScheme)
                    .AddScheme<AuthenticationSchemeOptions, JwtTokenAuthenticationHandler>(JwtTokenAuthenticationHandler.AuthenticationScheme, _ => { })
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.AuthenticationScheme, _ => { })
                    .AddPolicyScheme(DefaultScheme, "Bearer or Basic Authentication", polSchemOpt =>
                    {
                        polSchemOpt.ForwardDefaultSelector = context =>
                        {
                            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authValue))
                            {
                                if (authValue.First().StartsWith($"{JwtBearerDefaults.AuthenticationScheme} ", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    return JwtBearerDefaults.AuthenticationScheme;
                                }
                            }
                            return BasicAuthenticationHandler.AuthenticationScheme;
                        };
                    });
            }

            return services;
        }
    }
}