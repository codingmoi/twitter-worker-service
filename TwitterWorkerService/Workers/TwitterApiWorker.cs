using TwitterWorker.Services.Interfaces;

namespace TwitterWorkerService.Workers
{
    public class TwitterApiWorker : BackgroundService
    {
        private readonly ILogger<TwitterApiWorker> _logger;
        private readonly ITwitterApiService _twitterApiService;
        private readonly ITwitterReportingService _twitterReportingService;

        public TwitterApiWorker(ILogger<TwitterApiWorker> logger, ITwitterApiService twitterApiService, ITwitterReportingService twitterReportingService)
        {
            _logger = logger;
            _twitterApiService = twitterApiService;
            _twitterReportingService = twitterReportingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {        
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await _twitterApiService.GetTweetsStreamAsync();

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Twitter Api Worker service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}