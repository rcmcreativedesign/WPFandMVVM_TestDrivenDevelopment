using FriendStorage.Model;
using FriendStorage.UI.Command;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.Wrapper;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel
{
  public interface IFriendEditViewModel
  {
    void Load(int? friendId);
    FriendWrapper Friend { get; }
  }
  public class FriendEditViewModel : ViewModelBase, IFriendEditViewModel
  {
    private IFriendDataProvider _dataProvider;
    private IEventAggregator _eventAggregator;
    private FriendWrapper _friend;

    public FriendEditViewModel(IFriendDataProvider dataProvider, IEventAggregator eventAggregator)
    {
      _dataProvider = dataProvider;
      _eventAggregator = eventAggregator;
      SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
      DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
    }

    private void OnDeleteExecute(object obj)
    {
      _dataProvider.DeleteFriend(Friend.Id);
      _eventAggregator.GetEvent<FriendDeletedEvent>().Publish(Friend.Id);
    }

    private bool OnDeleteCanExecute(object arg)
    {
      return Friend != null && Friend.Id > 0;
    }

    private void OnSaveExecute(object obj)
    {
      _dataProvider.SaveFriend(Friend.Model);
      Friend.AcceptChanges();
      _eventAggregator.GetEvent<FriendSavedEvent>().Publish(Friend.Model);
    }

    private bool OnSaveCanExecute(object arg)
    {
      return Friend != null && Friend.IsChanged;
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public FriendWrapper Friend 
    { 
      get
      {
        return _friend;
      }
      private set
      {
        _friend = value;
        OnPropertyChanged();
      }
    }

    public void Load(int? friendId)
    {
      var friend = friendId.HasValue ? _dataProvider.GetFriendById(friendId.Value) : new Friend();
      Friend = new FriendWrapper(friend);
      Friend.PropertyChanged += Friend_PropertyChanged;
      InvalidateCommands();
    }

    private void Friend_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      InvalidateCommands();
    }

    private void InvalidateCommands()
    {
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
      ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
    }
  }
}
