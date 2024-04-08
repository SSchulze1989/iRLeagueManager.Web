using FluentValidation;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
public sealed class GetAllPaymentHandlerTests
    : AdminPanelTestsBase<GetAllPaymentsHandler, GetAllPaymentsRequest, IEnumerable<PaymentModel>>
{
    protected override GetAllPaymentsHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetAllPaymentsRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override GetAllPaymentsRequest DefaultRequest()
    {
        return DefaultRequest(null);
    }

    private static GetAllPaymentsRequest DefaultRequest(long? leagueId)
    {
        return new GetAllPaymentsRequest(leagueId);
    }

    protected override void DefaultAssertions(GetAllPaymentsRequest request, 
        IEnumerable<PaymentModel> result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        result.Should().HaveSameCount(dbContext.Payments);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var otherLeague = dbContext.Leagues.Skip(1).First();
        var paymentOnOtherLeague = PaymentEntityBuilder(otherLeague, null).Create();
        dbContext.Payments.Add(paymentOnOtherLeague);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ShouldHandleDefault()
    {
        await ShouldHandleDefault();
    }

    [Fact]
    public async Task Handle_ShouldHandleValidationFailed()
    {
        await ShouldHandleValidationFailed();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyPaymentsFromLeague_WhenLeagueIdSet()
    {
        var league = await dbContext.Leagues
            .Include(x => x.Payments)
            .FirstAsync(x => x.Id == TestLeagueId);
        var request = DefaultRequest(TestLeagueId);
        var sut = CreateTestHandler(dbContext, DefaultValidator());

        var result = await sut.Handle(request, default);

        result.Should().HaveSameCount(league.Payments);
    }
}
