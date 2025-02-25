﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Recipes.Client.Core.Messages;
using Recipes.Client.Core.Navigation;

namespace Recipes.Client.Core.ViewModels;

public class RecipeIngredientViewModel : ObservableObject, INavigatedFrom, INavigatedTo
{
    readonly int baseServings;
    readonly double baseAmount;

    public string IngredientName { get; }

    public string? Measurement { get; }

    double? _displayAmount;
    public double DisplayAmount
    {
        get => _displayAmount ?? baseAmount;
        set => SetProperty(ref _displayAmount, value);
    }


    public RecipeIngredientViewModel(string ingredientName, 
        double baseAmount, string? measurement = null, int baseServings = 4)
    {
        IngredientName = ingredientName;
        Measurement = measurement;
        this.baseAmount = baseAmount;
        this.baseServings = baseServings;

        WeakReferenceMessenger.Default
            .Register<ServingsChangedMessage>(this,  (r, m) => 
            ((RecipeIngredientViewModel)r)
            .UpdateServings(m.Value));
    }

    private void UpdateServings(int servings)
    {
        var factor = servings / (double)baseServings;
        DisplayAmount = factor * baseAmount;
    }

    public Task OnNavigatedFrom(NavigationType navigationType)
    {
        return Task.CompletedTask;
    }

    public Task OnNavigatedTo(NavigationType navigationType)
    {
        return Task.CompletedTask;
    }
}