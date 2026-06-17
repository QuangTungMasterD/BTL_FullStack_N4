import { defineStore } from 'pinia';
import teacherSpecializationService from '@/services/teacherSpecializationService';

export const useTeacherSpecializationStore = defineStore('teacherSpecialization', {
  state: () => ({
    teacherSpecs: [],
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
        const data = await teacherSpecializationService.getAllTeacherSpecializations();
        this.teacherSpecs = data;
      } catch (err) {
        this.error = err.message;
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
        const newItem = await teacherSpecializationService.createTeacherSpecialization(data);
        this.teacherSpecs.push(newItem);
        return newItem;
      } catch (err) {
        this.error = err.message;
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
        await teacherSpecializationService.deleteTeacherSpecialization(id);
        this.teacherSpecs = this.teacherSpecs.filter(item => item.id !== id);
      } catch (err) {
        this.error = err.message;
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