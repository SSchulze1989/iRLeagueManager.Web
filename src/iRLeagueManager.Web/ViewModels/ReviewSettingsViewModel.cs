using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ReviewSettingsViewModel : LeagueViewModelBase<ReviewSettingsViewModel>
{
    public ReviewSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
        base(loggerFactory, apiService)
    {
        voteCategories = [];
    }

    private IList<VoteCategoryViewModel> voteCategories;
    public IList<VoteCategoryViewModel> VoteCategories 
    { 
        get => voteCategories;
        set
        {
            if (voteCategories != value)
            {
                foreach (var category in voteCategories)
                {
                    category.HasChanged -= OnVoteCategoryChanged;
                }
                Set(ref voteCategories, value);
                foreach (var category in voteCategories)
                {
                    category.HasChanged += OnVoteCategoryChanged;
                }
            }
        }
    }

    private void OnVoteCategoryChanged(object? sender, EventArgs e)
    {
        OnHasChanged();
    }

    public async Task<StatusResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.VoteCategories()
                .Get(cancellationToken);
            var result = await request;
            if (result.Success && result.Content is not null)
            {
                VoteCategories = result.Content.Select(x => new VoteCategoryViewModel(LoggerFactory, ApiService, x)).ToList();
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> UpdateVoteCategoryOrder(IList<VoteCategoryViewModel> voteCategories, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            bool hasIndexUpdated = false;
            foreach (var (voteCategory, index) in voteCategories.WithIndex())
            {
                if (voteCategory.Index != index)
                {
                    voteCategory.Index = index;
                    var request = CurrentLeague
                        .VoteCategories()
                        .WithId(voteCategory.CatId)
                        .Put(voteCategory.GetModel(), cancellationToken);
                    var result = await request;
                    if (result.Success == false)
                    {
                        return result.ToStatusResult();
                    }
                    hasIndexUpdated = true;
                }
            }

            if (hasIndexUpdated)
            {
                return await LoadAsync(cancellationToken);
            }
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddVoteCategory(VoteCategoryModel voteCategory, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.VoteCategories()
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
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.VoteCategories()
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach(var category in VoteCategories)
            {
                category.HasChanged -= OnVoteCategoryChanged;
            }
        }
        base.Dispose(disposing);
    }
}
