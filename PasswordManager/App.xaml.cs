using Uno.Resizetizer;

namespace PasswordManager;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    internal static Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Load WinUI Resources
        Resources.Build(r => r.Merged(new XamlControlsResources()));

        // Load Uno.UI.Toolkit and Material Resources
        Resources.Build(r =>
            r.Merged(
                new MaterialToolkitTheme(
                    new Styles.ColorPaletteOverride(),
                    new Styles.MaterialFontsOverride()
                )
            )
        );
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host =>
                host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .ConfigureServices(
                        (context, services) =>
                        {
                            services.AddSingleton<IDBService, DBService>();
                            services.AddSingleton<IEncryptionService, EncryptionService>();
                            services.AddSingleton<IEncryptionKeyService, EncryptionKeyService>();
                        }
                    )
                    .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

#if __ANDROID__
        Uno.UI.FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages = true;
#endif

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<CreateMasterPasswordPage, CreateMasterPasswordModel>(),
            new ViewMap<PasswordsPage, PasswordsModel>()
        );

        routes.Register(
            new RouteMap(
                "",
                View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new(
                        "Main",
                        View: views.FindByViewModel<MainModel>(),
                        IsDefault: true,
                        Init: (request) =>
                        {
                            if (!DbHelpers.DoesDbExist())
                            {
                                DbHelpers.CreateDirectory();
                                request = request with { Route = Route.PageRoute("CreateMP") };
                            }
                            return request;
                        }
                    ),
                    new("CreateMP", View: views.FindByViewModel<CreateMasterPasswordModel>()),
                    new("Passwords", View: views.FindByViewModel<PasswordsModel>()),
                ]
            )
        );
    }
}
