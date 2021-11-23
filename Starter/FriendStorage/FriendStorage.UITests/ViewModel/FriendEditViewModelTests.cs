﻿using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using FriendStorage.UITests.Extensions;
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
  public class FriendEditViewModelTests
  {
    private const int _friendId = 5;
    private Mock<FriendDeletedEvent> _friendDeletedEventMock;
    private Mock<FriendSavedEvent> _friendSavedEventMock;
    private Mock<IEventAggregator> _eventAggregatorMock;
    private Mock<IFriendDataProvider> _dataProviderMock;
    private FriendEditViewModel _viewModel;

    public FriendEditViewModelTests()
    {
      _friendDeletedEventMock = new Mock<FriendDeletedEvent>();
      _friendSavedEventMock = new Mock<FriendSavedEvent>();
      _eventAggregatorMock = new Mock<IEventAggregator>();
      _eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>()).Returns(_friendSavedEventMock.Object);
      _eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>()).Returns(_friendDeletedEventMock.Object);

      _dataProviderMock = new Mock<IFriendDataProvider>();
      _dataProviderMock.Setup(dp => dp.GetFriendById(_friendId)).Returns(new Model.Friend { Id = _friendId, FirstName = "Thomas" });
      _viewModel = new FriendEditViewModel(_dataProviderMock.Object, _eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldLoadFriend()
    {
      _viewModel.Load(_friendId);

      Assert.NotNull(_viewModel.Friend);
      Assert.Equal(_friendId, _viewModel.Friend.Id);

      _dataProviderMock.Verify(dp => dp.GetFriendById(_friendId), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForFriend()
    {
      var fired = _viewModel.IsPropertyChangedFired(() => _viewModel.Load(_friendId), nameof(_viewModel.Friend));

      Assert.True(fired);
    }

    [Fact]
    public void ShouldDisableSaveCommandWhenFriendIsLoaded()
    {
      _viewModel.Load(_friendId);
      Assert.False(_viewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldEnableSaveCommandWhenFriendIsChanged()
    {
      _viewModel.Load(_friendId);
      _viewModel.Friend.FirstName = "Changed";

      Assert.True(_viewModel.SaveCommand.CanExecute(null));
    }
    [Fact]
    public void ShouldDisableSaveCommandLoad()
    {
      Assert.False(_viewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandWhenFriendIsChanged()
    {
      _viewModel.Load(_friendId);
      var fired = false;
      _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
      _viewModel.Friend.FirstName = "Changed";
      Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
    {
      var fired = false;
      _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
      _viewModel.Load(_friendId);
      Assert.True(fired);
    }

    [Fact]
    public void ShouldCallSaveMethodOfDataProviderWhenSaveCommandIsExecuted()
    {
      _viewModel.Load(_friendId);
      _viewModel.Friend.FirstName = "Changed";

      _viewModel.SaveCommand.Execute(null);
      _dataProviderMock.Verify(dp => dp.SaveFriend(_viewModel.Friend.Model), Times.Once);
    }

    [Fact]
    public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
    {
      _viewModel.Load(_friendId);
      _viewModel.Friend.FirstName = "Changed";

      _viewModel.SaveCommand.Execute(null);
      Assert.False(_viewModel.Friend.IsChanged);
    }

    [Fact]
    public void ShouldPublishFriendSaveEventWhenSaveCommandIsExecuted()
    {
      _viewModel.Load(_friendId);
      _viewModel.Friend.FirstName = "Changed";

      _viewModel.SaveCommand.Execute(null);
      _friendSavedEventMock.Verify(e => e.Publish(_viewModel.Friend.Model), Times.Once);
    }

    [Fact]
    public void ShouldCreateNewFriendWhenNullIsPassedToLoadMethod()
    {
      _viewModel.Load(null);

      Assert.NotNull(_viewModel.Friend);
      Assert.Equal(0, _viewModel.Friend.Id);
      Assert.Null(_viewModel.Friend.FirstName);
      Assert.Null(_viewModel.Friend.LastName);
      Assert.Null(_viewModel.Friend.Birthday);
      Assert.False(_viewModel.Friend.IsDeveloper);

      _dataProviderMock.Verify(dp => dp.GetFriendById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void ShouldEnableDeleteCommandForExistingFriend()
    {
      _viewModel.Load(_friendId);
      Assert.True(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandForNewFriend()
    {
      _viewModel.Load(null);
      Assert.False(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandWithoutLoad()
    {
      Assert.False(_viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
    {
      _viewModel.Load(_friendId);
      var fired = false;
      _viewModel.Friend.FirstName = "Changed";
      _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
      _viewModel.Friend.AcceptChanges();
      Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
    {
      var fired = false;
      _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
      _viewModel.Load(_friendId);
      Assert.True(fired);
    }

    [Fact]
    public void ShouldCallDeleteFriendWhenDeleteCommandIsExecuted()
    {
      _viewModel.Load(_friendId);

      _viewModel.DeleteCommand.Execute(null);

      _dataProviderMock.Verify(dp => dp.DeleteFriend(_friendId), Times.Once);
    }

    [Fact]
    public void ShouldPublishFriendDeleteEventWhenDeleteCommandIsExecuted()
    {
      _viewModel.Load(_friendId);

      _viewModel.DeleteCommand.Execute(null);

      _friendDeletedEventMock.Verify(e => e.Publish(_friendId), Times.Once);
    }

  }
}