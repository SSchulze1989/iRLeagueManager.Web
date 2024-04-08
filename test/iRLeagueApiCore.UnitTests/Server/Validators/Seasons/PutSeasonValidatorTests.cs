using FluentValidation.TestHelper;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.Server.Validation.Seasons;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Validators.Seasons;

public sealed class PutSeasonValidatorTests : DataAccessTestsBase
{
    private const long testLeagueId = 1;
    private const long testSeasonId = 1;
    private const string testSeasonName = "TestSeason";

    private static PutSeasonRequest DefaultRequest(long seasonId = testSeasonId)
    {
        var model = new PutSeasonModel()
        {
            HideComments = true,
            MainScoringId = null,
            Finished = true,
            SeasonName = testSeasonName,
        };
        return new PutSeasonRequest(LeagueUser.Empty, seasonId, model);
    }

    private static PutSeasonRequestValidator CreateValidator(LeagueDbContext dbContext)
    {
        return new PutSeasonRequestValidator(dbContext, new PutSeasonModelValidator(new()));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public async Task ValidateSeasonId(long seasonId, bool expectValid)
    {
        var request = DefaultRequest(seasonId: seasonId);
        var validator = CreateValidator(dbContext);
        var result = await validator.TestValidateAsync(request);
        Assert.Equal(expectValid, result.IsValid);
        if (expectValid == false)
        {
            result.ShouldHaveValidationErrorFor(x => x.SeasonId);
        }
    }

    [Theory]
    [InlineData("ValidName", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public async Task ValidateSeasonName(string name, bool expectValid)
    {
        var request = DefaultRequest();
        request.Model.SeasonName = name;
        var validator = CreateValidator(dbContext);
        var result = await validator.TestValidateAsync(request);
        Assert.Equal(expectValid, result.IsValid);
        if (expectValid == false)
        {
            result.ShouldHaveValidationErrorFor(x => x.Model.SeasonName);
        }
    }
}
