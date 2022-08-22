using TwitterWorker.Domain.Entities;

namespace TwitterWorker.Domain.Interfaces
{
    public interface IHashtagRepository
    {
        Task<IEnumerable<HashtagGroup>> GetTopTenHashtagsAsync();
    }
}
