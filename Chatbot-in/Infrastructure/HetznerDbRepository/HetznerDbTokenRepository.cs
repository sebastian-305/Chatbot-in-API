using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository
{
    public class HetznerDbTokenRepository : ITokenRepository
    {
        private readonly HetznerDbContext _dbContext;

        public HetznerDbTokenRepository(HetznerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task DeleteToken(Token token)
        {
            var existing = await _dbContext.Tokens.FindAsync(token.TokenId);

            if (existing == null)
                throw new ArgumentException("Token does not exist");

            _dbContext.Tokens.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveToken(Token token)
        {
            _dbContext.Add(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
