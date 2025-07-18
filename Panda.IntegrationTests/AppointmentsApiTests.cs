using Panda.DTOs;
using System.Net.Http.Json;
using FluentAssertions;

namespace Panda.IntegrationTests
{
    public class AppointmentsApiTests : IClassFixture<PandaApiFactory>
    {
        private readonly HttpClient _client;

        public AppointmentsApiTests(PandaApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_Appointment_Should_Fail_Without_Valid_Patient()
        {
            var appointment = new CreateAppointmentDto
            {
                PatientId = Guid.NewGuid(), // Non-existent
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
                DurationMinutes = 20,
                Attended = false,
                Clinician = "Dr X",
                Department = "Radiology",
                Postcode = "SW1A 1AA"
            };

            var response = await _client.PostAsJsonAsync("/api/appointments", appointment);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}