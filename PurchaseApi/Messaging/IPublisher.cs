namespace PurchaseApi.Messaging
{
    public interface IPublisher { Task PublishAsync<T>(string subject, T message); }
}
