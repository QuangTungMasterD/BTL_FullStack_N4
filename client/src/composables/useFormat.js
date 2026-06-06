// src/composables/useFormat.js

export function formatVND(value) {
  if (value === undefined || value === null) return '0₫';
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
}

export function formatDate(dateString) {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('vi-VN');
}

export function formatDateTime(dateTimeString) {
  if (!dateTimeString) return '';
  const date = new Date(dateTimeString);
  return date.toLocaleDateString('vi-VN') + ' ' + date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
}

export function courseLevelText(level) {
  const map = {
    1: 'Sơ cấp',
    2: 'Trung cấp',
    3: 'Nâng cao',
    4: 'Chuyên gia',
    5: 'Thạc sĩ',
  };
  return map[level] || 'Không xác định';
}

export function classStatusText(status) {
  const map = {
    1: 'Scheduled',
    2: 'In Progress',
    3: 'Completed',
    4: 'Cancelled',
  };
  return map[status] || 'Unknown';
}

export function roomTypeText(type) {
  const map = {
    1: 'Lý thuyết',
    2: 'Lab',
    3: 'Họp',
  };
  return map[type] || '';
}

export function roomStatusText(status) {
  const map = {
    1: 'Available',
    2: 'Maintenance',
    3: 'Reserved',
  };
  return map[status] || '';
}