namespace iRLeagueApiCore.Server.Models.Payments;

public record PaymentModel(
    Guid Id,
    PaymentType PaymentType,
    string PlanId,
    string PlanName,
    SubscriptionInterval Interval,
    string SubscriptionId,
    long LeagueId,
    string UserId,
    DateTime LastPayment,
    DateTime? NextPayment,
    PaymentStatus Status);
