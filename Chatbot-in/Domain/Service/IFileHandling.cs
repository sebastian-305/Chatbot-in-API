using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Service;

public interface IFileHandlingTemporary
{
    Task<ContextFileInfo?> SaveFile(string filename, string ownerToken, byte[] binary);

    Task LoadFile(string path);

    Task DeleteFile(string path);
    
}
