﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Recipes.Client.Core.Features.Favorites;
using Recipes.Client.Core.Features.Ratings;
using Recipes.Client.Core.Features.Recipes;
using Recipes.Client.Core.Navigation;
using Recipes.Client.Core.Services;
using System.Collections.ObjectModel;

namespace Recipes.Client.Core.ViewModels;

public partial class RecipeDetailViewModel : ObservableObject, INavigationParameterReceiver, INavigatedTo, INavigatedFrom
{
    private readonly IRecipeService recipeService;
    private readonly IFavoritesService favoritesService;
    private readonly IRatingsService ratingsService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;

    RecipeDetail recipeDto;

    private bool _hideAllergenInformation = true;

    int updateCount = 0;
    int maxUpdatedAllowed = 5;

    string _title;
    public string Title 
    { 
        get => _title; 
        set => SetProperty(ref _title, value);
    }

    string[] _allergens = new string[0];
    public string[] Allergens
    { 
        get => _allergens; 
        set => SetProperty(ref _allergens, value);
    }

    int? _calories;
    public int? Calories 
    {
        get => _calories; 
        set => SetProperty(ref _calories, value);
    }

    int? _readyInMinutes;
    public int? ReadyInMinutes 
    {
        get => _readyInMinutes;
        set => SetProperty(ref _readyInMinutes, value);
    }

    DateTime _lastUpdated;
    public DateTime LastUpdated 
    {
        get => _lastUpdated; 
        set => SetProperty(ref _lastUpdated, value);
    }

    string _author;
    public string Author 
    {
        get => _author; 
        set => SetProperty(ref _author, value);
    }

    string _image;
    public string Image 
    { 
        get => _image;
        set => SetProperty(ref _image, value);
    }

    RecipeRatingsSummaryViewModel _ratingSummary;
    public RecipeRatingsSummaryViewModel RatingSummary 
    {
        get => _ratingSummary;
        set => SetProperty(ref _ratingSummary, value);
    }

    List<InstructionBaseViewModel> _instructions;
    public List<InstructionBaseViewModel> Instructions 
    {
        get => _instructions;
        set => SetProperty(ref _instructions, value);
    }

    IngredientsListViewModel _ingredientsList;
    public IngredientsListViewModel IngredientsList
    {
        get => _ingredientsList;
        set => SetProperty(ref _ingredientsList, value);
    }

    public bool HideAllergenInformation
    {
        get => _hideAllergenInformation;
        set => SetProperty(ref _hideAllergenInformation, value);
    }

    private bool _isFavorite = false;
    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (SetProperty(ref _isFavorite, value))
            {
                AddAsFavoriteCommand.NotifyCanExecuteChanged();
                RemoveAsFavoriteCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isLoading = true;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }


    public ObservableCollection<RecipeIngredientViewModel> ShoppingList { get; } = new();

    public IRelayCommand AddAsFavoriteCommand { get; }
    public IRelayCommand RemoveAsFavoriteCommand { get; }
    public IRelayCommand AddToShoppingListCommand { get; }
    public IRelayCommand RemoveFromShoppingListCommand { get; }
    public IRelayCommand UserIsBrowsingCommand { get; }
    public IAsyncRelayCommand NavigateToRatingsCommand { get; }
    public IAsyncRelayCommand NavigateToAddRatingCommand { get; }

    public IRelayCommand FavoriteToggledCommand { get; }

    public RecipeDetailViewModel(IRecipeService recipeService, 
        IFavoritesService favoritesService, 
        IRatingsService ratingsService, INavigationService navigationService,
        IDialogService dialogService)
    {
        this.recipeService = recipeService;
        this.favoritesService  = favoritesService;
        this.ratingsService = ratingsService;
        this.navigationService = navigationService;
        this.dialogService = dialogService;

        AddAsFavoriteCommand =
               new AsyncRelayCommand(AddAsFavorite, CanAddAsFavorite);
        RemoveAsFavoriteCommand = 
            new AsyncRelayCommand(RemoveAsFavorite, CanRemoveAsFavorite);
        UserIsBrowsingCommand = new RelayCommand(UserIsBrowsing);
        AddToShoppingListCommand = new RelayCommand<RecipeIngredientViewModel>(AddToShoppingList);
        RemoveFromShoppingListCommand = new RelayCommand<RecipeIngredientViewModel>(RemoveFromShoppingList);
        NavigateToRatingsCommand = new AsyncRelayCommand(NavigateToRatings);
        NavigateToAddRatingCommand = new AsyncRelayCommand(NavigateToAddRating);
        FavoriteToggledCommand = new AsyncRelayCommand<bool>(FavoriteToggled, (e) => updateCount < maxUpdatedAllowed);
    }

