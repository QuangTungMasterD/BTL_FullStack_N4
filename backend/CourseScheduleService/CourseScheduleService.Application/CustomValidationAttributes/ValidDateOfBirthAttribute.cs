using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.CustomValidationAttributes
{
    public class ValidDateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            DateOnly dateOfBirth;
            if (value is DateOnly dob)
                dateOfBirth = dob;
            else if (value is DateTime dt)
                dateOfBirth = DateOnly.FromDateTime(dt);
            else
                return new ValidationResult("Ngày sinh không hợp lệ");

            var today = DateOnly.FromDateTime(DateTime.Now);

            int age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age)) age--;

            if (age <= 3)
                return new ValidationResult(ErrorMessage ?? "Ngày sinh không hợp lệ");

            return ValidationResult.Success;
        }
    }
}