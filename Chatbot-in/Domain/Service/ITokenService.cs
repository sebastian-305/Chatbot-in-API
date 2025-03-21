using Chatbot_in.Domain.Identity;
using Chatbot_in.Domain.Models;

namespace Chatbot_in.Domain.Service
{
    public interface ITokenService
    {
        public Task<TokenDTO> CreateTokenAsync(ApplicationUser user);
    }
}
