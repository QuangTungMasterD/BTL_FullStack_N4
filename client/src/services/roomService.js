import api from './api';

const roomService = {
  // Lấy danh sách có phân trang
  getRoomsPaged(params = {}) {
    return api.get('/Rooms/paged', { params });
  },
  // Lấy tất cả
  getAllRooms() {
    return api.get('/Rooms');
  },
  // Lấy chi tiết
  getRoomById(id) {
    return api.get(`/Rooms/${id}`);
  },
  // Tạo mới
  createRoom(data) {
    return api.post('/Rooms', data);
  },
  // Cập nhật
  updateRoom(id, data) {
    return api.put(`/Rooms/${id}`, data);
  },
  // Xóa mềm
  deleteRoom(id) {
    return api.delete(`/Rooms/${id}`);
  },
  // Xóa vĩnh viễn
  deleteRoomPermanent(id) {
    return api.delete(`/Rooms/${id}/permanent`);
  },
  // Khôi phục
  restoreRoom(id) {
    return api.patch(`/Rooms/${id}/restore`);
  },
};

export default roomService;