

using Application.Common.Models.User.Requests.Location;
using Application.Common.Models.User.Responses.Location;
using Domain.Entities.Map;

namespace Application.Common.Contracts.Services;

public interface ILocationService
{
    Task AddLocationsAsync(LocationsRequest locations);
    Task AddLocationAsync(Location location);
    Task<Location?> GetLocationByIdAsync(int id);
    Task<LocationsResponse> GetAllLocationsAsync();
    Task UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(int id);
    Task DeleteLocationsAsync(LocationsIdsRequest ids);
}
