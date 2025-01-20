using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using System.Diagnostics.CodeAnalysis;

namespace RS1_2024_25.API.Endpoints.YOSEndpoints
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

            bool isRenewing = await db.YearsOfStudy.Where(y => y.GodinaStudija == request.GodinaStudija && y.StudentId == request.StudentId).FirstOrDefaultAsync() != null;

            YearOfStudy yos = new YearOfStudy
            {
                StudentId = request.StudentId,
                GodinaStudija = request.GodinaStudija,
                DatumUpisa = request.DatumUpisa,
                AkademskaGodinaId = request.AkademskaGodinaId,
                Obnova = isRenewing,
                SnimioId = info.UserId,
                CijenaSkolarine = isRenewing ? 400f : 1800f
            };

            await db.YearsOfStudy.AddAsync(yos, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);



            await db.AcademicYears.LoadAsync(cancellationToken);
            await db.MyAppUsers.LoadAsync(cancellationToken);
            return Ok(new YOSGetResponse
            {
                Id = yos.Id,
                AkademskaGodina = yos.AkademskaGodina != null ? yos.AkademskaGodina.Description : "",
                Snimio = yos.Snimio != null ? yos.Snimio.Email : "",
                DatumUpisa = DateOnly.FromDateTime(yos.DatumUpisa),
                GodinaStudija = yos.GodinaStudija,
                Obnova = yos.Obnova,
                StudentId = yos.StudentId,
                DatumOvjere = yos.DatumOvjere,
                Napomena = yos.Napomena,
            });
        }
    }

    public class YOSCreateRequest
    {
        public int StudentId { get; set; }
        public int GodinaStudija { get; set; }
        public DateTime DatumUpisa { get; set; }
        public int AkademskaGodinaId { get; set; }
    }
}
