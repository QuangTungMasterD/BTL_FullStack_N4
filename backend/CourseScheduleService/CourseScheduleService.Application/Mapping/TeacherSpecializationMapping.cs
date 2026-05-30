using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.TeacherSpecializationDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class TeacherSpecializationMapping : Profile
    {
        public TeacherSpecializationMapping()
        {
            CreateMap<TeacherSpecialization, TeacherSpecializationResDto>();
            CreateMap<TeacherSpecializationReqDto, TeacherSpecialization>();
        }
    }
}