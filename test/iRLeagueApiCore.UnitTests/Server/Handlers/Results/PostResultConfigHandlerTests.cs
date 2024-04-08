using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Results;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Results;

public sealed class PostResultConfigHandlerTests : ResultHandlersTestsBase<PostResultConfigToChampSeasonHandler, PostResultConfigToChampSeasonRequest, ResultConfigModel>
{
    public PostResultConfigHandlerTests() : base()
    {
    }

    protected override PostResultConfigToChampSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostResultConfigToChampSeasonRequest> validator)
    {
        return new PostResultConfigToChampSeasonHandler(logger, dbContext, new IValidator<PostResultConfigToChampSeasonRequest>[] { validator });
    }

    protected override PostResultConfigToChampSeasonRequest DefaultRequest()
    {
        var postResultConfig = new PostResultConfigModel()
        {
            Name = "TestresultConfig",
            DisplayName = "TestResultConfig DisplayName",
            ResultsPerTeam = 10,
        };
        return new PostResultConfigToChampSeasonRequest(TestChampSeasonId, DefaultUser(), postResultConfig);
    }

    protected override void DefaultAssertions(PostResultConfigToChampSeasonRequest request, ResultConfigModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.ResultConfigId.Should().NotBe(0);
        result.Name.Should().Be(expected.Name);
        result.DisplayName.Should().Be(expected.DisplayName);
        result.ResultsPerTeam.Should().Be(expected.ResultsPerTeam);
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public override async Task<ResultConfigModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Fact]
    public async Task Handle_ShouldSetSourceResultConfig_WhenIdIsNotNull()
    {
        var request = DefaultRequest();
        request.Model.SourceResultConfig = new() { ResultConfigId = TestResultConfigId };
        await HandleSpecialAsync(request, async (request, model, dbContext) =>
        {
            model.SourceResultConfig.Should().NotBeNull();
            model.SourceResultConfig!.ResultConfigId.Should().Be(TestResultConfigId);
            var sourceConfig = await dbContext.ResultConfigurations.FirstAsync(x => x.ResultConfigId == model.SourceResultConfig.ResultConfigId);
            model.SourceResultConfig.DisplayName.Should().Be(sourceConfig.DisplayName);
            model.SourceResultConfig.Name.Should().Be(sourceConfig.Name);
            model.SourceResultConfig.LeagueId.Should().Be(sourceConfig.LeagueId);
        });
    }
}
