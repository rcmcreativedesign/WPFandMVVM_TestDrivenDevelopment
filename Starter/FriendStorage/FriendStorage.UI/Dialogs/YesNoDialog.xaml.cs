using System.Windows;

namespace FriendStorage.UI.Dialogs
{
  public partial class YesNoDialog : Window
  {
    public YesNoDialog(string title, string message)
    {
      InitializeComponent();
      Title = title;
      textBlock.Text = message;
    }

    private void ButtonYes_Click(object sender, RoutedEventArgs args)
    {
      DialogResult = true;
    }
    private void ButtonNo_Click(object sender, RoutedEventArgs args)
    {
      DialogResult = false;
    }
  }
}
