import { defineStore } from 'pinia';
import scheduleChangeRequestService from '@/services/scheduleChangeRequestService';

export const useScheduleChangeRequestStore = defineStore('scheduleChangeRequest', {
  state: () => ({
    myRequests: [],
    pendingRequests: [],
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

    async createRequest(requestData) {
      this.loading = true;
      this.clearErrors();
      try {
        const result = await scheduleChangeRequestService.create(requestData);
        await this.fetchMyRequests(); // reload danh sách của tôi
        return result;
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

    async fetchMyRequests() {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await scheduleChangeRequestService.getMyRequests();
        this.myRequests = data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async fetchPendingRequests() {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await scheduleChangeRequestService.getPendingRequests();
        this.pendingRequests = data;
      } catch (err) {
        this.error = err.response?.data?.message || err.message;
        this.errorStatusCode = err.statusCode;
        this.validationErrors = err.response.data.errors;
        this.timestamp = err.timestamp;
      } finally {
        this.loading = false;
      }
    },

    async processRequest(requestId, action, adminNote) {
      this.loading = true;
      this.clearErrors();
      try {
        const result = await scheduleChangeRequestService.processRequest({
          requestId,
          action,
          adminNote,
        });
        await this.fetchPendingRequests(); // reload danh sách pending
        return result;
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