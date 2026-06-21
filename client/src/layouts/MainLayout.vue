<template>
  <div class="app-layout">
    <!-- Sidebar -->
    <aside :class="['sidebar', { collapsed: isSidebarCollapsed }]">
      <div class="sidebar-header">
        <div class="logo-area" @click="toggleSidebar">
          <div class="logo-icon">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none">
              <path d="M12 3L1 9L12 15L21 10.5V17" stroke="currentColor" stroke-width="2" fill="none"/>
              <path d="M12 15V21M12 21L9 18M12 21L15 18" stroke="currentColor" stroke-width="2"/>
              <circle cx="12" cy="9" r="2" fill="currentColor"/>
            </svg>
          </div>
          <div class="logo-text" v-show="!isSidebarCollapsed">
            <span class="logo-title">EduTrack</span>
            <!-- <span class="logo-subtitle">Academic System</span> -->
          </div>
        </div>
        <button class="collapse-btn" @click="toggleSidebar">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
            <path d="M15 18L9 12L15 6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
          </svg>
        </button>
      </div>

      <nav class="sidebar-nav">
        <div 
          v-for="item in menuItems" 
          :key="item.path"
          class="nav-item"
          :class="{ active: isActive(item.path) }"
          @click="navigate(item.path)"
        >
          <div class="nav-icon">
            <v-icon :icon="item.icon" size="20" />
          </div>
          <span class="nav-label" v-show="!isSidebarCollapsed">{{ item.title }}</span>
          <div v-if="isActive(item.path)" class="active-indicator"></div>
        </div>
      </nav>

      <div class="sidebar-footer">
        <div class="nav-item" @click="logout">
          <div class="nav-icon">
            <v-icon icon="mdi-logout" size="20" />
          </div>
          <span class="nav-label" v-show="!isSidebarCollapsed">Đăng xuất</span>
        </div>
      </div>
    </aside>

    <!-- Main Content Area -->
    <div class="main-container">
      <!-- Topbar -->
      <header class="topbar">
        <div class="topbar-left">
          <button class="mobile-menu-btn" @click="toggleSidebar">
            <v-icon icon="mdi-menu" size="24" />
          </button>
          <div class="search-box">
            <v-icon icon="mdi-magnify" size="18" />
            <input type="text" placeholder="Tìm kiếm khóa học, sinh viên...">
          </div>
        </div>
        <div class="topbar-right">
          <!-- Notification Panel Component -->
          <NotificationPanel />
          
          <!-- Role Badge -->
          <div class="role-badge" :class="userRole?.toLowerCase()">
            <span>{{ userRole }}</span>
          </div>
          
          <div class="user-menu" @click="toggleUserMenu">
            <div class="user-avatar">
              <span>{{ userInitial }}</span>
            </div>
            <div class="user-info" v-show="!isSidebarCollapsed">
              <span class="user-name">{{ user?.fullName || 'Người dùng' }}</span>
              <!-- <span class="user-role">{{ userRole }}</span> -->
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="page-content">
        <transition name="fade-slide" mode="out-in">
          <router-view />
        </transition>
      </main>
    </div>

    <!-- User Dropdown Menu -->
    <transition name="dropdown">
      <div v-if="showUserMenu" class="dropdown-menu" @click.stop>
        <div class="dropdown-header">
          <div class="dropdown-avatar">
            <span>{{ userInitial }}</span>
          </div>
          <div class="dropdown-info">
            <div class="dropdown-name">{{ user?.fullName || 'Người dùng' }}</div>
            <div class="dropdown-email">{{ user?.email }}</div>
          </div>
        </div>
        <div class="dropdown-divider"></div>
        <div class="dropdown-item" @click="goToProfile">
          <v-icon icon="mdi-account" size="18" />
          <span>Thông tin cá nhân</span>
        </div>
        <div class="dropdown-divider"></div>
        <div class="dropdown-item logout" @click="logout">
          <v-icon icon="mdi-logout" size="18" />
          <span>Đăng xuất</span>
        </div>
      </div>
    </transition>
    <ToastContainer />
    <GlobalErrorWatcher />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import NotificationPanel from '@/components/NotificationPanel.vue'
