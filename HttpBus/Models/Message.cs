namespace HttpBus.Models;

public class Message
{
    public int Id { get; set; }

    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public string Topic { get; set; }
    public string Url { get; set; }
    public string Payload { get; set; }
    public DateTime PublishTime { get; set; }

    public bool IsDelivered { get; set; } = false;
    public DateTime? DeliveredTime { get; set; } = null;
}

public class MessageDTO
{
    public string Topic { get; set; }
    public string Payload { get; set; }
}