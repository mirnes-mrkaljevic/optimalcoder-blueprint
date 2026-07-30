namespace OptimalCoder.Blueprint.DB.Entities
{
    public  class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string AuthToken { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Locked { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public virtual IEnumerable<Role> Roles { get; set; }
    }
}
