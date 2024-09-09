using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Service
{
public class KafkaTask : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly KafkaState _state;
    private Task _executingTask;
    private CancellationTokenSource _cts;
    private readonly string _bootstrapServers;

    public KafkaTask(KafkaState state, string bootstrapServers)
    {
        _state = state;
        _bootstrapServers = bootstrapServers;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = Task.Run(() => ConsumeMessages(_cts.Token), _cts.Token);
        return Task.CompletedTask;
    }

    private async Task ConsumeMessages(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "test-consumer",
            BootstrapServers = _bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("test");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = consumer.Consume(cancellationToken);
                    Console.WriteLine(consumerResult.Message.Value);
                    _state.LastMessage = consumerResult.Message.Value;
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
                }

                await Task.Delay(100, cancellationToken);
            }
        }
        finally
        {
            consumer.Close();
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
}