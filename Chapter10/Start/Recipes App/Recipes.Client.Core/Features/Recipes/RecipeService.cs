namespace Recipes.Client.Core.Features.Recipes;

public class RecipeService : IRecipeService
{
    readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public Task<Result<RecipeDetail>> LoadRecipe(string id)
    {
        return _recipeRepository.LoadRecipe(id);
    }

    public Task<Result<LoadRecipesResponse>> LoadRecipes(
        int pageSize = 7, int page = 0)
    {
        return _recipeRepository.LoadRecipes(pageSize, page);
    }
}
