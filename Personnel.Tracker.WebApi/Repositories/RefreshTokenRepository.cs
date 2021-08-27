using Microsoft.EntityFrameworkCore;
using Personnel.Tracker.Model.Auth;
using Personnel.Tracker.WebApi.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personnel.Tracker.WebApi.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PersonnelContext _contex;
        public RefreshTokenRepository(PersonnelContext contex)
        {
            _contex = contex;
        }

        public async Task AddAsync(RefreshToken token)
        {

            await _contex.RefreshTokens.AddAsync(token);
            await _contex.SaveChangesAsync();

        }

        public async Task<RefreshToken> GetAsync(string token)
        {
            return await _contex.RefreshTokens.FirstOrDefaultAsync(a => a.Token == token);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            var dbToken = await _contex.RefreshTokens.FirstOrDefaultAsync(a => a.Token == token.Token);
            if (dbToken != null)
            {
                dbToken.Revoke();
                await _contex.SaveChangesAsync();
            }
        }
    }
}
