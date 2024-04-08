using iRLeagueApiCore.Common.Enums;

namespace iRLeagueApiCore.Server.Models.Payments;

public record SetLeagueSubscriptionModel(SubscriptionStatus Status, DateTime? Expires);
