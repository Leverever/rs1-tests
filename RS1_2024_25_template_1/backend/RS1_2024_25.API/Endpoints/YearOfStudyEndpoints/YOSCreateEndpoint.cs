using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YearOfStudyEndpoints
{
    [Route("yos")]
    public class YOSCreateEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<YOSCreateRequest>.WithActionResult<YOSGetResponse>
    {
        [HttpPost("create")]
        public override async Task<ActionResult<YOSGetResponse>> HandleAsync(YOSCreateRequest request, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var s = await db.StudentsAll.FindAsync(request.StudentId, cancellationToken);

            if(s == null)
            {
                return NotFound();
            }

            bool isRenewing = await db.YearsOfStudies.Where(yos => yos.StudentId == request.StudentId && yos.GodinaStudija == request.GodinaStudija)
                .FirstOrDefaultAsync(cancellationToken) != null;

            YearOfStudy yos = new YearOfStudy
            {
                StudentId = request.StudentId,
                GodinaStudija = request.GodinaStudija,
                AkademskaGodinaId = request.AkademskaGodinaId,
                DatumUpisa = request.DatumUpisa,
                SnimioId = request.SnimioId,
            };

            yos.CijenaSkolarine = isRenewing ? 400f : 1800f;
            yos.Obnova = isRenewing;

            await db.YearsOfStudies.AddAsync(yos, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await db.AcademicYears.LoadAsync(cancellationToken);

            return Ok(new YOSGetResponse
            {
                Id = yos.Id,
                AkademskaGodina = yos.AkademskaGodina?.Description ?? "",
                DatumUpisa = yos.DatumUpisa,
                GodinaStudija = yos.GodinaStudija,
                Obnova = yos.Obnova,
                Snimio = yos.Snimio?.Email ?? "",
                DatumOvjere = yos.DatumOvjere,
                Komentar = yos.NapomenaZaOvjeru
            });
        }
    }

    public class YOSCreateRequest
    {
        public int StudentId { get; set; }
        public DateTime DatumUpisa { get; set; }
        public int GodinaStudija { get; set; }
        public int AkademskaGodinaId { get; set; }
        public int SnimioId     { get; set; }
    }

    public class YOSGetResponse
    {
        public int Id { get; set; }
        public string AkademskaGodina { set; get; } = string.Empty;
        public int GodinaStudija {  set; get; }
        public bool Obnova { get; set; }
        public DateTime DatumUpisa {  set; get; }
        public string Snimio { set; get; } = string.Empty;
        public DateTime? DatumOvjere { set; get; }
        public string? Komentar {  set; get; }
    }
}
