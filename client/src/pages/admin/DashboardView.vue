<template>
  <div class="admin-dashboard">
    <!-- Welcome Section -->
    <div class="welcome-section">
      <div class="welcome-text">
        <h1 class="greeting">Chào mừng, {{ adminInfo.fullName }}!</h1>
        <p class="subtitle">Hệ thống quản lý giáo dục • Học kỳ Fall 2024</p>
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
          <div class="stat-trend" v-if="stat.trend">
            <v-icon icon="mdi-trending-up" size="14" />
            <span>{{ stat.trend }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Statistics Cards Row 2 -->
    <div class="stats-grid-second">
      <div class="stat-card-horizontal">
        <div class="stat-icon enrollment">
          <v-icon icon="mdi-format-list-bulleted" size="24" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ activeEnrollments }}</div>
          <div class="stat-title">Lượt đăng ký học</div>
        </div>
      </div>
      <div class="stat-card-horizontal">
        <div class="stat-icon attendance-rate">
          <v-icon icon="mdi-calendar-check" size="24" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ averageAttendance }}%</div>
          <div class="stat-title">Tỷ lệ điểm danh TB</div>
        </div>
      </div>
      <div class="stat-card-horizontal">
        <div class="stat-icon completion">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ completionRate }}%</div>
          <div class="stat-title">Tỷ lệ hoàn thành</div>
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
            <div class="chart-bars">
              <div v-for="(item, index) in enrollmentData" :key="index" class="bar-item">
                <div class="bar-wrapper">
                  <div class="bar" :style="{ height: `${Math.min(item.value * 1.2, 180)}px`, background: gradients[index % gradients.length] }"></div>
                </div>
                <div class="bar-label">{{ item.label }}</div>
                <div class="bar-value">{{ item.value }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Department Distribution -->
      <div class="card">
        <div class="card-header">
          <h3>Phân bố sinh viên theo khoa</h3>
        </div>
        <div class="card-body">
          <div class="department-list">
            <div v-for="dept in departmentStats" :key="dept.name" class="department-item">
              <div class="dept-info">
                <div class="dept-name">{{ dept.name }}</div>
                <div class="dept-count">{{ dept.count }} sinh viên</div>
              </div>
              <div class="dept-bar">
                <div class="dept-fill" :style="{ width: `${dept.percent}%`, background: dept.color }"></div>
              </div>
              <div class="dept-percent">{{ dept.percent }}%</div>
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
          <div class="activity-timeline">
            <div class="activity-item" v-for="activity in recentActivities" :key="activity.id">
              <div class="activity-icon" :class="activity.type">
                <v-icon :icon="getActivityIcon(activity.type)" size="16" />
              </div>
              <div class="activity-content">
                <div class="activity-action">{{ activity.action }}</div>
                <div class="activity-time">{{ formatTimeAgo(activity.createdAt) }}</div>
              </div>
            </div>
            <div v-if="recentActivities.length === 0" class="empty-activities">
              <v-icon icon="mdi-information" size="32" color="#cbd5e1" />
              <p>Chưa có hoạt động nào</p>
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
              <span>Thêm sinh viên</span>
            </div>
            <div class="quick-action" @click="openAddLecturerDialog">
              <div class="action-icon add-lecturer">
                <v-icon icon="mdi-school-plus" size="24" />
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
          Thêm sinh viên mới
        </v-card-title>
        <v-divider />
        <v-card-text class="dialog-content">
          <v-text-field v-model="newStudent.studentId" label="Mã sinh viên" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.fullName" label="Họ và tên" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.email" label="Email" variant="outlined" density="comfortable" />
          <v-text-field v-model="newStudent.phone" label="Số điện thoại" variant="outlined" density="comfortable" />
          <v-select v-model="newStudent.faculty" :items="faculties" label="Khoa" variant="outlined" density="comfortable" />
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import api from '@/utils/api'

const router = useRouter()
const authStore = useAuthStore()
const currentDate = ref(new Date().toLocaleDateString('vi-VN'))
const chartYear = ref('2024')
const showAddStudentDialog = ref(false)

const adminInfo = ref({
  fullName: authStore.user?.fullName || 'Trần Văn Admin'
})

const stats = ref([
  { title: 'Tổng sinh viên', value: '0', icon: 'mdi-account-group', gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', trend: '' },
  { title: 'Tổng giảng viên', value: '0', icon: 'mdi-school', gradient: 'linear-gradient(135deg, #10b981, #059669)', trend: '' },
  { title: 'Tổng khóa học', value: '0', icon: 'mdi-book-open-variant', gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', trend: '' },
  { title: 'Khoa/Viện', value: '0', icon: 'mdi-domain', gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', trend: '' },
])

const activeEnrollments = ref('0')
const averageAttendance = ref('0')
const completionRate = ref('0')

const years = ['2022', '2023', '2024']
const gradients = ['#3b82f6', '#10b981', '#8b5cf6', '#f59e0b', '#ef4444']
const faculties = ['Công nghệ thông tin', 'Quản trị kinh doanh', 'Ngôn ngữ Anh']
const statusOptions = ['Đang học', 'Đã tốt nghiệp']

const enrollmentData = ref([])
const departmentStats = ref([])
const recentActivities = ref([])

const newStudent = ref({
  studentId: '',
  fullName: '',
  email: '',
  phone: '',
  faculty: '',
  major: '',
  class: '',
  status: 'Đang học'
})

// Mock data
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

const mockDepartmentStats = [
  { name: 'Công nghệ thông tin', count: 486, percent: 39, color: '#3b82f6' },
  { name: 'Quản trị kinh doanh', count: 325, percent: 26, color: '#10b981' },
  { name: 'Ngôn ngữ Anh', count: 218, percent: 17.5, color: '#f59e0b' },
  { name: 'Điện tử viễn thông', count: 128, percent: 10.3, color: '#8b5cf6' },
  { name: 'Khác', count: 91, percent: 7.2, color: '#ef4444' }
]

const mockRecentActivities = [
  { id: 1, action: 'Sinh viên Nguyễn Văn A đăng ký khóa CS301', createdAt: new Date().toISOString(), type: 'enrollment' },
  { id: 2, action: 'Giảng viên PGS.TS Trần Văn X cập nhật điểm', createdAt: new Date(Date.now() - 30 * 60000).toISOString(), type: 'grade' },
  { id: 3, action: 'Thêm khóa học mới: Trí tuệ nhân tạo', createdAt: new Date(Date.now() - 2 * 3600000).toISOString(), type: 'course' },
]

const getActivityIcon = (type) => {
  const icons = { enrollment: 'mdi-account-plus', grade: 'mdi-chart-line', course: 'mdi-book-plus', notification: 'mdi-bell' }
  return icons[type] || 'mdi-information'
}

const formatTimeAgo = (dateString) => {
  if (!dateString) return 'vừa xong'
  const date = new Date(dateString)
  const now = new Date()
  const diffMins = Math.floor((now - date) / 60000)
  if (diffMins < 1) return 'vừa xong'
  if (diffMins < 60) return `${diffMins} phút trước`
  if (diffMins < 1440) return `${Math.floor(diffMins / 60)} giờ trước`
  return `${Math.floor(diffMins / 1440)} ngày trước`
}

// API Calls with fallback
const loadStatistics = async () => {
  try {
    const response = await api.get('/studentattendance/api/admin/statistics')
    const data = response.data
    
    stats.value = [
      { title: 'Tổng sinh viên', value: data.totalStudents.toLocaleString(), icon: 'mdi-account-group', gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', trend: `+${data.studentGrowth}%` },
      { title: 'Tổng giảng viên', value: data.totalLecturers.toLocaleString(), icon: 'mdi-school', gradient: 'linear-gradient(135deg, #10b981, #059669)', trend: '' },
      { title: 'Tổng khóa học', value: data.totalCourses.toLocaleString(), icon: 'mdi-book-open-variant', gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', trend: '' },
      { title: 'Khoa/Viện', value: data.totalDepartments.toLocaleString(), icon: 'mdi-domain', gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', trend: '' },
    ]
    
    activeEnrollments.value = data.activeEnrollments.toLocaleString()
    averageAttendance.value = data.averageAttendance
    completionRate.value = data.completionRate
  } catch (error) {
    console.error('API failed, using mock data:', error)
    stats.value = [
      { title: 'Tổng sinh viên', value: mockStats.totalStudents.toLocaleString(), icon: 'mdi-account-group', gradient: 'linear-gradient(135deg, #3b82f6, #2563eb)', trend: `+${mockStats.studentGrowth}%` },
      { title: 'Tổng giảng viên', value: mockStats.totalLecturers.toLocaleString(), icon: 'mdi-school', gradient: 'linear-gradient(135deg, #10b981, #059669)', trend: '' },
      { title: 'Tổng khóa học', value: mockStats.totalCourses.toLocaleString(), icon: 'mdi-book-open-variant', gradient: 'linear-gradient(135deg, #8b5cf6, #7c3aed)', trend: '' },
      { title: 'Khoa/Viện', value: mockStats.totalDepartments.toLocaleString(), icon: 'mdi-domain', gradient: 'linear-gradient(135deg, #f59e0b, #d97706)', trend: '' },
    ]
    activeEnrollments.value = mockStats.activeEnrollments.toLocaleString()
    averageAttendance.value = mockStats.averageAttendance
    completionRate.value = mockStats.completionRate
  }
}

const loadEnrollmentTrend = async () => {
  try {
    const response = await api.get(`/studentattendance/api/admin/enrollment-trend?year=${chartYear.value}`)
    enrollmentData.value = response.data
  } catch (error) {
    console.error('API failed, using mock data:', error)
    enrollmentData.value = mockEnrollmentTrend
  }
}

const loadDepartmentStats = async () => {
  try {
    const response = await api.get('/studentattendance/api/admin/department-stats')
    departmentStats.value = response.data
  } catch (error) {
    console.error('API failed, using mock data:', error)
    departmentStats.value = mockDepartmentStats
  }
}

const loadRecentActivities = async () => {
  try {
    const response = await api.get('/studentattendance/api/admin/recent-activities')
    recentActivities.value = response.data
  } catch (error) {
    console.error('API failed, using mock data:', error)
    recentActivities.value = mockRecentActivities
  }
}

const openAddStudentDialog = () => {
  showAddStudentDialog.value = true
}

const saveNewStudent = async () => {
  try {
    await api.post('/studentattendance/api/admin/students', newStudent.value)
    showAddStudentDialog.value = false
    newStudent.value = { studentId: '', fullName: '', email: '', phone: '', faculty: '', major: '', class: '', status: 'Đang học' }
    loadStatistics()
  } catch (error) {
    console.error('Save failed:', error)
    alert('Không thể thêm sinh viên, vui lòng thử lại sau')
  }
}

const openAddLecturerDialog = () => {
  router.push('/lecturers')
}

const openAddCourseDialog = () => {
  router.push('/admin-courses')
}

const exportReport = () => {
  console.log('Export report')
}

onMounted(() => {
  loadStatistics()
  loadEnrollmentTrend()
  loadDepartmentStats()
  loadRecentActivities()
})
</script>

<style scoped>
/* Giữ nguyên style từ file gốc */
.admin-dashboard {
  max-width: 1400px;
  margin: 0 auto;
}

.welcome-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  padding: 24px 32px;
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 24px;
  color: white;
}

.greeting {
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 8px;
}

.subtitle {
  opacity: 0.9;
  font-size: 14px;
}

.date-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 40px;
  backdrop-filter: blur(10px);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
  margin-bottom: 24px;
}

.stat-card {
  background: white;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.12);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
}

.stat-title {
  font-size: 13px;
  color: #64748b;
  margin-top: 4px;
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
  gap: 24px;
  margin-bottom: 32px;
}

.stat-card-horizontal {
  background: white;
  border-radius: 16px;
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
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
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.card:hover {
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #eef2f6;
}

.card-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
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
  align-items: flex-end;
  justify-content: center;
  padding: 0 8px;
  overflow-x: auto;
  overflow-y: hidden;
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
}

.bar {
  width: 45px;
  border-radius: 12px 12px 6px 6px;
  transition: height 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  min-height: 4px;
  max-height: 180px;
}

.bar-label {
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
}

.bar-value {
  font-size: 14px;
  font-weight: 700;
  color: #1e293b;
  background: #f1f5f9;
  padding: 4px 10px;
  border-radius: 20px;
  min-width: 48px;
  text-align: center;
}

.department-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.department-item {
  display: flex;
  align-items: center;
  gap: 16px;
}

.dept-info {
  width: 180px;
}

.dept-name {
  font-weight: 600;
  color: #1e293b;
}

.dept-count {
  font-size: 12px;
  color: #64748b;
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
}

.activity-timeline {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.activity-item {
  display: flex;
  gap: 12px;
  padding: 12px;
  border-radius: 12px;
  transition: all 0.3s ease;
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
}

.activity-icon.enrollment { background: #dbeafe; color: #3b82f6; }
.activity-icon.grade { background: #dcfce7; color: #10b981; }
.activity-icon.course { background: #fef3c7; color: #f59e0b; }
.activity-icon.notification { background: #e9d5ff; color: #8b5cf6; }

.activity-content { flex: 1; }
.activity-action { font-size: 14px; font-weight: 500; margin-bottom: 4px; }
.activity-time { font-size: 11px; color: #94a3b8; }

.empty-activities {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.quick-actions-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.quick-action {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 20px;
  background: #f8fafc;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.quick-action:hover {
  transform: translateY(-4px);
  background: #f1f5f9;
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
.action-icon.add-lecturer { background: #dcfce7; color: #10b981; }
.action-icon.add-course { background: #fef3c7; color: #f59e0b; }
.action-icon.export { background: #e9d5ff; color: #8b5cf6; }

.quick-action span {
  font-size: 13px;
  font-weight: 500;
  color: #1e293b;
}

.custom-dialog {
  border-radius: 24px !important;
}

.dialog-title {
  font-size: 20px;
  font-weight: 700;
  padding: 24px 24px 16px 24px;
  color: #1e293b;
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
  .stats-grid-second { grid-template-columns: 1fr; }
  .chart-bars { gap: 20px; }
  .bar-item { width: 55px; }
  .bar { width: 35px; }
}
</style>