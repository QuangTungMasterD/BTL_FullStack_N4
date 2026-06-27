<template>
  <div class="login-container">
    <!-- Background Effects -->
    <div class="background-effect">
      <div class="gradient-bg"></div>
      <div class="animated-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
        <div class="shape shape-3"></div>
        <div class="shape shape-4"></div>
      </div>
    </div>

    <!-- Content -->
    <div class="content-wrapper">
      <!-- Left Brand Section - Centered -->
      <div class="brand-section">
        <div class="brand-content">
          <div class="logo">
            <div class="logo-icon">
              <svg width="36" height="36" viewBox="0 0 24 24" fill="none">
                <path d="M12 3L1 9L12 15L21 10.5V17" stroke="currentColor" stroke-width="2" fill="none"/>
                <path d="M12 15V21M12 21L9 18M12 21L15 18" stroke="currentColor" stroke-width="2"/>
                <circle cx="12" cy="9" r="2" fill="currentColor"/>
              </svg>
            </div>
            <div class="logo-text">
              <span class="logo-title">EduTrack</span>
              <span class="logo-subtitle">Academic Management System</span>
            </div>
          </div>

          <h1 class="main-title">
            Hệ Thống Quản Lý
            <span class="highlight">Điểm Danh</span>
            Học Viên
          </h1>
          
          <p class="description">
            Nền tảng quản lý giáo dục toàn diện, giúp theo dõi điểm danh, 
            đánh giá kết quả học tập và quản lý khóa học hiệu quả.
          </p>

          <div class="stats">
            <div class="stat-item">
              <div class="stat-number">10,000+</div>
              <div class="stat-label">Học viên</div>
            </div>
            <div class="stat-item">
              <div class="stat-number">500+</div>
              <div class="stat-label">Khóa học</div>
            </div>
            <div class="stat-item">
              <div class="stat-number">98%</div>
              <div class="stat-label">Hài lòng</div>
            </div>
          </div>

          <div class="features">
            <div class="feature">
              <div class="feature-check">✓</div>
              <span>Điểm danh thông minh</span>
            </div>
            <div class="feature">
              <div class="feature-check">✓</div>
              <span>Theo dõi tiến độ học tập</span>
            </div>
            <div class="feature">
              <div class="feature-check">✓</div>
              <span>Báo cáo chi tiết</span>
            </div>
            <div class="feature">
              <div class="feature-check">✓</div>
              <span>Tích hợp AI</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Auth Section - Centered -->
      <div class="auth-section">
        <div class="auth-card">
          <router-link
            to="/"
            class="absolute w-[60px] h-[60px] top-4 left-4 p-2 rounded-full hover:bg-gray-200 bg-gray-100 transition-colors z-10 flex items-center justify-center"
            title="Quay lại trang chủ"
          >
            <span class="material-symbols-outlined text-gray-800 hover:text-primary">arrow_back</span>
          </router-link>
          <!-- Tab switching -->
          <div class="auth-tabs">
            <button 
              :class="['tab-btn', { active: activeTab === 'login' }]"
              @click="activeTab = 'login'"
            >
              Đăng nhập
            </button>
            <button 
              :class="['tab-btn', { active: activeTab === 'register' }]"
              @click="activeTab = 'register'"
            >
              Đăng ký
            </button>
          </div>

          <!-- Login Form -->
          <div v-if="activeTab === 'login'" class="auth-form">
            <div class="form-header">
              <h2>Chào mừng trở lại</h2>
              <p>Nhập thông tin để đăng nhập vào hệ thống</p>
            </div>

            <form @submit.prevent="handleLogin">
              <div class="input-group">
                <label>Email</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" stroke-width="2"/>
                    <path d="M22 6L12 13L2 6" stroke="currentColor" stroke-width="2"/>
                  </svg>
                  <input 
                    v-model="loginForm.email" 
                    type="email" 
                    placeholder="example@email.com"
                    required
                  >
                </div>
              </div>

              <div class="input-group">
                <label>Mật khẩu</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M19 11H5C3.9 11 3 11.9 3 13V20C3 21.1 3.9 22 5 22H19C20.1 22 21 21.1 21 20V13C21 11.9 20.1 11 19 11Z" stroke="currentColor" stroke-width="2"/>
                    <path d="M7 11V7C7 4.24 9.24 2 12 2C14.76 2 17 4.24 17 7V11" stroke="currentColor" stroke-width="2"/>
                    <circle cx="12" cy="16" r="1.5" fill="currentColor"/>
                  </svg>
                  <input 
                    :type="showPassword ? 'text' : 'password'" 
                    v-model="loginForm.password" 
                    placeholder="••••••••"
                    required
                  >
                  <button type="button" class="toggle-password" @click="showPassword = !showPassword">
                    {{ showPassword ? '🙈' : '👁️' }}
                  </button>
                </div>
              </div>

              <div class="form-options">
                <label class="checkbox">
                  <input type="checkbox" v-model="rememberMe">
                  <span>Ghi nhớ đăng nhập</span>
                </label>
                <a href="#" class="forgot-link">Quên mật khẩu?</a>
              </div>

              <button type="submit" class="submit-btn" :disabled="loading">
                <span v-if="!loading">Đăng nhập</span>
                <span v-else>Đang xử lý...</span>
              </button>

              <div v-if="loginError" class="error-message">
                {{ loginError }}
              </div>
            </form>
          </div>

          <!-- Register Form -->
          <div v-if="activeTab === 'register'" class="auth-form">
            <div class="form-header">
              <h2>Tạo tài khoản mới</h2>
              <p>Đăng ký để trải nghiệm hệ thống</p>
            </div>

            <form @submit.prevent="handleRegister">
              <div class="input-group">
                <label>Họ và tên</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M20 21V19C20 16.8 18.2 15 16 15H8C5.8 15 4 16.8 4 19V21" stroke="currentColor" stroke-width="2"/>
                    <circle cx="12" cy="7" r="4" stroke="currentColor" stroke-width="2"/>
                  </svg>
                  <input 
                    v-model="registerForm.fullName" 
                    type="text" 
                    placeholder="Nguyễn Văn A"
                    required
                  >
                </div>
              </div>

              <div class="input-group">
                <label>Email</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" stroke-width="2"/>
                    <path d="M22 6L12 13L2 6" stroke="currentColor" stroke-width="2"/>
                  </svg>
                  <input 
                    v-model="registerForm.email" 
                    type="email" 
                    placeholder="example@email.com"
                    required
                  >
                </div>
              </div>

              <div class="input-group">
                <label>Mật khẩu</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M19 11H5C3.9 11 3 11.9 3 13V20C3 21.1 3.9 22 5 22H19C20.1 22 21 21.1 21 20V13C21 11.9 20.1 11 19 11Z" stroke="currentColor" stroke-width="2"/>
                    <path d="M7 11V7C7 4.24 9.24 2 12 2C14.76 2 17 4.24 17 7V11" stroke="currentColor" stroke-width="2"/>
                  </svg>
                  <input 
                    :type="showRegisterPassword ? 'text' : 'password'" 
                    v-model="registerForm.password" 
                    placeholder="••••••••"
                    required
                  >
                  <button type="button" class="toggle-password" @click="showRegisterPassword = !showRegisterPassword">
                    {{ showRegisterPassword ? '🙈' : '👁️' }}
                  </button>
                </div>
              </div>

              <div class="input-group">
                <label>Xác nhận mật khẩu</label>
                <div class="input-wrapper">
                  <svg class="input-icon" width="18" height="18" viewBox="0 0 24 24" fill="none">
                    <path d="M19 11H5C3.9 11 3 11.9 3 13V20C3 21.1 3.9 22 5 22H19C20.1 22 21 21.1 21 20V13C21 11.9 20.1 11 19 11Z" stroke="currentColor" stroke-width="2"/>
                    <path d="M9 11V7C9 4.24 11.24 2 14 2C16.76 2 19 4.24 19 7V11" stroke="currentColor" stroke-width="2"/>
                  </svg>
                  <input 
                    :type="showConfirmPassword ? 'text' : 'password'" 
                    v-model="registerForm.confirmPassword" 
                    placeholder="••••••••"
                    required
                  >
                  <button type="button" class="toggle-password" @click="showConfirmPassword = !showConfirmPassword">
                    {{ showConfirmPassword ? '🙈' : '👁️' }}
                  </button>
                </div>
              </div>

              <div class="form-options">
                <label class="checkbox">
                  <input type="checkbox" v-model="agreeTerms" required>
                  <span>Tôi đồng ý với <a href="#">Điều khoản sử dụng</a></span>
                </label>
              </div>

              <button type="submit" class="submit-btn" :disabled="registerLoading">
                <span v-if="!registerLoading">Đăng ký</span>
                <span v-else>Đang xử lý...</span>
              </button>

              <div v-if="registerError" class="error-message">
                {{ registerError }}
              </div>
            </form>
          </div>

          <div class="auth-footer">
            <p>© 2024 EduTrack. All rights reserved.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import api from '@/utils/api'

