using HttpBus.Interfaces;
using HttpBus.Models;
using Microsoft.AspNetCore.Mvc;

namespace HttpBus.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscription _subscriptionService;

    public SubscriptionsController(ISubscription subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Позволяет подписаться на Topic.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddSubscribe([FromBody] Subscription subscription)
    {
        await _subscriptionService.AddSubscribeAsync(subscription.Topic, subscription.CallbackUrl);
        return Ok($"Topic: {subscription.Topic}; Add subscribed {subscription.CallbackUrl}.");
    }

    /// <summary>
    /// Позволяет отписаться на Topic.
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> RemoveSubscribe([FromBody] Subscription subscription)
    {
        await _subscriptionService.RemoveSubscribeAsync(subscription.Topic, subscription.CallbackUrl);
        return Ok($"Topic: {subscription.Topic}; Remove subscribed {subscription.CallbackUrl}.");
    }

    /// <summary>
    /// Выводит все подписки.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSubscriptions()
    {
        var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
        return Ok(subscriptions.Select(_ => new
        {
            _.Topic,
            _.CallbackUrl
        }));
    }
}