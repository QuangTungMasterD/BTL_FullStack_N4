<template>
  <header class="sticky top-0 z-50 bg-white/95 backdrop-blur-md border-b border-gray-100 shadow-sm">
    <div class="container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Logo -->
        <router-link to="/" class="flex items-center gap-2 text-xl font-bold text-primary">
          <div class="w-8 h-8 bg-gradient-to-br from-primary to-purple-600 rounded-lg flex items-center justify-center text-white">
            <span class="material-symbols-outlined text-2xl">school</span>
          </div>
          <span>EduTrack</span>
        </router-link>

        <!-- Menu chính (Desktop) -->
        <nav class="hidden md:flex items-center gap-1 text-sm font-medium text-gray-700">
          <router-link to="/" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Trang chủ</router-link>
          <router-link to="/about" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Giới thiệu</router-link>
          <router-link to="/courses" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Khóa học</router-link>
          <router-link to="/contact" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Liên hệ</router-link>
        </nav>

        <!-- Hành động: Đăng nhập / Đăng ký hoặc User info -->
        <div class="flex items-center gap-3">
          <!-- Khi chưa đăng nhập: nút đăng nhập / đăng ký -->
          <template v-if="!isAuthenticated">
            <router-link to="/login" class="hidden sm:inline-block text-sm font-medium text-gray-700 hover:text-primary transition-colors">
              Đăng nhập
            </router-link>
            <router-link to="/login?tab=register" class="px-4 py-2 bg-primary text-white text-sm font-semibold rounded-lg hover:bg-primary/90 transition-all shadow-md hover:shadow-lg">
              Đăng ký
            </router-link>
          </template>

          <!-- Khi đã đăng nhập: role badge và user menu -->
          <template v-else>
            <div class="role-badge" :class="userRole?.toLowerCase()">
              <span>{{ userRole }}</span>
            </div>

            <div class="user-menu" @click="toggleUserMenu">
              <div class="user-avatar">
                <span>{{ userInitial }}</span>
              </div>
              <div class="user-info">
                <span class="user-name">{{ user?.fullName || 'Người dùng' }}</span>
              </div>
              <div v-if="showUserMenu" class="user-dropdown" @click.stop>
                <div class="dropdown-header">
                  <div class="dropdown-avatar">{{ userInitial }}</div>
                  <div class="dropdown-info">
                    <div class="dropdown-name">{{ user?.fullName || 'Người dùng' }}</div>
                    <div class="dropdown-email">{{ user?.email }}</div>
                  </div>
                </div>
                <div class="dropdown-divider"></div>
                <div class="dropdown-item" @click="goToProfile">
                  <span class="material-symbols-outlined">account_circle</span>
                  Thông tin cá nhân
                </div>
                <div class="dropdown-divider"></div>
                <div class="dropdown-item logout" @click="logout">
                  <span class="material-symbols-outlined">logout</span>
                  Đăng xuất
                </div>
              </div>
            </div>
          </template>

          <!-- Nút menu mobile (vẫn giữ nguyên) -->
          <button @click="toggleMobileMenu" class="md:hidden p-2 rounded-lg hover:bg-gray-100">
            <span class="material-symbols-outlined">menu</span>
          </button>
        </div>
      </div>

      <!-- Menu mobile (dropdown) - giữ nguyên -->
      <div v-if="mobileMenuOpen" class="md:hidden py-4 border-t border-gray-100">
        <nav class="flex flex-col gap-3 text-base font-medium text-gray-700">
          <router-link to="/" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Trang chủ</router-link>
          <router-link to="/about" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Giới thiệu</router-link>
          <router-link to="/courses" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Khóa học</router-link>
          <router-link to="/contact" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Liên hệ</router-link>
          <div class="border-t border-gray-100 my-2 pt-2">
            <router-link v-if="!isAuthenticated" to="/login" class="block px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Đăng nhập</router-link>
            <button v-else class="block px-2 py-2 hover:bg-gray-50 rounded-lg w-full text-left" @click="logout">Đăng xuất</button>
          </div>
        </nav>
      </div>

      <!-- Dropdown user menu (khi đã đăng nhập) -->
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const mobileMenuOpen = ref(false)
const showUserMenu = ref(false)

const isAuthenticated = computed(() => authStore.isAuthenticated)
const user = computed(() => authStore.user)
const userRole = computed(() => authStore.user?.role)

const userInitial = computed(() => {
  if (user.value?.fullName) {
    return user.value.fullName.charAt(0).toUpperCase()
  }
  return 'U'
})

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
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

const handleClickOutside = (event) => {
  if (!event.target.closest('.user-menu') && !event.target.closest('.user-dropdown')) {
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
/* Chỉ thêm style cho role-badge và user-menu, user-dropdown */
.role-badge {
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.5px;
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
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 4px 8px;
  border-radius: 30px;
  cursor: pointer;
  transition: background 0.2s;
}
.user-menu:hover {
  background: #f1f5f9;
}
.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 14px;
}
.user-name {
  font-weight: 500;
  font-size: 14px;
}

.user-dropdown {
  position: absolute;
  right: 0;
  top: 100%;
  margin-top: 8px;
  width: 260px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 12px 28px rgba(0,0,0,0.12);
  overflow: hidden;
  z-index: 50;
}
.dropdown-header {
  padding: 16px 20px;
  display: flex;
  gap: 12px;
  background: #f8fafc;
}
.dropdown-avatar {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 18px;
}
.dropdown-info {
  flex: 1;
}
.dropdown-name {
  font-weight: 600;
  color: #1e293b;
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
  transition: all 0.2s;
  color: #475569;
}
.dropdown-item:hover {
  background: #f1f5f9;
  color: #3b82f6;
}
.dropdown-item.logout:hover {
  color: #ef4444;
}
.dropdown-item .material-symbols-outlined {
  font-size: 20px;
}
</style>