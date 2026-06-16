<template>
  <div class="notification-wrapper">
    <button class="notification-btn" @click.stop="toggleMenu">
      <v-icon icon="mdi-bell-outline" size="20" />
      <span class="badge" v-if="unreadCount > 0">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
    </button>

    <transition name="notification-fade">
      <div v-if="menuVisible" class="notification-dropdown" @click.stop>
        <div class="notification-header">
          <span class="notification-title">Thông báo</span>
          <button class="mark-all-read" @click="markAllRead">Đánh dấu đã đọc</button>
        </div>
        <div class="notification-list">
          <div
            v-for="notif in notifications"
            :key="notif.id"
            class="notification-item"
            :class="{ unread: !notif.isRead }"
            @click="markAsRead(notif.id)"
          >
            <div class="notification-icon" :class="notif.type">
              <v-icon :icon="getIcon(notif.type)" size="18" />
            </div>
            <div class="notification-content">
              <div class="notification-title-text">{{ notif.title }}</div>
              <div class="notification-message">{{ notif.message }}</div>
              <div class="notification-time">{{ formatTime(notif.createdAt) }}</div>
            </div>
          </div>
          <div v-if="notifications.length === 0" class="empty-notifications">
            <v-icon icon="mdi-bell-off" size="40" color="#cbd5e1" />
            <p>Không có thông báo nào</p>
          </div>
        </div>
        <div class="notification-footer" v-if="notifications.length > 0">
          <button class="view-all-btn" @click="viewAll">Xem tất cả</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const menuVisible = ref(false)
const unreadCount = ref(3)

const notifications = ref([
  { id: 1, title: 'Lịch thi cuối kỳ', message: 'Lịch thi cuối kỳ học kỳ Fall 2024 đã được công bố', type: 'info', isRead: false, createdAt: new Date() },
  { id: 2, title: 'Đăng ký học phần', message: 'Đợt đăng ký học phần HK2 bắt đầu từ ngày 15/12', type: 'warning', isRead: false, createdAt: new Date(Date.now() - 3600000) },
  { id: 3, title: 'Cập nhật điểm', message: 'Điểm môn CS301 đã được cập nhật', type: 'success', isRead: false, createdAt: new Date(Date.now() - 7200000) },
])

const getIcon = (type) => {
  const icons = { info: 'mdi-information', warning: 'mdi-alert', success: 'mdi-check-circle', error: 'mdi-close-circle' }
  return icons[type] || 'mdi-bell'
}

const formatTime = (date) => {
  const diff = Date.now() - new Date(date).getTime()
  const mins = Math.floor(diff / 60000)
  if (mins < 1) return 'Vừa xong'
  if (mins < 60) return `${mins} phút trước`
  if (mins < 1440) return `${Math.floor(mins / 60)} giờ trước`
  return `${Math.floor(mins / 1440)} ngày trước`
}

const toggleMenu = () => {
  menuVisible.value = !menuVisible.value
}

const markAsRead = (id) => {
  const notif = notifications.value.find(n => n.id === id)
  if (notif && !notif.isRead) {
    notif.isRead = true
    unreadCount.value--
  }
}

const markAllRead = () => {
  notifications.value.forEach(n => { 
    if (!n.isRead) {
      n.isRead = true
      unreadCount.value--
    }
  })
  if (unreadCount.value < 0) unreadCount.value = 0
}

const viewAll = () => {
  console.log('View all notifications')
  menuVisible.value = false
}

// Đóng dropdown khi click ra ngoài
const handleClickOutside = (event) => {
  const wrapper = document.querySelector('.notification-wrapper')
  if (wrapper && !wrapper.contains(event.target)) {
    menuVisible.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.notification-wrapper {
  position: relative;
}

.notification-btn {
  position: relative;
  background: none;
  border: none;
  cursor: pointer;
  padding: 8px;
  border-radius: 10px;
  color: #1e293b;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.notification-btn:hover {
  background: rgba(0, 0, 0, 0.05);
}

.badge {
  position: absolute;
  top: 2px;
  right: 2px;
  min-width: 18px;
  height: 18px;
  background: #ef4444;
  color: white;
  font-size: 10px;
  font-weight: 600;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 5px;
}

/* Dropdown Styles */
.notification-dropdown {
  position: absolute;
  top: 45px;
  right: 0;
  width: 380px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
  z-index: 2000;
  overflow: hidden;
  animation: dropdownSlide 0.2s ease;
}

@keyframes dropdownSlide {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: #f8fafc;
  border-bottom: 1px solid #eef2f6;
}

.notification-title {
  font-size: 16px;
  font-weight: 700;
  color: #1e293b;
}

.mark-all-read {
  background: none;
  border: none;
  color: #3b82f6;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 6px;
  transition: all 0.2s;
}

.mark-all-read:hover {
  background: rgba(59, 130, 246, 0.1);
}

.notification-list {
  max-height: 400px;
  overflow-y: auto;
}

.notification-item {
  display: flex;
  gap: 14px;
  padding: 14px 20px;
  cursor: pointer;
  transition: all 0.2s;
  border-bottom: 1px solid #f1f5f9;
}

.notification-item:hover {
  background: #f8fafc;
}

.notification-item.unread {
  background: #f0f9ff;
}

.notification-item.unread:hover {
  background: #e0f2fe;
}

.notification-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.notification-icon.info {
  background: #dbeafe;
  color: #3b82f6;
}

.notification-icon.warning {
  background: #fef3c7;
  color: #f59e0b;
}

.notification-icon.success {
  background: #dcfce7;
  color: #10b981;
}

.notification-icon.error {
  background: #fee2e2;
  color: #ef4444;
}

.notification-content {
  flex: 1;
}

.notification-title-text {
  font-weight: 600;
  font-size: 14px;
  color: #1e293b;
  margin-bottom: 4px;
}

.notification-message {
  font-size: 12px;
  color: #64748b;
  margin-bottom: 4px;
  line-height: 1.4;
}

.notification-time {
  font-size: 10px;
  color: #94a3b8;
}

.empty-notifications {
  text-align: center;
  padding: 48px 20px;
  color: #94a3b8;
}

.empty-notifications p {
  margin-top: 12px;
  font-size: 14px;
}

.notification-footer {
  padding: 12px 16px;
  background: #f8fafc;
  border-top: 1px solid #eef2f6;
}

.view-all-btn {
  width: 100%;
  background: none;
  border: none;
  color: #3b82f6;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  padding: 8px;
  border-radius: 8px;
  transition: all 0.2s;
}

.view-all-btn:hover {
  background: rgba(59, 130, 246, 0.1);
}

/* Transition */
.notification-fade-enter-active,
.notification-fade-leave-active {
  transition: all 0.2s ease;
}

.notification-fade-enter-from,
.notification-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Responsive */
@media (max-width: 480px) {
  .notification-dropdown {
    position: fixed;
    top: 60px;
    right: 10px;
    left: 10px;
    width: auto;
    max-width: none;
  }
}
</style>