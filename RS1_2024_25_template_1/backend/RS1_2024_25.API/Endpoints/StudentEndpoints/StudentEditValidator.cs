using FluentValidation;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    public class StudentEditValidator : AbstractValidator<StudentEditRequest>
    {
        public StudentEditValidator(ApplicationDbContext db)
        {
            RuleFor(x => x.Id).NotEmpty().NotEmpty().GreaterThan(0).WithMessage("Must provide Student Id");

            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone must be provided")
                .Matches("^06\\d-\\d\\d\\d-\\d\\d\\d").WithMessage("Phone must match format 06x-xxx-xxx");

            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Date of birth must be provided")
                .GreaterThan(new DateTime(1950, 1, 1)).WithMessage("Date of birth can't be lower than 1/1/1950")
                .LessThan(new DateTime(2019, 1, 1)).WithMessage("Date of birth can't be larger than 1/1/2019");

            RuleFor(x => x.MunicipalityId).NotEmpty().GreaterThan(0).WithMessage("Must provide Municipality Id")
                .Must(id =>
                {
                    return db.Municipalities.Where(m => m.ID == id).FirstOrDefault() != null;
                }).WithMessage("Municipality for provided Id not found!");
        }
    }
}
