using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using System.Text.RegularExpressions;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentEditEndpoint(ApplicationDbContext db, IMyAuthService myAuthService) : MyEndpointBaseAsync.WithRequest<StudentEditRequest>.WithActionResult
    {
        [HttpPut("edit")]
        public override async Task<ActionResult> HandleAsync([FromBody] StudentEditRequest request, CancellationToken cancellationToken = default)
        {
            var info = myAuthService.GetAuthInfoFromRequest();

            if(!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var s = await db.StudentsAll.FindAsync(request.Id, cancellationToken);

            if(s == null)
            {
                return NotFound();
            }

            s.BirthDate = DateOnly.FromDateTime(request.DateOfBirth);
            s.ContactMobilePhone = request.Phone;
            s.BirthMunicipalityId = request.MunicipalityId;

            await db.SaveChangesAsync(cancellationToken);

            await db.Municipalities.LoadAsync(cancellationToken);

            return Ok(new StudentGetIdResponse
            {
                Phone = s.ContactMobilePhone,
                BirthMunicipalityId = s.BirthMunicipalityId.GetValueOrDefault(),
                BirthMunicipality = s.BirthMunicipality?.Name ?? "",
                DateOfBirth = s.BirthDate != null ? new DateTime(s.BirthDate.Value!,TimeOnly.MinValue) : DateTime.Now,
                Id = s.ID,
            });
        }
    }

    public class StudentEditRequest
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int MunicipalityId { get; set; }
    }
}
