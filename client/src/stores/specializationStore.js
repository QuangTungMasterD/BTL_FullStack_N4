import { defineStore } from 'pinia';
import specializationService from '@/services/specializationService';

export const useSpecializationStore = defineStore('specialization', {
  state: () => ({
    specializations: [],
    currentSpecialization: null,
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
        const data = await specializationService.getAllSpecializations();
        this.specializations = data;
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
        const data = await specializationService.getSpecializationsPaged({ pageNumber, pageSize });
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
        const data = await specializationService.getSpecializationById(id);
        this.currentSpecialization = data;
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

    async create(specData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newSpec = await specializationService.createSpecialization(specData);
        this.specializations.push(newSpec);
        return newSpec;
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

    async update(id, specData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await specializationService.updateSpecialization(id, specData);
        const index = this.specializations.findIndex(s => s.id === id);
        if (index !== -1) this.specializations[index] = updated;
        if (this.currentSpecialization?.id === id) this.currentSpecialization = updated;
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

    async deletePermanent(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await specializationService.deleteSpecializationPermanent(id);
        this.specializations = this.specializations.filter(s => s.id !== id);
        if (this.currentSpecialization?.id === id) this.currentSpecialization = null;
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