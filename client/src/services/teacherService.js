import api from './api';

const teacherService = {
  // Lấy danh sách có phân trang
  getTeachersPaged(params = {}) {
    return api.get('/Teachers/paged', { params });
  },
  // Lấy tất cả
  getAllTeachers() {
    return api.get('/Teachers');
  },
  // Lấy chi tiết
  getTeacherById(id) {
    return api.get(`/Teachers/${id}`);
  },
  // Tạo mới
  createTeacher(data) {
    return api.post('/Teachers', data);
  },
  // Cập nhật
  updateTeacher(id, data) {
    return api.put(`/Teachers/${id}`, data);
  },
  // Xóa
  deleteTeacher(id) {
    return api.delete(`/Teachers/${id}`);
  },
  deleteTeacherPermanent(id) {
    return api.delete(`/Teachers/${id}/permanent`);
  },

  // Khôi phục
  restoreTeacher(id) {
    return api.patch(`/Teachers/${id}/restore`);
  },
};

export default teacherService;