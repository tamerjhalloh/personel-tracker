using System;
using System.Collections.Generic;
using System.Text;

namespace Personnel.Tracker.Model.Auth
{
    public class SignIn
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
