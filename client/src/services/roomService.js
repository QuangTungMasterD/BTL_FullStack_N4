import api from './api';

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
};

export default roomService;