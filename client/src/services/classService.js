import api from './api';

const classService = {
  // Lấy danh sách có phân trang
  getClassesPaged(params = {}) {
    return api.get('/Classes/paged', { params });
  },
  // Lấy tất cả
  getAllClasses() {
    return api.get('/Classes');
  },
  // Lấy chi tiết
  getClassById(id) {
    return api.get(`/Classes/${id}`);
  },
  // Lấy lớp theo khóa học
  getClassesByCourseId(courseId) {
    return api.get(`/Classes/course/${courseId}`);
  },
  // Tạo mới
  createClass(data) {
    return api.post('/Classes', data);
  },
  // Cập nhật
  updateClass(id, data) {
    return api.put(`/Classes/${id}`, data);
  },
  // Xóa mềm
  deleteClass(id) {
    return api.delete(`/Classes/${id}`);
  },
  // Xóa vĩnh viễn
  deleteClassPermanent(id) {
    return api.delete(`/Classes/${id}/permanent`);
  },
  // Khôi phục
  restoreClass(id) {
    return api.patch(`/Classes/${id}/restore`);
  },
};

export default classService;