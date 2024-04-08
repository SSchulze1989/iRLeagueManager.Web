using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class TeamSessionCalculationService : CalculationServiceBase
{
    private readonly SessionCalculationConfiguration config;

    public TeamSessionCalculationService(SessionCalculationConfiguration config)
    {
        this.config = config;
    }

    public override Task<SessionCalculationResult> Calculate(SessionCalculationData data)
    {
        var memberRows = data.ResultRows.Select(x => new ResultRowCalculationResult(x))
            .OrderBy(x => x.FinalPosition);
        var teamRowGroups = memberRows
            .GroupBy(x => x.TeamId)
            .Where(x => x.Key != null);
        // do not aggregate driver points if all members of all teams have the same race points, usually indicating a team based points value
        bool aggregateDriverPoints = teamRowGroups
            .Select(x => x.DistinctBy(y => y.RacePoints).Count())
            .Any(x => x > 1);
        var teamRows = teamRowGroups
            .Select(x => GetTeamResultRow(x, aggregateDriverPoints))
            .NotNull()
            .ToList();
        var pointRule = config.PointRule;
        var finalRows = ApplyPoints(teamRows, pointRule, data);

        var result = new SessionCalculationResult(data)
        {
            Name = config.Name,
            SessionResultId = config.SessionResultId,
            ResultRows = finalRows,
            SessionNr = data.SessionNr
        };

        return Task.FromResult(result);
    }

    private ResultRowCalculationResult? GetTeamResultRow(IEnumerable<ResultRowCalculationResult> teamMemberRows, bool aggregateDriverRows)
    {
        // 2. Keep two best driver results
        teamMemberRows = teamMemberRows.Take(config.MaxResultsPerGroup);
        if (teamMemberRows.Any() == false)
        {
            return null;
        }
        // 3. Accumulate results
        var dataRow = teamMemberRows.First();
        var teamRow = new ResultRowCalculationResult()
        {
            TeamId = dataRow.TeamId,
            TeamColor = dataRow.TeamColor,
            TeamName = dataRow.TeamName,
            ScoredResultRowId = null,
            MemberId = null,
            Firstname = string.Empty,
            Lastname = string.Empty,
            Car = dataRow.Car,
            CarClass = dataRow.CarClass,
            CarId = dataRow.CarId,
            CarNumber = dataRow.CarNumber,
            ClassId = dataRow.ClassId,
            Status = dataRow.Status,
            StartPosition = dataRow.StartPosition,
            FinishPosition = dataRow.FinishPosition,
            FinalPosition = dataRow.FinalPosition,
            AddPenalties = dataRow.AddPenalties,
        };
        foreach (var memberRow in teamMemberRows)
        {
            teamRow.CompletedLaps += memberRow.CompletedLaps;
            teamRow.LeadLaps += memberRow.LeadLaps;
            teamRow.Incidents += memberRow.Incidents;
            teamRow.Interval += memberRow.Interval;
            if (aggregateDriverRows)
            {
                teamRow.RacePoints += memberRow.RacePoints + memberRow.BonusPoints;
            }
            else
            {
                teamRow.RacePoints = dataRow.RacePoints + dataRow.BonusPoints;
            }
            teamRow.PenaltyPoints += memberRow.PenaltyPoints;
            teamRow.PointsEligible |= memberRow.PointsEligible;
            teamRow.AddPenalties = teamRow.AddPenalties.Concat(memberRow.AddPenalties.Where(x => x.MemberId != null));
        }
        teamRow.StartPosition = teamMemberRows.Min(x => x.StartPosition);
        (_, teamRow.QualifyingTime) = GetBestLapValue(teamMemberRows, x => x.MemberId, x => x.QualifyingTime);
        (_, teamRow.FastestLapTime) = GetBestLapValue(teamMemberRows, x => x.MemberId, x => x.FastestLapTime);
        teamRow.AvgLapTime = GetAverageLapValue(teamMemberRows, x => x.AvgLapTime, x => x.CompletedLaps);
        teamRow.ScoredMemberResultRowIds = teamMemberRows
            .Select(x => x.ScoredResultRowId)
            .NotNull()
            .ToList();

        return teamRow;
    }
}
