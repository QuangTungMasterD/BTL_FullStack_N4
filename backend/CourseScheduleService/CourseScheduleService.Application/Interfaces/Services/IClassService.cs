using System.Collections.Generic;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
  public interface IClassService
  {
    Task<ApiResponse<ClassResDto?>> GetOneByIdAsync(int id);
    Task<ApiResponse<IEnumerable<ClassResDto>>> GetAllClassAsync();
    Task<ApiResponse<ClassResDto?>> CreateClassAsync(ClassReqDto classReqDto);
    Task<ApiResponse<ClassResDto?>> UpdateClassAsync(int id, ClassReqDto classReqDto);
    Task<ApiResponse<bool>> DeleteClassAsync(int id);
    Task<ApiResponse<bool>> HardDeleteClassAsync(int id);
    Task<ApiResponse<ClassResDto?>> RestoreClassAsync(int id);
    Task<ApiResponse<IEnumerable<ClassResDto>>> GetClassesByCourseAsync(int courseId);
    Task<MemoryStream> ExportClassesToExcelAsync(ClassFilterRequest req);
  }
}