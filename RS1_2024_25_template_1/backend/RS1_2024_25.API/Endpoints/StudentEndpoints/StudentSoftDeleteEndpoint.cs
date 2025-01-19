using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentSoftDeleteEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<int>.WithActionResult<StudentGetAllEndpoint.StudentGetAllResponse>
    {
        [HttpDelete("delete/{id}")]
        public override async Task<ActionResult<StudentGetAllEndpoint.StudentGetAllResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
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

            s.IsDeleted = true;
            await db.SaveChangesAsync(cancellationToken);


            await db.Municipalities.LoadAsync();
            await db.Cities.LoadAsync();
            await db.Regions.LoadAsync();
            await db.MyAppUsers.LoadAsync();

            return Ok(new StudentGetAllEndpoint.StudentGetAllResponse
            {
                ID = s.ID,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                StudentNumber = s.StudentNumber,
                Citizenship = s.Citizenship != null ? s.Citizenship.Name : null,
                BirthMunicipality = s.BirthMunicipality != null ? s.BirthMunicipality.Name : null,
                BirthMunicipalityId = s.BirthMunicipalityId != null ? s.BirthMunicipalityId.Value : 0,
                CountryId = s.BirthMunicipality.City.Region.CountryId,
                DateOfBirth = s.BirthDate != null ? s.BirthDate.Value : DateOnly.MinValue,
                Phone = s.ContactMobilePhone ?? "",
                IsDeleted = s.IsDeleted
            });
        }
    }
}
