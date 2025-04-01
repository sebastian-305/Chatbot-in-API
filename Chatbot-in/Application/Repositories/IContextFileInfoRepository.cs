using Chatbot_in.Domain.Models;

namespace Chatbot_in.Application.Repositories;

public interface IContextFileInfoRepository
{
    Task<int> SaveFile(ContextFileInfo contextFileInfo);

    Task<IEnumerable<ContextFileInfo>> GetFiles(string tokenId);

    Task<int> DeleteFile(ContextFileInfo contextFileInfo);
}
