namespace WhatsappWebHook.Models
{
    public class WhatsAppMessage
    {
        public int Id { get; set; }
        public string? MessageSid { get; set; }
        public string? AccountSid { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Body { get; set; }
        public string? WaId { get; set; }
        public string? ProfileName { get; set; }
        public int NumMedia { get; set; }
        public string? ApiVersion { get; set; }
        public string? SmsStatus { get; set; }
        public string? SmsMessageSid { get; set; }
        public int NumSegments { get; set; }
        public string? MessageType { get; set; }
        public string? ChannelMetadata { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string? MediaContentType { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaSid { get; set; }
    }
}
