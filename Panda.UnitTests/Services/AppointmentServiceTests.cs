using Xunit;
using Moq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Panda.Models;
using Panda.Repositories;
using Panda.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IRepository<Appointment>> _apptRepoMock = new();
    private readonly Mock<IRepository<Patient>> _patientRepoMock = new();
    private readonly Mock<IValidator<Appointment>> _validatorMock = new();
    private readonly Mock<ILogger<AppointmentService>> _loggerMock = new();

    private AppointmentService CreateService() =>
        new(_apptRepoMock.Object, _patientRepoMock.Object, _validatorMock.Object, _loggerMock.Object);

    [Fact]
    public async Task CreateAsync_Should_Throw_WhenPatientNotFound()
    {
        var appointment = new Appointment { PatientId = Guid.NewGuid() };

        _validatorMock.Setup(v => v.Validate(appointment)).Returns(new ValidationResult());
        _patientRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(appointment));
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_WhenCancelled()
    {
        var id = Guid.NewGuid();
        var existing = new Appointment
        {
            Id = id,
            Status = "cancelled"
        };

        var update = new Appointment
        {
            Id = id,
            Status = "attended"
        };

        _validatorMock.Setup(v => v.Validate(update)).Returns(new ValidationResult());
        _apptRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(update));
    }

    [Fact]
    public async Task CancelAsync_Should_ReturnFalse_IfAlreadyCancelled()
    {
        var id = Guid.NewGuid();
        var existing = new Appointment
        {
            Id = id,
            Status = "cancelled"
        };

        _apptRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var service = CreateService();
        var result = await service.CancelAsync(id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_Should_MarkAsMissed_WhenPastAndNotAttended()
    {
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-30),
            Status = "scheduled",
            Attended = false,
            Duration = "20m"
        };

        _apptRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Appointment> { appointment });

        var service = CreateService();
        var result = await service.GetAllAsync();

        result.Should().ContainSingle();
        result.First().Status.Should().Be("missed");
    }

    [Fact]
    public void Should_MarkAsMissed_When_ScheduledAndExpiredAndNotAttended()
    {
        var service = CreateService();
        var appt = new Appointment
        {
            Status = "scheduled",
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-61),
            Duration = "60m",
            Attended = false
        };

        service.MarkAsMissedIfNeeded(appt);

        appt.Status.Should().Be("missed");
    }

    [Fact]
    public void Should_NotChangeStatus_When_AlreadyAttended()
    {
        var service = CreateService();
        var appt = new Appointment
        {
            Status = "scheduled",
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-61),
            Duration = "60m",
            Attended = true
        };

        service.MarkAsMissedIfNeeded(appt);

        appt.Status.Should().Be("scheduled");
    }

    [Fact]
    public void Should_NotChangeStatus_When_StillInProgress()
    {
        var service = CreateService();
        var appt = new Appointment
        {
            Status = "scheduled",
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Duration = "30m",
            Attended = false
        };

        service.MarkAsMissedIfNeeded(appt);

        appt.Status.Should().Be("scheduled");
    }

    [Fact]
    public void Should_NotChangeStatus_When_StatusIsNotScheduled()
    {
        var service = CreateService();
        var appt = new Appointment
        {
            Status = "cancelled",
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-61),
            Duration = "60m",
            Attended = false
        };

        service.MarkAsMissedIfNeeded(appt);

        appt.Status.Should().Be("cancelled");
    }
}
