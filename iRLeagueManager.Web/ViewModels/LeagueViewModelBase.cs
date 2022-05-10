using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels
{
    public class LeagueViewModelBase<T> : ViewModelBase
    {
        public LeagueViewModelBase(ILoggerFactory loggerFactory, LeagueApiService apiService)
        {
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger<T>();
            ApiService = apiService;
        }

        protected ILoggerFactory LoggerFactory { get; }
        protected ILogger<T> Logger { get; }
        protected LeagueApiService ApiService { get; }

        /// <summary>
        /// Set a value on a model property and call OnPropertyChanged() if value changed
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="get">current value of property</param>
        /// <param name="set">action to update property value</param>
        /// <param name="value">new value to set</param>
        /// <param name="propertyName">Name of the property for OnPropertyChanged()</param>
        /// <returns></returns>
        protected bool SetProp<TProperty>(TProperty get, Action<TProperty> set, TProperty value, [CallerMemberName] string? propertyName = null)
        {
            if (get == null && value != null || (get != null && get.Equals(value) == false))
            {
                set.Invoke(value);
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}
