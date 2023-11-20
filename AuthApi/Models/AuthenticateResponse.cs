using System.Text.Json.Serialization;

namespace AuthApi.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string JwtToken { get; set; }
        public int ExpiresInSeconds { get; set; }
        public string Message { get; set; }
    }
}
