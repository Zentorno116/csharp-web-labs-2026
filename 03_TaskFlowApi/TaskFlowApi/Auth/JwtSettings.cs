using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlowApi.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = "TaskFlowApi";
    public string Audience { get; set; } = "TaskFlowClient";
    public string Key { get; set; } = "TaskFlow_SuperSecret_JwtSigningKey_2026_Min32Chars!";
    public int ExpirationMinutes { get; set; } = 180;

    public SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(Key));
}
