using TwitterWorker.Services.Dtos;

namespace TwitterWorker.Services.Interfaces
{
    public interface ITwitterApiService
    {
        Task GetTweetsStreamAsync();
    }
}
