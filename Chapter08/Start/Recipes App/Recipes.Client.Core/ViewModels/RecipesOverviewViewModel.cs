using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes.Client.Core.Features.Favorites;
using Recipes.Client.Core.Features.Recipes;
using Recipes.Client.Core.Navigation;
using System.Collections.ObjectModel;

namespace Recipes.Client.Core.ViewModels;

public class RecipesOverviewViewModel : ObservableObject
{
    private readonly INavigationService navigationService;
    private readonly IRecipeService recipeService;
    private readonly IFavoritesService favoritesService;

    public ObservableCollection<RecipeListItemViewModel> Recipes { get; }

    RecipeListItemViewModel? _selectedRecipe;
    public RecipeListItemViewModel? SelectedRecipe
    {
        get => _selectedRecipe;
        set => SetProperty(ref _selectedRecipe, value);
    }

    int _totalNumberOfRecipes = 0;
    public int TotalNumberOfRecipes 
    {
        get => _totalNumberOfRecipes;
        set => SetProperty(ref _totalNumberOfRecipes, value);
    }

    public AsyncRelayCommand TryLoadMoreItemsCommand { get; }
    public AsyncRelayCommand NavigateToSelectedDetailCommand { get; }

    public RecipesOverviewViewModel(
        IRecipeService recipeService, 
        IFavoritesService favoritesService,
        INavigationService navigationService)
    {
        this.navigationService = navigationService;
        this.recipeService = recipeService;
        this.favoritesService = favoritesService;

        Recipes = new ();
        TryLoadMoreItemsCommand = 
            new AsyncRelayCommand(TryLoadMoreItems);
        NavigateToSelectedDetailCommand = 
            new AsyncRelayCommand(NavigateToSelectedDetail);

        LoadRecipes(7, 0);
    }

    private async Task LoadRecipes(int pageSize, int page)
    {
        var loadRecipesTask = recipeService.LoadRecipes(pageSize, page);
        var loadFavoritesTask = favoritesService.LoadFavorites();

        await Task.WhenAll(loadRecipesTask, loadFavoritesTask);

        var recipesResult = loadRecipesTask.Result;
        var favoritesResult = loadFavoritesTask.Result;

        TotalNumberOfRecipes = recipesResult.TotalItems;

        recipesResult.Recipes.ToList().ForEach(recipe =>
        {
            var isFavorite = favoritesResult.Contains(recipe.Id);
            Recipes.Add(new RecipeListItemViewModel(recipe.Id, recipe.Title, isFavorite, recipe.Image));
        });
    }

    private async Task NavigateToSelectedDetail()
    {
        if (SelectedRecipe is not null)
        {
            await navigationService.GoToRecipeDetail(SelectedRecipe.Id);
            SelectedRecipe = null;
        }
    }

    private async Task TryLoadMoreItems()
        => await LoadRecipes(7, Recipes.Count / 7);
}
