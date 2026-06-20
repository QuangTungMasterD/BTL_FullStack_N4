import { defineStore } from 'pinia';
import courseService from '@/services/courseService';

export const useCourseStore = defineStore('course', {
  state: () => ({
    courses: [],
    currentCourse: null,
    pagedData: {
      data: [],
      page: 1,
      pageSize: 12,
      totalRecords: 0,
      totalPages: 0,
      hasNext: false,
      hasPrev: false,
    },
    loading: false,
    error: null,
    errorStatusCode: null,
    validationErrors: null,
    timestamp: null,
    // Thêm state riêng cho export/import
    isExporting: false,
    isImporting: false,
    importResult: {
      show: false,
      total: 0,
      successCount: 0,
      errors: [],
    },
  }),

  getters: {
    levelOptions: () => [
      { value: 0, label: 'Tất cả trình độ' },
      { value: 1, label: 'Sơ cấp' },
      { value: 2, label: 'Căn bản' },
      { value: 3, label: 'Trung cấp' },
      { value: 4, label: 'Cao cấp' },
      { value: 5, label: 'Chuyên gia' },
    ],
    
    statusOptions: () => [
      { value: 0, label: 'Tất cả trạng thái' },
      { value: true, label: 'Đang mở' },
      { value: false, label: 'Ngừng mở' },
    ],
  },

  actions: {
    clearErrors() {
      this.error = null;
      this.errorStatusCode = null;
      this.validationErrors = null;
      this.timestamp = null;
    },

    closeImportResult() {
      this.importResult.show = false;
    },

    async fetchPaged(params = {}) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await courseService.getCoursesPaged(params);
        this.pagedData = data;
        console.log('Fetched paged courses:', this.pagedData);
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchAll() {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await courseService.getAllCourses();
        this.courses = data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchById(id) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await courseService.getCourseById(id);
        this.currentCourse = data;
        return data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async create(courseData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newCourse = await courseService.createCourse(courseData);
        this.courses.push(newCourse);
        return newCourse;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async update(id, courseData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await courseService.updateCourse(id, courseData);
        const index = this.courses.findIndex(c => c.id === id);
        if (index !== -1) this.courses[index] = updated;
        if (this.currentCourse?.id === id) this.currentCourse = updated;
        return updated;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async delete(id, permanent = false) {
      this.loading = true;
      this.clearErrors();
      try {
        if (permanent) {
          await courseService.deleteCoursePermanent(id);
        } else {
          await courseService.deleteCourse(id);
        }
        this.courses = this.courses.filter(c => c.id !== id);
        if (this.currentCourse?.id === id) this.currentCourse = null;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    async restore(id) {
      this.loading = true;
      this.clearErrors();
      try {
        const restored = await courseService.restoreCourse(id);
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
        });
        return restored;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },

    // EXPORT - dùng isExporting riêng
    async exportToExcel(params = {}) {
      this.isExporting = true;
      this.clearErrors();
      try {
        const result = await courseService.exportToExcel(params);
        return result;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        throw err;
      } finally {
        this.isExporting = false;
      }
    },

    // IMPORT - dùng isImporting riêng
    async importFromExcel(file) {
      this.isImporting = true;
      this.clearErrors();
      this.importResult = { show: false, total: 0, successCount: 0, errors: [] };
      
      try {
        const result = await courseService.importFromExcel(file);
        console.log(result)
        this.importResult = {
          show: true,
          total: result.total,
          successCount: result.success.length,
          errors: result.errors,
        };
        
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
        });
        
        return result;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        throw err;
      } finally {
        this.isImporting = false;
      }
    },
  },
});