import api from './api';

const teacherAssignmentService = {
  // Lấy tất cả phân công
  getAllTeacherAssignments() {
    return api.get('v1/TeacherAssignments');
  },
  // Tạo mới
  createTeacherAssignment(data) {
    return api.post('v1/TeacherAssignments', data);
  },
  // Xóa
  deleteTeacherAssignment(id) {
    return api.delete(`v1/TeacherAssignments/${id}`);
  },
};

export default teacherAssignmentService;