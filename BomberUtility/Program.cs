using Newtonsoft.Json;
using NLog;

namespace BomberUtility;

class Program
{
    private static HttpClient client = new HttpClient();
    private static Logger log = LogManager.GetCurrentClassLogger();

    static async Task Main(string[] args)
    {
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        client.BaseAddress = new Uri(config.BaseURL);

        var tasks = Enumerable.Range(0, config.RequestCount)
            .AsParallel()
            .WithDegreeOfParallelism(config.ParallelThread)
            .Select(i => SendPostRequest(i))
            .ToArray();

        await Task.WhenAll(tasks);
        log.Debug("Все запросы отправлены.");
    }

    private static async Task SendPostRequest(int count)
    {
        log.Debug($"Запрос {count} создан");
        await Task.Delay(1_000);
        var body = new
        {
            topic = "test",
            payload = $"string {count}"
        };

        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync("/api/messages/publish", content);
            if (!response.IsSuccessStatusCode)
            {
                log.Debug($"Запрос {count} отклонён, код статуса: {response.StatusCode}");
            }
        }
        catch (Exception exc)
        {
             log.Debug($"Запрос {count} ошибка: {exc.Message}");
        }
    }
}
