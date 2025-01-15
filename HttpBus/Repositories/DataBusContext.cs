using HttpBus.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpBus.Repositories;
public class DataBusContext : DbContext
{
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public DataBusContext(DbContextOptions<DataBusContext> options) : base(options) { }
}
