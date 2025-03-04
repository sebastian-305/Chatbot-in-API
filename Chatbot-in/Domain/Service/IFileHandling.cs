namespace Chatbot_in.Domain.Service
{
    public interface IFileHandling
    {
        void SaveFile(object binary);

        void LoadFile(string path);

        void DeleteFile(string path);
    }
}
