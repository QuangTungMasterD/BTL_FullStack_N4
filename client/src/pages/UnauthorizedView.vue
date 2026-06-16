<template>
  <div class="unauthorized-container">
    <div class="content">
      <div class="icon">
        <svg width="120" height="120" viewBox="0 0 24 24" fill="none">
          <path d="M12 15V17M12 7V13M12 21C16.9706 21 21 16.9706 21 12C21 7.02944 16.9706 3 12 3C7.02944 3 3 7.02944 3 12C3 16.9706 7.02944 21 12 21Z" stroke="currentColor" stroke-width="2"/>
          <circle cx="12" cy="17" r="1" fill="currentColor"/>
        </svg>
      </div>
      <h1>403 - Không có quyền truy cập</h1>
      <p>Bạn không có quyền truy cập vào trang này.</p>
      <p>Vui lòng liên hệ quản trị viên nếu bạn cho rằng đây là lỗi.</p>
      <button class="back-btn" @click="goBack">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
          <path d="M10 19L3 12M3 12L10 5M3 12H21" stroke="currentColor" stroke-width="2"/>
        </svg>
        Quay lại trang chủ
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const goBack = () => {
  const role = authStore.user?.role
  if (role === 'ADMIN') router.push('/')
  else if (role === 'LECTURER') router.push('/lecturer-dashboard')
  else router.push('/student-dashboard')
}
</script>

<style scoped>
.unauthorized-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 80vh;
  padding: 40px;
}

.content {
  text-align: center;
  max-width: 500px;
}

.icon {
  margin-bottom: 32px;
  color: #ef4444;
}

h1 {
  font-size: 32px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 16px;
}

p {
  color: #64748b;
  margin-bottom: 8px;
}

.back-btn {
  margin-top: 32px;
  padding: 12px 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.back-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.3);
}
</style>