using Recipes.Client.Core;
using Recipes.Client.Core.Features.Recipes;
using Recipes.Client.Repositories.Api;
using static Recipes.Client.Repositories.Mappers.RecipeMapper;

namespace Recipes.Client.Repositories;

public class RecipeApiGateway : ApiGateway, IRecipeRepository
{
    readonly IRecipeApi _api;

    public RecipeApiGateway(IRecipeApi api)
    {
        _api = api;
    }

    public Task<Result<RecipeDetail>> LoadRecipe(string id)
    {
        return InvokeAndMap(_api.GetRecipe(id), MapRecipe);
    }

    public Task<Result<LoadRecipesResponse>> LoadRecipes(int pageSize = 7, int page = 0)
    {
        return InvokeAndMap(_api.GetRecipes(pageSize, page), MapRecipesOverview);
    }
}
