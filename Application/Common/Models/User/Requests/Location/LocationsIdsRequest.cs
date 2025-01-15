
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests.Location;

public class LocationsIdsRequest
{
    [Required(ErrorMessage = "Ids is required")]
    public required IEnumerable<int> Ids { get; set; } = null!;
}
