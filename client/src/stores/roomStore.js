import { defineStore } from 'pinia';
import roomService from '@/services/roomService';

export const useRoomStore = defineStore('room', {
  state: () => ({
    rooms: [],
    currentRoom: null,
    pagedData: {
      data: [],
      page: 1,
      pageSize: 10,
      totalRecords: 0,
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
        const data = await roomService.getAllRooms();
        this.rooms = Array.isArray(data) ? data : [];
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
        const data = await roomService.getRoomsPaged(params);
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
        const data = await roomService.getRoomById(id);
        this.currentRoom = data;
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

    async create(roomData) {
      this.loading = true;
      this.clearErrors();
      try {
        const newRoom = await roomService.createRoom(roomData);
        this.rooms.push(newRoom);
        return newRoom;
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

    async update(id, roomData) {
      this.loading = true;
      this.clearErrors();
      try {
        const updated = await roomService.updateRoom(id, roomData);
        const index = this.rooms.findIndex(r => r.id === id);
        if (index !== -1) this.rooms[index] = updated;
        if (this.currentRoom?.id === id) this.currentRoom = updated;
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

    // Xóa mềm (chuyển vào thùng rác)
    async delete(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await roomService.deleteRoom(id);
        this.rooms = this.rooms.filter(r => r.id !== id);
        if (this.currentRoom?.id === id) this.currentRoom = null;
        // Reload lại trang hiện tại
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
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

    // Xóa vĩnh viễn
    async deletePermanent(id) {
      this.loading = true;
      this.clearErrors();
      try {
        await roomService.deleteRoomPermanent(id);
        this.rooms = this.rooms.filter(r => r.id !== id);
        if (this.currentRoom?.id === id) this.currentRoom = null;
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
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

    async restore(id) {
      this.loading = true;
      this.clearErrors();
      try {
        const restored = await roomService.restoreRoom(id);
        await this.fetchPaged({
          page: this.pagedData.page,
          pageSize: this.pagedData.pageSize,
        });
        return restored;
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
        const result = await roomService.exportToExcel(params);
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
        const result = await roomService.importFromExcel(file);
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
        this.error = err.message;
        throw err;
      } finally {
        this.isImporting = false;
      }
    },
  },
});