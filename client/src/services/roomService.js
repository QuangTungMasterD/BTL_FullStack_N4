import api from './api';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { roomStatusText, roomTypeText } from '@/composables/useFormat';

const roomService = {
  // Lấy danh sách có phân trang
  getRoomsPaged(params = {}) {
    return api.get('v1/Rooms/paged', { params });
  },
  // Lấy tất cả
  getAllRooms() {
    return api.get('v1/Rooms');
  },
  // Lấy chi tiết
  getRoomById(id) {
    return api.get(`v1/Rooms/${id}`);
  },
  // Tạo mới
  createRoom(data) {
    return api.post('v1/Rooms', data);
  },
  // Cập nhật
  updateRoom(id, data) {
    return api.put(`v1/Rooms/${id}`, data);
  },
  // Xóa mềm
  deleteRoom(id) {
    return api.delete(`v1/Rooms/${id}`);
  },
  // Xóa vĩnh viễn
  deleteRoomPermanent(id) {
    return api.delete(`v1/Rooms/${id}/permanent`);
  },
  // Khôi phục
  restoreRoom(id) {
    return api.patch(`v1/Rooms/${id}/restore`);
  },
  async exportToExcel(params = {}) {
    const response = await this.getRoomsPaged({
      ...params,
      page: 1,
      pageSize: 10000,
    });

    const rooms = response.data || [];

    if (rooms.length === 0) {
      throw new Error('Không có dữ liệu để xuất');
    }

    const excelData = rooms.map(room => ({
      // 'ID': room.id,
      'Tên phòng': room.roomName,
      'Loại phòng': roomTypeText(room.roomType),
      'Mô tả': room.descrt || '',
      'Trạng thái': roomStatusText(room.status),
    }));

    const worksheet = XLSX.utils.json_to_sheet(excelData);
    worksheet['!cols'] = [
      // { wch: 10 },  // ID
      { wch: 25 },  // Tên phòng
      { wch: 20 },  // Loại phòng
      { wch: 40 },  // Mô tả
      { wch: 15 },  // Trạng thái
    ];

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Rooms');
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    saveAs(blob, `rooms_${new Date().toISOString().split('T')[0]}.xlsx`);

    return { success: true, count: rooms.length };
  },

  // ---------- Import Excel (client-side) ----------
  async importFromExcel(file) {
    const data = await file.arrayBuffer();
    const workbook = XLSX.read(data);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const rows = XLSX.utils.sheet_to_json(worksheet);

    if (rows.length === 0) {
      throw new Error('File Excel không có dữ liệu');
    }

    const results = {
      success: [],
      errors: [],
      total: rows.length,
    };

    for (let i = 0; i < rows.length; i++) {
      const row = rows[i];
      const rowNumber = i + 2;

      try {
        const roomName = String(row['Tên phòng'] || row['roomName']);
        const roomTypeText = row['Loại phòng'] || row['roomType'];
        const descrt = row['Mô tả'] || row['descrt'] || '';
        const statusText = row['Trạng thái'] || row['status'];

        if (!roomName) throw new Error('Tên phòng không được để trống');

        const roomType = this._parseRoomType(roomTypeText);
        if (!roomType) throw new Error('Loại phòng không hợp lệ (Phòng học thường, Phòng máy tính, Phòng hội thảo, Phòng thực hành, Phòng họp)');

        const status = this._parseRoomStatus(statusText);
        if (!status) throw new Error('Trạng thái không hợp lệ (Có thể sử dụng, Đang được sử dụng, Đang bảo trì, Đóng cửa)');

        const roomData = {
          roomName: roomName.trim(),
          roomType,
          descrt: descrt.trim(),
          status,
        };

        const result = await this.createRoom(roomData);
        results.success.push({ row: rowNumber, data: result });
      } catch (err) {
        results.errors.push({ row: rowNumber, data: row, error: err.message });
      }
    }

    return results;
  },

  // Helper methods
  _parseNumber(value) {
    if (value === undefined || value === null) return NaN;
    if (typeof value === 'number') return value;
    const cleaned = String(value).replace(/[^\d.-]/g, '');
    const num = parseFloat(cleaned);
    return isNaN(num) ? NaN : num;
  },

  _parseRoomType(text) {
    if (!text) return null;
    const str = String(text).toLowerCase();
    const map = {
      'phòng học thường': 1,
      'phòng máy tính': 2,
      'phòng hội thảo': 3,
      'phòng thực hành': 4,
      'phòng họp': 5,
    };
    return map[str] || null;
  },

  _parseRoomStatus(text) {
    if (!text) return null;
    const str = String(text).toLowerCase();
    const map = {
      'có thể sử dụng': 1,
      'đang được sử dụng': 2,
      'đang bảo trì': 3,
      'đóng cửa': 4,
    };
    return map[str] || null;
  },
};

export default roomService;