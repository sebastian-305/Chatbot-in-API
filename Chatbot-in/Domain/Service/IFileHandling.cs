namespace Chatbot_in.Domain.Service;

public interface IFileHandlingTemporary
{
    Task SaveFile(object binary);

    Task LoadFile(string path);

    Task DeleteFile(string path);
}
