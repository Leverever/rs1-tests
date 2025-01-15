using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints
{
    [Route("municipalities")]
    public class MunicipalityGetAllByCountryEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync.WithRequest<int>.WithResult<List<MunicipalityResponse>>
    {
        [HttpGet("{countryId}")]
        public override async Task<List<MunicipalityResponse>> HandleAsync(int countryId, CancellationToken cancellationToken = default)
        {
            await db.Cities.LoadAsync();
            await db.Regions.LoadAsync();
            return await db.Municipalities.Where(m => m.City.Region.CountryId == countryId)
                .Select(m => new MunicipalityResponse
                {
                    Id = m.ID,
                    Name = m.Name,
                })
                .ToListAsync();
        }
    }

    public class MunicipalityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
