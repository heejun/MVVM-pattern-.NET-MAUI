using Recipes.Client.Core.Features.Recipes;
using Recipes.Client.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes.Mobile.Navigation;

public class NavigationService : INavigationService, INavigationInterceptor
{
    WeakReference<INavigatedFrom> _previousFrom;

    public async Task OnNavigatedTo(object bindingConetext, NavigationType navigationType)
    {
        if (_previousFrom is not null && _previousFrom.TryGetTarget(out INavigatedFrom from))
        {
            await from.OnNavigatedFrom(navigationType);
        }

        if (bindingConetext is INavigatedTo to)
        {
            await to.OnNavigatedTo(navigationType);
        }

        if (bindingConetext is INavigatedFrom navigatedFrom)
        {
            _previousFrom = new (navigatedFrom);
        }
        else
        {
            _previousFrom = null;
        }
    }

    public Task GoBack()
        => Shell.Current.GoToAsync("..");

    public Task GoToRecipeDetail(string recipeId)
        => Navigate("RecipeDetail", new () {{"id", recipeId}});

    public Task GoToRecipeRatingDetail(RecipeDetailDto recipe)
        => Navigate("RecipeRating", new() {{"recipe", recipe}});

    private async Task Navigate(string pageName, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(pageName);
        if (Shell.Current.CurrentPage.BindingContext is INavigationParameterReceiver receiver)
        {
            await receiver.OnNavigatedTo(parameters);
        }
    }
}
