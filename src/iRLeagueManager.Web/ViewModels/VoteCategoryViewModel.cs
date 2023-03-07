using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class VoteCategoryViewModel : LeagueViewModelBase<VoteCategoryViewModel, VoteCategoryModel>
{
    public VoteCategoryViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public VoteCategoryViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, VoteCategoryModel model) : 
        base(loggerFactory, apiService, model)
    {
    }

    public long CatId => model.Id;
    public string Text { get => model.Text; set => SetP(model.Text, value => model.Text = value, value); }
    public int DefaultPenalty { get => model.DefaultPenalty; set => SetP(model.DefaultPenalty, value => model.DefaultPenalty = value, value); }
    public int Index { get => model.Index; set => SetP(model.Index, value => model.Index = value, value); }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.VoteCategories()
                .WithId(model.Id)
                .Put(model, cancellationToken);
            var result = await request;
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}