import { defineStore } from 'pinia';
import specializationService from '@/services/specializationService';

export const useSpecializationStore = defineStore('specialization', {
  state: () => ({
    specializations: [],
    currentSpecialization: null,
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
        const data = await specializationService.getAllSpecializations();
        this.specializations = Array.isArray(data) ? data : [];
      } catch (err) {
        this.error = err.message;
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
        const data = await specializationService.getSpecializationsPaged(params);
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
        const data = await specializationService.getSpecializationById(id);
        this.currentSpecialization = data;
        return data;
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
        this.validationErrors = err.response.data.errors;
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
        this.validationErrors = err.response.data.errors;
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
        await this.fetchPaged({ 
          page: this.pagedData.page, 
          pageSize: this.pagedData.pageSize 
        });
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

    async exportToExcel(params = {}) {
      this.isExporting = true;
      this.clearErrors();
      try {
        const result = await specializationService.exportToExcel(params);
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
        const result = await specializationService.importFromExcel(file);
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