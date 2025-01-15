using HttpBus.Models;

namespace HttpBus.Interfaces;

public interface ISubscription
{
      Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
      Task AddSubscribeAsync(string topic, string callbackUrl);
      Task RemoveSubscribeAsync(string topic, string callbackUrl);
}