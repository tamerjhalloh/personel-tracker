using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Portal.Models
{
    public class UserIdentity
    {
        public Guid Id { get; set; } 
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; } 
        public string Token { get; set; }
        public string RefreshToken { get; set; } 
        public string Role { get; set; }

    }
}
