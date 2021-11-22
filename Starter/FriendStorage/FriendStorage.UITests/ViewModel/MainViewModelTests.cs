using FriendStorage.Model;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FriendStorage.UITests.ViewModel
{
  public class MainViewModelTests
  {
    private Mock<INavigationViewModel> _navigationViewModelMock;
    private Mock<IEventAggregator> _eventAggregatorMock;
    private OpenFriendEditViewEvent _openFriendEditViewEvent;
    private MainViewModel _viewModel;
    private List<Mock<IFriendEditViewModel>> _friendEditViewModelMocks;

    public MainViewModelTests()
    {
      _friendEditViewModelMocks = new List<Mock<IFriendEditViewModel>>();
      _navigationViewModelMock = new Mock<INavigationViewModel>();

      _eventAggregatorMock = new Mock<IEventAggregator>();
      _openFriendEditViewEvent = new OpenFriendEditViewEvent();
      _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenFriendEditViewEvent>()).Returns(_openFriendEditViewEvent);
      _viewModel = new MainViewModel(_navigationViewModelMock.Object, CreateFriendEditViewModel, _eventAggregatorMock.Object);


    }

    private IFriendEditViewModel CreateFriendEditViewModel()
    {
      var friendEditViewModelMock = new Mock<IFriendEditViewModel>();
      friendEditViewModelMock.Setup(vm => vm.Load(It.IsAny<int>())).Callback<int>(friendId => 
      { 
        friendEditViewModelMock.Setup(vm => vm.Friend).Returns(new Friend { Id = friendId }); 
      });
      _friendEditViewModelMocks.Add(friendEditViewModelMock);
      return friendEditViewModelMock.Object;
    }

    [Fact]
    public void ShouldCallTheLoadMethodOfTheNavigationViewModel()
    {
      _viewModel.Load();

      _navigationViewModelMock.Verify(vm => vm.Load(), Times.Once);
    }

    [Fact]
    public void ShouldAddFriendEditViewModelAndLoadAndSelectId()
    {
      const int friendId = 7;
      _openFriendEditViewEvent.Publish(friendId);

      Assert.Equal(1, _viewModel.FriendEditViewModels.Count);
      var friendEditViewModel = _viewModel.FriendEditViewModels.First();
      Assert.Equal(friendEditViewModel, _viewModel.SelectedFriendEditViewModel);
      _friendEditViewModelMocks.First().Verify(vm => vm.Load(friendId), Times.Once);
    }

    [Fact]
    public void ShouldAddFriendEditViewModelsOnlyOnce()
    {
      _openFriendEditViewEvent.Publish(5);
      _openFriendEditViewEvent.Publish(5);
      _openFriendEditViewEvent.Publish(6);
      _openFriendEditViewEvent.Publish(7);
      _openFriendEditViewEvent.Publish(7);

      Assert.Equal(3, _viewModel.FriendEditViewModels.Count);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForSelectedFriendEditViewModel()
    {
      var fired = false;
      _viewModel.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(_viewModel.SelectedFriendEditViewModel))
        {
          fired = true;
        }
      };

      var friendEditViewModelMock = new Mock<IFriendEditViewModel>();
      _viewModel.SelectedFriendEditViewModel = friendEditViewModelMock.Object;

      Assert.True(fired);
    }
  }
}
