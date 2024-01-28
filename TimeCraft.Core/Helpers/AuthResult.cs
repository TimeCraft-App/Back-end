namespace TimeCraft.Core.Helpers
{
    public class AuthResult
    {
        public string UserId { get; set; }
        public string Token { get; set; } // The generated JWT token
        public bool Succedded { get; set; } // True or False based on the Authorization Status
        public List<string> Errors { get; set; } // Custom Error Message
    }
}
