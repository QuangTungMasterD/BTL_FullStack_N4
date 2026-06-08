import { defineStore } from 'pinia';
import teacherService from '@/services/teacherService';

export const useTeacherStore = defineStore('teacher', {
  state: () => ({
    teachers: [],
    currentTeacher: null,
    pagedData: {
      data: [],           // ← sửa: items → data
      page: 1,
      pageSize: 10,
      totalRecords: 0,    // ← sửa: totalCount → totalRecords
      totalPages: 0,
      hasNext: false,
      hasPrev: false,
    },
    loading: false,
    error: null,
    errorStatusCode: null,
    validationErrors: null,
    timestamp: null,
    isExporting: false,
    isImporting: false,
    importResult: {
      show: false,
      total: 0,
      successCount: 0,
      errors: [],
    },
  }),

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

    async fetchAll() {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await teacherService.getAllTeachers();
        this.teachers = Array.isArray(data) ? data : [];
      } catch (err) {
        this.error = err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchPaged(params = {}) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await teacherService.getTeachersPaged(params);
        this.pagedData = {
          data: data?.data || [],
          page: data?.page || 1,
          pageSize: data?.pageSize || 10,
          totalRecords: data?.totalRecords || 0,
          totalPages: data?.totalPages || 0,
          hasNext: data?.hasNext || false,
          hasPrev: data?.hasPrev || false,
        };
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

    async delete(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await teacherService.deleteTeacher(id);
        this.teachers = this.teachers.filter(t => t.id !== id);
        if (this.currentTeacher?.id === id) this.currentTeacher = null;
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
        });
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

    async deletePermanent(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await teacherService.deleteTeacherPermanent(id);
        this.teachers = this.teachers.filter(t => t.id !== id);
        if (this.currentTeacher?.id === id) this.currentTeacher = null;
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
        });
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
        const restored = await teacherService.restoreTeacher(id);
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
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

    async exportToExcel(params = {}) {
      this.isExporting = true;
      this.clearErrors();
      try {
        const result = await teacherService.exportToExcel(params);
        return result;
      } catch (err) {
        this.error = err.message;
        throw err;
      } finally {
        this.isExporting = false;
      }
    },

    async importFromExcel(file) {
      this.isImporting = true;
      this.clearErrors();
      this.importResult = { show: false, total: 0, successCount: 0, errors: [] };
      
      try {
        const result = await teacherService.importFromExcel(file);
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
        this.error = err.message;
        throw err;
      } finally {
        this.isImporting = false;
      }
    },
  },
});