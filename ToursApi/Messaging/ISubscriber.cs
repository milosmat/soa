namespace ToursApi.Messaging
{
    public interface ISubscriber { Task SubscribeAsync<T>(string subject, Func<T, Task> handler, string? queueGroup = null); }
}
