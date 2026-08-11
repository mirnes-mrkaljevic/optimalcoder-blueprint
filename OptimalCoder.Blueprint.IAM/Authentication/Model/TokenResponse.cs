namespace OptimalCoder.Blueprint.IAM.Authentication.Model
{
    public class TokenResponse
    {
        public string AuthToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
