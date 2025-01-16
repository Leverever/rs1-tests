using FluentValidation;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.YearOfStudyEndpoints
{
    public class YOSCreateValidator : AbstractValidator<YOSCreateRequest>
    {
        public YOSCreateValidator(ApplicationDbContext db)
        {
            RuleFor(x => x.GodinaStudija).NotEmpty().GreaterThan(0).LessThan(6).WithMessage("Invalidna godina studija");

            RuleFor(x => x.AkademskaGodinaId).NotEmpty().WithMessage("Akademska potrebna")
                .Must(x =>
                {
                    return db.AcademicYears.Where(ay => ay.ID == x).FirstOrDefault() != null;
                }).WithMessage("Akademska godina sa ovim ID-om ne postoji");

            RuleFor(x => x.DatumUpisa).NotEmpty().WithMessage("Datum upisa potreban");

            RuleFor(x => x.SnimioId).NotEmpty().WithMessage("Snimatelj potreban")
                .Must(x =>
                    {
                        return db.MyAppUsers.Where(ay => ay.ID == x).FirstOrDefault() != null;
                    }).WithMessage("Korisnik sa ovim ID-om ne postoji");

            RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student potreban")
                .Must(x =>
                     {
                    return db.StudentsAll.Where(ay => ay.ID == x).FirstOrDefault() != null;
                    }).WithMessage("Student sa ovim ID-om ne postoji");

            /*
            RuleFor(x => x).Must(x =>
            {
                var akademska = db.AcademicYears.Where(ay => ay.ID == x.AkademskaGodinaId).FirstOrDefault();
                if(akademska == null)
                {
                    return false;
                }

                return akademska.StartDate < DateOnly.FromDateTime(x.DatumUpisa);
            }).WithMessage("Rok za upis ove akademske godine nije počeo");

            RuleFor(x => x).Must(x =>
            {
                var akademska = db.AcademicYears.Where(ay => ay.ID == x.AkademskaGodinaId).FirstOrDefault();
                if (akademska == null)
                {
                    return false;
                }

                return akademska.EndDate > DateOnly.FromDateTime(x.DatumUpisa);
            }).WithMessage("Rok za upis ove akademske godine je prošao");
            */
        }
    }
}
