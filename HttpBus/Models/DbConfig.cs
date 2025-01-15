namespace HttpBus.Models
{
    public class Config
    {
        public DbConfig DbConfig { get; set; }
    }

    public class DbConfig
    {
        public DbProviders Provider { get; set; }
        public string ConnectionString { get; set; }
    }

    public enum DbProviders
    {
        SQLServer,
        Postgres,
        InMemory
    }
}
