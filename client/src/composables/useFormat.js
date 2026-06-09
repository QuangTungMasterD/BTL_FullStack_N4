// src/composables/useFormat.js

/**
 * Định dạng số tiền theo VND
 */
export function formatVND(value) {
  if (value === undefined || value === null) return '0₫';
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
}

/**
 * Định dạng ngày (không giờ)
 */
export function formatDate(dateString) {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('vi-VN');
}

/**
 * Định dạng ngày giờ
 */
export function formatDateTime(dateTimeString) {
  if (!dateTimeString) return '';
  const date = new Date(dateTimeString);
  return date.toLocaleDateString('vi-VN') + ' ' + date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
}

/**
 * Chuyển đổi CourseLevel từ số sang text
 */
export function courseLevelText(level) {
  const map = {
    1: 'Sơ cấp',
    2: 'Căn bản',
    3: 'Trung cấp',
    4: 'Cao cấp',
    5: 'Chuyên gia',
  };
  return map[level] || 'Không xác định';
}

/**
 * Chuyển đổi ClassStatus từ số sang text
 */
export function classStatusText(status) {
  const map = {
    1: 'Chờ khai giảng',
    2: 'Đang học',
    3: 'Đã kết thúc',
    4: 'Đã hủy',
    5: 'Tạm dừng',
  };
  return map[status] || 'Không xác định';
}

/**
 * Chuyển đổi ClassSessionStatus từ số sang text
 */
export function classSessionStatusText(status) {
  const map = {
    1: 'Đã lên lịch',
    2: 'Đang diễn ra',
    3: 'Đã kết thúc',
    4: 'Đã hủy',
  };
  return map[status] || 'Không xác định';
}

export function classSessionStatusBadgeVariant(status) {
  const map = {
    1: 'info',     // Đã lên lịch
    2: 'success',  // Đang diễn ra
    3: 'default',  // Đã kết thúc
    4: 'error',    // Đã hủy
  };
  return map[status] || 'default';
}

/**
 * Chuyển đổi RoomStatus từ số sang text
 */
export function roomStatusText(status) {
  const map = {
    1: 'Có thể sử dụng',
    2: 'Đang được sử dụng',
    3: 'Đang bảo trì',
    4: 'Đóng cửa',
  };
  return map[status] || 'Không xác định';
}

/**
 * Chuyển đổi RoomType từ số sang text
 */
export function roomTypeText(type) {
  const map = {
    1: 'Phòng học thường',
    2: 'Phòng máy tính',
    3: 'Phòng hội thảo',
    4: 'Phòng thực hành',
    5: 'Phòng họp',
  };
  return map[type] || 'Không xác định';
}

/**
 * Chuyển đổi CourseLevel sang class badge variant (tuỳ chọn)
 */
export function courseLevelBadgeVariant(level) {
  const map = {
    1: 'info',
    2: 'info',
    3: 'warning',
    4: 'success',
    5: 'success',
  };
  return map[level] || 'default';
}

/**
 * Chuyển đổi ClassStatus sang class badge variant (tuỳ chọn)
 */
export function classStatusBadgeVariant(status) {
  const map = {
    1: 'info',     // Chờ khai giảng
    2: 'success',  // Đang học
    3: 'default',  // Đã kết thúc
    4: 'error',    // Đã hủy
    5: 'warning',  // Tạm dừng
  };
  return map[status] || 'default';
}