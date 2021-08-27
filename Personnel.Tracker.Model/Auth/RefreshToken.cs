using Microsoft.AspNetCore.Identity;
using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Model.Auth
{
    public class RefreshToken: IEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public bool Revoked => RevokedAt.HasValue;

        protected RefreshToken()
        {
        }

        public RefreshToken(Personnel.Personnel personnel, IPasswordHasher<Personnel.Personnel> passwordHasher)
        {
            Id = Guid.NewGuid();
            UserId = personnel.PersonnelId;
            CreatedAt = DateTime.UtcNow;
            Token = CreateToken(personnel, passwordHasher);
        }

        public void Revoke()
        {
            if (Revoked)
            {
                throw new Exception( $"Refresh token: '{Id}' was already revoked at '{RevokedAt}'.");
            }
            RevokedAt = DateTime.UtcNow;
        }

        private static string CreateToken(Personnel.Personnel personnel, IPasswordHasher<Personnel.Personnel> passwordHasher)
            => passwordHasher.HashPassword(personnel, Guid.NewGuid().ToString("N"))
                .Replace("=", string.Empty)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty);
    }
}
