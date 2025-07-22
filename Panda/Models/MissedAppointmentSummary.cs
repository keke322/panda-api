namespace Panda.Models
{
    public class MissedAppointmentSummary
    {
        public string Clinician { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
        public int MissedCount { get; set; }
        public int TotalSecondsMissed { get; set; }
    }
}