import ToastContainer from '@/components/ui/ToastContainer.vue';
import GlobalErrorWatcher from '@/components/ui/GlobalErrorWatcher.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const isSidebarCollapsed = ref(false)
const showUserMenu = ref(false)

const user = computed(() => authStore.user)
const userRole = computed(() => authStore.user?.role)

const userInitial = computed(() => {
  if (user.value?.fullName) {
    return user.value.fullName.charAt(0).toUpperCase()
  }
  return 'U'
})

// Dynamic menu items based on role
const menuItems = computed(() => {
  const role = userRole.value
  
  if (role === 'ADMIN') {
    return [
      { title: 'Dashboard', path: '/', icon: 'mdi-view-dashboard' },
      { title: 'Quản lý sinh viên', path: '/students', icon: 'mdi-account-group' },
      { title: 'Quản lý giảng viên', path: '/teachers', icon: 'mdi-school' },
      { title: 'Quản lý chuyên ngành', path: '/specializations', icon: 'mdi-book-open-variant' },
      { title: 'Quản lý khóa học', path: '/admin-courses', icon: 'mdi-book-open-variant' },
      { title: 'Quản lý lớp học', path: '/classes', icon: 'mdi-book-open-variant' },
      { title: 'Quản lý điểm danh', path: '/admin-attendance', icon: 'mdi-calendar-check' },
      { title: 'Quản lý điểm số', path: '/admin-grades', icon: 'mdi-chart-line' },
      { title: 'Yêu cầu lịch', path: '/admin/schedule-requests', icon: 'mdi-account' },
      { title: 'Báo cáo thống kê', path: '/reports', icon: 'mdi-file-chart' },
      { title: 'Thông tin cá nhân', path: '/profile', icon: 'mdi-account' },
    ]
  } else if (role === 'LECTURER') {
    return [
      { title: 'Dashboard', path: '/lecturer-dashboard', icon: 'mdi-view-dashboard' },
      { title: 'Sinh viên của tôi', path: '/my-students', icon: 'mdi-account-group' },
      { title: 'Điểm danh', path: '/lecturer-attendance', icon: 'mdi-calendar-check' },
      { title: 'Điểm số', path: '/lecturer-grades', icon: 'mdi-chart-line' },
      { title: 'Thông tin cá nhân', path: '/profile', icon: 'mdi-account' },
      { title: 'Lịch trình', path: '/teachers/schedule', icon: 'mdi-schedule' },
    ]
  } else {
    return [
      { title: 'Dashboard', path: '/student-dashboard', icon: 'mdi-view-dashboard' },
      { title: 'Thông tin cá nhân', path: '/profile', icon: 'mdi-account' },
      { title: 'Lịch học', path: 'student/schedule', icon: 'mdi-account' },
      { title: 'Điểm danh', path: '/student-attendance', icon: 'mdi-calendar-check' },
      { title: 'Điểm số', path: '/student-grades', icon: 'mdi-chart-line' },
      { title: 'Đăng ký khóa học', path: '/enrollments', icon: 'mdi-book-open-variant' },
    ]
  }
})

const isActive = (path) => {
  if (path === '/' || path === '/lecturer-dashboard' || path === '/student-dashboard') {
    return route.path === path
  }
  return route.path.startsWith(path)
}

const navigate = (path) => {
  router.push(path)
}

const toggleSidebar = () => {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
}

const toggleUserMenu = () => {
  showUserMenu.value = !showUserMenu.value
}

const goToProfile = () => {
  showUserMenu.value = false
  router.push('/profile')
}

const logout = () => {
  showUserMenu.value = false
  authStore.logout()
  router.push('/login')
}

