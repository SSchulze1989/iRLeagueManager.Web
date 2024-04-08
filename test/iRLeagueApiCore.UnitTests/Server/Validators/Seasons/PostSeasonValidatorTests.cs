using FluentValidation.TestHelper;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.Server.Validation.Seasons;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Validators.Seasons;

public sealed class PostSeasonValidatorTests : DataAccessTestsBase
{
    private const string testSeasonName = "TestSeason";

    private PostSeasonRequest DefaultRequest()
    {
        var model = new PostSeasonModel()
        {
            HideComments = true,
            MainScoringId = null,
            Finished = true,
            SeasonName = testSeasonName,
        };
        return new PostSeasonRequest(LeagueUser.Empty, model);
    }

    private static PostSeasonRequestValidator CreateValidator(LeagueDbContext dbContext)
    {
        return new PostSeasonRequestValidator(dbContext, new PostSeasonModelValidator());
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
