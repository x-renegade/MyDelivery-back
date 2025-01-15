

using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests.Location;

public class LocationsRequest
{
    [Required(ErrorMessage = "Locations is required")]
    public required IEnumerable<Domain.Entities.Map.Location> Locations { get; set; } = null!;
}
