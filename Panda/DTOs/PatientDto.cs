namespace Panda.DTOs
{
    public class PatientDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTimeOffset DateOfBirth { get; set; }

        public string NhsNumber { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;
    }
    public class CreatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset DateOfBirth { get; set; }
        public string NhsNumber { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
    }
    public class UpdatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset DateOfBirth { get; set; }
        public string NhsNumber { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
    }
}
