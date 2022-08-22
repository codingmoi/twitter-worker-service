using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterWorker.Domain.Entities;
using TwitterWorker.Domain.Interfaces;
using TwitterWorker.Infrastructure.EntityFramework.Contexts;

namespace TwitterWorker.Infrastructure.Repositories
{
    public class HashtagRepository : IHashtagRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TweetsContext _tweetContext;

        public HashtagRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var scope = _serviceProvider.CreateScope();
            _tweetContext = scope.ServiceProvider.GetRequiredService<TweetsContext>();
        }

        public async Task<IEnumerable<HashtagGroup>> GetTopTenHashtagsAsync()
        {
            var hashtags = _tweetContext.Hashtags.AsNoTracking();
            return await (
                from hashtag in hashtags
                group hashtag by hashtag.Tag into tagGroup
                select new HashtagGroup { Tag = tagGroup.Key, Count = tagGroup.Count() }
                ).Where(g => g.Count > 1)
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();
        }
    }
}
