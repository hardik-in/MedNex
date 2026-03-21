namespace DoctorPatientApp.API.Utilities
{
    public static class ReferenceIdGenerator
    {
        public static string Generate(string prefix, int year, int sequence)
        {
            return $"{prefix}-{year}-{sequence:D4}";
        }
    }
}