using TwitterWorker.Services.Dtos;

namespace TwitterWorker.Services.Interfaces
{
    public interface ITwitterReportingService
    {
        Task<ReportDto> UpdateStatisticsAsync();
    }
}
