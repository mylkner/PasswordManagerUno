namespace PasswordManager.Presentation;

public sealed partial class CreateMasterPasswordPage : Page
{
    public CreateMasterPasswordPage()
    {
        this.DataContext<CreateMasterPasswordViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new Border()
                            .SafeArea(SafeArea.InsetMask.VisibleBounds)
                            .Child(
                                new StackPanel()
                                    .MaxWidth(400)
                                    .Spacing(10)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Children(
                                        new TextBlock()
                                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                                            .FontSize(20)
                                            .Text("Create your master password: "),
                                        new PasswordBox()
                                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                                            .MaxWidth(300)
                                            .PasswordRevealMode(PasswordRevealMode.Hidden)
                                            .PlaceholderText("Password...")
                                            .Password(x =>
                                                x.Binding(() => vm.MasterPassword).TwoWay()
                                            ),
                                        new PasswordBox()
                                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                                            .MaxWidth(300)
                                            .PasswordRevealMode(PasswordRevealMode.Hidden)
                                            .PlaceholderText("Re-enter password...")
                                            .Password(x =>
                                                x.Binding(() => vm.MasterPasswordReEntered).TwoWay()
                                            ),
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
                                                        : "Create"
                                            )
                                            .Command(() => vm.SetMasterPassword),
                                        new TextBlock()
                                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                                            .Text(() => vm.CreationResponse)
                                            .Foreground(
                                                () => vm.CreationResponse,
                                                res =>
                                                    res == "Created - Redirecting..."
                                                        ? Colors.Green
                                                        : Colors.Red
                                            )
                                    )
                            )
                    )
        );
    }
}
