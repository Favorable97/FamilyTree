using FluentValidation;

namespace FamilyTree.API.Validators
{
    public class UpdatePersonValidator
        : AbstractValidator<RequestUpdatePersonDTO>
    {
        public UpdatePersonValidator() 
        {
            RuleFor(x => x.FirstName)
                .Length(1, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .Length(1, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.BirthDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.BirthDate.HasValue);

            RuleFor(x => x.Gender).IsInEnum().When(x => x.Gender.HasValue);

            RuleFor(x => x.MiddleName)
                .Length(1, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

            RuleFor(x => x.DeathDate)
                .GreaterThanOrEqualTo(x => x.BirthDate)
                .When(x => x.DeathDate.HasValue && x.BirthDate.HasValue);
        }
    }
}
