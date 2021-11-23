using System.Windows;

namespace FriendStorage.UI.Dialogs
{
  public class CustomMessageDialogService : IMessageDialogService
  {
    public MessageDialogResult ShowYesNoDialog(string title, string message)
    {
      return new YesNoDialog(title, message)
      {
        WindowStartupLocation = WindowStartupLocation.CenterOwner,
        Owner = App.Current.MainWindow
      }.ShowDialog().GetValueOrDefault() ? MessageDialogResult.Yes : MessageDialogResult.No;
    }
  }
}
