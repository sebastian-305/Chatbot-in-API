using Chatbot_in.Domain.Models;
using Newtonsoft.Json.Linq;

namespace Chatbot_in.Domain.Repositories;

public interface IBlackListRepository
{
    Task AddTokenAsync(BlackListTokenDTO token);

    Task<IEnumerable<BlackListTokenDTO>> GetAllTokensAsync();
}

