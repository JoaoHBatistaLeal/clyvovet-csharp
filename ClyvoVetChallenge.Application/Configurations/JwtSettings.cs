namespace ClyvoVetChallenge.Application.Configurations;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ByteCareApi";
    public string Audience { get; set; } = "ByteCareClient";
    public int ExpirationMinutes { get; set; } = 120;
}
