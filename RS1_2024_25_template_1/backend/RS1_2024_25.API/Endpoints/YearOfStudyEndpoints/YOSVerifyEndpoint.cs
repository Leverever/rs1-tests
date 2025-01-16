using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YearOfStudyEndpoints
{
    [Route("yos")]
    public class YOSVerifyEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<YOSVerifyRequest>.WithActionResult<YOSGetResponse>
    {
        [HttpPost("verify")]
        public override async Task<ActionResult<YOSGetResponse>> HandleAsync(YOSVerifyRequest request, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var yos = await db.YearsOfStudies.FindAsync(request.SemesterId, cancellationToken);

            if(yos == null)
                return NotFound();

            yos.NapomenaZaOvjeru = request.Comment;
            yos.DatumOvjere = DateTime.Now;

            await db.SaveChangesAsync(cancellationToken);

            await db.AcademicYears.LoadAsync(cancellationToken);
            await db.MyAppUsers.LoadAsync(cancellationToken);

            return Ok(new YOSGetResponse
            {
                Id = yos.Id,
                AkademskaGodina = yos.AkademskaGodina.Description,
                DatumUpisa = yos.DatumUpisa,
                GodinaStudija = yos.GodinaStudija,
                Obnova = yos.Obnova,
                Snimio = yos.Snimio.Email,
                DatumOvjere = yos.DatumOvjere,
                Komentar = yos.NapomenaZaOvjeru
            });
        }
    }

    public class YOSVerifyRequest
    {
        public int SemesterId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
