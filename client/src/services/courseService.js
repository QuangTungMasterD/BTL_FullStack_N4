import api from './api';

const courseService = {
  // Lấy danh sách có phân trang + filter
  getCoursesPaged(params = {}) {
    return api.get('/courses/paged', { params });
  },
  
  // Lấy tất cả (không phân trang)
  getAllCourses() {
    return api.get('/Courses');
  },
  
  // Lấy chi tiết theo id
  getCourseById(id) {
    return api.get(`/Courses/${id}`);
  },
  
  // Tạo mới
  createCourse(data) {
    return api.post('/Courses', data);
  },
  
  // Cập nhật
  updateCourse(id, data) {
    return api.put(`/Courses/${id}`, data);
  },
  
  // Xóa mềm
  deleteCourse(id) {
    return api.delete(`/Courses/${id}`);
  },
  
  // Xóa vĩnh viễn
  deleteCoursePermanent(id) {
    return api.delete(`/Courses/${id}/permanent`);
  },
  
  // Khôi phục
  restoreCourse(id) {
    return api.patch(`/Courses/${id}/restore`);
  },
};

export default courseService;