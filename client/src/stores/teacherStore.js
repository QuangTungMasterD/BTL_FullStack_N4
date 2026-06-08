import { defineStore } from 'pinia';
import teacherService from '@/services/teacherService';

export const useTeacherStore = defineStore('teacher', {
  state: () => ({
    teachers: [],
    currentTeacher: null,
    pagedData: {
      items: [],
      totalCount: 0,
      pageNumber: 1,
      pageSize: 10,
    },
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
        const data = await teacherService.getAllTeachers();
        this.teachers = data;
      } catch (err) {
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchPaged(pageNumber = 1, pageSize = 10) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await teacherService.getTeachersPaged({ pageNumber, pageSize });
        this.pagedData = data;
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
        const data = await teacherService.getTeacherById(id);
        this.currentTeacher = data;
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

    async create(teacherData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newTeacher = await teacherService.createTeacher(teacherData);
        this.teachers.push(newTeacher);
        return newTeacher;
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

    async update(id, teacherData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await teacherService.updateTeacher(id, teacherData);
        const index = this.teachers.findIndex(t => t.id === id);
        if (index !== -1) this.teachers[index] = updated;
        if (this.currentTeacher?.id === id) this.currentTeacher = updated;
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

    // Xóa mềm (theo Swagger, teacher chỉ có xóa cứng?)
    // Nhưng để đồng bộ, giữ method này
    async delete(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await teacherService.deleteTeacher(id);
        this.teachers = this.teachers.filter(t => t.id !== id);
        if (this.currentTeacher?.id === id) this.currentTeacher = null;
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

    // Xóa vĩnh viễn
    async deletePermanent(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await teacherService.deleteTeacherPermanent(id);
        this.teachers = this.teachers.filter(t => t.id !== id);
        if (this.currentTeacher?.id === id) this.currentTeacher = null;
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

    // Khôi phục
    async restore(id) {
      this.loading = true;
      this.clearErrors();
      try {
        const restored = await teacherService.restoreTeacher(id);
        // Refresh lại danh sách sau khi restore
        await this.fetchPaged(this.pagedData.pageNumber, this.pagedData.pageSize);
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