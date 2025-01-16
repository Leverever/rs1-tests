using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync.WithRequest<int>.WithActionResult<StudentGetIdResponse>
    {
        [HttpGet("get/{id}")]
        public override async Task<ActionResult<StudentGetIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var s = await db.StudentsAll.FindAsync(id);

            if(s == null)
            {
                return NotFound();
            }

            await db.Municipalities.LoadAsync(cancellationToken);
            await db.Cities.LoadAsync(cancellationToken);
            await db.Regions.LoadAsync(cancellationToken);
            await db.MyAppUsers.LoadAsync(cancellationToken);

            return Ok(new StudentGetIdResponse
            {
                Id = id,
                BirthMunicipality = s.BirthMunicipality?.Name ?? "",
                BirthMunicipalityId = s.BirthMunicipalityId.GetValueOrDefault(),
                DateOfBirth = s.BirthDate != null ? new DateTime(s.BirthDate.Value!, TimeOnly.MinValue) : DateTime.Now,
                Phone = s.ContactMobilePhone ?? "061-xxx-xxx",
                CountryId = s.BirthMunicipality?.City?.Region?.CountryId ?? 0,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
            });
        }
    }

    public class StudentGetIdResponse
    {
        public int Id { get; set; }
        public int BirthMunicipalityId { get; set; }
        public string BirthMunicipality {  get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
