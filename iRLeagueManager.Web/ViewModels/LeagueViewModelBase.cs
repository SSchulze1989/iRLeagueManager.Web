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
            this.ApiService = apiService;
        }

        protected ILoggerFactory LoggerFactory { get; }
        protected ILogger<T> Logger { get; }
        protected LeagueApiService ApiService { get; }

        private bool loading;
        public bool Loading { get => loading; set => Set(ref loading, value); }

        private bool saving;
        public bool Saving { get => saving; set => Set(ref saving, value); }

        /// <summary>
        /// Set a value on a model property and call OnPropertyChanged() if value changed
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="get">current value of property</param>
        /// <param name="set">action to update property value</param>
        /// <param name="value">new value to set</param>
        /// <param name="propertyName">Name of the property for OnPropertyChanged()</param>
        /// <returns></returns>
        protected bool SetP<TProperty>(TProperty get, Action<TProperty> set, TProperty value, [CallerMemberName] string? propertyName = null)
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
