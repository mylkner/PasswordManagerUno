namespace PasswordManager.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<MainViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Background(Theme.Brushes.Background.Default)
                    .Content(
                        new StackPanel()
                            .SafeArea(SafeArea.InsetMask.All)
                            .MaxWidth(400)
                            .Spacing(10)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Children(
                                new TextBlock()
                                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                                    .FontSize(20)
                                    .Text("Enter your master password: "),
                                new PasswordBox()
                                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                                    .MaxWidth(400)
                                    .PasswordRevealMode(PasswordRevealMode.Hidden)
                                    .PlaceholderText("Password...")
                                    .Password(x => x.Binding(() => vm.MasterPassword).TwoWay()),
                                new Button()
                                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                                    .Content(
                                        () => vm.Loading,
                                        loading =>
                                            loading
                                                ? new ProgressRing()
                                                    .Width(40)
                                                    .Height(40)
                                                    .Foreground(Colors.Black)
                                                : "Verify"
                                    )
                                    .Command(() => vm.VerifyButtonCommand),
                                new TextBlock()
                                    .MaxWidth(400)
                                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                                    .Text(() => vm.VerificationResponse)
                                    .Foreground(
                                        () => vm.VerificationResponse,
                                        res =>
                                            res == "Success - Redirecting..."
                                                ? new SolidColorBrush(Colors.Green)
                                                : new SolidColorBrush(Colors.Red)
                                    )
                            )
                    )
        );
    }
}
