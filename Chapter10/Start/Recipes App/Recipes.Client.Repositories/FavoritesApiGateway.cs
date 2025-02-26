using Recipes.Client.Core;
using Recipes.Client.Core.Features.Favorites;
using Recipes.Client.Repositories.Api;
using Recipes.Shared.Dto;
using System;

namespace Recipes.Client.Repositories;

public class FavoritesApiGateway : ApiGateway, IFavoritesRepository
{
    readonly IFavoritesApi _api;

    public FavoritesApiGateway(IFavoritesApi api)
    {
        _api = api;
    }

    public Task<Result<Nothing>> Add(string userId, string id)
        => InvokeAndMap(_api.AddFavorite(userId,new FavoriteDto(id)));
    
    public Task<Result<string[]>> LoadFavorites(string userId)
    {
        return InvokeAndMap(_api.GetFavorites(userId));
    }

    public Task<Result<Nothing>> Remove(string userId, string id)
    {
        return InvokeAndMap(_api.RemoveFavorite(userId, id));
    }
}
