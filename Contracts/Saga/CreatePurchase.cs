using System.Collections.Generic;

namespace Contracts.Saga.CreatePurchase;

// Command types (šta orkestrator traži da servisi urade)
public enum CreatePurchaseCommandType : byte
{
    UpdateReservation = 0,
    RollbackReservation = 1,
    ApprovePurchase = 2,
    CancelPurchase = 3,
    ConfirmReservation = 4
}

// Reply types (šta servisi vraćaju orkestratoru)
public enum CreatePurchaseReplyType : byte
{
    ReservationUpdated = 0,
    ReservationNotUpdated = 1,
    ReservationRolledBack = 2,
    OrderApproved = 3,
    OrderCancelled = 4,
    ReservationConfirmed = 5,
    Unknown = 255
}

public record OrderItemDto(string TourId, string TourName, decimal Price);

public record PurchaseOrderDetails(string Id, string UserId, List<OrderItemDto> Items);

public record CreatePurchaseCommand(CreatePurchaseCommandType Type, PurchaseOrderDetails Order);

public record CreatePurchaseReply(CreatePurchaseReplyType Type, PurchaseOrderDetails Order);
