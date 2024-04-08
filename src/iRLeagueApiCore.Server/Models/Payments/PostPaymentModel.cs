namespace iRLeagueApiCore.Server.Models.Payments;

public record PostPaymentModel(
    string PlanId,
    string? SubscriptionId,
    string UserId,
    PaymentType PaymentType,
    DateTime Received,
    DateTime? NextDue
);
