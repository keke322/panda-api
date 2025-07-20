using FluentValidation;
using Microsoft.Extensions.Localization;
using Panda.Api.Utils;
using Panda.Models;
using System.Text.RegularExpressions;

namespace Panda.Validators;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    public AppointmentValidator(IStringLocalizer<AppointmentValidator> localizer)
    {
        RuleFor(a => a.ScheduledAt)
            .GreaterThan(DateTimeOffset.UtcNow.AddMinutes(-5))
            .WithMessage(localizer["Schedule_NowOrFuture"]);

        RuleFor(a => a.Duration)
            .Must(duration => Utils.ParseDurationToSeconds(duration) > 0)
            .WithMessage(localizer["Duration_Positive"]);

        RuleFor(a => a.Clinician)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.Department)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.Postcode)
            .Must(Utils.IsValidUkPostcode)
            .WithMessage(localizer["Postcode_Invalid"]);

        RuleFor(a => a.Status)
            .NotEmpty()
            .Must(s => s == "scheduled" || s == "attended" || s == "missed" || s == "cancelled")
            .WithMessage(localizer["Incorrect_Status"]);
    }
}
