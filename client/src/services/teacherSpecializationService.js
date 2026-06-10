import api from './api';

const teacherSpecializationService = {
  // Lấy tất cả
  getAllTeacherSpecializations() {
    return api.get('v1/TeacherSpecializations');
  },
  // Tạo mới
  createTeacherSpecialization(data) {
    return api.post('v1/TeacherSpecializations', data);
  },
  // Xóa
  deleteTeacherSpecialization(id) {
    return api.delete(`v1/TeacherSpecializations/${id}`);
  },
};

export default teacherSpecializationService;