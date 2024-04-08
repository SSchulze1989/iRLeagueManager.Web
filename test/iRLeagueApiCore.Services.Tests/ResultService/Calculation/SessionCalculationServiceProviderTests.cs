using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class SessionCalculationServiceProviderTests
{
    private readonly Fixture fixture;

    public SessionCalculationServiceProviderTests()
    {
        fixture = new();
    }

    [Fact]
    public void GetCalculationService_ShouldProvideMemberCalculationService_WhenScoringKindIsMember()
    {
        var config = GetCalculationConfiguration();
        config.ResultKind = ResultKind.Member;
        var sut = CreateSut();

        var test = sut.GetCalculationService(config);

        test.Should().BeOfType<MemberSessionCalculationService>();
    }

    [Fact]
    public void GetCalculationService_ShouldProvideTeamCalculationService_WhenScoringKindIsTeam()
    {
        var config = GetCalculationConfiguration();
        config.ResultKind = ResultKind.Team;
        var sut = CreateSut();

        var test = sut.GetCalculationService(config);

        test.Should().BeOfType<TeamSessionCalculationService>();
    }

    private SessionCalculationServiceProvider CreateSut()
    {
        return fixture.Create<SessionCalculationServiceProvider>();
    }

    private SessionCalculationConfiguration GetCalculationConfiguration()
    {
        return fixture.Create<SessionCalculationConfiguration>();
    }
}
