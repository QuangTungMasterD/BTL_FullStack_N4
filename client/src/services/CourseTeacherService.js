import api from './api';

const CourseTeacherService = {
  // Lấy tất cả
  getAllCourseTeacher() {
    return api.get('v1/CourseTeacher');
  },
  // Tạo mới
  createCourseTeacher(data) {
    return api.post('v1/CourseTeacher', data);
  },
  // Xóa
  deleteCourseTeacher(id) {
    return api.delete(`v1/CourseTeacher/${id}`);
  },
};

export default CourseTeacherService;