using Chatbot_in.Domain.Identity;
using Chatbot_in.Models;

namespace Chatbot_in.Application.Service
{
    public interface ITokenService
    {
        public Task<TokenDTO> CreateTokenAsync(ApplicationUser user);
    }
}
