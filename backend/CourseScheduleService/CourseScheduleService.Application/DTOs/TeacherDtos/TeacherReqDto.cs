using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CourseScheduleService.Application.CustomValidationAttributes;

namespace CourseScheduleService.Application.DTOs.TeacherDtos
{
    public class TeacherReqDto
    {
        public int? UserId { get; set; }
        
        [Required(ErrorMessage = "Tên giáo viên không được để trống")]
        public required String FullName { get; set; }

        [Required(ErrorMessage = "Địa chỉ email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required String Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [PhoneNumberValidation(ErrorMessage = "Số điện thoại không hợp lệ")]
        public required String Phone { get; set; }

        [ValidDateOfBirth(ErrorMessage = "Ngày sinh không hợp lệ")]
        public DateOnly YoB { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public bool Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> SpecializationIds { get; set; } = new List<int>();
    }
}