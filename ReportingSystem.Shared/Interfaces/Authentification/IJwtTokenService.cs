using ReportingSystem.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="token">A string with raw token</param>
        /// <returns>string</returns>
        TokenModel GenerateToken(string email);

        /// <summary>
        /// Reads and validates the JWT token from a string
        /// </summary>
        /// <param name="token">A string with raw token</param>
        /// <returns>ClaimsPrincipal</returns>
        ClaimsPrincipal Validate(string token);

        /// <summary>
        /// Reads the JWT token
        /// </summary>
        /// <param name="token">A string with raw token</param>
        /// <returns>JwtSecurityToken</returns>
        JwtSecurityToken Read(string token);
    }
}
