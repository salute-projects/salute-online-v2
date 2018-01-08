namespace SaluteOnline.Domain.DTO.InnerMessage
{
    public class InnerMessageSenderSummary
    {
        public int? SenderId { get; set; }
        public EntityType SenderType { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
    }
}
