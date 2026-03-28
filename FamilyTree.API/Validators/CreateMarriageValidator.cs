using FluentValidation;

namespace FamilyTree.API.Validators
{
    public class CreateMarriageValidator
        : AbstractValidator<RequestAddMarriageDTO>
    {
        public CreateMarriageValidator()
        {
            RuleFor(x => x.Spouse1Id)
                .NotEqual(x => x.Spouse2Id);
        }
    }
}
