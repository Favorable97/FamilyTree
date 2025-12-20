using FluentValidation;

namespace FamilyTree.API.Validators
{
    public class CreatePersonValidator
        : AbstractValidator<RequestAddPersonDTO>
    {
        public CreatePersonValidator() 
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(1, 100);
            
            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(1, 100);
            
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow);

            RuleFor(x => x.Gender).IsInEnum();

            RuleFor(x => x.MiddleName)
                .Length(1, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

            RuleFor(x => x.DeathDate)
                .GreaterThanOrEqualTo(x => x.BirthDate)
                .When(x => x.DeathDate.HasValue);

        }
    }
}
