using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.YOSEndpoints
{
    [Route("yos")]
    public class YOSVerifyEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<YOSVerifyRequest>.WithActionResult
    {
        [HttpPut("verify")]
        public override async Task<ActionResult> HandleAsync(YOSVerifyRequest request, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var yos = await db.YearsOfStudy.Where(yos => yos.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (yos == null)
            {
                return BadRequest();
            }

            yos.DatumOvjere = DateTime.Now;
            yos.Napomena = request.Napomena;

            await db.SaveChangesAsync(cancellationToken);

            return Ok();
        }

    }
        public class YOSVerifyRequest
        {
            public int Id { get; set; }
            public string? Napomena { get; set; }
        }
}
