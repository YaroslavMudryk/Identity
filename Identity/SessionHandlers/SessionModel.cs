namespace Identity.SessionHandlers
{
    public class SessionModel
    {
        public Guid Id { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        public DateTime? LastAccess { get; set; }
        public List<TokenModel> Tokens { get; set; }
    }
}
