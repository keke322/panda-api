using Panda.Models;
using Panda.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Panda.Services;

namespace Panda.Services;

public class PatientService : IPatientService
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IValidator<Patient> _validator;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IRepository<Patient> patientRepository, IValidator<Patient> validator, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _patientRepository.GetAllAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _patientRepository.GetByIdAsync(id);
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        ValidatePatient(patient);

        patient.Id = Guid.NewGuid();
        var created = await _patientRepository.AddAsync(patient);

        _logger.LogInformation("Created patient with ID {PatientId}", created.Id);
        return created;
    }

    public async Task<Patient?> UpdateAsync(Patient patient)
    {
        ValidatePatient(patient);

        var existing = await _patientRepository.GetByIdAsync(patient.Id);
        if (existing == null)
        {
            _logger.LogWarning("Attempted to update non-existent patient with ID {PatientId}", patient.Id);
            return null;
        }

        existing.Name = patient.Name;
        existing.DateOfBirth = patient.DateOfBirth;
        existing.NhsNumber = patient.NhsNumber;
        existing.Postcode = patient.Postcode;

        await _patientRepository.UpdateAsync(existing);

        _logger.LogInformation("Updated patient with ID {PatientId}", existing.Id);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _patientRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _patientRepository.DeleteAsync(existing);
        _logger.LogInformation("Deleted patient with ID {PatientId}", id);
        return true;
    }

    private void ValidatePatient(Patient patient)
    {
        ValidationResult result = _validator.Validate(patient);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
