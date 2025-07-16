using Panda.Models;

namespace Panda.Services;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient?> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(Guid id);
}
