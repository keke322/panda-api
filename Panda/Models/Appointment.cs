namespace Panda.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; } // optional navigation property

        public string Status { get; set; } = "scheduled"; // scheduled, attended, missed, cancelled

        public DateTimeOffset ScheduledAt { get; set; }

        public string Duration { get; set; } = string.Empty;

        public bool Attended { get; set; }

        public string Clinician { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;
    }
}
