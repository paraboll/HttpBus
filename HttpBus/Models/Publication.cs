namespace HttpBus.Models
{
    public class Publication
    {
        public int Id { get; set; }
        public Guid InternalId { get; set; } = Guid.NewGuid();
        public string Topic { get; set; }
        public string Payload { get; set; } //Содержимое сообщения.
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}
