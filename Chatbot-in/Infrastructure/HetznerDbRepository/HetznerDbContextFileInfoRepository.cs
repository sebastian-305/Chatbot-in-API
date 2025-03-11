using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;

namespace Chatbot_in.Infrastructure.HetznerDbRepository
{
    public class HetznerDbContextFileInfoRepository : IContextFileInfoRepository
    {
        private readonly HetznerDbContext _dbContext;

        public HetznerDbContextFileInfoRepository(HetznerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DeleteFile(ContextFileInfo contextFileInfo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContextFileInfo> GetFiles(string token)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(ContextFileInfo contextFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
