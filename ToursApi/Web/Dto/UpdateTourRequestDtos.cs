using ToursApi.Store.Model;

namespace ToursApi.Web.Dto
{
    public class UpdateTourRequestDtos
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public string[]? Tags { get; set; }
        public decimal? Price { get; set; }
        public TourStatus? Status { get; set; }
    }
}
