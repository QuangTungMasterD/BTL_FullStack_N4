import { defineStore } from 'pinia';
import classSessionService from '@/services/classSessionService';

export const useClassSessionStore = defineStore('classSession', {
  state: () => ({
    sessions: [],
    currentSession: null,
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
        const data = await classSessionService.getAllClassSessions();
        this.sessions = data;
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
        const data = await classSessionService.getClassSessionsPaged({ pageNumber, pageSize });
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
        const data = await classSessionService.getClassSessionById(id);
        this.currentSession = data;
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

    async create(sessionData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newSession = await classSessionService.createClassSession(sessionData);
        this.sessions.push(newSession);
        return newSession;
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
        await classSessionService.deleteClassSession(id);
        this.sessions = this.sessions.filter(s => s.id !== id);
        if (this.currentSession?.id === id) this.currentSession = null;
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