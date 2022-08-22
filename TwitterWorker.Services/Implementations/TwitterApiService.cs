using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TwitterWorker.Domain.Entities;
using TwitterWorker.Infrastructure.EntityFramework.Contexts;
using TwitterWorker.Services.Dtos;
using TwitterWorker.Services.Interfaces;

namespace TwitterWorker.Services.Implementations
{
    public class TwitterApiService : ITwitterApiService
    {
        private const string Bearer = "Bearer";
        private const string TwitterApiBearerToken = "TwitterApi:BearerToken";
        private const string TwitterApiBaseUrl = "TwitterApi:BaseUrl";
        private const string TwitterApiEntitiesQuery = "TwitterApi:EntitiesQuery";

        private readonly HttpClient _httpClient;
        private readonly ILogger<TwitterApiService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TweetsContext _tweetsContext;
        private readonly IConfiguration _configuration;

        public TwitterApiService(HttpClient httpClient, ILogger<TwitterApiService> logger, 
            IServiceProvider serviceProvider, IConfiguration configuration)
        {            
            _httpClient = httpClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            var scope = _serviceProvider.CreateScope();
            _tweetsContext = scope.ServiceProvider.GetRequiredService<TweetsContext>();

            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>(TwitterApiBaseUrl));
            _httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Bearer,
                _configuration.GetValue<string>(TwitterApiBearerToken));
        }

        public async Task GetTweetsStreamAsync()
        {
            try
            {
                _logger.LogInformation("Connecting to Twitter's Data Stream");

                var entitiesQueryUriSegment = _configuration.GetValue<string>(TwitterApiEntitiesQuery);
                var stream = await _httpClient.GetStreamAsync(entitiesQueryUriSegment);

                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var currentLine = reader.ReadLine();
                    
                    if (currentLine == null) continue;

                    var tweet = JsonConvert.DeserializeObject<TweetDto>(currentLine);
                   
                    if (tweet == null || tweet.Data == null || 
                        tweet.Data.Entities == null || tweet.Data.Entities.Hashtags == null) continue;

                    ICollection<Hashtag> hashtags = new List<Hashtag>();

                    foreach (var hashtag in tweet.Data.Entities.Hashtags)
                    {
                        hashtags.Add(new Hashtag
                        {
                            Tag = hashtag.Tag
                        });
                    }

                    _tweetsContext.Add(new Tweet
                    {
                        Id = tweet.Data.Id,
                        Text = tweet.Data.Text,
                        Hashtags = hashtags
                    });

                    await _tweetsContext.SaveChangesAsync();
                }                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
