
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Responses.Location
{
    public class LocationsResponse
    {
        [Required(ErrorMessage = "Locations is required")]
        public required IEnumerable<Domain.Entities.Map.Location> Locations { get; set; } = null!;
    }
}
