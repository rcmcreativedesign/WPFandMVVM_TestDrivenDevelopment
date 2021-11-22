using FriendStorage.DataAccess;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FriendStorage.UI.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    private IFriendEditViewModel _selectedFriendEditViewModel;
    private Func<IFriendEditViewModel> _friendEditViewModelCreator;

    public MainViewModel(INavigationViewModel navigationViewModel, Func<IFriendEditViewModel> friendEditViewModelCreator, IEventAggregator eventAggregator)
    {
      NavigationViewModel = navigationViewModel;
      FriendEditViewModels = new ObservableCollection<IFriendEditViewModel>();
      _friendEditViewModelCreator = friendEditViewModelCreator;
      eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendEditView);
    }

    private void OnOpenFriendEditView(int friendId)
    {
      var friendEditViewModel = FriendEditViewModels.SingleOrDefault(vm => vm.Friend.Id == friendId);
      if (friendEditViewModel == null)
      {
        friendEditViewModel = _friendEditViewModelCreator();
        FriendEditViewModels.Add(friendEditViewModel);
        friendEditViewModel.Load(friendId);
      }
      SelectedFriendEditViewModel = friendEditViewModel;
    }

    public INavigationViewModel NavigationViewModel { get; private set; }
    
    public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; private set; }

    public IFriendEditViewModel SelectedFriendEditViewModel { get => _selectedFriendEditViewModel; set => _selectedFriendEditViewModel = value; }

    public void Load()
    {
      NavigationViewModel.Load();
    }
  }
}
