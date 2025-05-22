namespace PasswordManager.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<MainViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new StackPanel()
                            .Spacing(16)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Children(
                                new TextBlock().FontSize(20).Text("Enter your master password: "),
                                new Grid()
                                    .SafeArea(SafeArea.InsetMask.All)
                                    .ColumnDefinitions("2*, *")
                                    .Children(
                                        new PasswordBox()
                                            .Margin(4)
                                            .Width(200)
                                            .PasswordRevealMode(PasswordRevealMode.Hidden)
                                            .PlaceholderText("Password...")
                                            .Password(x =>
                                                x.Binding(() => vm.MasterPassword).TwoWay()
                                            ),
                                        new Button()
                                            .Grid(column: 1)
                                            .Content("Verify")
                                            .Command(() => vm.VerifyButtonCommand)
                                    )
                            )
                    )
        );
    }
}
