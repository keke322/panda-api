﻿using Microsoft.AspNetCore.Mvc;
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

    // GET: api/appointments
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AppointmentDto>))]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(appointments));
    }

    // GET: api/appointments/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDto))]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null)
            return NotFound(new { message = "Appointment not found." });

        return Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    // POST: api/appointments
    [HttpPost]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, description: "Bad request")]
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
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
    }

    // PUT: api/appointments/{id}
    [HttpPut("{id}")]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, description: "Bad request")]
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
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
    }

    // POST: api/appointments/{id}/cancel
    [HttpPost("{id}/cancel")]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, description: "Bad request")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var cancelled = await _appointmentService.CancelAsync(id);
        if (!cancelled)
            return NotFound(new { message = "Appointment not found or already cancelled." });

        return NoContent();
    }
}
