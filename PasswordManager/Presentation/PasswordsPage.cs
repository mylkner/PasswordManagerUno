namespace PasswordManager.Presentation;

public sealed partial class PasswordsPage : Page
{
    public PasswordsPage()
    {
        this.DataContext<PasswordsViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Background(Theme.Brushes.Background.Default)
                    .Content(
                        new ResponsiveView()
                            .ResponsiveLayout(new ResponsiveLayout().Wide(930))
                            .NormalTemplate(
                                () =>
                                    new Grid()
                                        .RowDefinitions("Auto, *")
                                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                                        .VerticalAlignment(VerticalAlignment.Stretch)
                                        .SafeArea(SafeArea.InsetMask.All)
                                        .Children(
                                            new Button()
                                                .Grid(row: 0)
                                                .Margin(20)
                                                .Content(
                                                    () => vm.ShowFunctions,
                                                    v => !v ? "\u2630" : "X"
                                                )
                                                .FontSize(16)
                                                .Command(() => vm.ChangeView),
                                            Functions(vm, this, 30)
                                                .Grid(row: 1)
                                                .Visibility(
                                                    () => vm.ShowFunctions,
                                                    view =>
                                                        view
                                                            ? Visibility.Visible
                                                            : Visibility.Collapsed
                                                ),
                                            PasswordsView(Feedback(vm), Feed(vm, this), 30)
                                                .Grid(row: 1)
                                                .Visibility(
                                                    () => vm.ShowFunctions,
                                                    view =>
                                                        !view
                                                            ? Visibility.Visible
                                                            : Visibility.Collapsed
                                                )
                                        )
                            )
                            .WideTemplate(
                                () =>
                                    new Grid()
                                        .SafeArea(SafeArea.InsetMask.All)
                                        .ColumnDefinitions("*, 2*")
                                        .Children(
                                            Functions(vm, this, 70).Grid(column: 0),
                                            PasswordsView(Feedback(vm), Feed(vm, this), 70)
                                                .Grid(column: 1)
                                        )
                            )
                    )
        );
    }

    private static ScrollViewer Functions(PasswordsViewModel vm, PasswordsPage root, int padding) =>
        new ScrollViewer().Content(
            new StackPanel()
                .Padding(padding)
                .Spacing(20)
                .VerticalAlignment(VerticalAlignment.Stretch)
                .Children(
                    new StackPanel()
                        .Spacing(10)
                        .Children(
                            new TextBlock().Text("Search").FontSize(20),
                            new TextBox()
                                .Grid(column: 2)
                                .PlaceholderText("Search")
                                .Text(x =>
                                    x.Binding(() => vm.SearchTerm)
                                        .TwoWay()
                                        .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)
                                )
                        ),
                    new StackPanel()
                        .Spacing(10)
                        .Children(
                            new TextBlock().Text("Add").FontSize(20),
                            new TextBox()
                                .PlaceholderText("Title")
                                .Text(x => x.Binding(() => vm.TitleOfPasswordToAdd).TwoWay()),
                            new PasswordBox()
                                .PlaceholderText("Password")
                                .Password(x => x.Binding(() => vm.PasswordToAdd).TwoWay()),
                            new Button()
                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                .Content("Generate Password")
                                .Command(() => vm.GeneratePassword),
                            new Button()
                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                .Content("Add")
                                .Command(x =>
                                    x.Source(root)
                                        .DataContext<PasswordsViewModel>()
                                        .Binding(() => vm.AddPassword)
                                )
                        ),
                    new StackPanel()
                        .Spacing(10)
                        .Children(
                            new TextBlock().Text("Configure DB").FontSize(20),
                            new Button()
                                .Content("Import DB")
                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                .Command(() => vm.ImportDB),
                            new Button()
                                .Content("Export DB")
                                .HorizontalAlignment(HorizontalAlignment.Stretch)
                                .Command(() => vm.ExportDB)
                        )
                )
        );

    private static Grid PasswordsView(TextBlock feedback, FeedView feed, int padding) =>
        new Grid()
            .RowDefinitions("Auto, Auto, *")
            .HorizontalAlignment(HorizontalAlignment.Stretch)
            .VerticalAlignment(VerticalAlignment.Stretch)
            .Padding(padding)
            .RowSpacing(20)
            .Children(
                new TextBlock()
                    .Grid(row: 0)
                    .VerticalAlignment(VerticalAlignment.Stretch)
                    .FontSize(20)
                    .Text("Passwords"),
                feedback.Grid(row: 1),
                new ScrollViewer().Grid(row: 2).Content(feed)
            );

    private static TextBlock Feedback(PasswordsViewModel vm) =>
        new TextBlock()
            .Text(() => vm.Response)
            .Foreground(
                () => vm.Response,
                res =>
                    res.Contains("Error:")
                        ? new SolidColorBrush(Colors.Red)
                        : new SolidColorBrush(Colors.Green)
            );

    private static FeedView Feed(PasswordsViewModel vm, PasswordsPage root) =>
        new FeedView()
            .HorizontalAlignment(HorizontalAlignment.Stretch)
            .Source(() => vm.Passwords)
            .NoneTemplate(() => new TextBlock().Text("No passwords found"))
            .ErrorTemplate(() => new TextBlock().Text("An error occured"))
            .ValueTemplate<FeedViewState>(data => PasswordTemplate(data, root));

    private static ListView PasswordTemplate(FeedViewState data, PasswordsPage root)
    {
        ListView template = new ListView()
            .Name(out var list)
            .IsItemClickEnabled(true)
            .IsMultiSelectCheckBoxEnabled(false)
            .ItemsSource(() => data.Data)
            .ItemTemplate<PasswordPreview>(pwd =>
                new Grid()
                    .Margin(0, 0, 0, 10)
                    .ColumnDefinitions("*, *")
                    .Padding(20)
                    .Background(new SolidColorBrush(Colors.DeepPink))
                    .CornerRadius(20)
                    .HorizontalAlignment(HorizontalAlignment.Stretch)
                    .Children(
                        new TextBlock()
                            .Grid(column: 0)
                            .HorizontalAlignment(HorizontalAlignment.Left)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Foreground(new SolidColorBrush(Colors.White))
                            .Text(() => pwd.Title),
                        new Button()
                            .Grid(column: 1)
                            .HorizontalAlignment(HorizontalAlignment.Right)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Content("Delete")
                            .Command(x =>
                                x.Source(root)
                                    .DataContext<PasswordsViewModel>()
                                    .Binding(v => v.DeletePassword)
                            )
                            .CommandParameter(() => pwd)
                    )
            );

        list.ItemClick += (s, e) =>
        {
            ListView lv = (ListView)s;
            PasswordPreview? clickedItem = e.ClickedItem as PasswordPreview;
            if (clickedItem is not null)
            {
                PasswordsViewModel? vm = root.DataContext as PasswordsViewModel;
                vm!.DecryptPassword.Execute(clickedItem);
            }
        };

        return template;
    }
}
