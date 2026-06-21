<template>
  <div class="admin-dashboard">
    <!-- Welcome Section -->
    <div class="welcome-section">
      <div class="welcome-text">
        <h1 class="greeting">Chào mừng, {{ adminInfo.fullName }}!</h1>
        <p class="subtitle">Hệ thống quản lý trung tâm dạy học • Năm học {{ currentYear }}</p>
      </div>
      <div class="date-badge">
        <v-icon icon="mdi-calendar" size="20" />
        <span>{{ currentDate }}</span>
      </div>
    </div>

    <!-- Statistics Cards Row 1 -->
    <div class="stats-grid">
      <div class="stat-card" v-for="stat in stats" :key="stat.title">
        <div class="stat-icon" :style="{ background: stat.gradient }">
          <v-icon :icon="stat.icon" size="28" color="white" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-title">{{ stat.title }}</div>
          <div class="stat-trend" v-if="stat.trend && stat.trend !== ''">
            <v-icon icon="mdi-trending-up" size="14" />
            <span>{{ stat.trend }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Statistics Cards Row 2 -->
    <div class="stats-grid-second">
      <div class="stat-card-horizontal" v-for="stat in statsSecond" :key="stat.title">
        <div class="stat-icon" :class="stat.iconClass">
          <v-icon :icon="stat.icon" size="24" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-title">{{ stat.title }}</div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="content-grid">
      <!-- Enrollment Chart -->
      <div class="card chart-card">
        <div class="card-header">
          <h3>Biểu đồ đăng ký học</h3>
          <div class="chart-controls">
            <v-select
              v-model="chartYear"
              :items="years"
              label="Năm học"
              variant="outlined"
              density="compact"
              class="year-select"
              hide-details
              @update:model-value="loadEnrollmentTrend"
            />
            <v-btn icon size="small" variant="text" @click="loadEnrollmentTrend">
              <v-icon icon="mdi-refresh" size="18" />
            </v-btn>
          </div>
        </div>
        <div class="card-body">
          <div class="chart-container">
            <div v-if="enrollmentLoading" class="loading-placeholder">
              <v-icon icon="mdi-loading" size="40" class="spin" />
              <p>Đang tải dữ liệu...</p>
            </div>
            <div v-else-if="enrollmentData.length === 0" class="empty-placeholder">
              <v-icon icon="mdi-chart-bar" size="40" color="#cbd5e1" />
              <p>Không có dữ liệu</p>
            </div>
            <div v-else class="chart-bars">
              <div v-for="(item, index) in enrollmentData" :key="index" class="bar-item">
                <div class="bar-wrapper">
                  <div class="bar" :style="{ height: `${getBarHeight(item.value)}px`, background: gradients[index % gradients.length] }">
                    <span class="bar-tooltip">{{ item.value }}</span>
                  </div>
                </div>
                <div class="bar-label">{{ item.label }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Course Distribution -->
      <div class="card">
        <div class="card-header">
          <h3>Phân bố học viên theo khóa học</h3>
        </div>
        <div class="card-body">
          <div v-if="courseLoading" class="loading-placeholder">
            <v-icon icon="mdi-loading" size="32" class="spin" />
            <p>Đang tải...</p>
          </div>
          <div v-else-if="courseDistribution.length === 0" class="empty-placeholder">
            <v-icon icon="mdi-format-list-bulleted" size="32" color="#cbd5e1" />
            <p>Không có dữ liệu</p>
          </div>
          <div v-else class="department-list">
            <div v-for="item in courseDistribution" :key="item.name" class="department-item">
              <div class="dept-info">
                <div class="dept-name">{{ item.name }}</div>
                <div class="dept-count">{{ item.count }} học viên</div>
              </div>
              <div class="dept-bar">
                <div class="dept-fill" :style="{ width: `${Math.min(item.percent, 100)}%`, background: item.color }"></div>
              </div>
              <div class="dept-percent">{{ item.percent }}%</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activities -->
      <div class="card">
        <div class="card-header">
          <h3>Hoạt động gần đây</h3>
          <v-btn variant="text" size="small" color="primary" @click="loadRecentActivities">
            <v-icon icon="mdi-refresh" size="16" class="mr-1" />
            Làm mới
          </v-btn>
        </div>
        <div class="card-body">
          <div v-if="activityLoading" class="loading-placeholder">
            <v-icon icon="mdi-loading" size="32" class="spin" />
            <p>Đang tải...</p>
          </div>
          <div v-else-if="recentActivities.length === 0" class="empty-activities">
            <v-icon icon="mdi-information" size="32" color="#cbd5e1" />
            <p>Chưa có hoạt động nào</p>
          </div>
          <div v-else class="activity-timeline">
            <div class="activity-item" v-for="activity in recentActivities" :key="activity.id">
              <div class="activity-icon" :class="activity.type">
                <v-icon :icon="getActivityIcon(activity.type)" size="16" />
              </div>
              <div class="activity-content">
                <div class="activity-action">{{ activity.action }}</div>
                <div class="activity-time">{{ formatTimeAgo(activity.createdAt) }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="card">
        <div class="card-header">
          <h3>Thao tác nhanh</h3>
        </div>
        <div class="card-body">
          <div class="quick-actions-grid">
            <div class="quick-action" @click="openAddStudentDialog">
              <div class="action-icon add-student">
                <v-icon icon="mdi-account-plus" size="24" />
              </div>
              <span>Thêm học viên</span>
            </div>
            <div class="quick-action" @click="openAddLecturerDialog">
              <div class="action-icon add-lecturer">
                <v-icon icon="mdi-account-tie-plus" size="24" />
              </div>
              <span>Thêm giảng viên</span>
            </div>
            <div class="quick-action" @click="openAddCourseDialog">
              <div class="action-icon add-course">
                <v-icon icon="mdi-book-plus" size="24" />
              </div>
              <span>Thêm khóa học</span>
            </div>
            <div class="quick-action" @click="exportReport">
              <div class="action-icon export">
                <v-icon icon="mdi-file-excel" size="24" />
              </div>
              <span>Xuất báo cáo</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Student Dialog -->
    <v-dialog v-model="showAddStudentDialog" max-width="500px" transition="dialog-transition">
      <v-card class="custom-dialog">
        <v-card-title class="dialog-title">
          <v-icon icon="mdi-account-plus" size="24" class="mr-2" color="primary" />
          Thêm học viên mới
        </v-card-title>
        <v-divider />
        <v-card-text class="dialog-content">
          <v-text-field v-model="newStudent.studentId" label="Mã học viên" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.fullName" label="Họ và tên" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.email" label="Email" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.phone" label="Số điện thoại" variant="outlined" density="comfortable" />
          <v-select v-model="newStudent.course" :items="courseOptions" label="Khóa học" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.major" label="Chuyên ngành" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.class" label="Lớp" variant="outlined" density="comfortable" />
          <v-select v-model="newStudent.status" :items="statusOptions" label="Trạng thái" variant="outlined" density="comfortable" />
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-btn variant="text" @click="showAddStudentDialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="saveNewStudent">Thêm mới</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import api from '@/utils/api'

const router = useRouter()
const authStore = useAuthStore()

// Refs
const currentDate = ref(new Date().toLocaleDateString('vi-VN'))
const currentYear = ref(new Date().getFullYear())
const chartYear = ref(String(new Date().getFullYear()))
const showAddStudentDialog = ref(false)
const enrollmentLoading = ref(false)
const courseLoading = ref(false)
const activityLoading = ref(false)

const adminInfo = ref({
  fullName: authStore.user?.fullName || 'Quản trị viên'
})

// Statistics
const stats = ref([
  { title: 'Tổng học viên', value: '0', icon: 'mdi-account-group', gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', trend: '' },
  { title: 'Tổng giảng viên', value: '0', icon: 'mdi-school', gradient: 'linear-gradient(135deg, #10b981, #059669)', trend: '' },
  { title: 'Tổng khóa học', value: '0', icon: 'mdi-book-open-variant', gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', trend: '' },
  { title: 'Chi nhánh/Phòng ban', value: '0', icon: 'mdi-domain', gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', trend: '' },
])

const statsSecond = ref([
  { title: 'Lượt đăng ký học', value: '0', icon: 'mdi-format-list-bulleted', iconClass: 'enrollment' },
  { title: 'Tỷ lệ điểm danh TB', value: '0%', icon: 'mdi-calendar-check', iconClass: 'attendance-rate' },
  { title: 'Tỷ lệ hoàn thành', value: '0%', icon: 'mdi-check-circle', iconClass: 'completion' },
])

// Data
const years = ['2022', '2023', '2024', '2025']
const gradients = ['#3b82f6', '#10b981', '#8b5cf6', '#f59e0b', '#ef4444', '#14b8a6', '#f97316', '#06b6d4']
const courseOptions = ['Lập trình Python', 'Lập trình Java', 'Tiếng Anh giao tiếp', 'Toán cao cấp', 'Kỹ năng mềm', 'Trí tuệ nhân tạo']
const statusOptions = ['Đang học', 'Đã hoàn thành', 'Tạm nghỉ']

const enrollmentData = ref([])
const courseDistribution = ref([])
const recentActivities = ref([])

const newStudent = ref({
  studentId: '',
  fullName: '',
  email: '',
  phone: '',
  course: '',
  major: '',
  class: '',
  status: 'Đang học'
})

// Mock data cho fallback
const mockStats = {
  totalStudents: 1248,
  totalLecturers: 86,
  totalCourses: 156,
  totalDepartments: 8,
  activeEnrollments: 2847,
  averageAttendance: 87,
  completionRate: 92,
  studentGrowth: 12.5
}

const mockEnrollmentTrend = [
  { label: 'Tháng 8', value: 245 },
  { label: 'Tháng 9', value: 412 },
  { label: 'Tháng 10', value: 389 },
  { label: 'Tháng 11', value: 356 },
  { label: 'Tháng 12', value: 298 }
]

const mockCourseDistribution = [
  { name: 'Lập trình Python', count: 486, percent: 39, color: '#3b82f6' },
  { name: 'Lập trình Java', count: 325, percent: 26, color: '#10b981' },
  { name: 'Tiếng Anh giao tiếp', count: 218, percent: 17.5, color: '#f59e0b' },
  { name: 'Toán cao cấp', count: 128, percent: 10.3, color: '#8b5cf6' },
  { name: 'Kỹ năng mềm', count: 91, percent: 7.2, color: '#ef4444' }
]

const mockRecentActivities = [
  { id: 1, action: 'Học viên Nguyễn Văn A đăng ký khóa Python', createdAt: new Date().toISOString(), type: 'enrollment' },
  { id: 2, action: 'Giảng viên PGS.TS Trần Văn X cập nhật điểm', createdAt: new Date(Date.now() - 30 * 60000).toISOString(), type: 'grade' },
  { id: 3, action: 'Thêm khóa học mới: Trí tuệ nhân tạo', createdAt: new Date(Date.now() - 2 * 3600000).toISOString(), type: 'course' },
]

// Computed
const maxEnrollmentValue = computed(() => {
  if (enrollmentData.value.length === 0) return 100
  const max = Math.max(...enrollmentData.value.map(item => item.value))
  return max || 100
})

// Methods
const getBarHeight = (value) => {
  const maxHeight = 180
  const minHeight = 20
  if (maxEnrollmentValue.value === 0) return minHeight
  return Math.max(minHeight, (value / maxEnrollmentValue.value) * maxHeight)
}

const getActivityIcon = (type) => {
  const icons = { 
    enrollment: 'mdi-account-plus', 
    grade: 'mdi-chart-line', 
    course: 'mdi-book-plus', 
    notification: 'mdi-bell' 
  }
  return icons[type] || 'mdi-information'
}

const formatTimeAgo = (dateString) => {
  if (!dateString) return 'vừa xong'
  try {
    const date = new Date(dateString)
    const now = new Date()
    const diffMins = Math.floor((now - date) / 60000)
    if (diffMins < 1) return 'vừa xong'
    if (diffMins < 60) return `${diffMins} phút trước`
    if (diffMins < 1440) return `${Math.floor(diffMins / 60)} giờ trước`
    return `${Math.floor(diffMins / 1440)} ngày trước`
  } catch (e) {
    return 'vừa xong'
  }
}

// API Calls với fallback
const loadStatistics = async () => {
  console.log('🔄 Đang tải thống kê...')
  try {
    const response = await api.get('/api/admin/dashboard/statistics')
    console.log('✅ Thống kê fetch thành công:', response.data)
    const data = response.data
    
    stats.value = [
      { 
        title: 'Tổng học viên', 
        value: (data.totalStudents || 0).toLocaleString(), 
        icon: 'mdi-account-group', 
        gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', 
        trend: data.studentGrowth ? `+${data.studentGrowth}%` : '' 
      },
      { 
        title: 'Tổng giảng viên', 
        value: (data.totalLecturers || 0).toLocaleString(), 
        icon: 'mdi-school', 
        gradient: 'linear-gradient(135deg, #10b981, #059669)', 
        trend: '' 
      },
      { 
        title: 'Tổng khóa học', 
        value: (data.totalCourses || 0).toLocaleString(), 
        icon: 'mdi-book-open-variant', 
        gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', 
        trend: '' 
      },
      { 
        title: 'Chi nhánh/Phòng ban', 
        value: (data.totalDepartments || 0).toLocaleString(), 
        icon: 'mdi-domain', 
        gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', 
        trend: '' 
      },
    ]
    
    statsSecond.value = [
      { title: 'Lượt đăng ký học', value: (data.activeEnrollments || 0).toLocaleString(), icon: 'mdi-format-list-bulleted', iconClass: 'enrollment' },
      { title: 'Tỷ lệ điểm danh TB', value: `${data.averageAttendance || 0}%`, icon: 'mdi-calendar-check', iconClass: 'attendance-rate' },
      { title: 'Tỷ lệ hoàn thành', value: `${data.completionRate || 0}%`, icon: 'mdi-check-circle', iconClass: 'completion' },
    ]
    
  } catch (error) {
    console.error('❌ Lỗi fetch thống kê, sử dụng mock data:', error.message)
    if (error.response) {
      console.error('Response status:', error.response.status)
      console.error('Response data:', error.response.data)
    }
    // Fallback to mock data
    stats.value = [
      { title: 'Tổng học viên', value: mockStats.totalStudents.toLocaleString(), icon: 'mdi-account-group', gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', trend: `+${mockStats.studentGrowth}%` },
      { title: 'Tổng giảng viên', value: mockStats.totalLecturers.toLocaleString(), icon: 'mdi-school', gradient: 'linear-gradient(135deg, #10b981, #059669)', trend: '' },
      { title: 'Tổng khóa học', value: mockStats.totalCourses.toLocaleString(), icon: 'mdi-book-open-variant', gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', trend: '' },
      { title: 'Chi nhánh/Phòng ban', value: mockStats.totalDepartments.toLocaleString(), icon: 'mdi-domain', gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', trend: '' },
    ]
    statsSecond.value = [
      { title: 'Lượt đăng ký học', value: mockStats.activeEnrollments.toLocaleString(), icon: 'mdi-format-list-bulleted', iconClass: 'enrollment' },
      { title: 'Tỷ lệ điểm danh TB', value: `${mockStats.averageAttendance}%`, icon: 'mdi-calendar-check', iconClass: 'attendance-rate' },
      { title: 'Tỷ lệ hoàn thành', value: `${mockStats.completionRate}%`, icon: 'mdi-check-circle', iconClass: 'completion' },
    ]
  }
}

const loadEnrollmentTrend = async () => {
  enrollmentLoading.value = true
  console.log(`🔄 Đang tải biểu đồ đăng ký cho năm ${chartYear.value}...`)
  
  try {
    const response = await api.get('/api/admin/dashboard/enrollment-trend', {
      params: { year: chartYear.value }
    })
    console.log('✅ Biểu đồ fetch thành công:', response.data)
    enrollmentData.value = Array.isArray(response.data) ? response.data : []
    
    if (enrollmentData.value.length === 0) {
      console.warn('⚠️ Dữ liệu biểu đồ trống, sử dụng mock data')
      enrollmentData.value = mockEnrollmentTrend
    }
    
  } catch (error) {
    console.error(`❌ Lỗi fetch biểu đồ cho năm ${chartYear.value}, sử dụng mock data:`, error.message)
    enrollmentData.value = mockEnrollmentTrend
  } finally {
    enrollmentLoading.value = false
  }
}

const loadCourseDistribution = async () => {
  courseLoading.value = true
  console.log('🔄 Đang tải phân bố khóa học...')
  
  try {
    const response = await api.get('/api/admin/dashboard/course-distribution')
    console.log('✅ Phân bố khóa học fetch thành công:', response.data)
    courseDistribution.value = Array.isArray(response.data) ? response.data : []
    
    if (courseDistribution.value.length === 0) {
      console.warn('⚠️ Dữ liệu phân bố khóa học trống, sử dụng mock data')
      courseDistribution.value = mockCourseDistribution
    }
    
  } catch (error) {
    console.error('❌ Lỗi fetch phân bố khóa học, sử dụng mock data:', error.message)
    courseDistribution.value = mockCourseDistribution
  } finally {
    courseLoading.value = false
  }
}

const loadRecentActivities = async () => {
  activityLoading.value = true
  console.log('🔄 Đang tải hoạt động gần đây...')
  
  try {
    const response = await api.get('/api/admin/dashboard/recent-activities')
    console.log('✅ Hoạt động gần đây fetch thành công:', response.data)
    recentActivities.value = Array.isArray(response.data) ? response.data : []
    
    if (recentActivities.value.length === 0) {
      console.warn('⚠️ Không có hoạt động nào, sử dụng mock data')
      recentActivities.value = mockRecentActivities
    }
    
  } catch (error) {
    console.error('❌ Lỗi fetch hoạt động gần đây, sử dụng mock data:', error.message)
    recentActivities.value = mockRecentActivities
  } finally {
    activityLoading.value = false
  }
}

// Quick Actions
const openAddStudentDialog = () => {
  showAddStudentDialog.value = true
}

const saveNewStudent = async () => {
  try {
    await api.post('/api/admin/students', newStudent.value)
    showAddStudentDialog.value = false
    newStudent.value = { studentId: '', fullName: '', email: '', phone: '', course: '', major: '', class: '', status: 'Đang học' }
    loadStatistics()
  } catch (error) {
    console.error('❌ Lỗi thêm học viên:', error.message)
    alert('Không thể thêm học viên, vui lòng thử lại sau')
  }
}

const openAddLecturerDialog = () => {
  router.push('/lecturers')
}

const openAddCourseDialog = () => {
  router.push('/admin-courses')
}

const exportReport = () => {
  console.log('📊 Xuất báo cáo...')
}

// Mounted
onMounted(() => {
  console.log('🚀 Dashboard đang khởi tạo...')
  loadStatistics()
  loadEnrollmentTrend()
  loadCourseDistribution()
  loadRecentActivities()
  console.log('✅ Dashboard khởi tạo hoàn tất')
})
</script>

<style scoped>
/* Giữ nguyên style từ file trước */
.admin-dashboard {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 16px;
}

.welcome-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  padding: 28px 36px;
  background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 50%, #f8fafc 100%);
  border-radius: 24px;
  color: #0f172a;
  border: 1px solid rgba(59, 130, 246, 0.1);
}

.welcome-text {
  text-align: left;
}

.greeting {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 8px;
  text-align: left;
}

.subtitle {
  color: #475569;
  font-size: 14px;
  text-align: left;
}

.date-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 18px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 40px;
  backdrop-filter: blur(10px);
  color: #1e293b;
  border: 1px solid rgba(0, 0, 0, 0.05);
  flex-shrink: 0;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card {
  background: white;
  border-radius: 20px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.3s ease;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.08);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
}

.stat-title {
  font-size: 13px;
  color: #64748b;
  margin-top: 2px;
}

.stat-trend {
  font-size: 11px;
  color: #10b981;
  display: flex;
  align-items: center;
  gap: 4px;
  margin-top: 4px;
}

.stats-grid-second {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
  margin-bottom: 32px;
}

.stat-card-horizontal {
  background: white;
  border-radius: 16px;
  padding: 18px 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
}

.stat-icon.enrollment,
.stat-icon.attendance-rate,
.stat-icon.completion {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-icon.enrollment { background: #dbeafe; color: #3b82f6; }
.stat-icon.attendance-rate { background: #dcfce7; color: #10b981; }
.stat-icon.completion { background: #fef3c7; color: #f59e0b; }

.content-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}

.card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
  transition: all 0.3s ease;
}

.card:hover {
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #f1f5f9;
}

.card-header h3 {
  font-size: 17px;
  font-weight: 600;
  color: #0f172a;
  margin: 0;
}

.chart-controls {
  display: flex;
  align-items: center;
  gap: 8px;
}

.year-select {
  width: 100px;
}

.card-body {
  padding: 24px;
}

.chart-container {
  height: 280px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 8px;
  overflow-x: auto;
  overflow-y: hidden;
}

.loading-placeholder, .empty-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  width: 100%;
  height: 200px;
  color: #94a3b8;
}

.loading-placeholder p, .empty-placeholder p {
  font-size: 14px;
  color: #94a3b8;
  margin: 0;
}

.spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.chart-bars {
  display: flex;
  gap: 24px;
  align-items: flex-end;
  height: 100%;
  min-width: min-content;
  padding: 0 16px;
}

.bar-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  width: 70px;
  flex-shrink: 0;
}

.bar-wrapper {
  display: flex;
  align-items: flex-end;
  height: 200px;
  position: relative;
}

.bar {
  width: 45px;
  border-radius: 12px 12px 6px 6px;
  transition: height 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  min-height: 20px;
  position: relative;
  cursor: pointer;
}

.bar-tooltip {
  position: absolute;
  top: -28px;
  left: 50%;
  transform: translateX(-50%);
  background: #1e293b;
  color: white;
  padding: 2px 8px;
  border-radius: 6px;
  font-size: 11px;
  opacity: 0;
  transition: opacity 0.2s ease;
  white-space: nowrap;
}

.bar:hover .bar-tooltip {
  opacity: 1;
}

.bar-label {
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
}

.department-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.department-item {
  display: flex;
  align-items: center;
  gap: 16px;
}

.dept-info {
  width: 200px;
  flex-shrink: 0;
  text-align: left;
}

.dept-name {
  font-weight: 600;
  color: #0f172a;
  text-align: left;
}

.dept-count {
  font-size: 12px;
  color: #64748b;
  text-align: left;
}

.dept-bar {
  flex: 1;
  height: 8px;
  background: #e2e8f0;
  border-radius: 4px;
  overflow: hidden;
}

.dept-fill {
  height: 100%;
  border-radius: 4px;
  transition: width 0.5s ease;
}

.dept-percent {
  width: 50px;
  font-weight: 600;
  color: #3b82f6;
  text-align: right;
  flex-shrink: 0;
}

.activity-timeline {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.activity-item {
  display: flex;
  gap: 12px;
  padding: 12px 14px;
  border-radius: 12px;
  transition: all 0.3s ease;
  align-items: flex-start;
}

.activity-item:hover {
  background: #f8fafc;
}

.activity-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.activity-icon.enrollment { background: #dbeafe; color: #3b82f6; }
.activity-icon.grade { background: #dcfce7; color: #10b981; }
.activity-icon.course { background: #fef3c7; color: #f59e0b; }
.activity-icon.notification { background: #e9d5ff; color: #8b5cf6; }

.activity-content {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.activity-action {
  font-size: 14px;
  font-weight: 500;
  color: #0f172a;
  text-align: left;
  flex: 1;
}

.activity-time {
  font-size: 11px;
  color: #94a3b8;
  white-space: nowrap;
  margin-left: 12px;
  padding-top: 1px;
}

.empty-activities {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.quick-actions-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
}

.quick-action {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 20px 16px;
  background: #f8fafc;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid transparent;
}

.quick-action:hover {
  transform: translateY(-4px);
  background: #f1f5f9;
  border-color: #e2e8f0;
}

.action-icon {
  width: 52px;
  height: 52px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.action-icon.add-student { background: #dbeafe; color: #3b82f6; }
.action-icon.add-lecturer { background: #e0e7ff; color: #4f46e5; }
.action-icon.add-course { background: #fef3c7; color: #f59e0b; }
.action-icon.export { background: #dcfce7; color: #10b981; }

.quick-action span {
  font-size: 13px;
  font-weight: 500;
  color: #0f172a;
}

.custom-dialog {
  border-radius: 24px !important;
}

.dialog-title {
  font-size: 20px;
  font-weight: 700;
  padding: 24px 24px 16px 24px;
  color: #0f172a;
  display: flex;
  align-items: center;
}

.dialog-content {
  padding: 16px 24px !important;
}

.dialog-actions {
  padding: 16px 24px 24px 24px !important;
  gap: 12px;
}

@media (max-width: 1200px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
  .stats-grid-second { grid-template-columns: repeat(3, 1fr); }
  .content-grid { grid-template-columns: 1fr; }
}

@media (max-width: 768px) {
  .welcome-section { flex-direction: column; align-items: flex-start; gap: 16px; padding: 20px; }
  .greeting { font-size: 22px; }
  .stats-grid { grid-template-columns: 1fr 1fr; }
  .stats-grid-second { grid-template-columns: 1fr; }
  .content-grid { grid-template-columns: 1fr; }
  .chart-bars { gap: 16px; }
  .bar-item { width: 50px; }
  .bar { width: 30px; }
  .dept-info { width: 140px; }
  .department-item { flex-wrap: wrap; gap: 8px; }
}

@media (max-width: 480px) {
  .stats-grid { grid-template-columns: 1fr; }
  .quick-actions-grid { grid-template-columns: 1fr 1fr; }
  .dept-info { width: 100%; }
}
</style>