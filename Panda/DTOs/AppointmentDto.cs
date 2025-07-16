namespace Panda.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public string Status { get; set; } = "scheduled";

        public DateTimeOffset ScheduledAt { get; set; }

        public int DurationMinutes { get; set; }

        public bool Attended { get; set; }

        public string Clinician { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;
    }

    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }

        public DateTimeOffset ScheduledAt { get; set; }

        public int DurationMinutes { get; set; }

        public bool Attended { get; set; }

        public string Clinician { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;
    }
    public class UpdateAppointmentDto
    {
        public Guid PatientId { get; set; }

        public DateTimeOffset ScheduledAt { get; set; }

        public int DurationMinutes { get; set; }

        public bool Attended { get; set; }

        public string Clinician { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;
    }
}
