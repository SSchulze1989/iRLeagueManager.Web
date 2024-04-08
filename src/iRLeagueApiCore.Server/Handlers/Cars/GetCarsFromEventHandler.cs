using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueApiCore.Services.ResultService.Extensions;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Cars;

public record GetCarsFromEventRequest(long EventId) : IRequest<CarListModel>;

public class GetCarsFromEventHandler : HandlerBase<GetCarsFromEventHandler, GetCarsFromEventRequest>, 
    IRequestHandler<GetCarsFromEventRequest, CarListModel>
{
    public GetCarsFromEventHandler(ILogger<GetCarsFromEventHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetCarsFromEventRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<CarListModel> Handle(GetCarsFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken).ConfigureAwait(false);
        var eventCars = (await GetEventCarsPerMemberAsync(request.EventId, cancellationToken));
        var groupedCars = eventCars
            .GroupBy(x => new { x.Number, x.Team?.TeamId })
            .Select(x => new EventCarInfoModel()
            {
                Number = x.First().Number,
                Car = x.First().Car,
                CarId = x.First().CarId,
                Class = x.First().Class,
                Members = x.SelectMany(x => x.Members),
                Team = x.First().Team,
            });
        bool isTeamEvent = groupedCars.Any(x => x.Members.Count() > 1) && groupedCars.None(x => x.Team == null);
        return new CarListModel()
        {
            IsTeamEvent = isTeamEvent,
            Cars = isTeamEvent ? groupedCars : eventCars,
        };
    }

    private async Task<IEnumerable<EventCarInfoModel>> GetEventCarsPerMemberAsync(long eventId, CancellationToken cancellationToken)
    {
        var memberCarRows = await dbContext.EventResults
            .Where(x => x.EventId == eventId)
            .SelectMany(x => x.SessionResults)
            .SelectMany(x => x.ResultRows)
            .Select(MapToCarInfoModelExpression)
            .ToListAsync(cancellationToken);
        return memberCarRows
            .DistinctBy(x => x.MemberId)
            .Select(x => x.EventCar);
    }

    private static Expression<Func<ResultRowEntity, MemberCarInfo>> MapToCarInfoModelExpression => row => new(
        row.MemberId,
        new() {
                Car = row.Car,
            CarId = row.CarId,
            Class = row.CarClass,
            Members = new[] { new MemberInfoModel()
                {
                    MemberId = row.MemberId,
                    FirstName = row.Member.Firstname,
                    LastName = row.Member.Lastname,
                } 
            },
            Team = row.Team == null ? default : new()
            {
                TeamId = row.Team.TeamId,
                Name = row.Team.Name,
                TeamColor = row.Team.TeamColor,
            },
            Number = row.CarNumber,
        }
    );

    private record MemberCarInfo(long MemberId, EventCarInfoModel EventCar);
}
