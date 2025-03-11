using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Repositories;

public interface IContextFileInfoRepository
{
    void SaveFile(ContextFileInfo contextFileInfo);

    IEnumerable<ContextFileInfo> GetFiles(string token);

    void DeleteFile(ContextFileInfo contextFileInfo);
}
