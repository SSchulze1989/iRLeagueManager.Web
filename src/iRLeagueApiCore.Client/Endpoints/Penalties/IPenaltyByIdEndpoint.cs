using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Penalties;
public interface IPenaltyByIdEndpoint : IPutEndpoint<PenaltyModel, PutPenaltyModel>, IDeleteEndpoint
{
}
