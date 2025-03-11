using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Repositories
{
    public interface ITokenRepository
    {
        Task SaveToken(Token token);

        Task DeleteToken(Token token);
    }
}
