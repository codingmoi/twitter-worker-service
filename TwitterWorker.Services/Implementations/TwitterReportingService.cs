using Microsoft.Extensions.Logging;
using TwitterWorker.Domain.Interfaces;
using TwitterWorker.Services.Dtos;
using TwitterWorker.Services.Interfaces;

namespace TwitterWorker.Services.Implementations
{
    public class TwitterReportingService : ITwitterReportingService
    {
        private readonly ILogger<TwitterReportingService> _logger;
        private readonly ITweetRepository _tweetRepository;
        private readonly IHashtagRepository _hashtagRepository;

        public TwitterReportingService(ILogger<TwitterReportingService> logger, 
            ITweetRepository tweetRepository, 
            IHashtagRepository hashtagRepository)
        {
            _logger = logger;
            _tweetRepository = tweetRepository;
            _hashtagRepository = hashtagRepository;
        }

        public async Task<ReportDto> UpdateStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Updating Report Statistics");
                return new ReportDto
                {
                    TopTenHashtags = await _hashtagRepository.GetTopTenHashtagsAsync(),
                    TweetsCount = await _tweetRepository.GetTweetsCount()
                };
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
