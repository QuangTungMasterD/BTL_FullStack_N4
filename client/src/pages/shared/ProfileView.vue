<template>
  <div class="profile-container">
    <div class="profile-header">
      <div class="cover-image"></div>
      <div class="profile-avatar-wrapper">
        <div class="profile-avatar">
          <span>{{ profileInfo.fullName?.charAt(0) || '?' }}</span>
        </div>
        <button class="edit-avatar-btn" @click="uploadAvatar">
          <v-icon icon="mdi-camera" size="16" />
        </button>
      </div>
      <div class="profile-info">
        <h1 class="profile-name">{{ profileInfo.fullName }}</h1>
        <p class="profile-role">{{ profileInfo.code }} • {{ profileInfo.faculty }}</p>
      </div>
    </div>

    <div class="profile-content">
      <!-- Left Column -->
      <div class="profile-left">
        <!-- Contact Information -->
        <div class="info-card">
          <h3>Thông tin liên hệ</h3>
          <div class="info-list">
            <div class="info-item">
              <v-icon icon="mdi-email" size="20" color="#64748b" />
              <div>
                <div class="info-label">Email</div>
                <div class="info-value">{{ profileInfo.email }}</div>
              </div>
            </div>
            <div class="info-item">
              <v-icon icon="mdi-phone" size="20" color="#64748b" />
              <div>
                <div class="info-label">Điện thoại</div>
                <div class="info-value">{{ profileInfo.phone || 'Chưa cập nhật' }}</div>
              </div>
            </div>
            <div class="info-item">
              <v-icon icon="mdi-map-marker" size="20" color="#64748b" />
              <div>
                <div class="info-label">Địa chỉ</div>
                <div class="info-value">{{ profileInfo.address || 'Chưa cập nhật' }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Academic Information (Student) or Professional Information (Lecturer) -->
        <div class="info-card">
          <h3>{{ userRole === 'STUDENT' ? 'Thông tin học tập' : 'Thông tin công tác' }}</h3>
          <div class="info-list">
            <div class="info-item">
              <v-icon icon="mdi-identifier" size="20" color="#64748b" />
              <div>
                <div class="info-label">{{ userRole === 'STUDENT' ? 'Mã sinh viên' : 'Mã giảng viên' }}</div>
                <div class="info-value">{{ profileInfo.code }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'STUDENT'">
              <v-icon icon="mdi-school" size="20" color="#64748b" />
              <div>
                <div class="info-label">Khoa</div>
                <div class="info-value">{{ profileInfo.faculty }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'STUDENT'">
              <v-icon icon="mdi-book-open" size="20" color="#64748b" />
              <div>
                <div class="info-label">Chuyên ngành</div>
                <div class="info-value">{{ profileInfo.major }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'STUDENT'">
              <v-icon icon="mdi-account-group" size="20" color="#64748b" />
              <div>
                <div class="info-label">Lớp</div>
                <div class="info-value">{{ profileInfo.class }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'LECTURER'">
              <v-icon icon="mdi-school" size="20" color="#64748b" />
              <div>
                <div class="info-label">Khoa</div>
                <div class="info-value">{{ profileInfo.faculty }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'LECTURER'">
              <v-icon icon="mdi-account-tie" size="20" color="#64748b" />
              <div>
                <div class="info-label">Học hàm/Học vị</div>
                <div class="info-value">{{ profileInfo.title }}</div>
              </div>
            </div>
            <div class="info-item" v-if="userRole === 'LECTURER'">
              <v-icon icon="mdi-teacher" size="20" color="#64748b" />
              <div>
                <div class="info-label">Chuyên ngành</div>
                <div class="info-value">{{ profileInfo.specialization }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Column -->
      <div class="profile-right">
        <!-- Academic Stats (Student only) -->
        <div class="stats-card" v-if="userRole === 'STUDENT'">
          <div class="stat">
            <div class="stat-value">{{ studentStats.gpa || 0 }}</div>
            <div class="stat-label">GPA</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ studentStats.creditsEarned || 0 }}</div>
            <div class="stat-label">Tổng tín chỉ</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ studentStats.remainingCredits || 0 }}</div>
            <div class="stat-label">Còn lại</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ studentStats.rank || 'Chưa xếp loại' }}</div>
            <div class="stat-label">Xếp loại</div>
          </div>
        </div>

        <!-- Lecturer Stats -->
        <div class="stats-card" v-if="userRole === 'LECTURER'">
          <div class="stat">
            <div class="stat-value">{{ lecturerStats.coursesCount || 0 }}</div>
            <div class="stat-label">Khóa đang dạy</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ lecturerStats.studentsCount || 0 }}</div>
            <div class="stat-label">Sinh viên</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ lecturerStats.attendanceRate || 0 }}%</div>
            <div class="stat-label">Tỷ lệ điểm danh</div>
          </div>
        </div>

        <!-- Admin Stats -->
        <div class="stats-card" v-if="userRole === 'ADMIN'">
          <div class="stat">
            <div class="stat-value">{{ adminStats.totalStudents || 0 }}</div>
            <div class="stat-label">Sinh viên</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ adminStats.totalLecturers || 0 }}</div>
            <div class="stat-label">Giảng viên</div>
          </div>
          <div class="stat-divider"></div>
          <div class="stat">
            <div class="stat-value">{{ adminStats.totalCourses || 0 }}</div>
            <div class="stat-label">Khóa học</div>
          </div>
        </div>

        <!-- Enrolled Courses (Student only) -->
        <div class="courses-card" v-if="userRole === 'STUDENT'">
          <h3>Khóa học đã đăng ký</h3>
          <div v-if="loading.courses" class="loading-placeholder">
            <v-progress-circular indeterminate size="32" />
          </div>
          <div v-else class="course-tags">
            <v-chip v-for="course in enrolledCourses" :key="course.id" color="primary" variant="flat" class="course-chip">
              {{ course.courseName }}
            </v-chip>
            <div v-if="enrolledCourses.length === 0" class="empty-text">Chưa đăng ký khóa học nào</div>
          </div>
        </div>

        <!-- Recent Grades (Student only) -->
        <div class="grades-card" v-if="userRole === 'STUDENT'">
          <h3>Điểm gần đây</h3>
          <div v-if="loading.grades" class="loading-placeholder">
            <v-progress-circular indeterminate size="32" />
          </div>
          <div v-else class="grade-list">
            <div class="grade-item" v-for="grade in recentGrades" :key="grade.id">
              <div class="grade-course">{{ grade.courseName }}</div>
              <div class="grade-exam">{{ grade.examType }}</div>
              <div class="grade-score" :class="getGradeClass(grade.score)">{{ grade.score }}</div>
            </div>
            <div v-if="recentGrades.length === 0" class="empty-text">Chưa có điểm nào</div>
          </div>
        </div>

        <!-- Teaching Courses (Lecturer only) -->
        <div class="courses-card" v-if="userRole === 'LECTURER'">
          <h3>Khóa học đang dạy</h3>
          <div v-if="loading.teachingCourses" class="loading-placeholder">
            <v-progress-circular indeterminate size="32" />
          </div>
          <div v-else class="course-tags">
            <v-chip v-for="course in teachingCourses" :key="course.id" color="info" variant="flat" class="course-chip">
              {{ course.courseName }}
            </v-chip>
            <div v-if="teachingCourses.length === 0" class="empty-text">Chưa có khóa học nào</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import api from '@/utils/api'

