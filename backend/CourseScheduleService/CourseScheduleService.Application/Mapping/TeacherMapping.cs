using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.TeacherDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class TeacherMapping : Profile
    {
        public TeacherMapping()
        {
            CreateMap<Teacher, TeacherResDto>();
            CreateMap<TeacherReqDto, Teacher>();
        }
    }
}