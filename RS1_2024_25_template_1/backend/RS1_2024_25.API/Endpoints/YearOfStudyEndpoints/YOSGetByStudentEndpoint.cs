using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YearOfStudyEndpoints
{
    [Route("yos")]
    public class YOSGetByStudentEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<int>.WithActionResult<List<YOSGetResponse>>
    {
        [HttpGet("get/{id}")]
        public override async Task<ActionResult<List<YOSGetResponse>>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var s = await db.StudentsAll.FindAsync(id, cancellationToken);

            if (s == null)
            {
                return NotFound();
            }

            await db.AcademicYears.LoadAsync(cancellationToken);
            await db.MyAppUsers.LoadAsync(cancellationToken);

            return Ok(await db.YearsOfStudies.Where(yos => yos.StudentId == id).Select(yos => new YOSGetResponse
            {
                Id = yos.Id,
                AkademskaGodina = yos.AkademskaGodina.Description,
                DatumUpisa = yos.DatumUpisa,
                GodinaStudija = yos.GodinaStudija,
                Obnova = yos.Obnova,
                Snimio = yos.Snimio.Email,
                DatumOvjere = yos.DatumOvjere,
                Komentar = yos.NapomenaZaOvjeru
            }).ToListAsync());
        }
    }
}
