import api from './api';

const classSessionService = {
  // Lấy danh sách có phân trang
  getClassSessionsPaged(params = {}) {
    return api.get('/ClassSessions/paged', { params });
  },
  // Lấy tất cả
  getAllClassSessions() {
    return api.get('/ClassSessions');
  },
  // Lấy chi tiết
  getClassSessionById(id) {
    return api.get(`/ClassSessions/${id}`);
  },
  // Tạo mới
  createClassSession(data) {
    return api.post('/ClassSessions', data);
  },
  // Cập nhật
  updateClassSession(id, data) {
    return api.put(`/ClassSessions/${id}`, data);
  },
  // Xóa
  deleteClassSession(id) {
    return api.delete(`/ClassSessions/${id}`);
  },
  // Cập nhật trạng thái buổi học
  updateClassSessionStatus(id, statusData) {
    return api.patch(`/ClassSessions/${id}/status`, statusData);
  },
};

export default classSessionService;