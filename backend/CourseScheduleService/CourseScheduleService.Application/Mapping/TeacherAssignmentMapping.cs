using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.TeacherAssignmentDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class TeacherAssignmentMapping : Profile
    {
        public TeacherAssignmentMapping()
        {
            CreateMap<TeacherAssignment, TeacherAssignmentResDto>();
            CreateMap<TeacherAssignmentReqDto, TeacherAssignment>();
        }
    }
}