const authStore = useAuthStore()
const userRole = computed(() => authStore.user?.role || 'STUDENT')
const userEmail = computed(() => authStore.user?.email)

const loading = ref({ courses: false, grades: false, teachingCourses: false })

// Profile info chung
const profileInfo = ref({
  fullName: '',
  code: '',
  email: '',
  phone: '',
  faculty: '',
  major: '',
  class: '',
  address: '',
  title: '',
  specialization: ''
})

// Student stats
const studentStats = ref({
  gpa: 0,
  creditsEarned: 0,
  remainingCredits: 0,
  rank: ''
})

// Lecturer stats
const lecturerStats = ref({
  coursesCount: 0,
  studentsCount: 0,
  attendanceRate: 0
})

// Admin stats
const adminStats = ref({
  totalStudents: 0,
  totalLecturers: 0,
  totalCourses: 0
})

const enrolledCourses = ref([])
const recentGrades = ref([])
const teachingCourses = ref([])

// Load profile dựa trên role
const loadProfile = async () => {
  try {
    let endpoint = ''
    if (userRole.value === 'STUDENT') {
      endpoint = '/studentattendance/api/student/profile'
    } else if (userRole.value === 'LECTURER') {
      endpoint = '/studentattendance/api/lecturer/profile'
    } else {
      // Admin lấy từ auth store
      profileInfo.value = {
        fullName: authStore.user?.fullName || 'Admin',
        code: authStore.user?.userId || 'ADM001',
        email: authStore.user?.email || '',
        phone: authStore.user?.phone || 'Chưa cập nhật',
        faculty: 'Quản trị hệ thống',
        major: '',
        class: '',
        address: 'Hà Nội, Việt Nam',
        title: 'Quản trị viên',
        specialization: 'Quản lý hệ thống'
      }
      return
    }
    
    const response = await api.get(endpoint)
    const data = response.data
    
    profileInfo.value = {
      fullName: data.fullName || data.name || '',
      code: data.studentId || data.lecturerId || data.code || '',
      email: data.email || '',
      phone: data.phone || '',
      faculty: data.faculty || '',
      major: data.major || '',
      class: data.class || '',
      address: data.address || 'Chưa cập nhật',
      title: data.title || '',
      specialization: data.specialization || ''
    }
  } catch (error) {
    console.error('Failed to load profile:', error)
    // Fallback từ auth store
    profileInfo.value = {
      fullName: authStore.user?.fullName || 'Người dùng',
      code: authStore.user?.studentId || authStore.user?.lecturerId || authStore.user?.userId || '',
      email: authStore.user?.email || '',
      phone: authStore.user?.phone || 'Chưa cập nhật',
      faculty: authStore.user?.faculty || 'Chưa cập nhật',
      major: authStore.user?.major || '',
      class: authStore.user?.class || '',
      address: 'Chưa cập nhật',
      title: authStore.user?.title || '',
      specialization: ''
    }
  }
}

