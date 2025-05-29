namespace PasswordManager.Presentation;

public sealed partial class PasswordsPage : Page
{
    public PasswordsPage()
    {
        this.DataContext<PasswordsViewModel>(
            (page, vm) =>
                page.NavigationCacheMode(NavigationCacheMode.Required)
                    .Content(
                        new ResponsiveView()
                            .NarrowTemplate(
                                () =>
                                    new Border()
                                        .SafeArea(SafeArea.InsetMask.VisibleBounds)
                                        .Child(
                                            new Grid()
                                                .RowDefinitions("*, *")
                                                .Children(
                                                    new ScrollViewer().Content(
                                                        TabBar(vm, this).Grid(row: 0)
                                                    ),
                                                    PasswordsView(Feedback(vm), Feed(vm, this))
                                                        .Grid(row: 1)
                                                )
                                        )
                            )
                            .WidestTemplate(
                                () =>
                                    new Border()
                                        .SafeArea(SafeArea.InsetMask.VisibleBounds)
                                        .Child(
                                            new Grid()
                                                .ColumnDefinitions("*, 2*")
                                                .Children(
                                                    TabBar(vm, this).Grid(column: 0),
                                                    PasswordsView(Feedback(vm), Feed(vm, this))
                                                        .Grid(column: 1)
                                                )
                                        )
                            )
                    )
        );
    }

    private static StackPanel TabBar(PasswordsViewModel vm, PasswordsPage root) =>
        new StackPanel()
            .Padding(70)
            .Spacing(20)
            .VerticalAlignment(VerticalAlignment.Stretch)
            .Children(
                new Grid()
                    .RowDefinitions("*, *, *, *, *")
                    .RowSpacing(10)
                    .Children(
                        new TextBlock().Grid(row: 0).Text("Add").FontSize(20),
                        new TextBox()
                            .Grid(row: 1)
                            .PlaceholderText("Title")
                            .Text(x => x.Binding(() => vm.TitleOfPasswordToAdd).TwoWay()),
                        new PasswordBox()
                            .Grid(row: 2)
                            .PlaceholderText("Password")
                            .Password(x => x.Binding(() => vm.PasswordToAdd).TwoWay()),
                        new Button()
                            .Grid(row: 3)
                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                            .Content("Generate Password")
                            .Command(() => vm.GeneratePassword),
                        new Button()
                            .Grid(row: 4)
                            .HorizontalAlignment(HorizontalAlignment.Stretch)
                            .VerticalAlignment(VerticalAlignment.Center)
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
                        new TextBlock().Text("Search").FontSize(20),
                        new TextBox()
                            .Grid(column: 2)
                            .PlaceholderText("Search")
                            .Text(x =>
                                x.Binding(() => vm.SearchTerm)
                                    .TwoWay()
                                    .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)
                            )
                    )
            );

    private static ScrollViewer PasswordsView(TextBlock feedback, FeedView feed) =>
        new ScrollViewer().Content(
            new StackPanel()
                .Spacing(20)
                .Padding(70)
                .Children(
                    new TextBlock()
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .FontSize(20)
                        .Text("Passwords"),
                    feedback,
                    feed
                )
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
                vm!.DecryptPassword.Execute(clickedItem.Id);
            }
        };

        return template;
    }
}
