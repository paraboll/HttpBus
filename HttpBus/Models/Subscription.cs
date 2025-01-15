namespace HttpBus.Models;

/// <summary>
/// Хранит данные о подписчиках, которые хотят получать сообщения.
/// </summary>
public class Subscription
{
    public int Id { get; set; }
    public string Topic { get; set; }
    public string CallbackUrl { get; set; } // URL микросервиса для получения сообщений
}