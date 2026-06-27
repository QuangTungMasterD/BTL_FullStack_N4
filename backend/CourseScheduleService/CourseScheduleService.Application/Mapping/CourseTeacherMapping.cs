using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.CourseTeacherDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class CourseTeacherMapping : Profile
    {
        public CourseTeacherMapping()
        {
            CreateMap<CourseTeacher, CourseTeacherResDto>();
            CreateMap<CourseTeacherReqDto, CourseTeacher>();
        }
    }
}