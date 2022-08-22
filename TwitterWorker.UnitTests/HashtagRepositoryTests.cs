using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TwitterWorker.Domain.Entities;
using TwitterWorker.Infrastructure.EntityFramework.Contexts;
using TwitterWorker.Infrastructure.Repositories;

namespace TwitterWorker.UnitTests
{
    public class HashtagRepositoryTests
    {
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        private IServiceProvider _serviceProvider;
        private readonly Mock<IServiceScope> _serviceScopeMock = new Mock<IServiceScope>();
        private readonly Mock<IServiceScopeFactory>  _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        [SetUp]
        public void Setup()
        {
            _serviceCollection.AddDbContext<TweetsContext>(options => options.UseInMemoryDatabase(databaseName: "test_db"));
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            _serviceScopeMock.SetupGet(s => s.ServiceProvider).Returns(_serviceProvider);
            _serviceScopeFactoryMock.Setup(s => s.CreateScope()).Returns(_serviceScopeMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            var context = _serviceProvider.GetRequiredService<TweetsContext>();
            context.Hashtags.ForEachAsync(hashtag =>
            {
                context.Remove(hashtag);
            });
            context.SaveChanges();
        }

        [Test]
        public void GetTopTenHashtagsAsync_NoTopHashtags_WhenEmpty()
        {
            var hashtagRepository = new HashtagRepository(_serviceProvider);

            var response = hashtagRepository.GetTopTenHashtagsAsync().Result;

            Assert.That(response.Count(), Is.Zero);
        }

        [Test]
        public void GetTopTenHashtagsAsync_OneTopHashtag_OneGroupRepeats()
        {                        
            var context = _serviceProvider.GetRequiredService<TweetsContext>();

            context.Add(new Hashtag { Id = 1, Tag = "tag1" });
            context.Add(new Hashtag { Id = 2, Tag = "tag2" });
            context.Add(new Hashtag { Id = 3, Tag = "tag1" });

            context.SaveChanges();            

            var hashtagRepository = new HashtagRepository(_serviceProvider);

            var response = hashtagRepository.GetTopTenHashtagsAsync().Result;

            Assert.That(response.Count(), Is.EqualTo(1));
            Assert.That(response.ElementAt(0).Tag, Is.EqualTo("tag1"));
            Assert.That(response.ElementAt(0).Count, Is.EqualTo(2));
        }

        [Test]
        public void GetTopTenHashtagsAsync_TwoTopHashtags_TwoGroupRepeats()
        {
            var context = _serviceProvider.GetRequiredService<TweetsContext>();

            context.Add(new Hashtag { Id = 1, Tag = "tag2" });
            context.Add(new Hashtag { Id = 2, Tag = "tag2" });
            context.Add(new Hashtag { Id = 3, Tag = "tag1" });
            context.Add(new Hashtag { Id = 4, Tag = "tag2" });
            context.Add(new Hashtag { Id = 5, Tag = "tag1" });
            context.Add(new Hashtag { Id = 6, Tag = "tag3" });

            context.SaveChanges();

            var hashtagRepository = new HashtagRepository(_serviceProvider);

            var response = hashtagRepository.GetTopTenHashtagsAsync().Result;

            Assert.That(response.Count(), Is.EqualTo(2));
            Assert.That(response.ElementAt(0).Tag, Is.EqualTo("tag2"));
            Assert.That(response.ElementAt(0).Count, Is.EqualTo(3));
            Assert.That(response.ElementAt(1).Tag, Is.EqualTo("tag1"));
            Assert.That(response.ElementAt(1).Count, Is.EqualTo(2));
        }
    }
}
