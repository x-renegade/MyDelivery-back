

using Application.Common.Contracts.Repositories;
using Application.Common.Models.User.Requests.Location;
using Application.Common.Models.User.Responses.Location;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Location;

public class LocationRepository(Contex contex) : ILocationResository
{
    public async Task AddAsync(Domain.Entities.Map.Location location)
    {
        await contex.Set<Domain.Entities.Map.Location>().AddAsync(location);
        await contex.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var location = await GetByIdAsync(id);
        if (location != null)
        {
            contex.Set<Domain.Entities.Map.Location>().Remove(location);
            await contex.SaveChangesAsync();
        }
    }

    public async Task<LocationsResponse> GetAllAsync()
    {
        return new LocationsResponse { Locations = await contex.Set<Domain.Entities.Map.Location>().ToListAsync() };
    }

    public async Task<Domain.Entities.Map.Location?> GetByIdAsync(int id)
    {
        return await contex.Set<Domain.Entities.Map.Location>().FindAsync(id);
    }

    public async Task UpdateAsync(Domain.Entities.Map.Location location)
    {
        contex.Set<Domain.Entities.Map.Location>().Update(location);
        await contex.SaveChangesAsync();
    }
    public async Task DeleteLocationsAsync(LocationsIdsRequest ids)
    {
        var locationsToRemove = await contex.Set<Domain.Entities.Map.Location>()
            .Where(location => ids.Ids.Contains(location.Id))
            .ToListAsync();

        if (locationsToRemove.Count != 0)
        {
            contex.Set<Domain.Entities.Map.Location>().RemoveRange(locationsToRemove);
            await contex.SaveChangesAsync();
        }
    }
    public async Task AddLocationsAsync(LocationsRequest locations)
    {
        if (locations == null || !locations.Locations.Any())
        {
            throw new ArgumentException("At least one location must be provided.", nameof(locations));
        }

        await contex.Set<Domain.Entities.Map.Location>().AddRangeAsync(locations.Locations);
        await contex.SaveChangesAsync();
    }
}
