namespace SaluteOnline.Domain.DTO
{
    public class EntityFilter : BaseFilter
    {
        public int? EntityId { get; set; }
        public string SearchBy { get; set; }
    }
}
