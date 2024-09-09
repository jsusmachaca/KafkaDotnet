using Service;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
var bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER");

builder.Services.AddSingleton<KafkaState>();
builder.Services.AddHostedService(sp => new KafkaTask(sp.GetRequiredService<KafkaState>(), bootstrapServers));

var app = builder.Build();

app.MapGet("/", (HttpRequest request, HttpResponse response, KafkaState state) =>
{
    response.ContentType = "application/json";

    return $"{{\"status\": \"{state.LastMessage}\"}}";
});

app.Run();
