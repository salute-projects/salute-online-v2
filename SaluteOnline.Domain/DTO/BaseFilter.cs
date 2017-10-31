namespace SaluteOnline.Domain.DTO
{
    public class BaseFilter
    {
        public int? PageSize { get; set; }
        public int Page { get; set; }
        public bool Asc { get; set; }
    }
}
