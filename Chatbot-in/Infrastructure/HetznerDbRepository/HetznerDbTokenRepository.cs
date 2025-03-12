using Chatbot_in.Controllers;
using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository
{
    public class HetznerDbTokenRepository : ITokenRepository
    {
        private readonly HetznerDbContext _dbContext;
        private readonly ILogger<HetznerDbTokenRepository> _logger;

        public HetznerDbTokenRepository(ILogger<HetznerDbTokenRepository> logger, HetznerDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<int> DeleteToken(Token token)
        {
            var existing = await _dbContext.Tokens.FindAsync(token.TokenId);

            if (existing == null)
                throw new ArgumentException("Token does not exist");

            _dbContext.Tokens.Remove(existing);
            int states = await _dbContext.SaveChangesAsync();
            return states;
        }

        public async Task<int> SaveToken(Token token)
        {
            _logger.Log(LogLevel.Information, "Saving Token in Database");
            _logger.Log(LogLevel.Debug, $"Id: {token.TokenId} \nCreatedAt: {token.CreatedAt} \nLastUse: {token.LastUse}");
            _dbContext.Add(token);
            int states = await _dbContext.SaveChangesAsync();

            if (states == 0)
            {
                _logger.Log(LogLevel.Warning, "0 Rows saved in Database");
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"Token saved, {states} rows affected");
            }

            return states;
        }
    }
}
