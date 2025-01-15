

namespace Domain.Entities.Map;
public class Location
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
