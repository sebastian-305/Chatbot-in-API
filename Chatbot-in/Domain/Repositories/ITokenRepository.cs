using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Repositories
{
    public interface ITokenRepository
    {
        Task<int> SaveToken(Token token);

        Task<int> DeleteToken(Token token);
    }
}
