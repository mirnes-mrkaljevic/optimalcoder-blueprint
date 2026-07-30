namespace OptimalCoder.Blueprint.API.Exceptions
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
