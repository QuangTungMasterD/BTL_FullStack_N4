import { defineStore } from 'pinia';
import classService from '@/services/classService';

export const useClassStore = defineStore('class', {
  state: () => ({
    classes: [],
    currentClass: null,
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
        const data = await classService.getAllClasses();
        this.classes = data;
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
        const data = await classService.getClassesPaged({ pageNumber, pageSize });
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
        const data = await classService.getClassById(id);
        this.currentClass = data;
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

    async create(classData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newClass = await classService.createClass(classData);
        this.classes.push(newClass);
        return newClass;
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

    async update(id, classData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await classService.updateClass(id, classData);
        const index = this.classes.findIndex(c => c.id === id);
        if (index !== -1) this.classes[index] = updated;
        if (this.currentClass?.id === id) this.currentClass = updated;
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

    async delete(id, permanent = false) {
      this.loading = true;
      this.clearErrors();
      try {
        if (permanent) {
          await classService.deleteClassPermanent(id);
        } else {
          await classService.deleteClass(id);
        }
        this.classes = this.classes.filter(c => c.id !== id);
        if (this.currentClass?.id === id) this.currentClass = null;
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
        const restored = await classService.restoreClass(id);
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

    async fetchByCourseId(courseId) {
      this.loading = true;
      this.clearErrors();
      try {
        const data = await classService.getClassesByCourseId(courseId);
        this.classes = data;
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
  },
});