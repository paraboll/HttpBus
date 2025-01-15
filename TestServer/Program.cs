using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddNLog("NLog.config");
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();

builder.Host.UseWindowsService()
    .ConfigureAppConfiguration((_, config) =>
    {
        _.Configuration.GetSection("Kestrel");
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
