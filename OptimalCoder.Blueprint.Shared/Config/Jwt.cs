namespace OptimalCoder.Blueprint.Shared.Config
{
    public class Jwt
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }

    }
}
