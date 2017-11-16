namespace SaluteOnline.Domain.DTO.InnerMessage
{
    public class InnerMessagesFilter : BaseFilter
    {
        public int ReceiverId { get; set; }
        public EntityType ReceiverType { get; set; }
        public MessageStatus Status { get; set; }
    }
}
