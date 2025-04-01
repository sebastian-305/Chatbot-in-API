using Chatbot_in.Domain.Models;
using Newtonsoft.Json.Linq;

namespace Chatbot_in.Application.Repositories;

public interface IBlackListRepository
{
    Task AddTokenAsync(BlackListToken token);

    Task<IEnumerable<BlackListToken>> GetAllTokensAsync();
}

