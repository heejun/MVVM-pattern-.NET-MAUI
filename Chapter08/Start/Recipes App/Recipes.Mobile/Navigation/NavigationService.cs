using Recipes.Client.Core.Features.Recipes;
using Recipes.Client.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes.Mobile.Navigation;

public class NavigationService : INavigationService
{
    public Task GoBack()
        => Shell.Current.GoToAsync("..");

    public Task GoToRecipeDetail(string recipeId)
        => Navigate("RecipeDetail", new () {{"id", recipeId}});

    public Task GoToRecipeRatingDetail(RecipeDetailDto recipe)
        => Navigate("RecipeRating", new() {{"recipe", recipe}});

    private async Task Navigate(string pageName, Dictionary<string, object> parameters)
        => await Shell.Current.GoToAsync(pageName);
}
