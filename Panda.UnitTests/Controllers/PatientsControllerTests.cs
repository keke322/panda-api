using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Panda.Api.Controllers;
using Panda.Services;
using Panda.DTOs;
using Panda.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Panda.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IPatientService> _serviceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<PatientsController>> _loggerMock = new();

    private PatientsController CreateController() =>
        new(_serviceMock.Object, _mapperMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetAll_ReturnsOkWithPatients()
    {
        var patients = new List<Patient> { new() { Id = Guid.NewGuid(), Name = "Test Patient" } };
        var dtos = new List<PatientDto> { new() { Id = patients[0].Id, Name = "Test Patient" } };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(patients);
        _mapperMock.Setup(m => m.Map<IEnumerable<PatientDto>>(patients)).Returns(dtos);

        var controller = CreateController();
        var result = await controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetById_NotFoundIfMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

        var controller = CreateController();
        var result = await controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAt_WhenValid()
    {
        var inputDto = new CreatePatientDto
        {
            Name = "New Patient",
            DateOfBirth = DateTimeOffset.Now.AddYears(-30),
            NhsNumber = "1234567890",
            Postcode = "AB12 3CD"
        };

        var patientEntity = new Patient { Id = Guid.NewGuid(), Name = inputDto.Name };
        var resultDto = new PatientDto { Id = patientEntity.Id, Name = inputDto.Name };

        _mapperMock.Setup(m => m.Map<Patient>(inputDto)).Returns(patientEntity);
        _serviceMock.Setup(s => s.CreateAsync(patientEntity)).ReturnsAsync(patientEntity);
        _mapperMock.Setup(m => m.Map<PatientDto>(patientEntity)).Returns(resultDto);

        var controller = CreateController();
        var result = await controller.Create(inputDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }
}
