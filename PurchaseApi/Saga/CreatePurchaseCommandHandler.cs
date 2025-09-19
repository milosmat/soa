using Contracts.Saga.CreatePurchase;
using PurchaseApi.Messaging;
using MongoDB.Driver;
using PurchaseApi.Store;
using PurchaseApi.Store.Model;

namespace PurchaseApi.Saga;

public sealed class CreatePurchaseCommandHandler
{
    private readonly ISubscriber _subscriber;
    private readonly IPublisher _publisher;
    private readonly AppDb _db;
    private readonly string _cmdSubject;
    private readonly string _replySubject;

    public CreatePurchaseCommandHandler(ISubscriber sub, IPublisher pub, AppDb db, IConfiguration cfg)
    {
        _subscriber = sub; _publisher = pub; _db = db;
        _cmdSubject = cfg["CREATE_PURCHASE_COMMAND_SUBJECT"] ?? "purchase.create.command";
        _replySubject = cfg["CREATE_PURCHASE_REPLY_SUBJECT"] ?? "purchase.create.reply";
    }

    public async Task StartAsync()
    {
        await _subscriber.SubscribeAsync<CreatePurchaseCommand>(_cmdSubject, Handle, queueGroup: "purchase");
    }

    private async Task Handle(CreatePurchaseCommand command)
    {
        var order = command.Order;

        switch (command.Type)
        {
            case CreatePurchaseCommandType.ApprovePurchase:
                // 1) upiši tokene (idempotentno; postoji unique indeks)
                var tokens = order.Items.Select(i => new TourPurchaseToken
                {
                    UserId = order.UserId,
                    TourId = i.TourId
                }).ToList();

                try { await _db.Purchases.InsertManyAsync(tokens, new InsertManyOptions { IsOrdered = false }); }
                catch { /* duplicate ok */ }

                // 2) ISPRAZNI KORPU za tog usera 
                var clear = Builders<ShoppingCart>.Update.Set(c => c.Items, new List<OrderItem>());
                await _db.Carts.UpdateOneAsync(
                    c => c.UserId == order.UserId,
                    clear,
                    new UpdateOptions { IsUpsert = true } // ako slučajno nema dokumenta, napravi prazan
                );

                // 3) reply
                await _publisher.PublishAsync(_replySubject,
                    new CreatePurchaseReply(CreatePurchaseReplyType.OrderApproved, order));
                break;


            case CreatePurchaseCommandType.CancelPurchase:
                // ako si ranije nešto upisao, možeš čistiti (DeleteMany)
                await _publisher.PublishAsync(_replySubject,
                    new CreatePurchaseReply(CreatePurchaseReplyType.OrderCancelled, order));
                break;
        }
    }
}
