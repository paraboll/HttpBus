using HttpBus.Interfaces;
using HttpBus.Models;
using HttpBus.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HttpBus.Services
{
    public class SubscriptionService : ISubscription
    {
        private readonly IDbContextFactory<DataBusContext> _dbFactory;

        public SubscriptionService(IDbContextFactory<DataBusContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task AddSubscribeAsync(string topic, string callbackUrl)
        {
            var subscription = new Subscription { Topic = topic, CallbackUrl = callbackUrl };

            using (var context = await _dbFactory.CreateDbContextAsync())
            {
                await context.Subscriptions.AddAsync(subscription);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveSubscribeAsync(string topic, string callbackUrl)
        {
            using (var context = await _dbFactory.CreateDbContextAsync())
            {
                var subscription = await context.Subscriptions
                    .FirstOrDefaultAsync(_ => _.Topic == topic && _.CallbackUrl == callbackUrl);
                if (subscription != null)
                {
                    context.Subscriptions.Remove(subscription);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            using (var context = await _dbFactory.CreateDbContextAsync())
            {
                return await context.Subscriptions.ToListAsync();
            }
        }
    }
}
