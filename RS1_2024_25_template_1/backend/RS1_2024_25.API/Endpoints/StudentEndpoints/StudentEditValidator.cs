using FluentValidation;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    public class StudentEditValidator : AbstractValidator<StudentEditRequest>
    {
        public StudentEditValidator(ApplicationDbContext db)
        {
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Must provide phone nunmber")
                .Matches("^06\\d-\\d\\d\\d-\\d\\d\\d").WithMessage("Must match format 06x-xxx-xxx");

            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Must provide date of birth")
                .GreaterThan(new DateTime(1900, 1, 1)).LessThan(new DateTime(2020, 1, 1)).WithMessage("Date must belong in range 1900-01-01 - 2020-01-01");

            RuleFor(x => x.MunicipalityId).NotEmpty().GreaterThan(0).
                Must(id => db.Municipalities.Where(m => m.ID == id).FirstOrDefault() != null)
                .WithMessage("Must be a valid Municipality Id");
        }
    }
}
