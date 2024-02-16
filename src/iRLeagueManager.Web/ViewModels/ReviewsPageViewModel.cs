using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueApiCore.Common.Models.Users;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Diagnostics.Contracts;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ReviewsPageViewModel : LeagueViewModelBase<ReviewsPageViewModel>
{
    public ReviewsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        leagueModel = new();
        eventCars = new();
    }

    private LeagueModel leagueModel;

    public bool ProtestsEnabled => leagueModel.EnableProtests;
    public bool ProtestsPublic => leagueModel.ProtestsPublic != ProtestPublicSetting.Hidden;
    public TimeSpan CooldownPeriod => leagueModel.ProtestCoolDownPeriod;
    public TimeSpan ProtestClosedAfter => leagueModel.ProtestsClosedAfter;


    private ObservableCollection<ReviewViewModel> reviews = new();
    public ObservableCollection<ReviewViewModel> Reviews { get => reviews; set => Set(ref reviews, value); }

    private ObservableCollection<ProtestViewModel> protests = new();
    public ObservableCollection<ProtestViewModel> Protests { get => protests; set => Set(ref protests, value); }

    private ObservableCollection<UserModel> leagueUsers = new();
    public ObservableCollection<UserModel> LeagueUsers { get => leagueUsers; set => Set(ref leagueUsers, value); }

    private ICollection<VoteCategoryViewModel> voteCategories = Array.Empty<VoteCategoryViewModel>();
    public ICollection<VoteCategoryViewModel> VoteCategories { get => voteCategories; set => Set(ref voteCategories, value); }

    private CarListModel eventCars;
    public CarListModel EventCars { get => eventCars; set => Set(ref eventCars, value); }

    public async Task<StatusResult> LoadFromEventAsync(long eventId, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;

            var leagueRequest = ApiService.Client.Leagues()
                .WithName(ApiService.CurrentLeague.Name)
                .Get(cancellationToken);
            var leagueResult = await leagueRequest;
            if (leagueResult.Success == false || leagueResult.Content is null)
            {
                return leagueResult.ToStatusResult();
            }
            leagueModel = leagueResult.Content;

            var eventEndpoint = ApiService.CurrentLeague
                .Events()
                .WithId(eventId);

            var reviewsEndpoint = eventEndpoint
                .Reviews();
            var result = await reviewsEndpoint.Get(cancellationToken);
            if (result.Success == false || result.Content is null)
            {
                return result.ToStatusResult();
            }

            var reviewModels = result.Content;
            Reviews = new(reviewModels.Select(x => new ReviewViewModel(LoggerFactory, ApiService, x)));

            var eventCars = await eventEndpoint
                .Cars()
                .Get(cancellationToken);
            if (eventCars.Success == false || eventCars.Content is null)
            {
                return eventCars.ToStatusResult();
            }
            EventCars = eventCars.Content;

            var protestsRequest = eventEndpoint
                .Protests()
                .Get(cancellationToken);
            var protestsResult = await protestsRequest;
            if (protestsResult.Success == false || protestsResult.Content is null)
            {
                return protestsResult.ToStatusResult();
            }
            Protests = new(protestsResult.Content.Select(x => new ProtestViewModel(LoggerFactory, ApiService, x)));
        }
        finally
        {
            Loading = false;
        }

        return StatusResult.SuccessResult();
    }

    public async Task<StatusResult> LoadUsers(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Users()
                .Get(cancellationToken);
            if (result.Success && result.Content is not null)
            {
                LeagueUsers = new(result.Content);
            }
            return result.ToStatusResult();
        }
        finally 
        { 
            Loading = false; 
        }
    }

    public async Task<StatusResult> LoadVoteCategories(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague
                .VoteCategories()
                .Get(cancellationToken);
            if (result.Success && result.Content is IEnumerable<VoteCategoryModel> models)
            {
                VoteCategories = models.Select(x => new VoteCategoryViewModel(LoggerFactory, ApiService, x)).ToList();
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> FileProtest(long sessionId, PostProtestModel protest, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Sessions()
                .WithId(sessionId)
                .Protests()
                .Post(protest, cancellationToken);
            var result = await request;
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public bool CanFileProtest(EventViewModel? @event)
    {
        if (@event is null || @event.HasResult == false || leagueModel.EnableProtests == false)
        {
            return false;
        }

        var canFile = true;
        var eventEnd = @event.End;
        if (leagueModel?.ProtestCoolDownPeriod > TimeSpan.Zero)
        {
            var canFileAfter = eventEnd + leagueModel.ProtestCoolDownPeriod;
            canFile &= ApiService.ClientTimeProvider.Now > canFileAfter;
        }
        if (leagueModel?.ProtestsClosedAfter > TimeSpan.Zero)
        {
            var canFileBefore = eventEnd + leagueModel.ProtestsClosedAfter;
            canFile &= ApiService.ClientTimeProvider.Now < canFileBefore;
        }
        return canFile;
    }
}
