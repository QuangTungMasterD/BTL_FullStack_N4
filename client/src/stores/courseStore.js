import { defineStore } from 'pinia';
import courseService from '@/services/courseService';

export const useCourseStore = defineStore('course', {
  state: () => ({
    courses: [],
    currentCourse: null,
    pagedData: {
      data: [],        // Đổi từ items thành data (theo API)
      page: 1,
      pageSize: 10,
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
  }),

  getters: {
    // Course level options cho filter
    levelOptions: () => [
      { value: null, label: 'Tất cả trình độ' },
      { value: 1, label: 'Sơ cấp' },
      { value: 2, label: 'Căn bản' },
      { value: 3, label: 'Trung cấp' },
      { value: 4, label: 'Cao cấp' },
      { value: 5, label: 'Chuyên gia' },
    ],
    
    // Status options cho filter
    statusOptions: () => [
      { value: null, label: 'Tất cả trạng thái' },
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

    async fetchPaged(params = {}) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await courseService.getCoursesPaged(params);
        this.pagedData = data;
        console.log('Fetched paged courses:', this.pagedData);
      } catch (err) {
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
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
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
        this.timestamp = err.timestamp;
        throw err;
      } finally {
        this.loading = false;
      }
    },
  },
});