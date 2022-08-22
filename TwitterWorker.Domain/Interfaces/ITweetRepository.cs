namespace TwitterWorker.Domain.Interfaces
{
    public interface ITweetRepository
    {
        Task<int> GetTweetsCount();
    }
}
