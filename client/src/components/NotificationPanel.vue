<template>
  <div class="notification-wrapper">
    <button class="notification-btn" @click.stop="toggleMenu">
      <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
        <path d="M13.73 21a2 2 0 0 1-3.46 0" />
      </svg>
      <span class="badge" v-if="unreadCount > 0">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
    </button>

    <transition name="notification-fade">
      <div v-if="menuVisible" class="notification-dropdown" @click.stop>
        <div class="notification-header">
          <div class="header-left">
            <span class="notification-title">Thông báo</span>
            <span class="notification-count" v-if="notifications.length > 0">{{ notifications.length }}</span>
          </div>
          <button class="mark-all-read" @click="markAllRead" v-if="unreadCount > 0">
            Đánh dấu đã đọc
          </button>
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
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" v-if="notif.type === 'info'"/>
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" v-if="notif.type === 'warning'"/>
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" v-if="notif.type === 'success'"/>
                <circle cx="12" cy="12" r="10" v-if="notif.type === 'error'"/>
                <line x1="15" y1="9" x2="9" y2="15" v-if="notif.type === 'error'"/>
                <line x1="9" y1="9" x2="15" y2="15" v-if="notif.type === 'error'"/>
              </svg>
            </div>
            <div class="notification-content">
              <div class="notification-title-text">{{ notif.title }}</div>
              <div class="notification-message">{{ notif.message }}</div>
              <div class="notification-time">{{ formatTime(notif.createdAt) }}</div>
            </div>
            <div class="notification-status" v-if="!notif.isRead">
              <span class="dot"></span>
            </div>
          </div>

          <div v-if="notifications.length === 0" class="empty-notifications">
            <svg width="60" height="60" viewBox="0 0 24 24" fill="none" stroke="#cbd5e1" stroke-width="1.5">
              <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
              <path d="M13.73 21a2 2 0 0 1-3.46 0" />
            </svg>
            <p>Không có thông báo nào</p>
          </div>
        </div>

        <div class="notification-footer" v-if="notifications.length > 0">
          <button class="view-all-btn" @click="viewAll">
            Xem tất cả thông báo
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M5 12h14M12 5l7 7-7 7"/>
            </svg>
          </button>
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
  { 
    id: 1, 
    title: 'Lịch thi cuối kỳ', 
    message: 'Lịch thi cuối kỳ học kỳ Fall 2024 đã được công bố', 
    type: 'info', 
    isRead: false, 
    createdAt: new Date() 
  },
  { 
    id: 2, 
    title: 'Đăng ký học phần', 
    message: 'Đợt đăng ký học phần HK2 bắt đầu từ ngày 15/12', 
    type: 'warning', 
    isRead: false, 
    createdAt: new Date(Date.now() - 3600000) 
  },
  { 
    id: 3, 
    title: 'Cập nhật điểm', 
    message: 'Điểm môn CS301 đã được cập nhật', 
    type: 'success', 
    isRead: false, 
    createdAt: new Date(Date.now() - 7200000) 
  },
])

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
  display: inline-block;
}

.notification-btn {
  position: relative;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 10px;
  border-radius: 12px;
  color: #64748b;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
}

.notification-btn:hover {
  background: rgba(102, 126, 234, 0.08);
  color: #667eea;
}

.notification-btn:active {
  transform: scale(0.95);
}

.notification-btn svg {
  transition: all 0.2s ease;
}

.notification-btn:hover svg {
  color: #667eea;
}

.badge {
  position: absolute;
  top: 4px;
  right: 4px;
  min-width: 20px;
  height: 20px;
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: white;
  font-size: 10px;
  font-weight: 700;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 6px;
  border: 2px solid white;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

/* Dropdown Styles */
.notification-dropdown {
  position: absolute;
  top: calc(100% + 12px);
  right: 0;
  width: 420px;
  max-width: 90vw;
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15), 0 4px 12px rgba(0, 0, 0, 0.05);
  z-index: 2000;
  overflow: hidden;
  animation: dropdownSlide 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes dropdownSlide {
  from {
    opacity: 0;
    transform: translateY(-8px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 18px 20px;
  background: #f8fafc;
  border-bottom: 1px solid #eef2f6;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.notification-title {
  font-size: 17px;
  font-weight: 700;
  color: #0f172a;
}

.notification-count {
  background: #e2e8f0;
  color: #475569;
  font-size: 11px;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: 12px;
}

.mark-all-read {
  background: none;
  border: none;
  color: #667eea;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  padding: 6px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.mark-all-read:hover {
  background: rgba(102, 126, 234, 0.08);
}

.notification-list {
  max-height: 400px;
  overflow-y: auto;
  padding: 4px 0;
}

.notification-list::-webkit-scrollbar {
  width: 4px;
}

.notification-list::-webkit-scrollbar-track {
  background: transparent;
}

.notification-list::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 2px;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  gap: 14px;
  padding: 14px 20px;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
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

.notification-item:not(:last-child)::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 20px;
  right: 20px;
  height: 1px;
  background: #f1f5f9;
}

.notification-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-top: 2px;
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
  min-width: 0;
  text-align: left;
}

.notification-title-text {
  font-weight: 600;
  font-size: 14px;
  color: #0f172a;
  margin-bottom: 4px;
  text-align: left;
}

.notification-message {
  font-size: 13px;
  color: #64748b;
  margin-bottom: 6px;
  line-height: 1.5;
  text-align: left;
  word-wrap: break-word;
  white-space: normal;
  overflow: visible;
  display: block;
}

.notification-time {
  font-size: 11px;
  color: #94a3b8;
  display: flex;
  align-items: center;
  gap: 4px;
  text-align: left;
}

.notification-time::before {
  content: '•';
  font-size: 14px;
}

.notification-status {
  flex-shrink: 0;
  padding-top: 4px;
}

.dot {
  display: inline-block;
  width: 8px;
  height: 8px;
  background: #3b82f6;
  border-radius: 50%;
  animation: blink 1.5s infinite;
}

@keyframes blink {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.3;
  }
}

.empty-notifications {
  text-align: center;
  padding: 60px 20px;
  color: #94a3b8;
}

.empty-notifications svg {
  opacity: 0.5;
  margin-bottom: 16px;
}

.empty-notifications p {
  font-size: 14px;
  font-weight: 500;
  color: #64748b;
}

.notification-footer {
  padding: 14px 20px;
  background: #f8fafc;
  border-top: 1px solid #eef2f6;
}

.view-all-btn {
  width: 100%;
  background: none;
  border: none;
  color: #667eea;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  padding: 10px;
  border-radius: 10px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.view-all-btn:hover {
  background: rgba(102, 126, 234, 0.08);
}

.view-all-btn svg {
  transition: transform 0.2s ease;
}

.view-all-btn:hover svg {
  transform: translateX(4px);
}

/* Transition */
.notification-fade-enter-active,
.notification-fade-leave-active {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

.notification-fade-enter-from,
.notification-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px) scale(0.98);
}

/* Responsive */
@media (max-width: 480px) {
  .notification-dropdown {
    position: fixed;
    top: 70px;
    right: 12px;
    left: 12px;
    width: auto;
    max-width: none;
  }
  
  .notification-header {
    padding: 14px 16px;
  }
  
  .notification-item {
    padding: 12px 16px;
  }
  
  .notification-title {
    font-size: 15px;
  }
}
</style>