// Load student stats
const loadStudentStats = async () => {
  if (userRole.value !== 'STUDENT') return
  try {
    const response = await api.get('/studentattendance/api/student/stats')
    studentStats.value = response.data
  } catch (error) {
    console.error('Failed to load student stats:', error)
  }
}

// Load enrolled courses (student)
const loadEnrolledCourses = async () => {
  if (userRole.value !== 'STUDENT') return
  loading.value.courses = true
  try {
    const response = await api.get('/studentattendance/api/student/enrolled-courses')
    enrolledCourses.value = response.data
  } catch (error) {
    console.error('Failed to load enrolled courses:', error)
  } finally {
    loading.value.courses = false
  }
}

// Load recent grades (student)
const loadRecentGrades = async () => {
  if (userRole.value !== 'STUDENT') return
  loading.value.grades = true
  try {
    const response = await api.get('/studentattendance/api/student/recent-grades')
    recentGrades.value = response.data
  } catch (error) {
    console.error('Failed to load recent grades:', error)
  } finally {
    loading.value.grades = false
  }
}

// Load teaching courses (lecturer)
const loadTeachingCourses = async () => {
  if (userRole.value !== 'LECTURER') return
  loading.value.teachingCourses = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/courses')
    teachingCourses.value = response.data
    lecturerStats.value.coursesCount = teachingCourses.value.length
    lecturerStats.value.studentsCount = teachingCourses.value.reduce((sum, c) => sum + (c.studentCount || 0), 0)
  } catch (error) {
    console.error('Failed to load teaching courses:', error)
  } finally {
    loading.value.teachingCourses = false
  }
}

// Load lecturer attendance rate
const loadLecturerAttendance = async () => {
  if (userRole.value !== 'LECTURER') return
  try {
    const response = await api.get('/studentattendance/api/lecturer/average-grade')
    lecturerStats.value.attendanceRate = response.data.average || 85
  } catch (error) {
    console.error('Failed to load lecturer attendance:', error)
  }
}

// Load admin stats
const loadAdminStats = async () => {
  if (userRole.value !== 'ADMIN') return
  try {
    const response = await api.get('/studentattendance/api/admin/statistics')
    adminStats.value = response.data
  } catch (error) {
    console.error('Failed to load admin stats:', error)
  }
}

const getGradeClass = (score) => {
  if (score >= 8.5) return 'excellent'
  if (score >= 7) return 'good'
  if (score >= 5) return 'average'
  return 'poor'
}

