using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ReportingSystem.Shared.Interfaces.Authentification;

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
                services.AddSingleton<IAsyncAuthorizationFilter, AuthenticationFilter>();
                services.AddSingleton<IAuthenticationHandler, JwtTokenAuthorizationHandler>();

                services.AddAuthentication(DefaultScheme)
                    .AddScheme<AuthenticationSchemeOptions, JwtTokenAuthorizationHandler>(JwtTokenAuthorizationHandler.AuthenticationScheme, _ => { })
                    .AddPolicyScheme(DefaultScheme, "Authorization Bearer", polSchemOpt =>
                    {
                        polSchemOpt.ForwardDefaultSelector = context =>
                        {
                            return JwtTokenAuthorizationHandler.AuthenticationScheme;
                        };
                    });
            }

            return services;
        }
    }
}