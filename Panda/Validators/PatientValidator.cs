using FluentValidation;
using Panda.Api.Utils;
using Panda.Models;
using System.Text.RegularExpressions;

namespace Panda.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTimeOffset.UtcNow).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.NhsNumber)
            .Must(ValidationUtils.BeValidNhsNumber)
            .WithMessage("Invalid NHS number checksum.");

        RuleFor(x => x.Postcode)
            .Must(ValidationUtils.IsValidUkPostcode)
            .WithMessage("Invalid UK postcode format.");
    }
}
