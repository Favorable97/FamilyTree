using FluentValidation;

namespace FamilyTree.API.Validators
{
    public class CreateDivorceValidator
        : AbstractValidator<RequestAddDivorceDTO>
    {
        public CreateDivorceValidator()
        {
            RuleFor(x => x.MarriageId)
                .NotEmpty();

            RuleFor(x => x.DivorceDate)
                .NotEmpty();

            RuleFor(x => x.EndReason)
                .IsInEnum();
        }
    }
}
