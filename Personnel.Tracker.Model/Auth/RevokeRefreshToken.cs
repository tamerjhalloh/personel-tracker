using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Model.Auth
{
    public class RevokeRefreshToken
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
