using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Panda.DTOs;
using Panda.Models;
using Panda.Services;
using FluentValidation;

namespace Panda.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patientService, IMapper mapper, ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<PatientDto>>(patients));
    }

    // GET: api/patients/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null)
            return NotFound(new { message = "Patient not found." });

        return Ok(_mapper.Map<PatientDto>(patient));
    }

    // POST: api/patients
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
    {
        try
        {
            var patient = _mapper.Map<Patient>(dto);
            var created = await _patientService.CreateAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<PatientDto>(created));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        }
    }

    // PUT: api/patients/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        try
        {
            var patient = _mapper.Map<Patient>(dto);
            patient.Id = id;
            var updated = await _patientService.UpdateAsync(patient);
            if (updated == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(_mapper.Map<PatientDto>(updated));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
        }
    }

    // DELETE: api/patients/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _patientService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = "Patient not found." });

        return NoContent();
    }
}
