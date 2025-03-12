using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Repositories;

public interface IContextFileInfoRepository
{
    Task SaveFile(ContextFileInfo contextFileInfo);

    Task<IEnumerable<ContextFileInfo>> GetFiles(string token);

    Task DeleteFile(ContextFileInfo contextFileInfo);
}
