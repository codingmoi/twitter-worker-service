using TwitterWorker.Services.Implementations;
using TwitterWorker.Services.Interfaces;
using TwitterWorkerService.Workers;
using TwitterWorker.Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using TwitterWorker.Domain.Interfaces;
using TwitterWorker.Infrastructure.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddDbContext<TweetsContext>(options => options.UseInMemoryDatabase(databaseName: "tweets_db"));
        services.AddTransient<ITwitterApiService, TwitterApiService>();
        services.AddTransient<ITwitterReportingService, TwitterReportingService>();
        services.AddTransient<ITweetRepository, TweetRepository>();
        services.AddTransient<IHashtagRepository, HashtagRepository>();
        services.AddHostedService<TwitterApiWorker>();
        services.AddHostedService<TwitterReportingWorker>();
    })
    .Build();

await host.RunAsync();
