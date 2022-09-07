using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Linq.Expressions;
using System.Reflection;
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
        public bool Loading 
        { 
            get => loading;
            protected set => Set(ref loading, value);
        }

        private bool saving;
        public bool Saving
        {
            get => saving;
            protected set => Set(ref saving, value);
        }

        private bool hasChanged = false;
        public bool HasChanged
        {
            get => hasChanged;
            protected set => Set(ref hasChanged, value);
        }

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
            if (!EqualityComparer<TProperty>.Default.Equals(get, value))
            {
                set.Invoke(value);
                HasChanged = true;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected bool Set<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> property, TProperty value, [CallerMemberName] string ? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(property);

            var propertyValue = property.Compile().Invoke(model);
            if (!EqualityComparer<TProperty>.Default.Equals(propertyValue, value))
            {
                var propertyExpression = property.Body as MemberExpression
                    ?? throw new ArgumentException("Argument must be a member Expression", nameof(property));
                var propertyInfo = propertyExpression.Member as PropertyInfo
                    ?? throw new ArgumentException("Expression must target a Property", nameof(property));
                propertyInfo.SetValue(model, value);
                HasChanged = true;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}
