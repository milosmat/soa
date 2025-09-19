using Contracts.Saga.CreatePurchase;
using ToursApi.Messaging;
using MongoDB.Driver;
using ToursApi.Store;

namespace ToursApi.Saga;

public sealed class ReservationCommandHandler
{
    private readonly ISubscriber _subscriber;
    private readonly IPublisher _publisher;
    private readonly AppDb _db;
    private readonly string _cmdSubject;
    private readonly string _replySubject;

    public ReservationCommandHandler(ISubscriber sub, IPublisher pub, AppDb db, IConfiguration cfg)
    {
        _subscriber = sub; _publisher = pub; _db = db;
        _cmdSubject = cfg["RESERVATION_COMMAND_SUBJECT"] ?? "tour.reservation.command";
        _replySubject = cfg["RESERVATION_REPLY_SUBJECT"] ?? "tour.reservation.reply";
    }

    public async Task StartAsync()
    {
        await _subscriber.SubscribeAsync<CreatePurchaseCommand>(_cmdSubject, Handle, queueGroup: "tours");
    }

    private async Task Handle(CreatePurchaseCommand cmd)
    {
        var order = cmd.Order;

        if (cmd.Type == CreatePurchaseCommandType.UpdateReservation)
        {
            // pokušaj rezervacije svih tura (idempotentno + state-check)
            var ids = order.Items.Select(i => i.TourId).ToList();
            var filter = Builders<Store.Model.Tour>.Filter.Where(t =>
                ids.Contains(t.Id) && t.status == Store.Model.TourStatus.published &&
                t.IsReserved == false && t.IsCancelled == false);

            var update = Builders<Store.Model.Tour>.Update.Set(t => t.IsReserved, true);
            var result = await _db.Tours.UpdateManyAsync(filter, update);

            if (result.ModifiedCount == ids.Count)
            {
                await _publisher.PublishAsync(_replySubject,
                    new CreatePurchaseReply(CreatePurchaseReplyType.ReservationUpdated, order));
            }
            else
            {
                // rollback bilo čega što je možda prošlo
                var rbFilter = Builders<Store.Model.Tour>.Filter.Where(t => ids.Contains(t.Id) && t.IsReserved == true);
                var rbUpdate = Builders<Store.Model.Tour>.Update.Set(t => t.IsReserved, false);
                await _db.Tours.UpdateManyAsync(rbFilter, rbUpdate);

                await _publisher.PublishAsync(_replySubject,
                    new CreatePurchaseReply(CreatePurchaseReplyType.ReservationNotUpdated, order));
            }
        }
        else if (cmd.Type == CreatePurchaseCommandType.ConfirmReservation)
        {
            var ids = order.Items.Select(i => i.TourId).ToList();

            // potvrdi samo ture koje su bile rezervisane i nisu otkazane
            var filter = Builders<Store.Model.Tour>.Filter.Where(t =>
                ids.Contains(t.Id) && t.IsReserved == true && t.IsCancelled == false);

            var update = Builders<Store.Model.Tour>.Update
                .Set(t => t.IsReserved, false)   // više nisu rezervisane
                .Set(t => t.IsCancelled, false); // i sigurno nisu otkazane

            await _db.Tours.UpdateManyAsync(filter, update);

            await _publisher.PublishAsync(_replySubject,
                new CreatePurchaseReply(CreatePurchaseReplyType.ReservationConfirmed, order));
        }
        else if (cmd.Type == CreatePurchaseCommandType.RollbackReservation)
        {
            var ids = order.Items.Select(i => i.TourId).ToList();
            var filter = Builders<Store.Model.Tour>.Filter.Where(t => ids.Contains(t.Id) && t.IsReserved == true);
            var update = Builders<Store.Model.Tour>.Update.Set(t => t.IsReserved, false);
            await _db.Tours.UpdateManyAsync(filter, update);

            await _publisher.PublishAsync(_replySubject,
                new CreatePurchaseReply(CreatePurchaseReplyType.ReservationRolledBack, order));
        }
    }
}
