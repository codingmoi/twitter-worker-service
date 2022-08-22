using TwitterWorker.Services.Interfaces;

namespace TwitterWorkerService.Workers
{
    public class TwitterReportingWorker : BackgroundService
    {
        private const string TwitterApiReportingMiliseconds = "TwitterApi:ReportingMiliseconds";

        private readonly IConfiguration _configuration;
        private readonly ILogger<TwitterReportingWorker> _logger;
        private readonly ITwitterReportingService _twitterReportingService;
        

        public TwitterReportingWorker(ILogger<TwitterReportingWorker> logger, 
            ITwitterReportingService twitterReportingService,
            IConfiguration configuration)
        {
            _logger = logger;
            _twitterReportingService = twitterReportingService;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TwitterReportingWorker running at: {time}", DateTimeOffset.Now);
            var waitTime = _configuration.GetValue<int>(TwitterApiReportingMiliseconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var report = await _twitterReportingService.UpdateStatisticsAsync();

                Console.WriteLine("======= REPORTING {0} =======", DateTimeOffset.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                Console.WriteLine($"TWEETS COUNT {report.TweetsCount}");
                if (report.TweetsCount > 0 && report.TopTenHashtags != null && report.TopTenHashtags.Any())
                {
                    Console.WriteLine($"TOP 10 HASHTAGS");
                    int position = 1;
                    foreach (var hashtag in report.TopTenHashtags)
                    {
                        Console.WriteLine($"{position++}. {hashtag.Tag} ({hashtag.Count})");
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(waitTime), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Twitter Reporting Worker service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
