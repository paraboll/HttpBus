using System.Text;
using System.Threading.Channels;
using HttpBus.Interfaces;
using HttpBus.Models;
using HttpBus.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HttpBus.Services;

public class MessageService : IMessage
{
    private readonly IDbContextFactory<DataBusContext> _dbFactory;
    private readonly Channel<Publication> _publicationChannel;
    private readonly ILogger _logger;

    public MessageService(IDbContextFactory<DataBusContext> dbFactory, ILogger<MessageService> logger)
    {
        _logger = logger;

        _dbFactory = dbFactory;
        _publicationChannel = Channel.CreateUnbounded<Publication>();
        Task.Run(ProcessPublicationAsync);
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var publication = new Publication { Topic = topic, Payload = payload };
        _logger.LogInformation($"PublishAsync: {JsonConvert.SerializeObject(publication)}");

        using var context = await _dbFactory.CreateDbContextAsync();
        await context.Publications.AddAsync(publication);
        await context.SaveChangesAsync();
        
        _publicationChannel.Writer.TryWrite(publication);
    }

    public async Task RepublishByTopicAsync(string topic)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var unsentMessages = await context.Messages
            .Where(_ => !_.IsDelivered && _.Topic == topic)
            .OrderBy(dl => dl.PublishTime)
            .ToListAsync();

        foreach (var message in unsentMessages)
        { 
            await SendMessageToSubscriber(message); 
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Publication>> GetAllPublicationsAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Publications.ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesByTopicAsync(string topic)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Messages
            .Where(_ => _.Topic == topic)
            .ToListAsync();
    }

    public async Task<StatisticDTO> GetStatisticsAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var publishCount = await context.Publications.CountAsync();
        var messagesStats = await context.Messages
            .GroupBy(_ => _.IsDelivered)
            .Select(g => new
            {
                IsDelivered = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var deliveredCount = messagesStats.FirstOrDefault(_ => _.IsDelivered)?.Count ?? 0;
        var notDeliveredCount = messagesStats.FirstOrDefault(_ => !_.IsDelivered)?.Count ?? 0;

        return new StatisticDTO
        {
            Publish = publishCount,
            Delivered = deliveredCount,
            NotDelivered = notDeliveredCount
        };
    }

    #region PrivateMethods

    private async Task ProcessPublicationAsync()
    {
        while (await _publicationChannel.Reader.WaitToReadAsync())
        {
            if (_publicationChannel.Reader.TryRead(out var publication))
                await DeliverMessageAsync(publication);
            
        }
    }

    private async Task DeliverMessageAsync(Publication publication)
    {
        _logger.LogInformation($"DeliverMessageAsync: {JsonConvert.SerializeObject(publication)}");
        using var context = await _dbFactory.CreateDbContextAsync();
        var subscribers = await context.Subscriptions
            .Where(s => s.Topic == publication.Topic)
            .ToListAsync();

        foreach (var subscriber in subscribers)
        {
            var message = 
                await context.Messages.AddAsync(new Message()
                {
                    Url = subscriber.CallbackUrl,
                    Topic = publication.Topic,
                    Payload = publication.Payload,
                    PublishTime = publication.CreateTime
                });
            
            await SendMessageToSubscriber(message.Entity);
            await context.SaveChangesAsync();
        }
    }

    private async Task SendMessageToSubscriber(Message message)
    {
        _logger.LogInformation($"SendMessageToSubscriber: {JsonConvert.SerializeObject(message)}");
        var body = JsonConvert.SerializeObject(new MessageRequest { Message = message.Payload});
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.PostAsync(message.Url, content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

            message.DeliveredTime = DateTime.UtcNow;
            message.IsDelivered = true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending message {message.ExternalId}: {ex.Message}");
        }
    }

    #endregion
}