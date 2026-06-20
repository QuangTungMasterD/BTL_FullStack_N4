<template>
  <div class="lecturer-dashboard">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-school" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Chào mừng, {{ lecturerInfo.fullName }}!</h1>
          <p class="hero-subtitle">Học kỳ Fall 2024 • Đang giảng dạy {{ teachingCourses.length }} lớp</p>
        </div>
      </div>
      <div class="date-badge">
        <v-icon icon="mdi-calendar" size="18" />
        <span>{{ currentDate }}</span>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper courses">
          <v-icon icon="mdi-book-open-variant" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ teachingCourses.length }}</div>
          <div class="stat-label">Lớp đang dạy</div>
          <div class="stat-trend positive" v-if="courseGrowth">+{{ courseGrowth }}</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper students">
          <v-icon icon="mdi-account-group" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalStudents }}</div>
          <div class="stat-label">Sinh viên quản lý</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper attendance">
          <v-icon icon="mdi-calendar-check" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ averageAttendance }}%</div>
          <div class="stat-label">Tỷ lệ điểm danh TB</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper grades">
          <v-icon icon="mdi-chart-line" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ averageGrade }}</div>
          <div class="stat-label">Điểm TB sinh viên</div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="content-grid">
      <!-- Teaching Courses -->
      <div class="workspace-card">
        <div class="card-header">
          <h3>Lớp học đang giảng dạy</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewAllCourses">Xem tất cả</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.courses" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="course-list">
            <div class="course-item" v-for="course in teachingCourses" :key="course.id">
              <div class="course-icon" :style="{ background: getRandomGradient() }">
                <v-icon icon="mdi-school" size="20" color="white" />
              </div>
              <div class="course-info">
                <div class="course-name">{{ course.courseName }}</div>
                <div class="course-meta">
                  <span><v-icon icon="mdi-account-group" size="12" /> {{ course.studentCount }} sinh viên</span>
                  <span><v-icon icon="mdi-clock" size="12" /> {{ course.schedule }}</span>
                  <span><v-icon icon="mdi-map-marker" size="12" /> {{ course.room }}</span>
                </div>
              </div>
              <div class="course-actions">
                <v-btn icon size="small" variant="text" class="action-btn-view" @click="viewCourse(course.id)">
                  <v-icon icon="mdi-eye" size="18" />
                </v-btn>
              </div>
            </div>
            <div v-if="teachingCourses.length === 0" class="empty-state">
              <v-icon icon="mdi-book-open" size="48" color="#cbd5e1" />
              <p>Chưa có lớp học nào được phân công</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Today's Schedule -->
      <div class="workspace-card">
        <div class="card-header">
          <h3>Lịch dạy hôm nay</h3>
          <v-chip color="primary" size="small" variant="tonal">{{ todayClasses.length }} lớp</v-chip>
        </div>
        <div class="card-body">
          <div v-if="loading.schedule" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="schedule-list">
            <div class="schedule-item" v-for="item in todayClasses" :key="item.id">
              <div class="schedule-time">
                <div class="time-range">{{ item.time }}</div>
              </div>
              <div class="schedule-info">
                <div class="course">{{ item.courseName }}</div>
                <div class="location">{{ item.room }} • {{ item.studentCount }} sinh viên</div>
              </div>
              <div class="schedule-action">
                <v-btn size="small" color="primary" variant="tonal" class="action-btn" @click="takeAttendance(item.id)">
                  Điểm danh
                </v-btn>
              </div>
            </div>
            <div v-if="todayClasses.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-blank" size="48" color="#cbd5e1" />
              <p>Hôm nay không có lịch dạy</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Attendance -->
      <div class="workspace-card">
        <div class="card-header">
          <h3>Điểm danh gần đây</h3>
          <v-btn variant="text" size="small" color="primary" @click="viewAllAttendance">Xem tất cả</v-btn>
        </div>
        <div class="card-body">
          <div v-if="loading.attendance" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="attendance-list">
            <div class="attendance-item" v-for="record in recentAttendance" :key="record.id">
              <div class="attendance-icon" :class="record.status">
                <v-icon :icon="record.status === 'present' ? 'mdi-check' : 'mdi-close'" size="14" />
              </div>
              <div class="attendance-info">
                <div class="attendance-course">{{ record.courseName }}</div>
                <div class="attendance-meta">
                  <span>{{ record.date }}</span>
                  <span>{{ record.presentCount }}/{{ record.totalCount }} có mặt</span>
                </div>
              </div>
              <div class="attendance-rate">
                <span :class="getRateClass(record.rate)">{{ record.rate }}%</span>
              </div>
            </div>
            <div v-if="recentAttendance.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-check" size="48" color="#cbd5e1" />
              <p>Chưa có dữ liệu điểm danh</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Top Students -->
      <div class="workspace-card">
        <div class="card-header">
          <h3>Top sinh viên xuất sắc</h3>
          <v-select
            v-model="selectedCourseForRanking"
            :items="courseOptions"
            label="Chọn lớp"
            variant="plain"
            density="compact"
            class="ranking-select"
            hide-details
            @update:model-value="loadTopStudents"
          />
        </div>
        <div class="card-body">
          <div v-if="loading.topStudents" class="loading-placeholder">
            <v-progress-circular indeterminate />
          </div>
          <div v-else class="ranking-list">
            <div class="ranking-item" v-for="(student, index) in topStudents" :key="student.id">
              <div class="ranking-number" :class="getRankClass(index + 1)">
                {{ index + 1 }}
              </div>
              <div class="ranking-avatar">
                <span>{{ student.name.charAt(0) }}</span>
              </div>
              <div class="ranking-info">
                <div class="ranking-name">{{ student.name }}</div>
                <div class="ranking-code">{{ student.studentId }}</div>
              </div>
              <div class="ranking-score">{{ student.averageScore }}</div>
            </div>
            <div v-if="topStudents.length === 0" class="empty-state">
              <v-icon icon="mdi-trophy" size="48" color="#cbd5e1" />
              <p>Chưa có dữ liệu xếp hạng</p>
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

