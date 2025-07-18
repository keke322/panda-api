using FluentValidation;
using Microsoft.Extensions.Localization;
using Panda.Api.Utils;
using Panda.Models;
using System.Text.RegularExpressions;

namespace Panda.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator(IStringLocalizer<PatientValidator> localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTimeOffset.UtcNow)
            .WithMessage(localizer["DateOfBirth_Invalid"].Value);

        RuleFor(x => x.NhsNumber)
            .Must(ValidationUtils.BeValidNhsNumber)
            .WithMessage(localizer["NhsNumber_Invalid"].Value);

        RuleFor(x => x.Postcode)
            .Must(ValidationUtils.IsValidUkPostcode)
            .WithMessage(localizer["Postcode_Invalid"].Value);
    }
}
