using System;

namespace Personnel.Tracker.Model.Auth
{
    public class RevokeAccessToken
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
