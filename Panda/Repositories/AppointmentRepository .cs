using Microsoft.EntityFrameworkCore;
using Panda.Data;
using Panda.Models;
using Panda.Repositories;

namespace Panda.Repositories;

public class AppointmentRepository : IRepository<Appointment>
{
    public PandaDbContext _context;

    public AppointmentRepository(PandaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Appointment appointment)
    {
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }
    public IQueryable<Appointment> Query()
    {
        return _context.Appointments.AsQueryable();
    }
}
