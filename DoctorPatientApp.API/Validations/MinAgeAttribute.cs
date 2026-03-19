using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.Validation
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
            ErrorMessage = $"Must be at least {_minAge} years old.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not DateTime dob)
                return ValidationResult.Success;

            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;

            return age < _minAge
                ? new ValidationResult(ErrorMessage)
                : ValidationResult.Success;
        }
    }
}