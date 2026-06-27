import { defineStore } from 'pinia';
import CourseTeacherService from '@/services/CourseTeacherService';

export const useTeacherSpecializationStore = defineStore('teacherSpecialization', {
  state: () => ({
    teacherCourses: [],
    loading: false,
    error: null,
    errorStatusCode: null,
    validationErrors: null,
    timestamp: null,
  }),

  actions: {
    clearErrors() {
      this.error = null;
      this.errorStatusCode = null;
      this.validationErrors = null;
      this.timestamp = null;
    },

    async fetchAll() {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await CourseTeacherService.getAllTeacherSpecializations();
        this.teacherCourses = data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async create(data) {
      this.loading = true;
      this.clearErrors();
      try {
        const newItem = await CourseTeacherService.createCourseTeacher(data);
        this.teacherCourses.push(newItem);
        return newItem;
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

    async delete(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await CourseTeacherService.deleteCourseTeacher(id);
        this.teacherCourses = this.teacherCourses.filter(item => item.id !== id);
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
  },
});