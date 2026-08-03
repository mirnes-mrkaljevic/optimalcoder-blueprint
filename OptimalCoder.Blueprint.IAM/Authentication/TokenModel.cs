namespace OptimalCoder.Blueprint.IAM.Authentication
{
    public class TokenModel
    {
        public string AuthToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
