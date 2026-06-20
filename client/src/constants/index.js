// src/constants/index.js

// Course constants
export const LEVEL_OPTIONS = [
  { value: 1, label: 'Sơ cấp' },
  { value: 2, label: 'Căn bản' },
  { value: 3, label: 'Trung cấp' },
  { value: 4, label: 'Cao cấp' },
  { value: 5, label: 'Chuyên gia' },
];

export const STATUS_OPTIONS = [
  { value: true, label: 'Đang mở' },
  { value: false, label: 'Ngừng mở' },
];

// Class constants
export const CLASS_STATUS_OPTIONS = [
  { value: 1, label: 'Chờ khai giảng' },
  { value: 2, label: 'Đang học' },
  { value: 3, label: 'Đã kết thúc' },
  { value: 4, label: 'Đã hủy' },
  { value: 5, label: 'Tạm dừng' },
];

// ClassSession constants
export const CLASS_SESSION_STATUS_OPTIONS = [
  { value: 1, label: 'Đã lên lịch' },
  { value: 2, label: 'Đang diễn ra' },
  { value: 3, label: 'Đã kết thúc' },
  { value: 4, label: 'Đã hủy' },
];

// Room constants
export const ROOM_STATUS_OPTIONS = [
  { value: 1, label: 'Có thể sử dụng' },
  { value: 2, label: 'Đang được sử dụng' },
  { value: 3, label: 'Đang bảo trì' },
  { value: 4, label: 'Đóng cửa' },
];

export const ROOM_TYPE_OPTIONS = [
  { value: 1, label: 'Phòng học thường' },
  { value: 2, label: 'Phòng máy tính' },
  { value: 3, label: 'Phòng hội thảo' },
  { value: 4, label: 'Phòng thực hành' },
  { value: 5, label: 'Phòng họp' },
];

// Pagination
export const DEFAULT_PAGE_SIZE = 12;
export const PAGE_SIZE_OPTIONS = [12, 24, 48, 96];