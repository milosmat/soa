using NATS.Client.Core;
using System.Text.Json;

namespace ToursApi.Messaging.Nats
{
    public sealed class NatsSubscriber : ISubscriber
    {
        private readonly NatsConnection _conn;
        public NatsSubscriber(NatsConnection conn) => _conn = conn;

        public Task SubscribeAsync<T>(string subject, Func<T, Task> handler, string? queueGroup = null)
        {
            _ = Task.Run(async () =>
            {
                await foreach (var msg in _conn.SubscribeAsync<byte[]>(subject, queueGroup: queueGroup))
                {
                    try
                    {
                        var payload = JsonSerializer.Deserialize<T>(msg.Data)!;
                        await handler(payload);
                    }
                    catch
                    {
                        // TODO: log error
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
