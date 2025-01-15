

using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Models.User.Requests.Location;
using Application.Common.Models.User.Responses.Location;
using Domain.Entities.Map;

namespace Infrastructure.Services.Location;

public class LocationService(ILocationResository repository) : ILocationService
{
    public async Task AddLocationsAsync(LocationsRequest locations)
    {
        if (locations == null || !locations.Locations.Any())
        {
            throw new ArgumentException("At least one location must be provided.", nameof(locations));
        }

        // Дополнительные проверки, например, на обязательность адреса
        foreach (var location in locations.Locations)
        {
            if (string.IsNullOrWhiteSpace(location.Address))
            {
                throw new ArgumentException("Each location must have an address.", nameof(locations));
            }
            if (string.IsNullOrWhiteSpace(location.Description))
            {
                throw new ArgumentException("Each location must have an description.", nameof(locations));
            }
        }

        await repository.AddLocationsAsync(locations);
    }
    public async Task AddLocationAsync(Domain.Entities.Map.Location location)
    {
        if (location == null)
        {
            throw new ArgumentNullException(nameof(location), "Location cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(location.Address))
        {
            throw new ArgumentException("Address is required.", nameof(location.Address));
        }
        if (string.IsNullOrWhiteSpace(location.Description))
        {
            throw new ArgumentException("Description is required", nameof(location.Description));
        }

        await repository.AddAsync(location);
    }

    public async Task DeleteLocationAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        _ = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Location with Id {id} not found.");
        await repository.DeleteAsync(id);
    }

    public async Task DeleteLocationsAsync(LocationsIdsRequest ids)
    {
        if (ids == null || !ids.Ids.Any())
        {
            throw new ArgumentException("At least one Id must be provided.", nameof(ids));
        }

        var locations = await repository.GetAllAsync();
        var invalidIds = ids.Ids.Except(locations.Locations.Select(l => l.Id)).ToList();

        if (invalidIds.Count != 0)
        {
            throw new KeyNotFoundException($"Locations with the following Ids were not found: {string.Join(", ", invalidIds)}.");
        }

        await repository.DeleteLocationsAsync(ids);
    }

    public async Task<LocationsResponse> GetAllLocationsAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<Domain.Entities.Map.Location?> GetLocationByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return await repository.GetByIdAsync(id);
    }

    public async Task UpdateLocationAsync(Domain.Entities.Map.Location location)
    {
        if (location == null)
        {
            throw new ArgumentNullException(nameof(location), "Location cannot be null.");
        }

        if (location.Id <= 0)
        {
            throw new ArgumentException("Valid Id is required.", nameof(location.Id));
        }

        _ = await repository.GetByIdAsync(location.Id)
            ?? throw new KeyNotFoundException($"Location with Id {location.Id} not found.");
        await repository.UpdateAsync(location);
    }
}
