using FluentValidation;
using LenLib.Api.Dto.V1;

namespace LenLib.Api.Validation.V1
{
    public class TgSearchRequestValidator : AbstractValidator<TgSearchRequest>
    {
        public TgSearchRequestValidator()
        {
            RuleFor(x => x.Tokens)
                .NotEmpty().WithMessage("Query should have 1 or more tokens")
                .Must(t => t == null || t.Length < 8).WithMessage("Query should have less than 8 tokens");
            RuleForEach(x => x.Tokens).MaximumLength(32);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
        }
    }
}