const uploadAvatar = () => {
  console.log('Upload avatar')
}

// Load admin profile
const loadAdminProfile = async () => {
  if (userRole.value !== 'ADMIN') return
  try {
    const response = await api.get('/studentattendance/api/admin/profile')
    const data = response.data
    
    profileInfo.value = {
      fullName: data.fullName || data.name || '',
      code: data.adminId || data.code || 'ADM001',
      email: data.email || '',
      phone: data.phone || 'Chưa cập nhật',
      faculty: data.faculty || 'Phòng Đào tạo',
      major: data.major || '',
      class: data.class || '',
      address: data.address || 'Hà Nội, Việt Nam',
      title: data.title || 'Quản trị viên',
      specialization: data.specialization || 'Quản lý hệ thống'
    }
  } catch (error) {
    console.error('Failed to load admin profile:', error)
    // Fallback từ auth store
    profileInfo.value = {
      fullName: authStore.user?.fullName || 'Admin',
      code: authStore.user?.userId || 'ADM001',
      email: authStore.user?.email || '',
      phone: authStore.user?.phone || '0912345678',
      faculty: 'Phòng Đào tạo',
      major: '',
      class: '',
      address: 'Hà Nội, Việt Nam',
      title: 'Quản trị viên',
      specialization: 'Quản lý hệ thống'
    }
  }
}

onMounted(async () => {
  await loadProfile()
  await loadAdminProfile()
  await loadStudentStats()
  await loadEnrolledCourses()
  await loadRecentGrades()
  await loadTeachingCourses()
  await loadLecturerAttendance()
  await loadAdminStats()
})
</script>

<style scoped>
.profile-container {
  max-width: 1200px;
  margin: 0 auto;
}

.profile-header {
  position: relative;
  margin-bottom: 80px;
}

.cover-image {
  height: 200px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 24px;
}

.profile-avatar-wrapper {
  position: absolute;
  bottom: -60px;
  left: 40px;
}

.profile-avatar {
  width: 120px;
  height: 120px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 48px;
  font-weight: 600;
  color: white;
  border: 4px solid white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.edit-avatar-btn {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 32px;
  height: 32px;
  background: white;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.profile-info {
  position: absolute;
  bottom: 20px;
  left: 180px;
}

.profile-name {
  font-size: 28px;
  font-weight: 700;
  color: white;
  margin-bottom: 4px;
}

.profile-role {
  color: rgba(255, 255, 255, 0.9);
}

.profile-content {
  display: grid;
  grid-template-columns: 1fr 1.5fr;
  gap: 24px;
  margin-top: 32px;
}

.info-card,
.stats-card,
.courses-card,
.grades-card {
  background: white;
  border-radius: 20px;
  padding: 24px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.info-card h3,
.courses-card h3,
.grades-card h3 {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 20px;
  color: #1e293b;
}

.info-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.info-item {
  display: flex;
  gap: 12px;
  align-items: flex-start;
}

.info-label {
  font-size: 12px;
  color: #64748b;
  margin-bottom: 4px;
}

.info-value {
  font-size: 14px;
  color: #1e293b;
  font-weight: 500;
}

.stats-card {
  display: flex;
  justify-content: space-around;
  align-items: center;
  text-align: center;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #3b82f6;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #64748b;
}

.stat-divider {
  width: 1px;
  height: 50px;
  background: #e2e8f0;
}

.course-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.course-chip {
  margin: 4px;
}

.grade-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.grade-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: #f8fafc;
  border-radius: 12px;
}

.grade-course {
  font-weight: 500;
  color: #1e293b;
}

.grade-exam {
  font-size: 12px;
  color: #64748b;
}

.grade-score {
  font-weight: 600;
  font-size: 18px;
}

.grade-score.excellent { color: #10b981; }
.grade-score.good { color: #3b82f6; }
.grade-score.average { color: #f59e0b; }
.grade-score.poor { color: #ef4444; }

.loading-placeholder {
  display: flex;
  justify-content: center;
  padding: 20px;
}

.empty-text {
  color: #94a3b8;
  font-size: 14px;
  text-align: center;
  padding: 20px;
}

@media (max-width: 968px) {
  .profile-content {
    grid-template-columns: 1fr;
  }
  
  .stats-card {
    flex-wrap: wrap;
  }
}
</style>