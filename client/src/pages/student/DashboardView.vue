<template>
  <div class="student-dashboard">
    <!-- Welcome Section -->
    <div class="welcome-section">
      <div class="welcome-text">
        <h1 class="greeting">Chào mừng, {{ studentInfo.fullName }}!</h1>
        <p class="subtitle">Học kỳ Fall 2024 • Tuần 12 • Còn 4 tuần đến kỳ thi</p>
      </div>
      <div class="date-badge">
        <v-icon icon="mdi-calendar" size="20" />
        <span>{{ currentDate }}</span>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon gpa">
          <v-icon icon="mdi-chart-line" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ studentStats.gpa || 0 }}</div>
          <div class="stat-title">GPA</div>
          <div class="stat-trend" v-if="studentStats.gpaTrend">+{{ studentStats.gpaTrend }}</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon credits">
          <v-icon icon="mdi-credit-card" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ studentStats.creditsEarned || 0 }}</div>
          <div class="stat-title">Tín chỉ đã tích lũy</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon attendance">
          <v-icon icon="mdi-calendar-check" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ studentStats.attendance || 0 }}%</div>
          <div class="stat-title">Chuyên cần</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon courses">
          <v-icon icon="mdi-book-open" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ studentStats.currentCourses || 0 }}</div>
          <div class="stat-title">Đang học</div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="content-grid">
      <!-- Current Courses -->
      <div class="card">
        <div class="card-header">
          <h3>Khóa học hiện tại</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewAllCourses">Xem tất cả</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.courses" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="course-list">
            <div class="course-item" v-for="course in currentCourses" :key="course.id">
              <div class="course-icon" :style="{ background: getRandomGradient() }">
                <v-icon icon="mdi-book-open-variant" size="20" color="white" />
              </div>
              <div class="course-info">
                <div class="course-name">{{ course.courseName }}</div>
                <div class="course-meta">
                  <span><v-icon icon="mdi-clock" size="12" /> {{ course.schedule }}</span>
                  <span><v-icon icon="mdi-map-marker" size="12" /> {{ course.room }}</span>
                </div>
              </div>
              <div class="course-progress">
                <div class="progress-circle">
                  <span>{{ course.attendance }}%</span>
                </div>
              </div>
            </div>
            <div v-if="currentCourses.length === 0" class="empty-state">
              <v-icon icon="mdi-book-open" size="48" color="#cbd5e1" />
              <p>Chưa có khóa học nào</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Upcoming Classes -->
      <div class="card">
        <div class="card-header">
          <h3>Lịch học sắp tới</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewSchedule">Xem lịch</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.schedule" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="schedule-list">
            <div class="schedule-item" v-for="classItem in upcomingClasses" :key="classItem.id">
              <div class="schedule-time">
                <div class="day">{{ classItem.day }}</div>
                <div class="time">{{ classItem.time }}</div>
              </div>
              <div class="schedule-info">
                <div class="course">{{ classItem.courseName }}</div>
                <div class="location">{{ classItem.room }} • {{ classItem.lecturer }}</div>
              </div>
            </div>
            <div v-if="upcomingClasses.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-blank" size="48" color="#cbd5e1" />
              <p>Không có lịch học sắp tới</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Grades -->
      <div class="card">
        <div class="card-header">
          <h3>Điểm gần đây</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewAllGrades">Xem tất cả</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.grades" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="grade-list">
            <div class="grade-item" v-for="grade in recentGrades" :key="grade.id">
              <div class="grade-info">
                <div class="grade-course">{{ grade.courseName }}</div>
                <div class="grade-exam">{{ grade.examType }}</div>
              </div>
              <div class="grade-score" :class="getGradeClass(grade.score)">{{ grade.score }}</div>
            </div>
            <div v-if="recentGrades.length === 0" class="empty-state">
              <v-icon icon="mdi-chart-line" size="48" color="#cbd5e1" />
              <p>Chưa có điểm nào</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Announcements -->
      <div class="card">
        <div class="card-header">
          <h3>Thông báo</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewAllAnnouncements">Xem tất cả</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.announcements" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="announcement-list">
            <div class="announcement-item" v-for="item in announcements" :key="item.id">
              <div class="announcement-icon" :class="item.priority">
                <v-icon icon="mdi-bell" size="16" />
              </div>
              <div class="announcement-content">
                <div class="announcement-title">{{ item.title }}</div>
                <div class="announcement-date">{{ formatDate(item.date) }}</div>
              </div>
            </div>
            <div v-if="announcements.length === 0" class="empty-state">
              <v-icon icon="mdi-bell-off" size="48" color="#cbd5e1" />
              <p>Không có thông báo mới</p>
            </div>
          </div>
        </div>
      </div>
    </div>
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

const loading = ref({
  courses: true,
  schedule: true,
  grades: true,
  announcements: true
})

const studentInfo = ref({
  fullName: authStore.user?.fullName || 'Sinh viên',
  studentId: authStore.user?.studentId || ''
})

