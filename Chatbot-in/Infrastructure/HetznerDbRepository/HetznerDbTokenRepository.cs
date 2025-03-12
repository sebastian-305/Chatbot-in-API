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


        public async Task<int> DeleteToken(string tokenId)
        {
            _logger.Log(LogLevel.Information, "Deleting Token in Database");
            _logger.Log(LogLevel.Debug, "Id: {tokenId}", tokenId);
            var existing = await _dbContext.Tokens.FindAsync(tokenId);

            if (existing == null) {
                _logger.Log(LogLevel.Error, "Token to delete does not exist in Database");
                throw new ArgumentException("Token does not exist");
            }


            _dbContext.Tokens.Remove(existing);
            int states = await _dbContext.SaveChangesAsync();

            if (states == 0)
            {
                _logger.Log(LogLevel.Warning, "0 Rows deleted in Database");
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"Token deleted, {states} rows affected");
            }

            return states;
        }

        public Task<Token> FindToken(string tokenId)
        {
            throw new NotImplementedException();
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

        public Task<int> UpdateLastUse(string tokenId)
        {
            throw new NotImplementedException();
        }
    }
}