const loading = ref({ courses: true, schedule: true, attendance: true, topStudents: true })

const lecturerInfo = ref({
  fullName: authStore.user?.fullName || 'Giảng viên',
  faculty: authStore.user?.faculty || 'Công nghệ thông tin'
})

const teachingCourses = ref([])
const totalStudents = ref(0)
const averageAttendance = ref(0)
const averageGrade = ref(0)
const courseGrowth = ref(0)
const todayClasses = ref([])
const recentAttendance = ref([])
const topStudents = ref([])
const selectedCourseForRanking = ref('')
const courseOptions = ref([])

const getRandomGradient = () => {
  const gradients = [
    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
  ]
  return gradients[Math.floor(Math.random() * gradients.length)]
}

const loadTeachingCourses = async () => {
  loading.value.courses = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/courses')
    teachingCourses.value = response.data
    totalStudents.value = teachingCourses.value.reduce((sum, c) => sum + (c.studentCount || 0), 0)
    courseOptions.value = teachingCourses.value.map(c => ({ title: c.courseName, value: c.id }))
    if (teachingCourses.value.length > 0 && !selectedCourseForRanking.value) {
      selectedCourseForRanking.value = teachingCourses.value[0].id
      loadTopStudents()
    }
  } catch (error) {
    console.error('Failed to load teaching courses:', error)
  } finally {
    loading.value.courses = false
  }
}

const loadTodaySchedule = async () => {
  loading.value.schedule = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/today-schedule')
    todayClasses.value = response.data
  } catch (error) {
    console.error('Failed to load today schedule:', error)
  } finally {
    loading.value.schedule = false
  }
}

const loadRecentAttendance = async () => {
  loading.value.attendance = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/recent-attendance')
    recentAttendance.value = response.data
    if (recentAttendance.value.length > 0) {
      const avg = recentAttendance.value.reduce((sum, r) => sum + r.rate, 0) / recentAttendance.value.length
      averageAttendance.value = Math.round(avg)
    }
  } catch (error) {
    console.error('Failed to load recent attendance:', error)
  } finally {
    loading.value.attendance = false
  }
}

const loadAverageGrade = async () => {
  try {
    const response = await api.get('/studentattendance/api/lecturer/average-grade')
    averageGrade.value = response.data.average || 0
  } catch (error) {
    console.error('Failed to load average grade:', error)
  }
}

const loadTopStudents = async () => {
  loading.value.topStudents = true
  try {
    const response = await api.get(`/studentattendance/api/lecturer/top-students`, {
      params: { courseId: selectedCourseForRanking.value }
    })
    topStudents.value = response.data
  } catch (error) {
    console.error('Failed to load top students:', error)
  } finally {
    loading.value.topStudents = false
  }
}

const getRateClass = (rate) => {
  if (rate >= 90) return 'excellent'
  if (rate >= 75) return 'good'
  if (rate >= 60) return 'average'
  return 'poor'
}

const getRankClass = (rank) => {
  if (rank === 1) return 'gold'
  if (rank === 2) return 'silver'
  if (rank === 3) return 'bronze'
  return ''
}

const viewCourse = (courseId) => {
  router.push(`/my-students?course=${courseId}`)
}

const viewAllCourses = () => {
  router.push('/my-students')
}

const takeAttendance = (classId) => {
  router.push(`/lecturer-attendance?class=${classId}`)
}