const router = useRouter()
const authStore = useAuthStore()

// Tab state
const activeTab = ref('login')

// Login form
const loginForm = ref({ email: '', password: '' })
const showPassword = ref(false)
const rememberMe = ref(false)
const loading = ref(false)
const loginError = ref('')

// Register form
const registerForm = ref({ 
  fullName: '', 
  email: '', 
  password: '', 
  confirmPassword: '' 
})
const showRegisterPassword = ref(false)
const showConfirmPassword = ref(false)
const registerLoading = ref(false)
const registerError = ref('')
const agreeTerms = ref(false)

// LOGIN WITH REAL API
const handleLogin = async () => {
  loginError.value = ''
  loading.value = true
  
  try {
    const response = await api.post('/auth/login', {
      email: loginForm.value.email,
      password: loginForm.value.password,
    })
    
    console.log('Login response:', response.data)
    
    const { token, userId, fullName, role, email } = response.data
    
    authStore.setToken(token, { 
      userId, 
      email: email || loginForm.value.email, 
      fullName, 
      role 
    })
    
    if (rememberMe.value) {
      localStorage.setItem('rememberMe', 'true')
    }
    
    if (role === 'ADMIN') {
      router.push('/')
    } else if (role === 'LECTURER') {
      router.push('/lecturer-dashboard')
    } else {
      router.push('/student-dashboard')
    }
    
  } catch (err) {
    console.error('Login error:', err)
    if (err.response) {
      loginError.value = err.response.data?.message || 'Đăng nhập thất bại'
    } else if (err.request) {
      loginError.value = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối.'
    } else {
      loginError.value = 'Đăng nhập thất bại. Vui lòng thử lại.'
    }
  } finally {
    loading.value = false
  }
}

