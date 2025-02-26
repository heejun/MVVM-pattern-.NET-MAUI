using Recipes.Client.Core;
using Recipes.Shared.Dto;
using Refit;

namespace Recipes.Client.Repositories.Api;

public interface IFavoritesApi
{
    [Get("/users/{userId}/favorites")]
    Task<ApiResponse<string[]>> GetFavorites(string userId);

    [Post("/users/{userId}/favorites")]
    Task<ApiResponse<Nothing>> AddFavorite(string userId, [Body]FavoriteDto favorite);

    [Delete("/users/{userId}/favorites/{recipeId")]
    Task<ApiResponse<Nothing>> RemoveFavorite(string userId, string recipeId);
}
