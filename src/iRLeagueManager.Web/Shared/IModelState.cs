using System.ComponentModel;

namespace iRLeagueManager.Web.Shared
{
    public interface IModelState : INotifyPropertyChanged
    {
        public bool Loading { get; }
        public bool Saving { get; }
        public bool HasChanged { get; }
    }
}