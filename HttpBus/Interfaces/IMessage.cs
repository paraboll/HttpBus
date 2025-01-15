using HttpBus.Models;

namespace HttpBus.Interfaces;

public interface IMessage
{
    Task PublishAsync(string topic, string payload);
    Task RepublishByTopicAsync(string topic);
    Task<IEnumerable<Publication>> GetAllPublicationsAsync();
    Task<IEnumerable<Message>> GetMessagesByTopicAsync(string topic);
}