const viewAllAttendance = () => {
  router.push('/lecturer-attendance')
}

onMounted(() => {
  loadTeachingCourses()
  loadTodaySchedule()
  loadRecentAttendance()
  loadAverageGrade()
})
</script>

<style scoped>
.lecturer-dashboard {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 4px;
}

/* Hero Header */
.hero-header {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 28px;
  padding: 28px 32px;
  margin-bottom: 32px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.hero-content {
  display: flex;
  align-items: center;
  gap: 20px;
}

.hero-icon {
  width: 64px;
  height: 64px;
  background: rgba(255, 255, 255, 0.12);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(8px);
}

.hero-title {
  font-size: 28px;
  font-weight: 700;
  color: white;
  margin-bottom: 6px;
}

.hero-subtitle {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.7);
}

.date-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 40px;
  backdrop-filter: blur(8px);
  color: white;
}

/* Stats Cards */
.stats-grid-modern {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
  margin-bottom: 32px;
}

.stat-card-modern {
  background: white;
  border-radius: 24px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.stat-card-modern:hover {
  transform: translateY(-3px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.stat-icon-wrapper {
  width: 56px;
  height: 56px;
  border-radius: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-icon-wrapper.courses { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }
.stat-icon-wrapper.students { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.attendance { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }
.stat-icon-wrapper.grades { background: linear-gradient(135deg, #e9d5ff, #d8b4fe); color: #8b5cf6; }

.stat-info-modern .stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
}

.stat-info-modern .stat-label {
  font-size: 13px;
  color: #64748b;
  margin-top: 4px;
}

.stat-trend.positive {
  font-size: 11px;
  color: #10b981;
  margin-top: 4px;
}

/* Content Grid */
.content-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}

.workspace-card {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.workspace-card:hover {
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
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

.card-body {
  padding: 24px;
  min-height: 300px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
}

/* Course List */
.course-list, .schedule-list, .attendance-list, .ranking-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.course-item, .schedule-item, .attendance-item, .ranking-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-radius: 14px;
  transition: all 0.3s ease;
}

.course-item:hover, .schedule-item:hover, .attendance-item:hover, .ranking-item:hover {
  background: #f8fafc;
}

.course-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.course-info { flex: 1; }
.course-name { font-weight: 600; color: #1e293b; margin-bottom: 4px; }
.course-meta { display: flex; gap: 12px; font-size: 12px; color: #64748b; }
.course-meta span { display: flex; align-items: center; gap: 4px; }

.action-btn-view {
  transition: all 0.2s ease;
}
.action-btn-view:hover { background: #dbeafe; color: #3b82f6; }

/* Schedule */
.schedule-time { min-width: 100px; }
.time-range { font-weight: 600; color: #3b82f6; font-size: 14px; }
.schedule-info { flex: 1; }
.schedule-info .course { font-weight: 500; margin-bottom: 4px; }
.location { font-size: 12px; color: #64748b; }

.action-btn {
  border-radius: 10px;
  text-transform: none;
}

/* Attendance */
.attendance-icon {
  width: 32px;
  height: 32px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.attendance-icon.present { background: #dcfce7; color: #10b981; }
.attendance-icon.absent { background: #fee2e2; color: #ef4444; }

.attendance-info { flex: 1; }
.attendance-course { font-weight: 500; margin-bottom: 4px; }
.attendance-meta { font-size: 11px; color: #64748b; display: flex; gap: 12px; }

.attendance-rate span { font-weight: 600; font-size: 16px; }
.attendance-rate .excellent { color: #10b981; }
.attendance-rate .good { color: #3b82f6; }
.attendance-rate .average { color: #f59e0b; }
.attendance-rate .poor { color: #ef4444; }

/* Ranking */
.ranking-number {
  width: 32px;
  height: 32px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  background: #f1f5f9;
  color: #64748b;
}
.ranking-number.gold { background: #fef3c7; color: #f59e0b; }
.ranking-number.silver { background: #f1f5f9; color: #94a3b8; }
.ranking-number.bronze { background: #ffedd5; color: #b45309; }

.ranking-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
}

.ranking-info { flex: 1; }
.ranking-name { font-weight: 500; margin-bottom: 2px; }
.ranking-code { font-size: 11px; color: #64748b; }
.ranking-score { font-weight: 700; font-size: 18px; color: #3b82f6; }

.ranking-select {
  width: 160px;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}
.empty-state p { margin-top: 12px; }

@media (max-width: 1200px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
  .content-grid { grid-template-columns: 1fr; }
}

@media (max-width: 768px) {
  .hero-header { flex-direction: column; align-items: flex-start; }
  .stats-grid-modern { grid-template-columns: 1fr; }
}
</style>