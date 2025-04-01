using Chatbot_in.Application.Repositories;
using Chatbot_in.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class HetznerDbPromptCountRepository : IPromptCountRepository
{

    private readonly int _RPM = 15;
    private readonly int _RPD = 15000;
    private readonly HetznerDbContext _dbContext;
    private readonly ILogger<HetznerDbContextFileInfoRepository> _logger;

    public HetznerDbPromptCountRepository(HetznerDbContext dbContext, ILogger<HetznerDbContextFileInfoRepository> logger)
    {

        _dbContext = dbContext;
        _logger = logger;
    }

    public int PromptsLeftMinute => _RPM - GetAllPrompts().Result.Select((elem) => elem.Prompt).ToList().Count(p => p > DateTime.Now.AddMinutes(-1));

    public int PromptsLeftDay => _RPD - GetAllPrompts().Result.Select((elem) => elem.Prompt).ToList().Count(p => p > DateTime.Now.AddDays(-1));

    public int TotalPrompts => GetAllPrompts().Result.Select((elem) => elem.Prompt).ToList().Count;

    public async Task AddPrompt()
    {
        var requests = await GetAllPrompts();

        var delete = requests.Where(p => p.Prompt < DateTime.Now.AddDays(-1));

        foreach (var obj in delete)
        {
            _dbContext.Requests.Remove(obj);
        }

        _dbContext.Add(new RequestCounter() { Prompt = DateTime.Now });

        await _dbContext.SaveChangesAsync();
    }

    private async Task<IEnumerable<RequestCounter>> GetAllPrompts()
    {
        return await _dbContext.Requests.ToListAsync();
    }


}
