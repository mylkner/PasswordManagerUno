namespace PasswordManager.Presentation;

public sealed partial class PasswordsPage : Page
{
    public PasswordsPage()
    {
        this.DataContext<PasswordsViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new StackPanel()
                            .VerticalAlignment(VerticalAlignment.Center)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Children(
                                new ListView()
                                    .ItemsSource(() => vm.Passwords)
                                    .ItemTemplate<Password>(pwd =>
                                        new TextBlock().Text(() => pwd.Title)
                                    )
                            )
                    )
        );
    }
}
