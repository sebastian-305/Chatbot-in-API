using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class HetznerDbContextFileInfoRepository : IContextFileInfoRepository
{
    private readonly HetznerDbContext _dbContext;
    private readonly ILogger<HetznerDbContextFileInfoRepository> _logger;

    public HetznerDbContextFileInfoRepository(HetznerDbContext dbContext, ILogger<HetznerDbContextFileInfoRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> DeleteFile(ContextFileInfo contextFileInfo)
    {
        _logger.Log(LogLevel.Information, "Deleting Fileinfo in Database");
        _logger.Log(LogLevel.Debug, "FileName: {FileName} \n Owner: {Owner}", contextFileInfo.FileName, contextFileInfo.OwnerToken);
        var existing = await _dbContext.Files.FindAsync(contextFileInfo.Id);

        if (existing == null)
        {
            _logger.Log(LogLevel.Error, "File to delete does not exist in Database");
            throw new ArgumentException("File does not exist");
        }

        _dbContext.Files.Remove(existing);
        int states = await _dbContext.SaveChangesAsync();

        if (states == 0)
        {
            _logger.Log(LogLevel.Warning, "0 Rows deleted in Database");
        }
        else
        {
            _logger.Log(LogLevel.Debug, "File deleted, {states} rows affected", states);
        }

        return states;
    }

    public async Task<IEnumerable<ContextFileInfo>> GetFiles(string tokenId)
    {
        _logger.Log(LogLevel.Information, "Getting all context files");
        _logger.Log(LogLevel.Debug, "From owner {token}", tokenId);

        var files = await _dbContext.Files.ToListAsync();

        files = files.Where(f => f.OwnerToken.Equals(tokenId)).ToList();

        _logger.Log(LogLevel.Debug, "Returning {num} file infos", files.Count);

        return files;
    }

    public async Task<int> SaveFile(ContextFileInfo contextFileInfo)
    {
        _logger.Log(LogLevel.Information, "Saving ContextFileInfo in Database");
        _logger.Log(LogLevel.Debug, "Filename: {filename} \nOnwer: {owner}", contextFileInfo.FileName, contextFileInfo.OwnerToken);
        _dbContext.Add(contextFileInfo);
        int states = await _dbContext.SaveChangesAsync();

        if (states == 0)
        {
            _logger.Log(LogLevel.Warning, "0 Rows saved in Database");
        }
        else
        {
            _logger.Log(LogLevel.Debug, "ContextFileInfo saved, {states} rows affected", states);
        }

        return states;
    }
}