// REGISTER WITH REAL API
const handleRegister = async () => {
  registerError.value = ''
  
  if (!registerForm.value.fullName.trim()) {
    registerError.value = 'Vui lòng nhập họ tên'
    return
  }
  
  if (!registerForm.value.email.trim()) {
    registerError.value = 'Vui lòng nhập email'
    return
  }
  
  if (!registerForm.value.password) {
    registerError.value = 'Vui lòng nhập mật khẩu'
    return
  }
  
  if (registerForm.value.password !== registerForm.value.confirmPassword) {
    registerError.value = 'Mật khẩu xác nhận không khớp'
    return
  }
  
  if (registerForm.value.password.length < 6) {
    registerError.value = 'Mật khẩu phải có ít nhất 6 ký tự'
    return
  }
  
  registerLoading.value = true
  
  try {
    const response = await api.post('/auth/register', {
      fullName: registerForm.value.fullName,
      email: registerForm.value.email,
      password: registerForm.value.password,
    })
    
    console.log('Register response:', response.data)
    
    loginForm.value = { 
      email: registerForm.value.email, 
      password: registerForm.value.password 
    }
    activeTab.value = 'login'
    registerError.value = 'Đăng ký thành công! Vui lòng đăng nhập.'
    
    registerForm.value = { fullName: '', email: '', password: '', confirmPassword: '' }
    agreeTerms.value = false
    
  } catch (err) {
    console.error('Register error:', err)
    if (err.response) {
      registerError.value = err.response.data?.message || 'Đăng ký thất bại'
    } else if (err.request) {
      registerError.value = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối.'
    } else {
      registerError.value = 'Đăng ký thất bại. Vui lòng thử lại.'
    }
  } finally {
    registerLoading.value = false
  }
}
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.login-container {
  width: 100vw;
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  overflow: hidden;
}

/* Background Effects */
.background-effect {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
}

.gradient-bg {
  position: absolute;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
  background-size: 200% 200%;
  animation: gradientMove 10s ease infinite;
}

@keyframes gradientMove {
  0% { background-position: 0% 0%; }
  50% { background-position: 100% 100%; }
  100% { background-position: 0% 0%; }
}

.animated-shapes {
  position: absolute;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.shape {
  position: absolute;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.08);
  animation: float 20s infinite ease-in-out;
}

.shape-1 {
  width: 300px;
  height: 300px;
  top: -100px;
  right: -100px;
}

.shape-2 {
  width: 200px;
  height: 200px;
  bottom: -50px;
  left: -50px;
  animation-delay: -5s;
}

.shape-3 {
  width: 150px;
  height: 150px;
  top: 60%;
  right: 15%;
  animation-delay: -10s;
}

.shape-4 {
  width: 180px;
  height: 180px;
  bottom: 25%;
  left: 10%;
  animation-delay: -15s;
}

@keyframes float {
  0%, 100% { transform: translateY(0) rotate(0deg); }
  50% { transform: translateY(-30px) rotate(180deg); }
}

