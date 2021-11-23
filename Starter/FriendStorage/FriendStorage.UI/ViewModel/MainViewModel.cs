using FriendStorage.DataAccess;
using FriendStorage.UI.Command;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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
      eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);
      CloseFriendTabCommand = new DelegateCommand(OnCloseFriendTabExecute);
      AddFriendCommand = new DelegateCommand(OnAddFriendExecute);
    }

    private void OnFriendDeleted(int friendId)
    {
      var friendEditViewModel = FriendEditViewModels.Single(vm => vm.Friend.Id == friendId);
      FriendEditViewModels.Remove(friendEditViewModel);
    }

    private void OnCloseFriendTabExecute(object obj)
    {
      var friendEditViewModel = (IFriendEditViewModel)obj;
      FriendEditViewModels.Remove(friendEditViewModel);
    }

    private void OnAddFriendExecute(object obj)
    {
      SelectedFriendEditViewModel = CreaetAndLoadFriendEditViewModel(null);
    }

    private IFriendEditViewModel CreaetAndLoadFriendEditViewModel(int? friendId)
    {
      var friendEditViewModel = _friendEditViewModelCreator();
      FriendEditViewModels.Add(friendEditViewModel);
      friendEditViewModel.Load(friendId);
      return friendEditViewModel;
    }

    private void OnOpenFriendEditView(int friendId)
    {
      var friendEditViewModel = FriendEditViewModels.SingleOrDefault(vm => vm.Friend.Id == friendId);
      if (friendEditViewModel == null)
      {
        friendEditViewModel = CreaetAndLoadFriendEditViewModel(friendId);
      }
      SelectedFriendEditViewModel = friendEditViewModel;
    }

    public ICommand CloseFriendTabCommand { get; private set; }
    public ICommand AddFriendCommand { get; private set; }

    public INavigationViewModel NavigationViewModel { get; private set; }

    public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; private set; }

    public IFriendEditViewModel SelectedFriendEditViewModel
    {
      get => _selectedFriendEditViewModel;
      set
      {
        _selectedFriendEditViewModel = value;
        OnPropertyChanged();
      }
    }

    public void Load()
    {
      NavigationViewModel.Load();
    }
  }
}
