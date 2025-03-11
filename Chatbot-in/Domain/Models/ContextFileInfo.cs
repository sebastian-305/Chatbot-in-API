namespace Chatbot_in.Domain.Models
{
    public class ContextFileInfo 
    {
        public int Id{ get; set; }
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string OwnerToken { get; set; }  

    }
}
