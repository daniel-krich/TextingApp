using System.Collections.Generic;
using System.Security.Claims;

namespace TextAppApi.Authentication
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}