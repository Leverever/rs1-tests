using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YOSEndpoints
{
    [Route("yos")]
    public class AcademicYearsGetAllEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithoutRequest.WithActionResult<List<AcademicYearGetResponse>>
    {
        [HttpGet("ay")]
        public override async Task<ActionResult<List<AcademicYearGetResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await db.AcademicYears.Select(ay => new AcademicYearGetResponse
            {
                EndDate = ay.EndDate,
                Id = ay.ID,
                Name = ay.Description,
                StartDate = ay.StartDate
            }).ToListAsync());
        }
    }

    public class AcademicYearGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
