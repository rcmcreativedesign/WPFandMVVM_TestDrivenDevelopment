using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
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
  public class NavigationViewModelTests
  {
    private NavigationViewModel _viewModel;

    public NavigationViewModelTests()
    {
      var eventAggregatorMock = new Mock<IEventAggregator>();
      var navigationDataProviderMock = new Mock<INavigationDataProvider>();
      navigationDataProviderMock.Setup(dp => dp.GetAllFriends()).Returns(new List<LookupItem>
      {
        new LookupItem { Id = 1, DisplayMember = "Julia" },
        new LookupItem { Id = 2, DisplayMember = "Thomas" }
      });
      _viewModel = new NavigationViewModel(navigationDataProviderMock.Object, eventAggregatorMock.Object);

    }
    [Fact]
    public void ShouldLoadFriends()
    {
      _viewModel.Load();

      Assert.Equal(2, _viewModel.Friends.Count);
      var friend = _viewModel.Friends.SingleOrDefault(f => f.Id == 1);
      Assert.NotNull(friend);
      Assert.Equal("Julia", friend.DisplayMember);
      var friend2 = _viewModel.Friends.SingleOrDefault(f => f.Id == 2);
      Assert.NotNull(friend2);
      Assert.Equal("Thomas", friend2.DisplayMember);
    }

    [Fact]
    public void ShouldLoadFriendsOnlyOnce()
    {
      _viewModel.Load();
      _viewModel.Load();

      Assert.Equal(2, _viewModel.Friends.Count);
    }
  }
}
