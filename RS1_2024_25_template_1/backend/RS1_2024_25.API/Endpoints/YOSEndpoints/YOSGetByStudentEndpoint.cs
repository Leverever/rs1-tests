using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YOSEndpoints
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

            await db.AcademicYears.LoadAsync(cancellationToken);
            await db.MyAppUsers.LoadAsync(cancellationToken);
            return Ok(await db.YearsOfStudy.Where(yos => yos.StudentId == id).Select(yos => new YOSGetResponse
            {
                Id = yos.Id,
                AkademskaGodina = yos.AkademskaGodina != null ? yos.AkademskaGodina.Description : "",
                Snimio = yos.Snimio != null ? yos.Snimio.Email : "",
                DatumUpisa = DateOnly.FromDateTime(yos.DatumUpisa),
                GodinaStudija = yos.GodinaStudija,
                Obnova = yos.Obnova,
                StudentId = id,
                DatumOvjere = yos.DatumOvjere,
                Napomena = yos.Napomena,
            }).ToListAsync());
        }
    }

    public class YOSGetResponse
    {
        public int Id { get; set; }
        public int GodinaStudija { get; set; }
        public DateOnly DatumUpisa { get; set; }
        public bool Obnova { get; set; }
        public string AkademskaGodina { get; set; } = string.Empty;
        public string Snimio { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public DateTime? DatumOvjere { get; set; }
        public string? Napomena {  get; set; }
    }
}
