using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("municipalities")]
    public class MunicipalityGetByCountryIdEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<int>.WithActionResult<List<MunicipalityGetResponse>>
    {
        [HttpGet("get/{id}")]
        public override async Task<ActionResult<List<MunicipalityGetResponse>>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            await db.Cities.LoadAsync();
            await db.Regions.LoadAsync();
            return Ok(await db.Municipalities.Where(m => m.City.Region.CountryId == id).Select(m => new MunicipalityGetResponse
            {
                Id = m.ID,
                Name = m.Name,
            }).ToListAsync(cancellationToken));
        }
    }

    public class MunicipalityGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
