using Xunit;
using Moq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Panda.Models;
using Panda.Repositories;
using Panda.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Panda.Models;
using Panda.Repositories;
using Panda.Services;

public class PatientServiceTests
{
    private readonly Mock<IRepository<Patient>> _repoMock = new();
    private readonly Mock<IValidator<Patient>> _validatorMock = new();
    private readonly Mock<ILogger<PatientService>> _loggerMock = new();

    private PatientService CreateService() =>
        new(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);

    [Fact]
    public async Task CreateAsync_Should_AddPatient_WhenValid()
    {
        var patient = new Patient
        {
            Name = "Jane Doe",
            NhsNumber = "1234567890",
            DateOfBirth = DateTimeOffset.Now.AddYears(-30),
            Postcode = "AB1 2CD"
        };

        _validatorMock.Setup(v => v.Validate(patient))
            .Returns(new ValidationResult());

        _repoMock.Setup(r => r.AddAsync(patient))
            .ReturnsAsync(patient);

        var service = CreateService();
        var result = await service.CreateAsync(patient);

        result.Should().Be(patient);
        _repoMock.Verify(r => r.AddAsync(patient), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_WhenInvalid()
    {
        var patient = new Patient();
        var failures = new List<ValidationFailure> {
            new("NhsNumber", "Invalid NHS number")
        };

        _validatorMock.Setup(v => v.Validate(patient))
            .Returns(new ValidationResult(failures));

        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(patient));
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnFalse_WhenPatientNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Patient?)null);

        var service = CreateService();
        var result = await service.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}
