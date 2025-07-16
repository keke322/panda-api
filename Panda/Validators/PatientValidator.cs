using FluentValidation;
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
            .Must(BeValidNhsNumber)
            .WithMessage("Invalid NHS number checksum.");

        RuleFor(x => x.Postcode)
            .Must(BeValidUkPostcode)
            .WithMessage("Invalid UK postcode format.");
    }

    private bool BeValidNhsNumber(string nhsNumber)
    {
        if (string.IsNullOrWhiteSpace(nhsNumber))
            return false;

        // Remove spaces
        nhsNumber = nhsNumber.Replace(" ", "");

        if (!Regex.IsMatch(nhsNumber, @"^\d{10}$"))
            return false;

        // MOD11 checksum validation
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (10 - i) * (nhsNumber[i] - '0');
        }
        int remainder = sum % 11;
        int checkDigit = 11 - remainder;
        if (checkDigit == 11) checkDigit = 0;
        if (checkDigit == 10) return false;

        return checkDigit == (nhsNumber[9] - '0');
    }

    private bool BeValidUkPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return false;

        // General UK postcode regex
        var regex = new Regex(
            @"^(GIR 0AA|[A-Z]{1,2}\d{1,2} ?\d[A-Z]{2}|[A-Z]{1,2}\d[A-Z] ?\d[A-Z]{2}|[A-Z]{1,2}\d{2} ?\d[A-Z]{2})$",
            RegexOptions.IgnoreCase);

        return regex.IsMatch(postcode.Trim());
    }
}
