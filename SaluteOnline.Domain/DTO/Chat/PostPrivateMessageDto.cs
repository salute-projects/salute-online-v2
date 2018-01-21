namespace SaluteOnline.Domain.DTO.Chat
{
    public class PostPrivateMessageDto
    {
        public int SenderId { get; set; }
        public EntityType SenderType { get; set; }
        public int ReceiverId { get; set; }
        public EntityType ReceiverType { get; set; }
        public string Message { get; set; }
    }
}
