using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentSoftDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync.WithRequest<int>.WithActionResult
    {
        [HttpDelete("delete/{id}")]
        public override async Task<ActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var s = await db.StudentsAll.FindAsync(id, cancellationToken);

            if(s == null)
                return NotFound();

            s.IsDeleted = true;

            await db.SaveChangesAsync(cancellationToken);
            return Ok(s);
        }
    }
}
