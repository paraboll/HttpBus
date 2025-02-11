using HttpBus.Interfaces;
using HttpBus.Models;
using HttpBus.Repositories;
using HttpBus.Services;
using Microsoft.EntityFrameworkCore;

namespace HttpBus.Extensions;

public static class ContextServiceCollectionExtension
{
    public static IServiceCollection AddOrderContext(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DbConfig").Get<DbConfig>();
        ConfigureDatabase(services, dbSettings);

        services.AddSingleton<IMessage, MessageService>();
        services.AddScoped<ISubscription, SubscriptionService>();

        if (dbSettings.Provider == DbProviders.InMemory)
            InitializeSubscriptionsAsync(services).GetAwaiter().GetResult();
        
        return services;
    }

    private static void ConfigureDatabase(IServiceCollection services, DbConfig dbSettings)
    {
        services.AddDbContextFactory<DataBusContext>(options =>
        {
            switch (dbSettings.Provider)
            {
                /*case DbProviders.SQLServer:
                    options.UseSqlServer(dbSettings.ConnectionString);
                    break;*/
                case DbProviders.Postgres:
                    options.UseNpgsql(dbSettings.ConnectionString);
                    break;
                case DbProviders.InMemory:
                    options.UseInMemoryDatabase("DataBus");
                    break;
                default:
                    throw new Exception($"Провайдер {dbSettings.Provider} не определён.");
            }
        });
    }

    private static async Task InitializeSubscriptionsAsync(IServiceCollection services)
    {
        using (var provider = services.BuildServiceProvider())
        {
            var subscriptionService = provider.GetRequiredService<ISubscription>() as SubscriptionService;

            await subscriptionService.AddSubscribeAsync("test", "http://172.20.5.3:5555/api/v1/test1");
            await subscriptionService.AddSubscribeAsync("test", "http://172.20.5.3:5555/api/v1/test2");
        }
    }
}