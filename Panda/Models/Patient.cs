namespace Panda.Models
{
    public class Patient
    {
        public Guid Id { get; set; }

        // NHS Number with checksum validated by PatientValidator
        public string NhsNumber { get; set; } = string.Empty;

        // Full name, respecting GDPR (stored exactly as given)
        public string Name { get; set; } = string.Empty;

        // DOB stored as timezone-aware
        public DateTimeOffset DateOfBirth { get; set; }

        // UK Postcode formatted
        public string Postcode { get; set; } = string.Empty;
    }
}
