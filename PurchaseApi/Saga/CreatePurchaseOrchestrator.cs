using Contracts.Saga.CreatePurchase;
using PurchaseApi.Messaging;

namespace PurchaseApi.Saga;

public sealed class CreatePurchaseOrchestrator
{
    private readonly IPublisher _publisher;
    private readonly ISubscriber _subscriber;
    private readonly string _cmdSubject;
    private readonly string _replySubject;
    private readonly string _tourCmdSubject;
    private readonly string _tourReplySubject;

    public CreatePurchaseOrchestrator(IPublisher publisher, ISubscriber subscriber, IConfiguration cfg)
    {
        _publisher = publisher;
        _subscriber = subscriber;
        _cmdSubject = cfg["CREATE_PURCHASE_COMMAND_SUBJECT"] ?? "purchase.create.command";
        _replySubject = cfg["CREATE_PURCHASE_REPLY_SUBJECT"] ?? "purchase.create.reply";
        _tourCmdSubject = cfg["RESERVATION_COMMAND_SUBJECT"] ?? "tour.reservation.command";
        _tourReplySubject = cfg["RESERVATION_REPLY_SUBJECT"] ?? "tour.reservation.reply";
    }

    public async Task StartAsync()
    {
        // sluša REPLY-e specifično za ovaj korak sage
        await _subscriber.SubscribeAsync<CreatePurchaseReply>(_replySubject, HandleReply, queueGroup: "purchase");
        await _subscriber.SubscribeAsync<CreatePurchaseReply>(_tourReplySubject, HandleTourReply, queueGroup: "purchase");
    }

    // 1) pokretanje sage
    public async Task BeginAsync(PurchaseOrderDetails order)
    {
        var cmd = new CreatePurchaseCommand(CreatePurchaseCommandType.UpdateReservation, order);
        await _publisher.PublishAsync(_tourCmdSubject, cmd); // tražimo rezervaciju u Tours servisu
    }

    // 2) odgovor iz Tours servisa
    private async Task HandleTourReply(CreatePurchaseReply reply)
    {
        switch (reply.Type)
        {
            case CreatePurchaseReplyType.ReservationUpdated:
                // traži od Purchase servisa da odobri narudžbu (npr. upis tokena, pražnjenje korpe)
                await _publisher.PublishAsync(_cmdSubject,
                    new CreatePurchaseCommand(CreatePurchaseCommandType.ApprovePurchase, reply.Order));
                break;

            case CreatePurchaseReplyType.ReservationNotUpdated:
                // rollback / cancel
                await _publisher.PublishAsync(_cmdSubject,
                    new CreatePurchaseCommand(CreatePurchaseCommandType.CancelPurchase, reply.Order));
                break;

            case CreatePurchaseReplyType.ReservationRolledBack:
                await _publisher.PublishAsync(_cmdSubject,
                    new CreatePurchaseCommand(CreatePurchaseCommandType.CancelPurchase, reply.Order));
                break;

            default:
                await _publisher.PublishAsync(_cmdSubject,
                    new CreatePurchaseCommand(CreatePurchaseCommandType.CancelPurchase, reply.Order));
                break;
        }
    }

    // 3) odgovor iz purchase handlera (approve/cancel)
    private async Task HandleReply(CreatePurchaseReply reply)
    {
        switch (reply.Type)
        {
            case CreatePurchaseReplyType.OrderApproved:
                // ⬇️ tražimo finalno potvrđivanje rezervacija u Tours
                await _publisher.PublishAsync(_tourCmdSubject,
                    new CreatePurchaseCommand(CreatePurchaseCommandType.ConfirmReservation, reply.Order));
                break;

            case CreatePurchaseReplyType.OrderCancelled:
                // gotova kompenzacija
                break;
        }
    }
}
