namespace PasswordManager.Presentation;

public sealed partial class PasswordsPage : Page
{
    public PasswordsPage()
    {
        this.DataContext<PasswordsViewModel>(
            (page, vm) =>
                page.Name(out var root)
                    .NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new ScrollViewer().Content(
                            new Border()
                                .SafeArea(SafeArea.InsetMask.VisibleBounds)
                                .Child(
                                    new StackPanel()
                                        .MaxWidth(800)
                                        .Padding(10, 70)
                                        .Spacing(20)
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                                        .Children(
                                            new Grid()
                                                .ColumnDefinitions("*, *, *")
                                                .ColumnSpacing(10)
                                                .VerticalAlignment(VerticalAlignment.Center)
                                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                                .Children(
                                                    new TextBlock()
                                                        .VerticalAlignment(VerticalAlignment.Center)
                                                        .FontSize(20)
                                                        .Text("Passwords"),
                                                    new ToggleButton()
                                                        .Grid(row: 0, column: 1)
                                                        .VerticalAlignment(VerticalAlignment.Center)
                                                        .Content("+")
                                                        .Command(() => vm.AddPassword),
                                                    new TextBox()
                                                        .Grid(row: 0, column: 2)
                                                        .PlaceholderText("Search")
                                                        .Text(x =>
                                                            x.Binding(() => vm.SearchTerm)
                                                                .TwoWay()
                                                                .UpdateSourceTrigger(
                                                                    UpdateSourceTrigger.PropertyChanged
                                                                )
                                                        )
                                                ),
                                            new ListView()
                                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                                .ItemsSource(() => vm.Passwords)
                                                .ItemTemplate<PasswordPreview>(pwd =>
                                                    new Button()
                                                        .Padding(20)
                                                        .Margin(0, 0, 0, 20)
                                                        .Background(
                                                            new SolidColorBrush(Colors.WhiteSmoke)
                                                        )
                                                        .HorizontalAlignment(
                                                            HorizontalAlignment.Stretch
                                                        )
                                                        .Command(x =>
                                                            x.Source(root)
                                                                .DataContext<PasswordsViewModel>()
                                                                .Binding(v => v.DecryptPassword)
                                                        )
                                                        .CommandParameter(() => pwd.Id)
                                                        .Content(
                                                            new Grid()
                                                                .ColumnDefinitions("*, *")
                                                                .CornerRadius(20)
                                                                .Children(
                                                                    new TextBlock()
                                                                        .Grid(column: 0)
                                                                        .HorizontalAlignment(
                                                                            HorizontalAlignment.Left
                                                                        )
                                                                        .VerticalAlignment(
                                                                            VerticalAlignment.Center
                                                                        )
                                                                        .Text(() => pwd.Title),
                                                                    new Button()
                                                                        .Grid(column: 1)
                                                                        .HorizontalAlignment(
                                                                            HorizontalAlignment.Right
                                                                        )
                                                                        .VerticalAlignment(
                                                                            VerticalAlignment.Center
                                                                        )
                                                                        .Content("Delete")
                                                                        .Command(x =>
                                                                            x.Source(root)
                                                                                .DataContext<PasswordsViewModel>()
                                                                                .Binding(v =>
                                                                                    v.DeletePassword
                                                                                )
                                                                        )
                                                                        .CommandParameter(
                                                                            () => pwd.Id
                                                                        )
                                                                )
                                                        )
                                                )
                                        )
                                )
                        )
                    )
        );
    }
}