// Close dropdown when clicking outside
const handleClickOutside = (event) => {
  if (!event.target.closest('.user-menu') && !event.target.closest('.dropdown-menu')) {
    showUserMenu.value = false
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
.app-layout {
  display: flex;
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #e9eef3 100%);
}

/* Sidebar Styles */
.sidebar {
  position: fixed;
  left: 0;
  top: 0;
  width: 280px;
  height: 100vh;
  background: linear-gradient(180deg, #0F172A 0%, #1E293B 100%);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1000;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.sidebar.collapsed {
  width: 80px;
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.logo-area {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
}

.logo-icon {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #3B82F6, #8B5CF6);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.logo-title {
  font-size: 18px;
  font-weight: 700;
  color: white;
}

.logo-subtitle {
  font-size: 10px;
  color: rgba(255, 255, 255, 0.6);
}

.collapse-btn {
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 8px;
  width: 32px;
  height: 32px;
  cursor: pointer;
  color: white;
  transition: all 0.3s ease;
}

.collapse-btn:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: scale(1.05);
}

.sidebar-nav {
  flex: 1;
  padding: 20px 12px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: rgba(255, 255, 255, 0.7);
  position: relative;
}

.nav-item:hover {
  background: rgba(255, 255, 255, 0.08);
  color: white;
  transform: translateX(4px);
}

.nav-item.active {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.2), rgba(139, 92, 246, 0.2));
  color: #3B82F6;
}

.nav-icon {
  width: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-label {
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
}

.active-indicator {
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 20px;
  background: linear-gradient(135deg, #3B82F6, #8B5CF6);
  border-radius: 3px;
}

.sidebar-footer {
  padding: 20px 12px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

/* Main Container */
.main-container {
  flex: 1;
  margin-left: 280px;
  transition: margin-left 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.sidebar.collapsed ~ .main-container {
  margin-left: 80px;
}

/* Topbar */
.topbar {
  position: sticky;
  top: 0;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  padding: 12px 32px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
  z-index: 99;
}

.topbar-left {
  display: flex;
  align-items: center;
  gap: 24px;
}

.mobile-menu-btn {
  display: none;
  background: none;
  border: none;
  cursor: pointer;
  color: #1e293b;
  padding: 8px;
  border-radius: 8px;
}

.mobile-menu-btn:hover {
  background: rgba(0, 0, 0, 0.05);
}

.search-box {
  display: flex;
  align-items: center;
  gap: 12px;
  background: white;
  padding: 8px 18px;
  border-radius: 40px;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
}

.search-box:focus-within {
  border-color: #3B82F6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  transform: scale(1.02);
}

.search-box input {
  border: none;
  outline: none;
  font-size: 14px;
  width: 250px;
  background: transparent;
}

.topbar-right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.role-badge {
  padding: 6px 14px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 600;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.role-badge.admin {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
  color: white;
}

.role-badge.lecturer {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
}

.role-badge.student {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 6px 12px 6px 8px;
  border-radius: 40px;
  cursor: pointer;
  background: white;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
}

.user-menu:hover {
  background: #f8fafc;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.user-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #3B82F6, #8B5CF6);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
}

.user-info {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 14px;
  font-weight: 600;
  color: #1e293b;
}

.user-role {
  font-size: 10px;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Page Content */
.page-content {
  padding: 32px;
  flex: 1;
}

/* Dropdown Menu */
.dropdown-menu {
  position: fixed;
  top: 70px;
  right: 32px;
  width: 280px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
  z-index: 1000;
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

.dropdown-header {
  padding: 16px 20px;
  display: flex;
  gap: 12px;
  background: #f8fafc;
}

.dropdown-avatar {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #3B82F6, #8B5CF6);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 18px;
}

.dropdown-info {
  flex: 1;
}

.dropdown-name {
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 4px;
}

.dropdown-email {
  font-size: 12px;
  color: #64748b;
}

.dropdown-divider {
  height: 1px;
  background: #e2e8f0;
}

.dropdown-item {
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #475569;
}

.dropdown-item:hover {
  background: #f1f5f9;
  color: #3B82F6;
  padding-left: 24px;
}

.dropdown-item.logout:hover {
  color: #ef4444;
}

/* Transitions */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Responsive */
@media (max-width: 768px) {
  .mobile-menu-btn {
    display: flex;
  }
  
  .sidebar {
    transform: translateX(-100%);
  }
  
  .sidebar.open {
    transform: translateX(0);
  }
  
  .main-container {
    margin-left: 0 !important;
  }
  
  .search-box {
    display: none;
  }
  
  .page-content {
    padding: 20px;
  }
  
  .role-badge {
    display: none;
  }
}

@media (max-width: 480px) {
  .topbar {
    padding: 12px 16px;
  }
  
  .user-info {
    display: none;
  }
  
  .dropdown-menu {
    right: 16px;
    width: 260px;
  }
}
</style>