using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterWorker.Domain.Interfaces;
using TwitterWorker.Infrastructure.EntityFramework.Contexts;

namespace TwitterWorker.Infrastructure.Repositories
{
    public class TweetRepository : ITweetRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TweetsContext _tweetContext;

        public TweetRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var scope = _serviceProvider.CreateScope();
            _tweetContext = scope.ServiceProvider.GetRequiredService<TweetsContext>();
        }

        public async Task<int> GetTweetsCount() => await _tweetContext.Tweets.CountAsync();
    }
}
