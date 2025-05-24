namespace PasswordManager.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<MainViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new Border()
                            .SafeArea(SafeArea.InsetMask.VisibleBounds)
                            .Child(
                                new Grid()
                                    .ColumnDefinitions("*, *")
                                    .RowDefinitions("*, *, *")
                                    .RowSpacing(6)
                                    .ColumnSpacing(6)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Children(
                                        new TextBlock()
                                            .Grid(row: 0)
                                            .FontSize(20)
                                            .Text("Enter your master password: "),
                                        new PasswordBox()
                                            .Grid(row: 1, column: 0)
                                            .MaxWidth(300)
                                            .PasswordRevealMode(PasswordRevealMode.Hidden)
                                            .PlaceholderText("Password...")
                                            .Password(x =>
                                                x.Binding(() => vm.MasterPassword).TwoWay()
                                            ),
                                        new Button()
                                            .Grid(row: 1, column: 1)
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
                                            .Grid(row: 2)
                                            .Text(() => vm.VerificationResponse)
                                            .Foreground(
                                                () => vm.VerificationResponse,
                                                res =>
                                                    res == "Success - Redirecting..."
                                                        ? Colors.Green
                                                        : Colors.Red
                                            )
                                    )
                            )
                    )
        );
    }
}
