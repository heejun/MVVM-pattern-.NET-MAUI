using Recipes.Client.Core.Navigation;

namespace Recipes.Mobile;


public partial class AppShell : Shell
{
    private readonly INavigationInterceptor _interceptor;

    public AppShell(INavigationInterceptor interceptor)
    {
        _interceptor = interceptor;
        InitializeComponent();
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        var navigationType = GetNavigationType(args.Source);
        await _interceptor.OnNavigatedTo(CurrentPage?.BindingContext, navigationType);
    }

    private NavigationType GetNavigationType(ShellNavigationSource source)
        => source switch
        {
            ShellNavigationSource.Push or
            ShellNavigationSource.Insert
                => NavigationType.Forward,
            ShellNavigationSource.Pop or
            ShellNavigationSource.PopToRoot or
            ShellNavigationSource.Remove
                => NavigationType.Backward,
            ShellNavigationSource.ShellItemChanged or
            ShellNavigationSource.ShellSectionChanged or
            ShellNavigationSource.ShellContentChanged
                => NavigationType.SectionChange,
            _ => NavigationType.Unknown
        };
}