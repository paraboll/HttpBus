using HttpBus.Interfaces;
using HttpBus.Models;
using Microsoft.AspNetCore.Mvc;

namespace HttpBus.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessage _messageService;

    public MessagesController(IMessage messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Позволяет публиковать сообщения.
    /// </summary>
    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] MessageDTO message)
    {
        //await _messageService.PublishAsync(message.Topic, message.Payload);
        Task.Run(() => _messageService.PublishAsync(message.Topic, message.Payload));
        return Ok($"Topic: {message.Topic};Message: {message.Payload} published.");
    }

    /// <summary>
    /// Позволяет переопубликовать сообщения.
    /// </summary>
    [HttpPut("publish/{topic}")]
    public async Task<IActionResult> Republish(string topic)
    {
        //await _messageService.RepublishByTopicAsync(topic);
        Task.Run(() => _messageService.RepublishByTopicAsync(topic));
        return Ok($"Topic: {topic} republished.");
    }

    /// <summary>
    /// Выводит все публикации.
    /// </summary>
    [HttpGet("publish")]
    public async Task<IActionResult> GetPublications()
    {
        var publications = await _messageService.GetAllPublicationsAsync();
        return Ok(publications.Select(_ => new
        {
            _.Topic,
            _.Payload,
            _.CreateTime
        }));
    }

    /// <summary>
    /// Выводит все сообщения по topic.
    /// </summary>
    [HttpGet("publish/{topic}")]
    public async Task<IActionResult> GetMessages(string topic)
    {
        var messages = await _messageService.GetMessagesByTopicAsync(topic);
        return Ok(messages.Select(_ => new
        {
            _.ExternalId,
            _.Payload,
            _.IsDelivered,
            _.DeliveredTime,
            _.Url
        }));
    }
}