using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ReviewSettingsViewModel : LeagueViewModelBase<ReviewSettingsViewModel>
{
    public ReviewSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
        base(loggerFactory, apiService)
    {
        voteCategories = new();
    }

    private ObservableCollection<VoteCategoryViewModel> voteCategories;
    public ObservableCollection<VoteCategoryViewModel> VoteCategories { get => voteCategories; set => Set(ref voteCategories, value); }

    public async Task<StatusResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.VoteCategories()
                .Get(cancellationToken);
            var result = await request;
            if (result.Success && result.Content is not null)
            {
                VoteCategories = new(result.Content.Select(x => new VoteCategoryViewModel(LoggerFactory, ApiService, x)));
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddVoteCategory(VoteCategoryModel voteCategory, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.VoteCategories()
                .Post(voteCategory, cancellationToken);
            var result = await request;
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            return await LoadAsync(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteVoteCategory(VoteCategoryModel voteCategory, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.VoteCategories()
                .WithId(voteCategory.Id)
                .Delete(cancellationToken);
            var result = await request;
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            return await LoadAsync(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }
}
