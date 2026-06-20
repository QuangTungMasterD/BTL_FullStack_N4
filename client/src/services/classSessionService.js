import api from './api';

const classSessionService = {
  // Lấy danh sách có phân trang
  getClassSessionsPaged(params = {}) {
    return api.get('v1/ClassSessions/paged', { params });
  },
  // Lấy tất cả
  getAllClassSessions() {
    return api.get('v1/ClassSessions');
  },
  // Lấy chi tiết
  getClassSessionById(id) {
    return api.get(`v1/ClassSessions/${id}`);
  },
  // Tạo mới
  createClassSession(data) {
    return api.post('v1/ClassSessions', data);
  },
  // Cập nhật
  updateClassSession(id, data) {
    return api.put(`v1/ClassSessions/${id}`, data);
  },
  // Xóa
  deleteClassSession(id) {
    return api.delete(`v1/ClassSessions/${id}`);
  },
  // Cập nhật trạng thái buổi học
  updateClassSessionStatus(id, statusData) {
    return api.patch(`v1/ClassSessions/${id}/status`, statusData);
  },
};

export default classSessionService;