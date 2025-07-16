using Panda.Models;

namespace Panda.Services;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> UpdateAsync(Appointment appointment);
    Task<bool> CancelAsync(Guid id);
}
