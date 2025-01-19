using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentEditEndpoint(ApplicationDbContext db, IMyAuthService auth) : MyEndpointBaseAsync.WithRequest<StudentEditRequest>.WithActionResult<StudentGetAllEndpoint.StudentGetAllResponse>
    {
        [HttpPut("edit")]
        public override async Task<ActionResult<StudentGetAllEndpoint.StudentGetAllResponse>> HandleAsync(StudentEditRequest request, CancellationToken cancellationToken = default)
        {
            var info = auth.GetAuthInfoFromRequest();

            if (!info.IsLoggedIn)
            {
                return Unauthorized();
            }

            var s = await db.StudentsAll.FindAsync(request.Id, cancellationToken);
            if (s == null)
            {
                return NotFound();
            }

            s.ContactMobilePhone = request.Phone;
            s.BirthDate = DateOnly.FromDateTime(request.DateOfBirth);
            s.BirthMunicipalityId = request.MunicipalityId;

            await db.SaveChangesAsync();


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
                Phone = s.ContactMobilePhone ?? ""
            });
        }
    }

    public class StudentEditRequest
    {
        public int Id { get; set; }
        public string Phone {  get; set; } = string.Empty;
        public int MunicipalityId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
