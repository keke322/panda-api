using FluentValidation;
using FluentValidation.Results;
using Panda.Models;
using Panda.Repositories;
using Microsoft.Extensions.Logging;

namespace Panda.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IRepository<Appointment> _appointmentRepo;
    private readonly IRepository<Patient> _patientRepo;
    private readonly IValidator<Appointment> _validator;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IRepository<Appointment> appointmentRepo,
        IRepository<Patient> patientRepo,
        IValidator<Appointment> validator,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        var all = await _appointmentRepo.GetAllAsync();

        // auto-mark missed appointments
        foreach (var appt in all)
        {
            MarkAsMissedIfNeeded(appt);
        }

        return all;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        var appt = await _appointmentRepo.GetByIdAsync(id);
        if (appt != null)
        {
            MarkAsMissedIfNeeded(appt);
        }
        return appt;
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        Validate(appointment);

        // Check patient exists
        var patient = await _patientRepo.GetByIdAsync(appointment.PatientId);
        if (patient == null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("PatientId", "Patient does not exist.")
            });
        }

        appointment.Id = Guid.NewGuid();
        appointment.Status = "scheduled";

        var created = await _appointmentRepo.AddAsync(appointment);

        _logger.LogInformation("Created appointment {AppointmentId} for patient {PatientId}", created.Id, created.PatientId);
        return created;
    }

    public async Task<Appointment?> UpdateAsync(Appointment appointment)
    {
        Validate(appointment);

        var existing = await _appointmentRepo.GetByIdAsync(appointment.Id);
        if (existing == null)
            return null;

        if (existing.Status == "cancelled")
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("Status", "Cannot update a cancelled appointment.")
            });
        }

        existing.ScheduledAt = appointment.ScheduledAt;
        existing.Department = appointment.Department;
        existing.Clinician = appointment.Clinician;
        existing.Attended = appointment.Attended;

        await _appointmentRepo.UpdateAsync(existing);

        _logger.LogInformation("Updated appointment {AppointmentId}", existing.Id);

        return existing;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var existing = await _appointmentRepo.GetByIdAsync(id);
        if (existing == null || existing.Status == "cancelled")
            return false;

        existing.Status = "cancelled";
        await _appointmentRepo.UpdateAsync(existing);

        _logger.LogInformation("Cancelled appointment {AppointmentId}", id);
        return true;
    }

    private void MarkAsMissedIfNeeded(Appointment appt)
    {
        if (appt.Status == "scheduled"
            && appt.ScheduledAt < DateTimeOffset.UtcNow
            && !appt.Attended)
        {
            appt.Status = "missed";
        }
    }

    private void Validate(Appointment appointment)
    {
        var result = _validator.Validate(appointment);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
