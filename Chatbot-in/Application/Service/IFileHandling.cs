using Chatbot_in.Domain.Models;

namespace Chatbot_in.Application.Service;

public interface IFileHandlingTemporary
{
    Task<ContextFileInfo?> SaveFile(string owner, string filename, byte[] binary);

    Task<string[]> LoadFile(string owner, string filename);

    Task<bool> DeleteFile(string owner, string filename);


}
