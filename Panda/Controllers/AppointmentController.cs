using Microsoft.AspNetCore.Mvc;
using Panda.Services;
using Panda.DTOs;
using AutoMapper;
using Panda.Models;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace Panda.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(IAppointmentService appointmentService, IMapper mapper, ILogger<AppointmentController> logger)
    {
        _appointmentService = appointmentService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/appointment
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AppointmentDto>))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    // GET: api/appointment/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDto))]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null)
            return NotFound(new { message = "Appointment not found." });

        return Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    // POST: api/appointment
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var appointment = _mapper.Map<Appointment>(dto);
            var created = await _appointmentService.CreateAsync(appointment);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<AppointmentDto>(created));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new ValidationErrorItem
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }
    }

    // PUT: api/appointment/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
    public async Task<ActionResult<AppointmentDto>> Update(Guid id, [FromBody] UpdateAppointmentDto dto)
    {
        try
        {
            var appointment = _mapper.Map<Appointment>(dto);
            appointment.Id = id;

            var updated = await _appointmentService.UpdateAsync(appointment);
            if (updated == null)
                return NotFound(new { message = "Appointment not found." });

            return Ok(_mapper.Map<AppointmentDto>(updated));
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new ValidationErrorItem
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }
    }

    // POST: api/appointment/{id}/cancel
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var cancelled = await _appointmentService.CancelAsync(id);
        if (!cancelled)
            return NotFound(new { message = "Appointment not found or already cancelled." });

        return NoContent();
    }

    // GET: api/analytics/missed
    [HttpGet("analytics/missed")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MissedAppointmentSummary>))]
    public IActionResult GetMissedImpact()
    {
        var result = _appointmentService.GetMissedAppointmentImpactAsync();
        return Ok(result);
    }
}
