using FluentValidation.TestHelper;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.Server.Validation.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;

namespace iRLeagueApiCore.UnitTests.Server.Validators.Leagues;

public sealed class PostLeagueRequestDbTestFixture : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private const string testLeagueName = "ValidationLeague123_-";
    private const string testLeagueNameFull = "Full test league name";

    private string ExistingLeagueName => fixture.DbContext.Leagues.First().Name;

    public PostLeagueRequestDbTestFixture(DbTestFixture fixture)
    {
        this.fixture = fixture;
    }

    private static PostLeagueRequest DefaultRequest(string name = testLeagueName)
    {
        var model = new PostLeagueModel()
        {
            Name = name,
            NameFull = testLeagueNameFull,
        };
        return new PostLeagueRequest(LeagueUser.Empty, model);
    }

    private static PostLeagueRequestValidator CreateValidator(LeagueDbContext dbContext)
    {
        return new PostLeagueRequestValidator(new PostLeagueModelValidator(dbContext), MockHelpers.TestUserManager<ApplicationUser>());
    }

    [Fact]
    public async Task ValidRequest()
    {
        using var dbContext = fixture.CreateDbContext();
        var request = DefaultRequest();
        var validator = CreateValidator(dbContext);
        var result = await validator.TestValidateAsync(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("1league", true)] // number at beginning: ok
    [InlineData(null, false)] // league name exists
    [InlineData("1337", false)] // only numbers
    [InlineData("123.league.de", false)] // no dots
    [InlineData("sh", false)] // too short
    [InlineData("abcdefghiklmnopqrstuvwxyzabcdefghiklmnopqrstuvwxyzabcdefghiklmnopqrstuvwxyzabcdefghiklmnopqrstuvwxyz", false)] // too long
    [InlineData("league name", false)] // invalid character: space
    public async Task ValidateName(string? leagueName, bool isValid)
    {
        leagueName ??= ExistingLeagueName;
        using var dbContext = fixture.CreateDbContext();
        var request = DefaultRequest(leagueName);
        var validator = CreateValidator(dbContext);
        var result = await validator.TestValidateAsync(request);
        Assert.Equal(isValid, result.IsValid);
        if (isValid == false)
        {
            result.ShouldHaveValidationErrorFor(x => x.Model.Name);
        }
    }
}
