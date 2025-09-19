namespace ToursApi.Web.Dto
{
    public class CreateTourRequest
    {
        public string title { get; set; } = default!;
        public string description { get; set; } = default!;
        public string difficulty { get; set; } = "easy";
        public string[] tags { get; set; } = Array.Empty<string>();
    }

    public class AddPointRequest
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public string name { get; set; } = default!;
        public string? description { get; set; }
        public string? imageUrl { get; set; }
    }
}
