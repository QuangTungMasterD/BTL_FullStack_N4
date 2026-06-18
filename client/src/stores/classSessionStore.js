import { defineStore } from 'pinia';
import classSessionService from '@/services/classSessionService';

export const useClassSessionStore = defineStore('classSession', {
  state: () => ({
    sessions: [],
    currentSession: null,
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
        const data = await classSessionService.getAllClassSessions();
        this.sessions = Array.isArray(data) ? data : [];
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchPaged(params = {}) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await classSessionService.getClassSessionsPaged(params);
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
        const data = await classSessionService.getClassSessionById(id);
        this.currentSession = data;
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

    async create(sessionData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newSession = await classSessionService.createClassSession(sessionData);
        this.sessions.push(newSession);
        return newSession;
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

    async update(id, sessionData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await classSessionService.updateClassSession(id, sessionData);
        const index = this.sessions.findIndex(s => s.id === id);
        if (index !== -1) this.sessions[index] = updated;
        if (this.currentSession?.id === id) this.currentSession = updated;
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

    async delete(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await classSessionService.deleteClassSession(id);
        this.sessions = this.sessions.filter(s => s.id !== id);
        if (this.currentSession?.id === id) this.currentSession = null;
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
        });
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

    async updateStatus(id, status) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await classSessionService.updateClassSessionStatus(id, { status });
        const index = this.sessions.findIndex(s => s.id === id);
        if (index !== -1) this.sessions[index] = updated;
        if (this.currentSession?.id === id) this.currentSession = updated;
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

    async exportToExcel(params = {}) {
      this.isExporting = true;
      this.clearErrors();
      try {
        const result = await classSessionService.exportToExcel(params);
        return result;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
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
        const result = await classSessionService.importFromExcel(file);
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