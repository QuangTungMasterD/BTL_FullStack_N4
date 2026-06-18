import { defineStore } from 'pinia';
import teacherAssignmentService from '@/services/teacherAssignmentService';

export const useTeacherAssignmentStore = defineStore('teacherAssignment', {
  state: () => ({
    assignments: [],
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
        const data = await teacherAssignmentService.getAllTeacherAssignments();
        this.assignments = data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async create(assignmentData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newAssignment = await teacherAssignmentService.createTeacherAssignment(assignmentData);
        this.assignments.push(newAssignment);
        return newAssignment;
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
        await teacherAssignmentService.deleteTeacherAssignment(id);
        this.assignments = this.assignments.filter(a => a.id !== id);
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