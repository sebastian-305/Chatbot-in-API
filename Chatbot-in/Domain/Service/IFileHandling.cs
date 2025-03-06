namespace Chatbot_in.Domain.Service
{
    public interface IFileHandlingTemporary
    {
        void SaveFile(object binary);

        void LoadFile(string path);

        void DeleteFile(string path);
    }
}
