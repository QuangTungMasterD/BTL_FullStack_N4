import api from './api';

const specializationService = {
  // Lấy danh sách có phân trang
  getSpecializationsPaged(params = {}) {
    return api.get('v1/Specializations/paged', { params });
  },
  // Lấy tất cả
  getAllSpecializations() {
    return api.get('v1/Specializations');
  },
  // Lấy chi tiết
  getSpecializationById(id) {
    return api.get(`v1/Specializations/${id}`);
  },
  // Tạo mới
  createSpecialization(data) {
    return api.post('v1/Specializations', data);
  },
  // Cập nhật
  updateSpecialization(id, data) {
    return api.put(`v1/Specializations/${id}`, data);
  },
  // Xóa vĩnh viễn (không có xóa mềm theo Swagger)
  deleteSpecializationPermanent(id) {
    return api.delete(`v1/Specializations/${id}/permanent`);
  },
};

export default specializationService;