/* Content Layout - Center both sides */
.content-wrapper {
  position: relative;
  z-index: 1;
  display: flex;
  width: 100%;
  height: 100%;
}

/* Left Brand Section - Centered content */
.brand-section {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.25);
}

.brand-content {
  width: 100%;
  max-width: 480px;
  padding: 20px;
  color: white;
}

.logo {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 40px;
}

.logo-icon {
  width: 48px;
  height: 48px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.logo-text {
  display: flex;
  flex-direction: column;
}

.logo-title {
  font-size: 24px;
  font-weight: bold;
}

.logo-subtitle {
  font-size: 11px;
  opacity: 0.75;
}

.main-title {
  font-size: 36px;
  font-weight: 700;
  line-height: 1.3;
  margin-bottom: 20px;
}

.highlight {
  color: #fbbf24;
}

.description {
  font-size: 14px;
  opacity: 0.9;
  line-height: 1.5;
  margin-bottom: 32px;
}

.stats {
  display: flex;
  gap: 40px;
  margin-bottom: 32px;
  padding: 20px 0;
  border-top: 1px solid rgba(255, 255, 255, 0.15);
  border-bottom: 1px solid rgba(255, 255, 255, 0.15);
}

.stat-number {
  font-size: 26px;
  font-weight: bold;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 12px;
  opacity: 0.8;
}

.features {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.feature {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}

.feature-check {
  width: 18px;
  height: 18px;
  background: #4caf50;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
}

/* Right Auth Section - Centered content */
.auth-section {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.96);
  backdrop-filter: blur(10px);
  padding: 20px;
  overflow-y: auto;
}

.auth-card {
  width: 100%;
  max-width: 420px;
  padding: 24px 28px;
}

.auth-tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 28px;
  background: #f1f5f9;
  padding: 6px;
  border-radius: 50px;
}

.tab-btn {
  flex: 1;
  padding: 10px 24px;
  border: none;
  background: transparent;
  font-size: 15px;
  font-weight: 600;
  border-radius: 50px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #64748b;
}

.tab-btn.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.auth-form {
  animation: fadeIn 0.4s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.form-header {
  text-align: center;
  margin-bottom: 24px;
}

.form-header h2 {
  font-size: 24px;
  color: #1e293b;
  margin-bottom: 4px;
}

.form-header p {
  color: #64748b;
  font-size: 13px;
}

.input-group {
  margin-bottom: 16px;
}

.input-group label {
  display: block;
  margin-bottom: 5px;
  font-size: 13px;
  font-weight: 600;
  color: #334155;
  text-align: left;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 14px;
  color: #94a3b8;
  pointer-events: none;
  flex-shrink: 0;
}

.input-wrapper input {
  width: 100%;
  padding: 11px 42px 11px 42px;
  border: 1.5px solid #e2e8f0;
  border-radius: 12px;
  font-size: 14px;
  transition: all 0.3s ease;
  background: white;
  text-align: left;
}

.input-wrapper input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.08);
}

.input-wrapper input::placeholder {
  text-align: left;
}

.toggle-password {
  position: absolute;
  right: 14px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 18px;
  opacity: 0.6;
  padding: 4px;
}

.toggle-password:hover {
  opacity: 1;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  font-size: 13px;
}

.checkbox {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: #475569;
}

.checkbox input {
  width: 15px;
  height: 15px;
  cursor: pointer;
  flex-shrink: 0;
}

.forgot-link {
  color: #667eea;
  text-decoration: none;
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
}

.forgot-link:hover {
  text-decoration: underline;
}

.submit-btn {
  width: 100%;
  padding: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(102, 126, 234, 0.3);
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error-message {
  margin-top: 16px;
  padding: 10px;
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 10px;
  color: #dc2626;
  font-size: 13px;
  text-align: center;
}

.auth-footer {
  margin-top: 24px;
  text-align: center;
  padding-top: 16px;
  border-top: 1px solid #e2e8f0;
}

.auth-footer p {
  font-size: 11px;
  color: #94a3b8;
}

/* Responsive */
@media (max-width: 968px) {
  .brand-section {
    display: none;
  }
  
  .auth-section {
    flex: 1;
    background: white;
    padding: 16px;
  }
  
  .auth-card {
    padding: 16px 20px;
  }
}

@media (max-width: 480px) {
  .auth-card {
    padding: 12px 16px;
  }
  
  .form-options {
    flex-direction: column;
    gap: 10px;
    align-items: flex-start;
  }
  
  .auth-tabs .tab-btn {
    font-size: 13px;
    padding: 8px 16px;
  }
}
</style>