
using Application.Common.Contracts.Services;
using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Requests.Location;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController(ILocationService location) : ControllerBase
    {
        [HttpGet("getLocations")]
        public async Task<IActionResult> GetLocations()
        {
            var items= await location.GetAllLocationsAsync();
            return Ok(items);
        }
        [HttpPost("addLocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin")]
        public async Task<IActionResult> AddLocations([FromBody] LocationsRequest request)
        {
            await location.AddLocationsAsync(request);
            return Ok(new { message = "Locations added successfully" });
        }
        [HttpPost("removeLocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> RemoveLocations([FromBody] LocationsIdsRequest request)
        {
            await location.DeleteLocationsAsync(request);
            return Ok(new { message = "Locations added successfully" });
        }
    }
}
