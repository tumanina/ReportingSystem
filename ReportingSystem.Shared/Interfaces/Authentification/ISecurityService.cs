using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface ISecurityService
    {
        SecurityTypeEnum Type { get; }

        string GenerateToken(JwtPayload payload);

        SecurityKey GetSecurityKey();
    }
}