const studentStats = ref({
  gpa: 0,
  gpaTrend: 0,
  creditsEarned: 0,
  attendance: 0,
  currentCourses: 0
})

const currentCourses = ref([])
const upcomingClasses = ref([])
const recentGrades = ref([])
const announcements = ref([])

const getRandomGradient = () => {
  const gradients = [
    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
  ]
  return gradients[Math.floor(Math.random() * gradients.length)]
}

const loadStudentStats = async () => {
  try {
    const response = await api.get('/studentattendance/api/student/stats')
    studentStats.value = response.data
  } catch (error) {
    console.error('Failed to load student stats:', error)
  }
}

const loadCurrentCourses = async () => {
  loading.value.courses = true
  try {
    const response = await api.get('/studentattendance/api/student/courses')
    currentCourses.value = response.data
    studentStats.value.currentCourses = currentCourses.value.length
  } catch (error) {
    console.error('Failed to load current courses:', error)
  } finally {
    loading.value.courses = false
  }
}

const loadUpcomingClasses = async () => {
  loading.value.schedule = true
  try {
    const response = await api.get('/studentattendance/api/student/upcoming-classes')
    upcomingClasses.value = response.data
  } catch (error) {
    console.error('Failed to load upcoming classes:', error)
  } finally {
    loading.value.schedule = false
  }
}

const loadRecentGrades = async () => {
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

const loadAnnouncements = async () => {
  loading.value.announcements = true
  try {
    const response = await api.get('/studentattendance/api/student/announcements')
    announcements.value = response.data
  } catch (error) {
    console.error('Failed to load announcements:', error)
  } finally {
    loading.value.announcements = false
  }
}

const getGradeClass = (score) => {
  if (score >= 8.5) return 'excellent'
  if (score >= 7) return 'good'
  if (score >= 5) return 'average'
  return 'poor'
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN')
}

const viewAllCourses = () => {
  router.push('/enrollments')
}

const viewSchedule = () => {
  router.push('/student-attendance')
}

const viewAllGrades = () => {
  router.push('/student-grades')
}

const viewAllAnnouncements = () => {
  // router.push('/announcements')
  console.log('View all announcements')
}

onMounted(() => {
  loadStudentStats()
  loadCurrentCourses()
  loadUpcomingClasses()
  loadRecentGrades()
  loadAnnouncements()
})
</script>

<style scoped>
.student-dashboard {
  max-width: 1400px;
  margin: 0 auto;
}

.welcome-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  padding: 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
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
  background: rgba(255, 255, 255, 0.2);
  border-radius: 40px;
  backdrop-filter: blur(10px);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
  margin-bottom: 32px;
}

.stat-card {
  background: white;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.stat-icon.gpa { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
.stat-icon.credits { background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); }
.stat-icon.attendance { background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); }
.stat-icon.courses { background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%); }

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
}

.stat-title {
  font-size: 14px;
  color: #64748b;
}

.stat-trend {
  font-size: 12px;
  color: #10b981;
  display: flex;
  align-items: center;
  gap: 4px;
  margin-top: 4px;
}

.content-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}

.card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.card:hover {
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #e2e8f0;
}

.card-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
}

.card-body {
  padding: 24px;
  min-height: 280px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
}

.course-list,
.schedule-list,
.grade-list,
.announcement-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.course-item,
.schedule-item,
.grade-item,
.announcement-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  border-radius: 12px;
  transition: all 0.3s ease;
}

.course-item:hover,
.schedule-item:hover,
.grade-item:hover,
.announcement-item:hover {
  background: #f8fafc;
}

.course-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.course-info {
  flex: 1;
  margin-left: 12px;
}

.course-name {
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 4px;
}

.course-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #64748b;
}

.course-meta span {
  display: flex;
  align-items: center;
  gap: 4px;
}

.progress-circle {
  width: 48px;
  height: 48px;
  background: #e2e8f0;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: #3b82f6;
}

.schedule-time {
  min-width: 100px;
}

.day {
  font-weight: 600;
  color: #1e293b;
  font-size: 14px;
}

.time {
  font-size: 12px;
  color: #64748b;
}

.schedule-info {
  flex: 1;
}

.schedule-info .course {
  font-weight: 500;
  margin-bottom: 4px;
}

.location {
  font-size: 12px;
  color: #64748b;
}

.grade-info {
  flex: 1;
}

.grade-course {
  font-weight: 500;
  margin-bottom: 4px;
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

.announcement-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 12px;
}

.announcement-icon.high { background: #fee2e2; color: #ef4444; }
.announcement-icon.medium { background: #fef3c7; color: #f59e0b; }
.announcement-icon.low { background: #dbeafe; color: #3b82f6; }

.announcement-content {
  flex: 1;
}

.announcement-title {
  font-weight: 500;
  margin-bottom: 4px;
}

.announcement-date {
  font-size: 11px;
  color: #94a3b8;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 12px;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .content-grid {
    grid-template-columns: 1fr;
  }
}
</style>