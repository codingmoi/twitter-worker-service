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
            context.Hashtags.ForEachAsync(hashtag => context.Remove(hashtag));
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
        public void GetTopTenHashtagsAsync_TopOneHashtag_OneGroupRepeats()
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
        public void GetTopTenHashtagsAsync_TopTwoHashtags_TwoGroupsRepeat()
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

        [Test]
        public void GetTopTenHashtagsAsync_TopTenHashtags_TwelveGroupsRepeat()
        {
            var context = _serviceProvider.GetRequiredService<TweetsContext>();

            var repeatTag = new int[] { 15, 5, 2, 13, 17, 10, 6, 9, 19, 7, 8, 4 };
            var id = 1;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < repeatTag[i] - 1; j++)
                {
                    context.Add(new Hashtag { Id = id++, Tag = $"tag{i}" });
                }
            }

            context.SaveChanges();

            var hashtagRepository = new HashtagRepository(_serviceProvider);

            var response = hashtagRepository.GetTopTenHashtagsAsync().Result;

            Assert.That(response.Count(), Is.EqualTo(10));
            Assert.That(response.ElementAt(0).Tag, Is.EqualTo("tag8"));
            Assert.That(response.ElementAt(1).Tag, Is.EqualTo("tag4"));
            Assert.That(response.ElementAt(2).Tag, Is.EqualTo("tag0"));
            Assert.That(response.ElementAt(3).Tag, Is.EqualTo("tag3"));
            Assert.That(response.ElementAt(4).Tag, Is.EqualTo("tag5"));
            Assert.That(response.ElementAt(5).Tag, Is.EqualTo("tag7"));
            Assert.That(response.ElementAt(6).Tag, Is.EqualTo("tag10"));
            Assert.That(response.ElementAt(7).Tag, Is.EqualTo("tag9"));
            Assert.That(response.ElementAt(8).Tag, Is.EqualTo("tag6"));
            Assert.That(response.ElementAt(9).Tag, Is.EqualTo("tag1"));
        }
    }
}
