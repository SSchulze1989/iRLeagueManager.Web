using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Filters;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using System.Diagnostics.Contracts;
using System.Net;

namespace iRLeagueApiCore.UnitTests.Server.Filters;
public sealed class CheckLeagueSubscriptionAttributeTests : DataAccessTestsBase
{
    [Fact]
    public async Task Action_ShouldContinue_WhenRequiredAttributeMissing()
    {
        var league = await dbContext.Leagues.FirstAsync();
        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor(),
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };
        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null!
        );
        bool hasInvokedNext = false;
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext,
            executingContext: executingContext,
            executedDelegate: () => { hasInvokedNext = true; return Task.FromResult(executedContext); });

        executingContext.Result.Should().BeNull();
        hasInvokedNext.Should().BeTrue();
    }

    [Fact]
    public async Task Action_ShouldSetResultAndCancel_WhenRequiredAttributeExists()
    {
        var league = await dbContext.Leagues.FirstAsync();
        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };
        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null!
        );
        bool hasInvokedNext = false;
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext,
            executingContext: executingContext,
            executedDelegate: () => { hasInvokedNext = true; return Task.FromResult(executedContext); });

        executingContext.Result.Should().NotBeNull();
        executingContext.Result.Should().BeOfType<StatusCodeResult>()
            .Subject.StatusCode.Should().Be((int)HttpStatusCode.PaymentRequired);
        hasInvokedNext.Should().BeFalse();
    }

    [Theory]
    [InlineData(SubscriptionStatus.Expired, false)]
    [InlineData(SubscriptionStatus.Unknown, false)]
    [InlineData(SubscriptionStatus.FreeTrial, true)]
    [InlineData(SubscriptionStatus.PaidPlan, true)]
    [InlineData(SubscriptionStatus.Lifetime, true)]
    public async Task Action_ShouldContinue_WhenSubscriptionNotExpired(SubscriptionStatus subscriptionStatus, bool invokeNext)
    {
        var league = await dbContext.Leagues.FirstAsync();
        league.Subscription = subscriptionStatus;
        league.Expires = DateTime.UtcNow.AddDays(1);
        await dbContext.SaveChangesAsync();

        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };
        bool hasInvokedNext = false;
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext,
            executedDelegate: () => { hasInvokedNext = true; return Task.FromResult(executedContext); });

        hasInvokedNext.Should().Be(invokeNext);
    }

    [Theory]
    [InlineData(SubscriptionStatus.Expired, false)]
    [InlineData(SubscriptionStatus.Unknown, false)]
    [InlineData(SubscriptionStatus.FreeTrial, false)]
    [InlineData(SubscriptionStatus.PaidPlan, false)]
    [InlineData(SubscriptionStatus.Lifetime, true)]
    public async Task Action_ShouldNotContinue_WhenSubscriptionExpired(SubscriptionStatus subscriptionStatus, bool invokeNext)
    {
        var league = await dbContext.Leagues.FirstAsync();
        league.Subscription = subscriptionStatus;
        league.Expires = DateTime.UtcNow.AddDays(-1);
        await dbContext.SaveChangesAsync();

        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };
        bool hasInvokedNext = false;
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext,
            executedDelegate: () => { hasInvokedNext = true; return Task.FromResult(executedContext); });

        hasInvokedNext.Should().Be(invokeNext);
    }

    [Theory]
    [InlineData(SubscriptionStatus.Unknown)]
    [InlineData(SubscriptionStatus.FreeTrial)]
    [InlineData(SubscriptionStatus.PaidPlan)]
    public async Task Action_ShouldSetLeagueToExpired_WhenSubscriptionExpired(SubscriptionStatus subscriptionStatus)
    {
        var league = await dbContext.Leagues.FirstAsync();
        league.Subscription = subscriptionStatus;
        league.Expires = DateTime.UtcNow.AddDays(-1);
        await dbContext.SaveChangesAsync();

        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext);

        league.Subscription.Should().Be(SubscriptionStatus.Expired);
    }

    [Fact]
    public async Task Action_ShouldNotExpireLifetimeSubscription()
    {
        var league = await dbContext.Leagues.FirstAsync();
        league.Subscription = SubscriptionStatus.Lifetime;
        league.Expires = DateTime.UtcNow.AddDays(-1);
        await dbContext.SaveChangesAsync();

        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut,
            actionContext: actionContext);

        league.Subscription.Should().Be(SubscriptionStatus.Lifetime);
    }

    [Fact]
    public async Task Action_ShouldSetLeagueOnMemoryCache()
    {
        var league = await dbContext.Leagues.FirstAsync();
        var memoryCacheMock = MockMemoryCache();
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                    )
                ),
        };

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);

        await TestOnActionExecutionAsync(sut, actionContext: actionContext);

        memoryCacheMock.Verify(x => x.CreateEntry(CacheKeys.GetLeagueNameKey(league.Name)));
    }

    [Fact]
    public async Task Action_ShouldGetEntryFromCache_WhenExists()
    {
        var league = new LeagueEntity()
        {
            Name = "Fake League",
            Subscription = SubscriptionStatus.Lifetime,
        };
        var memoryCacheMock = MockMemoryCache();
        memoryCacheMock.Object.Set(CacheKeys.GetLeagueNameKey(league.Name), league);
        var actionContext = new ActionContext()
        {
            ActionDescriptor = new ActionDescriptor()
            {
                EndpointMetadata = new List<object>()
                {
                    new RequireSubscriptionAttribute(),
                },
            },
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(
                new RouteValueDictionary(
                    new Dictionary<string, object>() { { "leagueName", league.Name } }
                )
            ),
        };
        bool hasInvokedNext = false;
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );

        var sut = new CheckLeagueSubscriptionAttribute(dbContext, memoryCacheMock.Object);
        

        await TestOnActionExecutionAsync(sut, 
            actionContext: actionContext,
            executedDelegate: () => { hasInvokedNext = true; return Task.FromResult(executedContext); });

        memoryCacheMock.Verify(x => x.TryGetValue(CacheKeys.GetLeagueNameKey(league.Name), out It.Ref<object>.IsAny));
        hasInvokedNext.Should().BeTrue();
    }

    private static CheckLeagueSubscriptionAttribute CreateSut(
        LeagueDbContext dbContext,
        IMemoryCache memoryCache)
    {
        return new CheckLeagueSubscriptionAttribute(dbContext, memoryCache);
    }

    private static async Task TestOnActionExecutionAsync(
        CheckLeagueSubscriptionAttribute sut,
        ActionContext actionContext = default!,
        ActionExecutingContext executingContext = default!,
        ActionExecutionDelegate executedDelegate = default!)
    { 
        actionContext ??= new ActionContext()
        { 
            ActionDescriptor = new ActionDescriptor(),
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
        };
        executingContext ??= new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null!
        );
        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            null!
        );
        executedDelegate ??= () => Task.FromResult(executedContext);
        await sut.OnActionExecutionAsync(executingContext, executedDelegate);
    }

    private static Mock<IMemoryCache> MockMemoryCache()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var memoryCacheMock = new Mock<IMemoryCache>();
        memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns((object key) => cache.CreateEntry(key))
            .Verifiable();
        memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns((object key, out object value) => cache.TryGetValue(key, out value))
            .Verifiable();
        memoryCacheMock.Setup(x => x.Remove(It.IsAny<object>()))
            .Callback((object key) => cache.Remove(key))
            .Verifiable();
        return memoryCacheMock;
    }
}
