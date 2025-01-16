using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YearOfStudyEndpoints
{
    [Route("academic-years")]
    public class AcademicYearGetAllEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithoutRequest.WithActionResult<List<AcademicYearGetResponse>>
    {
        [HttpGet("get-all")]
        public override async Task<ActionResult<List<AcademicYearGetResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            return Ok(await db.AcademicYears.Select(ay => new AcademicYearGetResponse
            {
                Id = ay.ID,
                Name = ay.Description,
                StartDate = new DateTime(ay.StartDate, TimeOnly.MinValue),
                EndDate = new DateTime(ay.EndDate, TimeOnly.MinValue)
            }).ToListAsync(cancellationToken));
        }
    }

    public class AcademicYearGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