    private async Task FavoriteToggled(bool isFavorite)
    {
        if (isFavorite)
        {
            await favoritesService.Add(recipeDto.Id);
        }
        else
        {
            await favoritesService.Remove(recipeDto.Id);
        }

        updateCount++;
        FavoriteToggledCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadRecipe(string recipeId)
    {
        IsLoading = true;

        var loadRecipeTask = recipeService.LoadRecipe(recipeId);
        var loadIsFavoriteTask = favoritesService.IsFavorite(recipeId);
        var loadRatingsTask = ratingsService.LoadRatingsSummary(recipeId);

        await Task.WhenAll(loadRecipeTask, loadIsFavoriteTask, loadRatingsTask);

        if(!loadRecipeTask.Result.IsSuccess || !loadRatingsTask.Result.IsSuccess)
        {
            var result = await dialogService.AskYesNo("Unable to load recipe", "Want to retry?");
            if(result)
                await LoadRecipe(recipeId);
            else
                await navigationService.GoBack();
        }
        else
        {
            MapRecipeData(loadRecipeTask.Result.Data, loadRatingsTask.Result.Data, loadIsFavoriteTask.Result);
        }

        IsLoading = false;
    }

    private void MapRecipeData(RecipeDetail recipe, RatingsSummary ratings, bool isFavorite)
    {
        recipeDto = recipe;
        Title = recipe.Name;
        Allergens = recipe.Allergens;
        Calories =  recipe.Calories;
        ReadyInMinutes = recipe.ReadyInMinutes;
        LastUpdated = recipe.LastUpdated;
        Author = recipe.Author;
        Image = recipe.Image ?? "fallback.png";

        var instructionVMs = new List<InstructionBaseViewModel>();
        foreach (var item in recipe.Instructions)
        {
            if (item.IsNote)
                instructionVMs.Add(new NoteViewModel(item.Text));
            else
                instructionVMs.Add(new InstructionViewModel(item.Index ?? 0, item.Text));
        }

        Instructions = instructionVMs;

        IngredientsList = new IngredientsListViewModel(recipe.Ingredients);

        IsFavorite = isFavorite;

        RatingSummary = new RecipeRatingsSummaryViewModel(ratings.TotalReviews, ratings.AverageRating, ratings.MaxRating);
    }

    private Task AddAsFavorite() => UpdateIsFavorite(true);

    private bool CanAddAsFavorite() => !IsFavorite;

    private Task RemoveAsFavorite() => UpdateIsFavorite(false);

    private Task UpdateIsFavorite(bool newValue)
    {
        IsFavorite = newValue;
        return FavoriteToggled(newValue);
    }

    private bool CanRemoveAsFavorite() => IsFavorite;

    private void UserIsBrowsing()
    {
        //Do Logging
    }

    private void AddToShoppingList(RecipeIngredientViewModel viewModel)
    {
        if (ShoppingList.Contains(viewModel))
            return;
        ShoppingList.Add(viewModel);
    }

    private void RemoveFromShoppingList(RecipeIngredientViewModel viewModel)
    {
        if (ShoppingList.Contains(viewModel))
            ShoppingList.Remove(viewModel);
    }

    private Task NavigateToRatings()
        =>  navigationService.GoToRecipeRatingDetail(recipeDto);

    public Task OnNavigatedTo(Dictionary<string, object> parameters)
        => LoadRecipe(parameters["id"].ToString());


    private Task NavigateToAddRating()
        => navigationService.GoToAddRating(recipeDto);


    public Task OnNavigatedTo(NavigationType navigationType)
    {
        return Task.CompletedTask;
    }

    public Task OnNavigatedFrom(NavigationType navigationType)
    {
        return Task.CompletedTask;
    }
}
