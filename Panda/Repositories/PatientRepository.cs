using Panda.Data;
using Panda.Models;
using Microsoft.EntityFrameworkCore;
using Panda.Repositories;

namespace Panda.Repositories;

public class PatientRepository : IRepository<Patient>
{
    private readonly PandaDbContext _context;

    public PatientRepository(PandaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _context.Patients.AsNoTracking().ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<Patient> AddAsync(Patient entity)
    {
        _context.Patients.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Patient entity)
    {
        _context.Patients.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Patient entity)
    {
        _context.Patients.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
