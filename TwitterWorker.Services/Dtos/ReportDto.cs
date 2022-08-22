using TwitterWorker.Domain.Entities;

namespace TwitterWorker.Services.Dtos
{
    public class ReportDto
    {
        public int? TweetsCount { get; set; }
        public IEnumerable<HashtagGroup>? TopTenHashtags { get; set; }
    }
}
