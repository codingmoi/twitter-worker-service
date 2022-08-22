using Microsoft.EntityFrameworkCore;
using TwitterWorker.Domain.Entities;

namespace TwitterWorker.Infrastructure.EntityFramework.Contexts
{
    public class TweetsContext : DbContext
    {
        public TweetsContext(DbContextOptions<TweetsContext> options) : base(options)
        {

        }

        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
    }
}
