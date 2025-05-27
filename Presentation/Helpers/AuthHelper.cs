using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace Presentation.Helpers;

public static class AuthUtils
{
    public static string? ExtractUserId(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            throw new UnauthorizedAccessException("Authorization header not found.");
        
        var authHeader = req.Headers.GetValues("Authorization").FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return null;

        var token = authHeader.Substring("Bearer ".Length);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
    }
}