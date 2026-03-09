using FluentValidation;

namespace FamilyTree.API.Validators
{
    public class CreateMarriageValidator
        : AbstractValidator<RequestAddMarriageDTO>
    {
        public CreateMarriageValidator()
        {
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.BeginDate)
                .When(x => x.EndDate.HasValue);

            RuleFor(x => x.Spouse1Id)
                .NotEqual(x => x.Spouse2Id);
        }
    }
}
