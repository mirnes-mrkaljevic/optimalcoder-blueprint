namespace OptimalCoder.Blueprint.DB.Entities
{
    public  class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
        public required string AuthToken { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Locked { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Role>? Roles { get; set; }
    }
}
