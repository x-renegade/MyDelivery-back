

using Application.Common.Models.User.Requests.Location;
using Application.Common.Models.User.Responses.Location;
using Domain.Entities.Map;

namespace Application.Common.Contracts.Repositories;
public interface ILocationResository
{
    Task<LocationsResponse> GetAllAsync();
    Task<Location?> GetByIdAsync(int id);
    Task AddAsync(Location location);
    Task UpdateAsync(Location location);
    Task DeleteAsync(int id);
    Task DeleteLocationsAsync(LocationsIdsRequest ids);
    Task AddLocationsAsync(LocationsRequest locations);
}
