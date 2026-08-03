namespace OptimalCoder.Blueprint.IAM.Authentication
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message): base(message) { }
        public UnauthorizedException(string code, string message): base(message)
        {
            Code = code;
        }
        public string Code { get; set; } = string.Empty;
    }
}
