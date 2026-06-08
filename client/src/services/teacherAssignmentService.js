import api from './api';

const teacherAssignmentService = {
  // Lấy tất cả phân công
  getAllTeacherAssignments() {
    return api.get('/TeacherAssignments');
  },
  // Tạo mới
  createTeacherAssignment(data) {
    return api.post('/TeacherAssignments', data);
  },
  // Xóa
  deleteTeacherAssignment(id) {
    return api.delete(`/TeacherAssignments/${id}`);
  },
};

export default teacherAssignmentService;