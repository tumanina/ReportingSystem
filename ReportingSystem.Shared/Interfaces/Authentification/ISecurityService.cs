using Microsoft.IdentityModel.Tokens;
using ReportingSystem.Shared.Enums;

namespace ReportingSystem.Shared.Interfaces.Authentification
{
    public interface ISecurityService
    {
        SecurityTypeEnum Type { get; }

        SecurityKey GetSecurityKey();
    }
}
