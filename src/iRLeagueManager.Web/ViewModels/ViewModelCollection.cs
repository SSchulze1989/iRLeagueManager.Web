using iRLeagueManager.Web.Data;
using System.Collections;
using System.Collections.Specialized;

namespace iRLeagueManager.Web.ViewModels;

public abstract class ViewModelCollection<TViewModel, TModel, TSelf> : LeagueViewModelBase<TSelf, IEnumerable<TModel>>, 
    ICollection<TViewModel>, INotifyCollectionChanged
    where TViewModel : LeagueViewModelBase<TViewModel, TModel>
    where TModel : class, new()
    where TSelf : ViewModelCollection<TViewModel, TModel, TSelf>
{
    private ObservableCollection<TViewModel> values;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public ViewModelCollection(ILoggerFactory loggerFactory, LeagueApiService apiService) 
        : base(loggerFactory, apiService, Enumerable.Empty<TModel>())
    {
        values = new();
        SetModel(model);
    }

    /// <summary>
    /// Create a new instance of a view model for the collection, wrapping the provided model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public abstract TViewModel CreateInstance(TModel model);

    public override void SetModel(IEnumerable<TModel> modelCollection)
    {
        ArgumentNullException.ThrowIfNull(modelCollection);
        values.CollectionChanged -= CollectionChanged;
        values = new(modelCollection.Select(CreateInstance));
        values.CollectionChanged += CollectionChanged;
    }

    public override IEnumerable<TModel> GetModel()
    {
        return values.Select(x => x.GetModel());
    }

    public int Count => values.Count;

    public bool IsReadOnly => false;

    public void Add()
    {
        values.Add(CreateInstance(new()));
    }

    public void Add(TViewModel item)
    {
        values.Add(item);
    }

    public void Clear()
    {
        values.Clear();
    }

    public bool Contains(TViewModel item)
    {
        return values.Contains(item);
    }

    public void CopyTo(TViewModel[] array, int arrayIndex)
    {
        values.CopyTo(array, arrayIndex);
    }

    public IEnumerator<TViewModel> GetEnumerator()
    {
        return values.GetEnumerator();
    }

    public bool Remove(TViewModel item)
    {
        return values.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)values).GetEnumerator();
    }
}
