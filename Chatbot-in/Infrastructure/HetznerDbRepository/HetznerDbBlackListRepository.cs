using Chatbot_in.Application.Repositories;
using Chatbot_in.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class HetznerDbBlackListRepository : IBlackListRepository
{
    private readonly HetznerDbContext _dbContext;
    private readonly ILogger<HetznerDbContextFileInfoRepository> _logger;

    public HetznerDbBlackListRepository(HetznerDbContext dbContext, ILogger<HetznerDbContextFileInfoRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddTokenAsync(BlackListToken token)
    {
        var expiredToken = GetAllTokensAsync().Result.Where(tokenDTO=>tokenDTO.Expiration < DateTime.Now);

        foreach (var tokenDTO in expiredToken)
        {
            _dbContext.BlackList.Remove(tokenDTO);
        }

        _dbContext.Add(token);  

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<BlackListToken>> GetAllTokensAsync()
    {
        return await _dbContext.BlackList.ToListAsync();
    }
}
