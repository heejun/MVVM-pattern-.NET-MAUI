namespace Recipes.Client.Core.Navigation;

public interface INavigationInterceptor
{
    Task OnNavigatedTo(object bindingConetext, NavigationType navigationType);
}
