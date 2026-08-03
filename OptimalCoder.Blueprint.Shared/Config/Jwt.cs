namespace OptimalCoder.Blueprint.Shared.Config
{
    public class Jwt
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string[] Audiences { get; set; } = Array.Empty<string>();
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }

    }
}
