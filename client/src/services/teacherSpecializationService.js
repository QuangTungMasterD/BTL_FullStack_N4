import api from './api';

const teacherSpecializationService = {
  // Lấy tất cả
  getAllTeacherSpecializations() {
    return api.get('/TeacherSpecializations');
  },
  // Tạo mới
  createTeacherSpecialization(data) {
    return api.post('/TeacherSpecializations', data);
  },
  // Xóa
  deleteTeacherSpecialization(id) {
    return api.delete(`/TeacherSpecializations/${id}`);
  },
};

export default teacherSpecializationService;