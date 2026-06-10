import api from './api';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver'; 
import { courseLevelText } from '@/composables/useFormat.js'

const courseService = {
  // Lấy danh sách có phân trang + filter
  getCoursesPaged(params = {}) {
    return api.get('v1/courses/paged', { params });
  },
  
  // Lấy tất cả (không phân trang)
  getAllCourses() {
    return api.get('v1/Courses');
  },
  
  // Lấy chi tiết theo id
  getCourseById(id) {
    return api.get(`v1/Courses/${id}`);
  },
  
  // Tạo mới
  createCourse(data) {
    return api.post('v1/Courses', data);
  },
  
  // Cập nhật
  updateCourse(id, data) {
    return api.put(`v1/Courses/${id}`, data);
  },
  
  // Xóa mềm
  deleteCourse(id) {
    return api.delete(`v1/Courses/${id}`);
  },
  
  // Xóa vĩnh viễn
  deleteCoursePermanent(id) {
    return api.delete(`v1/Courses/${id}/permanent`);
  },
  
  // Khôi phục
  restoreCourse(id) {
    return api.patch(`v1/Courses/${id}/restore`);
  },

  // Export Excel
  async exportToExcel(params = {}) {
    const response = await this.getCoursesPaged({ 
      ...params, 
      page: 1,
      pageSize: 10000
    });
    
    const courses = response.data || [];
    
    if (courses.length === 0) {
      throw new Error('Không có dữ liệu để xuất');
    }
    
    // Dùng courseLevelText từ useFormat.js
    const excelData = courses.map(course => ({
      // 'ID': course.id,
      'Tên khóa học': course.courseName,
      'Mô tả': course.desct || '',
      'Học phí (VNĐ)': course.tuitionFee,
      'Số buổi': course.lesson,
      'Trình độ': courseLevelText(course.level),  // ← dùng hàm có sẵn
      'Trạng thái': course.isActive ? 'Đang mở' : 'Ngừng mở',
      'Chuyên môn ID': course.specializationId || '',
    }));
    
    const worksheet = XLSX.utils.json_to_sheet(excelData);
    
    // Độ rộng cột
    worksheet['!cols'] = [
      // { wch: 10 },  // ID
      { wch: 35 },  // Tên khóa học
      { wch: 50 },  // Mô tả
      { wch: 15 },  // Học phí
      { wch: 10 },  // Số buổi
      { wch: 15 },  // Trình độ
      { wch: 12 },  // Trạng thái
      { wch: 15 },  // Chuyên môn ID
    ];
    
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Courses');
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    saveAs(blob, `courses_${new Date().toISOString().split('T')[0]}.xlsx`);
    
    return { success: true, count: courses.length };
  },

  // Import Excel
  async importFromExcel(file) {
    const data = await file.arrayBuffer();
    const workbook = XLSX.read(data);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const rows = XLSX.utils.sheet_to_json(worksheet);
    
    if (rows.length === 0) {
      throw new Error('File Excel không có dữ liệu');
    }
    
    const results = {
      success: [],
      errors: [],
      total: rows.length,
    };
    
    for (let i = 0; i < rows.length; i++) {
      const row = rows[i];
      const rowNumber = i + 2;
      
      try {
        // Lấy dữ liệu từ các cột
        const courseName = row['Tên khóa học'] || row['courseName'];
        const desct = row['Mô tả'] || row['desct'] || '';
        const tuitionFee = this._parseNumber(row['Học phí (VNĐ)'] || row['Học phí'] || row['tuitionFee']);
        const lesson = this._parseNumber(row['Số buổi'] || row['lesson']);
        const levelText = row['Trình độ'] || row['level'];
        const isActive = this._parseStatus(row['Trạng thái'] || row['isActive']);
        const specializationId = this._parseNumber(row['Chuyên môn ID'] || row['specializationId']) || null;
        
        // Validate
        if (!courseName) {
          throw new Error('Tên khóa học không được để trống');
        }
        if (isNaN(tuitionFee) || tuitionFee < 0) {
          throw new Error('Học phí không hợp lệ');
        }
        if (isNaN(lesson) || lesson <= 0) {
          throw new Error('Số buổi phải lớn hơn 0');
        }
        
        // Chuyển đổi level từ text sang số (dùng LEVEL_OPTIONS)
        const level = this._parseLevelFromText(levelText);
        if (!level) {
          throw new Error('Trình độ không hợp lệ (Sơ cấp, Căn bản, Trung cấp, Cao cấp, Chuyên gia)');
        }
        
        const courseData = {
          courseName: courseName.trim(),
          desct: desct.trim(),
          tuitionFee: Number(tuitionFee),
          lesson: Number(lesson),
          level: level,
          isActive: isActive,
          specializationId: specializationId,
        };
        
        const result = await this.createCourse(courseData);
        results.success.push({ row: rowNumber, data: result });
        
      } catch (err) {
        results.errors.push({ 
          row: rowNumber, 
          data: row, 
          error: err.message 
        });
      }
    }
    
    return results;
  },
  _parseNumber(value) {
    if (value === undefined || value === null) return NaN;
    if (typeof value === 'number') return value;
    // Xử lý string: loại bỏ dấu phân cách hàng nghìn, ký tự đặc biệt
    const cleaned = String(value).replace(/[^\d.-]/g, '');
    const num = parseFloat(cleaned);
    return isNaN(num) ? NaN : num;
  },

  _parseStatus(value) {
    if (value === undefined || value === null) return true; // default active
    if (typeof value === 'boolean') return value;
    const str = String(value).toLowerCase().trim();
    if (str === 'đang mở' || str === 'active' || str === 'true' || str === '1') return true;
    if (str === 'ngừng mở' || str === 'inactive' || str === 'false' || str === '0') return false;
    return true; // default
  },

  _parseLevelFromText(text) {
    if (!text) return null;
    const str = String(text).toLowerCase().trim();
    const map = {
      'sơ cấp': 1,
      'căn bản': 2,
      'trung cấp': 3,
      'cao cấp': 4,
      'chuyên gia': 5
    };
    return map[str] || null;
  },
};

export default courseService;