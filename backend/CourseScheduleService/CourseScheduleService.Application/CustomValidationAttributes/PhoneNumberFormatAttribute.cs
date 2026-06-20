using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.CustomValidationAttributes
{
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        private static readonly Regex PhoneRegex = new(
            @"^(0|\+84)(3[2-9]|5[25689]|7[06789]|8[0-9]|9[0-9])\d{7}$",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(250)
        );

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            string phone = value.ToString()!;
            if (!PhoneRegex.IsMatch(phone))
                return new ValidationResult("Số điện thoại không hợp lệ");

            return ValidationResult.Success;
        }

        // Tùy chọn: ghi đè FormatErrorMessage để có message tùy chỉnh
        public override string FormatErrorMessage(string name)
        {
            return "Số điện thoại không hợp lệ";
        }
    }
}