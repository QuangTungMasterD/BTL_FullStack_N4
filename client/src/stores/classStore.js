import { defineStore } from "pinia";
import classService from "@/services/classService";
import { useCourseStore } from '@/stores';

export const useClassStore = defineStore("class", {
  state: () => ({
    classes: [],
    currentClass: null,
    pagedData: {
      data: [], // ← sửa: items → data
      page: 1,
      pageSize: 10,
      totalRecords: 0, // ← sửa: totalCount → totalRecords
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
        const data = await classService.getAllClasses();
        this.classes = Array.isArray(data) ? data : [];
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
        const data = await classService.getClassesPaged(params);
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
        const data = await classService.getClassById(id);
        this.currentClass = data;
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

    async create(classData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newClass = await classService.createClass(classData);
        this.classes.push(newClass);
        return newClass;
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

    async update(id, classData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await classService.updateClass(id, classData);
        const index = this.classes.findIndex((c) => c.id === id);
        if (index !== -1) this.classes[index] = updated;
        if (this.currentClass?.id === id) this.currentClass = updated;
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

    async delete(id, permanent = false) {
      this.loading = true;
      this.clearErrors();
      try {
        if (permanent) {
          await classService.deleteClassPermanent(id);
        } else {
          await classService.deleteClass(id);
        }
        this.classes = this.classes.filter((c) => c.id !== id);
        if (this.currentClass?.id === id) this.currentClass = null;
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
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

    async restore(id) {
      this.loading = true;
      this.clearErrors();
      try {
        const restored = await classService.restoreClass(id);
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
        });
        return restored;
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

    async fetchByCourseId(courseId) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await classService.getClassesByCourseId(courseId);
        this.classes = Array.isArray(data) ? data : [];
        return this.classes;
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
        const result = await classService.exportToExcel(params);
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
      this.importResult = {
        show: false,
        total: 0,
        successCount: 0,
        errors: [],
      };

      try {
        // Lấy danh sách khóa học để map tên -> id
        const courseStore = useCourseStore();
        await courseStore.fetchAll();
        const courseMap = {};
        courseStore.courses.forEach((c) => {
          courseMap[c.courseName] = c.id;
        });

        const result = await classService.importFromExcel(file, courseMap);
        this.importResult = {
          show: true,
          total: result.total,
          successCount: result.success.length,
          errors: result.errors,
        };
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
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
