using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Panda.Controllers;
using Panda.Services;
using Panda.DTOs;
using Panda.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Panda.Api.Controllers;

public class AppointmentsControllerTests
{
    private readonly Mock<IAppointmentService> _serviceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<AppointmentController>> _loggerMock = new();

    private AppointmentController CreateController() =>
        new(_serviceMock.Object, _mapperMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetAll_ReturnsOkWithAppointments()
    {
        var appointments = new List<Appointment>
        {
            new() { Id = Guid.NewGuid(), Clinician = "Dr Smith" }
        };

        var dtos = new List<AppointmentDto>
        {
            new() { Id = appointments[0].Id, Clinician = "Dr Smith" }
        };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(appointments);
        _mapperMock.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments)).Returns(dtos);

        var controller = CreateController();
        var result = await controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task Cancel_ReturnsNoContent_WhenSuccess()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.CancelAsync(id)).ReturnsAsync(true);

        var controller = CreateController();
        var result = await controller.Cancel(id);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Cancel_ReturnsNotFound_WhenAlreadyCancelled()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.CancelAsync(id)).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.Cancel(id);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
