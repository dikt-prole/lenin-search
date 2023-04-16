using FluentValidation;
using LenLib.Api.Dto.V1;

namespace LenLib.Api.Validation.V1
{
    public class TgParagraphRequestValidator : AbstractValidator<TgParagraphRequest>
    {
        public TgParagraphRequestValidator()
        {
            RuleFor(x => x.CorpusId).NotEmpty();
            RuleFor(x => x.Path).NotEmpty();
        }
    }
}