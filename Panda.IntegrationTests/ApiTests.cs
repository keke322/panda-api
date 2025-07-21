using Panda.DTOs;
using System.Net.Http.Json;
using FluentAssertions;
using System.Net;
using Panda.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper.Configuration.Annotations;

namespace Panda.IntegrationTests
{
    public class ApiTests : IClassFixture<PandaApiFactory>
    {
        private readonly HttpClient _client;

        public ApiTests(PandaApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkAndList()
        {
            var response = await _client.GetAsync("/api/patients");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedPatient()
        {
            var dto = new CreatePatientDto
            {
                Name = "Dr Glenn Clark",
                DateOfBirth = new DateTimeOffset(1996, 1, 1, 0, 0, 0, TimeSpan.Zero),
                NhsNumber = "1373645350",
                Postcode = "N6 2FA"
            };

            var response = await _client.PostAsJsonAsync("/api/patients", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<PatientDto>();
            created.Should().NotBeNull();
            created!.Name.Should().Be(dto.Name);
            created.Postcode.Should().Be(dto.Postcode);
        }

        [Fact]
        public async Task GetById_ShouldReturnPatient()
        {
            var create = await _client.PostAsJsonAsync("/api/patients", new CreatePatientDto
            {
                Name = "Alice Smith",
                DateOfBirth = DateTimeOffset.Now.AddYears(-30),
                NhsNumber = "9876543210",
                Postcode = "XY99ZZ"
            });

            var created = await create.Content.ReadFromJsonAsync<PatientDto>();

            var response = await _client.GetAsync($"/api/patients/{created!.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var patient = await response.Content.ReadFromJsonAsync<PatientDto>();
            patient!.Name.Should().Be("Alice Smith");
        }

        [Fact]
        public async Task Update_ShouldModifyPatient()
        {
            var created = await _client.PostAsJsonAsync("/api/patients", new CreatePatientDto
            {
                Name = "Original Name",
                DateOfBirth = DateTimeOffset.Now.AddYears(-40),
                NhsNumber = "0000000000",
                Postcode = "ZZ99YY"
            });

            var patient = await created.Content.ReadFromJsonAsync<PatientDto>();

            var updateDto = new UpdatePatientDto
            {
                Name = "Updated Name",
                DateOfBirth = patient!.DateOfBirth,
                NhsNumber = patient.NhsNumber,
                Postcode = patient.Postcode
            };

            var response = await _client.PutAsJsonAsync($"/api/patients/{patient.Id}", updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<PatientDto>();
            updated!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task Delete_ShouldRemovePatient()
        {
            var create = await _client.PostAsJsonAsync("/api/patients", new CreatePatientDto
            {
                Name = "Delete Me",
                DateOfBirth = DateTimeOffset.Now.AddYears(-20),
                NhsNumber = "1373645350",
                Postcode = "N6 2FA"
            });

            var created = await create.Content.ReadFromJsonAsync<PatientDto>();

            var delete = await _client.DeleteAsync($"/api/patients/{created!.Id}");
            delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var check = await _client.GetAsync($"/api/patients/{created.Id}");
            check.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task POST_Appointment_Should_Fail_Without_Valid_Patient()
        {
            var appointment = new CreateAppointmentDto
            {
                PatientId = Guid.NewGuid(), // Non-existent
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
                Duration = "20m",
                Attended = false,
                Clinician = "Dr X",
                Department = "Radiology",
                Postcode = "SW1A 1AA"
            };

            var response = await _client.PostAsJsonAsync("/api/appointment", appointment);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        
        private async Task<CreateAppointmentDto> GetSampleDtoAsync()
        {
            var patDto = new CreatePatientDto
            {
                Name = "Dr Glenn Clark",
                DateOfBirth = new DateTimeOffset(1996, 1, 1, 0, 0, 0, TimeSpan.Zero),
                NhsNumber = "1373645350",
                Postcode = "N6 2FA"
            };

            var patResponse = await _client.PostAsJsonAsync("/api/patients", patDto);
            patResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await patResponse.Content.ReadFromJsonAsync<PatientDto>();

            return new CreateAppointmentDto
            {
                PatientId = created.Id,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
                Duration = "30m",
                Clinician = "Dr. Smith",
                Department = "Cardiology",
                Postcode = "AB12 3CD"
            };
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAppointment_WithExpectedFields()
        {
            var dto = await GetSampleDtoAsync();

            var response = await _client.PostAsJsonAsync("/api/appointment", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var appointment = await response.Content.ReadFromJsonAsync<AppointmentDto>();
            appointment!.Clinician.Should().Be(dto.Clinician);
            appointment.Status.Should().Be("scheduled");
            appointment.Duration.Should().Be(dto.Duration);
            appointment.Attended.Should().BeFalse();
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectAppointment()
        {
            var dto = await GetSampleDtoAsync();
            var create = await _client.PostAsJsonAsync("/api/appointment", dto);
            var created = await create.Content.ReadFromJsonAsync<AppointmentDto>();

            var response = await _client.GetAsync($"/api/appointment/{created!.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AppointmentDto>();
            result!.Id.Should().Be(created.Id);
            result.Clinician.Should().Be(dto.Clinician);
        }

        [Fact]
        public async Task Update_ShouldChangeAppointmentDetails()
        {
            var dto = await GetSampleDtoAsync();
            var create = await _client.PostAsJsonAsync("/api/appointment", dto);
            var created = await create.Content.ReadFromJsonAsync<AppointmentDto>();

            var update = new UpdateAppointmentDto
            {
                PatientId = created!.PatientId,
                ScheduledAt = created.ScheduledAt,
                Duration = "45m",
                Clinician = "Dr. Watson",
                Department = "Neurology",
                Postcode = "N6 2FA",
                Attended = true
            };

            var response = await _client.PutAsJsonAsync($"/api/appointment/{created.Id}", update);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await response.Content.ReadFromJsonAsync<AppointmentDto>();
            updated.Clinician.Should().Be("Dr. Watson");
            updated.Duration.Should().Be("45m");
            updated.Postcode.Should().Be("N6 2FA");
            updated.Attended.Should().BeTrue();
        }

        [Fact]
        public async Task Cancel_ShouldReturnNoContent_AndStatusShouldChange()
        {
            var dto = await GetSampleDtoAsync();
            var create = await _client.PostAsJsonAsync("/api/appointment", dto);
            var created = await create.Content.ReadFromJsonAsync<AppointmentDto>();

            var cancel = await _client.PostAsync($"/api/appointment/{created!.Id}/cancel", null);
            cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Fetch again to verify status change if your logic updates `Status`
            var get = await _client.GetAsync($"/api/appointment/{created.Id}");
            var afterCancel = await get.Content.ReadFromJsonAsync<AppointmentDto>();

            afterCancel!.Status.Should().Be("cancelled");
        }

        [Fact]
        public async Task CancelledAppointment_CannotBeUpdatedOrReinstated()
        {
            var dto = await GetSampleDtoAsync();
            var create = await _client.PostAsJsonAsync("/api/appointment", dto);
            var created = await create.Content.ReadFromJsonAsync<AppointmentDto>();

            var cancel = await _client.PostAsync($"/api/appointment/{created!.Id}/cancel", null);
            cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Fetch again to verify status change if your logic updates `Status`
            var get = await _client.GetAsync($"/api/appointment/{created.Id}");
            var afterCancel = await get.Content.ReadFromJsonAsync<AppointmentDto>();

            afterCancel!.Status.Should().Be("cancelled");

            var updateDto = new UpdateAppointmentDto
            {
                PatientId = created.PatientId,
                ScheduledAt = created.ScheduledAt.AddDays(1), // reschedule
                Duration = "45m",
                Clinician = "Dr. Reinstated",
                Department = "Neurology",
                Postcode = "N6 2FA",
                Attended = true
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/appointment/{created.Id}", updateDto);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            // Step 5: Confirm it is still cancelled
            var check = await _client.GetAsync($"/api/appointment/{created.Id}");
            var afterUpdate = await check.Content.ReadFromJsonAsync<AppointmentDto>();
            afterUpdate!.Status.Should().Be("cancelled");
            afterUpdate.Attended.Should().BeFalse(); // should not have changed
        }

        [Fact]
        public async Task Appointment_ShouldBeMarkedAsMissed_IfPastAndNotAttended()
        {
            var pastAppointment = await GetSampleDtoAsync();
            pastAppointment.ScheduledAt = DateTimeOffset.Now.AddMinutes(-3);
            pastAppointment.Duration = "1m";
            var createResponse = await _client.PostAsJsonAsync("/api/appointment", pastAppointment);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<AppointmentDto>();
            created.Should().NotBeNull();
            created!.Attended.Should().BeFalse();
            created.Status.Should().Be("scheduled");

            var checkResponse = await _client.GetAsync($"/api/appointment/{created.Id}");
            checkResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await checkResponse.Content.ReadFromJsonAsync<AppointmentDto>();
            updated!.Status.Should().Be("missed");
        }

        [Fact]
        public async Task GetMissedImpact_ShouldReturnImpact()
        {
            var pastAppointment = await GetSampleDtoAsync();
            pastAppointment.ScheduledAt = DateTimeOffset.Now.AddMinutes(-3);
            pastAppointment.Duration = "1m";
            var createResponse = await _client.PostAsJsonAsync("/api/appointment", pastAppointment);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<AppointmentDto>();
            created.Should().NotBeNull();
            created!.Attended.Should().BeFalse();
            created.Status.Should().Be("scheduled");

            var checkResponse = await _client.GetAsync($"/api/appointment/{created.Id}");
            checkResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await checkResponse.Content.ReadFromJsonAsync<AppointmentDto>();
            updated!.Status.Should().Be("missed");

            var impact = await _client.GetAsync($"/api/appointment/analytics/missed");
            impact.StatusCode.Should().Be(HttpStatusCode.OK);
            var impactBody = await impact.Content.ReadFromJsonAsync<List<MissedAppointmentSummary>>();
            impactBody.Should().NotBeNull();
        }

        [Theory]
        [InlineData("José")]
        [InlineData("Mårten")]
        [InlineData("Łukasz")]
        [InlineData("Émilie")]
        [InlineData("Zoë")]
        [InlineData("François")]
        public async Task DiacriticNames_ShouldBeCreatedAndGet(string name)
        {
            var dto = new CreatePatientDto
            {
                Name = name,
                DateOfBirth = DateTimeOffset.UtcNow.AddYears(-30),
                NhsNumber = "1373645350",
                Postcode = "AA1 1AA"
            };

            var response = await _client.PostAsJsonAsync("/api/patients", dto);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<PatientDto>();
            created!.Name.Should().Be(name);

            var getResponse = await _client.GetAsync($"/api/patients/{created!.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var patient = await getResponse.Content.ReadFromJsonAsync<PatientDto>();
            patient!.Name.Should().Be(name);
        }
    }
}