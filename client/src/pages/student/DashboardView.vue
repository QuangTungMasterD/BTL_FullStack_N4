<template>
  <div class="student-dashboard">
    <!-- Welcome Section -->
    <div class="welcome-section">
      <div class="welcome-text">
        <h1 class="greeting">Chào mừng, {{ studentInfo.fullName }}!</h1>
        <p class="subtitle">Hệ thống quản lý trung tâm dạy học • Năm học {{ currentYear }}</p>
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
const currentYear = ref(new Date().getFullYear())

const loading = ref({
  courses: true,
  schedule: true,
  grades: true,
  announcements: true
})

const studentInfo = ref({
  fullName: authStore.user?.fullName || 'Học viên',
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
    console.log('🔄 Đang tải thống kê học viên...')
    const response = await api.get('/api/student/stats')
    console.log('✅ Thống kê học viên fetch thành công:', response.data)
    studentStats.value = response.data
  } catch (error) {
    console.error('❌ Failed to load student stats:', error)
    // Mock data
    studentStats.value = {
      gpa: 3.65,
      gpaTrend: 0.15,
      creditsEarned: 98,
      attendance: 92,
      currentCourses: 4
    }
  }
}

const loadCurrentCourses = async () => {
  loading.value.courses = true
  try {
    console.log('🔄 Đang tải danh sách khóa học hiện tại...')
    const response = await api.get('/api/student/courses')
    console.log('✅ Danh sách khóa học fetch thành công:', response.data)
    currentCourses.value = response.data
    studentStats.value.currentCourses = currentCourses.value.length
  } catch (error) {
    console.error('❌ Failed to load current courses:', error)
    // Mock data
    currentCourses.value = [
      { id: 1, courseName: 'Lập trình Python cơ bản', code: 'PY101', schedule: 'Thứ 2, 13:30-16:30', room: 'A101', credits: 3, lecturer: 'PGS.TS Trần Văn Xuân', attendance: 85 },
      { id: 2, courseName: 'Lập trình Java cơ bản', code: 'JA101', schedule: 'Thứ 3, 08:00-11:00', room: 'B202', credits: 3, lecturer: 'TS. Nguyễn Thị Yến', attendance: 92 },
      { id: 3, courseName: 'Tiếng Anh giao tiếp', code: 'EN101', schedule: 'Thứ 4, 13:30-16:30', room: 'C303', credits: 2, lecturer: 'ThS. Lê Văn Anh', attendance: 78 },
    ]
    studentStats.value.currentCourses = currentCourses.value.length
  } finally {
    loading.value.courses = false
  }
}

const loadUpcomingClasses = async () => {
  loading.value.schedule = true
  try {
    console.log('🔄 Đang tải lịch học sắp tới...')
    const response = await api.get('/api/student/upcoming-classes')
    console.log('✅ Lịch học sắp tới fetch thành công:', response.data)
    upcomingClasses.value = response.data
  } catch (error) {
    console.error('❌ Failed to load upcoming classes:', error)
    // Mock data
    upcomingClasses.value = [
      { id: 1, courseName: 'Lập trình Python cơ bản', day: 'Thứ 2', time: '13:30-16:30', room: 'A101', lecturer: 'PGS.TS Trần Văn Xuân' },
      { id: 2, courseName: 'Lập trình Java cơ bản', day: 'Thứ 3', time: '08:00-11:00', room: 'B202', lecturer: 'TS. Nguyễn Thị Yến' },
    ]
  } finally {
    loading.value.schedule = false
  }
}

const loadRecentGrades = async () => {
  loading.value.grades = true
  try {
    console.log('🔄 Đang tải điểm gần đây...')
    const response = await api.get('/api/student/recent-grades')
    console.log('✅ Điểm gần đây fetch thành công:', response.data)
    recentGrades.value = response.data
  } catch (error) {
    console.error('❌ Failed to load recent grades:', error)
    // Mock data
    recentGrades.value = [
      { id: 1, courseName: 'Lập trình Python cơ bản', examType: 'Giữa kỳ', score: 8.5, letterGrade: 'B+', recordedDate: new Date() },
      { id: 2, courseName: 'Lập trình Java cơ bản', examType: 'Quiz 1', score: 9.0, letterGrade: 'A', recordedDate: new Date() },
      { id: 3, courseName: 'Tiếng Anh giao tiếp', examType: 'Cuối kỳ', score: 7.5, letterGrade: 'B', recordedDate: new Date() },
    ]
  } finally {
    loading.value.grades = false
  }
}

const loadAnnouncements = async () => {
  loading.value.announcements = true
  try {
    console.log('🔄 Đang tải thông báo...')
    const response = await api.get('/api/student/announcements')
    console.log('✅ Thông báo fetch thành công:', response.data)
    announcements.value = response.data
  } catch (error) {
    console.error('❌ Failed to load announcements:', error)
    // Mock data
    announcements.value = [
      { id: 1, title: 'Lịch thi cuối kỳ', date: new Date(), priority: 'high' },
      { id: 2, title: 'Đăng ký học phần HK2', date: new Date(), priority: 'medium' },
    ]
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
  if (!date) return ''
  return new Date(date).toLocaleDateString('vi-VN')
}

const viewAllCourses = () => {
  router.push('/enrollments')
}

const viewSchedule = () => {
  router.push('/student-schedule')
}

const viewAllGrades = () => {
  router.push('/student-grades')
}

const viewAllAnnouncements = () => {
  console.log('View all announcements')
}

onMounted(() => {
  console.log('🚀 Khởi tạo trang Dashboard Học viên...')
  loadStudentStats()
  loadCurrentCourses()
  loadUpcomingClasses()
  loadRecentGrades()
  loadAnnouncements()
  console.log('✅ Trang Dashboard Học viên đã sẵn sàng')
})
</script>

<style scoped>
.student-dashboard {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 4px;
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
  gap: 24px;
  margin-bottom: 32px;
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
  color: white;
  flex-shrink: 0;
}

.stat-icon.gpa { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
.stat-icon.credits { background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); }
.stat-icon.attendance { background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); }
.stat-icon.courses { background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%); }

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
  align-items: center;
  padding: 12px 14px;
  border-radius: 14px;
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
  color: #0f172a;
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

.course-progress {
  flex-shrink: 0;
}

.progress-circle {
  width: 48px;
  height: 48px;
  background: #f1f5f9;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: #3b82f6;
}

.schedule-time {
  min-width: 120px;
}

.day {
  font-weight: 600;
  color: #0f172a;
  font-size: 14px;
}

.time {
  font-size: 12px;
  color: #64748b;
}

.schedule-info {
  flex: 1;
  margin-left: 12px;
}

.schedule-info .course {
  font-weight: 500;
  color: #0f172a;
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
  color: #0f172a;
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
  flex-shrink: 0;
}

.announcement-icon.high { background: #fee2e2; color: #ef4444; }
.announcement-icon.medium { background: #fef3c7; color: #f59e0b; }
.announcement-icon.low { background: #dbeafe; color: #3b82f6; }

.announcement-content {
  flex: 1;
}

.announcement-title {
  font-weight: 500;
  color: #0f172a;
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

@media (max-width: 768px) {
  .welcome-section {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
    padding: 20px;
  }
  
  .greeting {
    font-size: 22px;
  }
  
  .stats-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>