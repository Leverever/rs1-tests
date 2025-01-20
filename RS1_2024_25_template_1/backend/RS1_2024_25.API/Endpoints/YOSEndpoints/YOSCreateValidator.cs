using FluentValidation;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.YOSEndpoints
{
    public class YOSCreateValidator : AbstractValidator<YOSCreateRequest>
    {
        public YOSCreateValidator(ApplicationDbContext db)
        {
            RuleFor(x => x.GodinaStudija).NotEmpty().GreaterThan(0).LessThan(6).WithMessage("Godina studija mora biti u rangu 1-5");

            RuleFor(x => x.AkademskaGodinaId).NotEmpty().GreaterThan(0).Must(y => db.AcademicYears.Where(ay => ay.ID == y).FirstOrDefault() != null)
                .WithMessage("Akademska godina ne postoji!");

            RuleFor(x => x.StudentId).NotEmpty().GreaterThan(0).Must(y => db.StudentsAll.Where(ay => ay.ID == y).FirstOrDefault() != null)
                .WithMessage("Student ne postoji!");

            RuleFor(x => x).Must(x =>
            {
                var ay = db.AcademicYears.Find(x.AkademskaGodinaId);
                if(ay == null)
                {
                    return false;
                }
                if(x.DatumUpisa < new DateTime(ay.StartDate, TimeOnly.MinValue) || x.DatumUpisa > new DateTime(ay.EndDate, TimeOnly.MinValue))
                {
                    return false;
                }
                return true;
            }).WithMessage("Rok za upis u ovu akademsku godinu je istekao.");
        }
    }
}
