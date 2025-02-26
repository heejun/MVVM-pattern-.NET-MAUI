using Recipes.Shared.Dto;
using Refit;

namespace Recipes.Client.Repositories.Api;

public interface IRatingsApi
{
    [Get("/recipe/{recipeId}/ratings")]
    Task<ApiResponse<RatingDto[]>> GetRatings(string recipeId);

    [Get("/recipe/{recipeId}/ratingssummary")]
    Task<ApiResponse<RatingsSummaryDto>> GetRatingsSummary(string recipeId);
}
