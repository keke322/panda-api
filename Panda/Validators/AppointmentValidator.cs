using FluentValidation;
using Panda.Api.Utils;
using Panda.Models;
using System.Text.RegularExpressions;

namespace Panda.Validators;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    public AppointmentValidator()
    {
        RuleFor(a => a.PatientId)
            .NotEmpty().WithMessage("PatientId is required.");

        RuleFor(a => a.ScheduledAt)
            .GreaterThan(DateTimeOffset.UtcNow.AddMinutes(-5))
            .WithMessage("Scheduled time must be in the present or future.");

        RuleFor(a => a.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be a positive number of minutes.");

        RuleFor(a => a.Clinician)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.Department)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.Postcode)
            .Must(ValidationUtils.IsValidUkPostcode)
            .WithMessage("Invalid UK postcode format.");

        RuleFor(a => a.Status)
            .NotEmpty()
            .Must(s => s == "scheduled" || s == "attended" || s == "missed" || s == "cancelled")
            .WithMessage("Status must be scheduled, attended, missed or cancelled.");
    }
}
