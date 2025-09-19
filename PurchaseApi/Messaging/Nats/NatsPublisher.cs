using NATS.Client.Core;
using System.Text.Json;

namespace PurchaseApi.Messaging.Nats
{
    public sealed class NatsPublisher : IPublisher
    {
        private readonly NatsConnection _conn;
        public NatsPublisher(NatsConnection conn) => _conn = conn;
        public async Task PublishAsync<T>(string subject, T message)
            => await _conn.PublishAsync(subject, JsonSerializer.SerializeToUtf8Bytes(message));
    }
}
