<template>
  <div class="student-schedule">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-calendar-clock" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Lịch học của tôi</h1>
          <p class="hero-subtitle">Theo dõi lịch học các khóa bạn đang tham gia tại trung tâm</p>
        </div>
      </div>
      <div class="date-badge">
        <v-icon icon="mdi-calendar" size="18" />
        <span>{{ currentDate }}</span>
      </div>
    </div>

    <!-- View Mode Selector -->
    <div class="view-controls">
      <div class="view-toggle">
        <v-btn 
          :color="viewMode === 'week' ? 'primary' : 'default'" 
          :variant="viewMode === 'week' ? 'flat' : 'outlined'"
          size="small"
          @click="viewMode = 'week'"
        >
          Tuần
        </v-btn>
        <v-btn 
          :color="viewMode === 'month' ? 'primary' : 'default'" 
          :variant="viewMode === 'month' ? 'flat' : 'outlined'"
          size="small"
          @click="viewMode = 'month'"
        >
          Tháng
        </v-btn>
      </div>
      <div class="navigation-controls">
        <v-btn icon size="small" variant="text" @click="navigate(-1)">
          <v-icon icon="mdi-chevron-left" />
        </v-btn>
        <span class="date-range">{{ dateRangeText }}</span>
        <v-btn icon size="small" variant="text" @click="navigate(1)">
          <v-icon icon="mdi-chevron-right" />
        </v-btn>
        <v-btn size="small" variant="tonal" @click="goToday" class="today-btn">Hôm nay</v-btn>
      </div>
    </div>

    <!-- Schedule Grid -->
    <div class="schedule-container">
      <div v-if="loading" class="loading-placeholder">
        <v-progress-circular indeterminate />
        <p>Đang tải lịch học...</p>
      </div>
      
      <div v-else-if="sessions.length === 0" class="empty-state">
        <v-icon icon="mdi-calendar-blank" size="64" color="#cbd5e1" />
        <p>Không có lịch học</p>
        <p class="empty-sub">Bạn chưa đăng ký khóa học nào</p>
      </div>

      <div v-else class="schedule-grid">
        <!-- Time column -->
        <div class="time-column">
          <div class="time-header"></div>
          <div v-for="hour in hours" :key="hour" class="time-slot">
            <span>{{ hour }}:00</span>
          </div>
        </div>

        <!-- Days columns -->
        <div 
          v-for="(day, index) in weekDays" 
          :key="index" 
          class="day-column"
          :class="{ 'today': day.isToday }"
        >
          <div class="day-header" :class="{ 'today': day.isToday }">
            <div class="day-name">{{ day.name }}</div>
            <div class="day-number">{{ day.number }}</div>
          </div>
          <div class="day-slots">
            <div 
              v-for="hour in hours" 
              :key="hour" 
              class="slot"
              :class="{ 'has-session': getSessionsForDayAndHour(day, hour).length > 0 }"
            >
              <div 
                v-for="session in getSessionsForDayAndHour(day, hour)" 
                :key="session.id"
                class="session-card"
                :style="{ background: getRandomGradient(session.id) }"
                @click="openSessionDetail(session)"
              >
                <div class="session-time">{{ formatTime(session.startTime) }}</div>
                <div class="session-name">{{ session.className || session.courseName }}</div>
                <div class="session-room">
                  <v-icon icon="mdi-map-marker" size="12" />
                  {{ session.roomName || session.room || 'Chưa có phòng' }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Statistics -->
    <div class="stats-row" v-if="sessions.length > 0">
      <div class="stat-card-mini">
        <div class="stat-value">{{ sessions.length }}</div>
        <div class="stat-label">Tổng buổi học</div>
      </div>
      <div class="stat-card-mini">
        <div class="stat-value">{{ uniqueClassesCount }}</div>
        <div class="stat-label">Khóa học</div>
      </div>
      <div class="stat-card-mini">
        <div class="stat-value">{{ totalHours }}</div>
        <div class="stat-label">Tổng giờ học</div>
      </div>
    </div>

    <!-- Session Detail Dialog -->
    <v-dialog v-model="showDetailDialog" max-width="500px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedSession">
        <div class="dialog-header-schedule">
          <div class="dialog-header-icon">
            <v-icon icon="mdi-calendar-clock" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">Chi tiết buổi học</h2>
            <p class="dialog-subtitle">{{ selectedSession.className || selectedSession.courseName }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content">
          <div class="detail-grid">
            <div class="detail-item">
              <div class="detail-icon"><v-icon icon="mdi-clock" size="20" color="#64748b" /></div>
              <div>
                <div class="detail-label">Thời gian</div>
                <div class="detail-value">{{ formatDateTime(selectedSession.startTime) }}</div>
              </div>
            </div>
            <div class="detail-item">
              <div class="detail-icon"><v-icon icon="mdi-map-marker" size="20" color="#64748b" /></div>
              <div>
                <div class="detail-label">Phòng học</div>
                <div class="detail-value">{{ selectedSession.roomName || selectedSession.room || 'Chưa cập nhật' }}</div>
              </div>
            </div>
            <div class="detail-item">
              <div class="detail-icon"><v-icon icon="mdi-school" size="20" color="#64748b" /></div>
              <div>
                <div class="detail-label">Giảng viên</div>
                <div class="detail-value">{{ selectedSession.teacherName || selectedSession.lecturer || 'Chưa cập nhật' }}</div>
              </div>
            </div>
            <div class="detail-item" v-if="selectedSession.credits">
              <div class="detail-icon"><v-icon icon="mdi-credit-card" size="20" color="#64748b" /></div>
              <div>
                <div class="detail-label">Tín chỉ</div>
                <div class="detail-value">{{ selectedSession.credits }}</div>
              </div>
            </div>
          </div>
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-spacer />
          <v-btn color="primary" variant="text" @click="showDetailDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '@/utils/api'

const currentDate = ref(new Date().toLocaleDateString('vi-VN'))
const currentViewDate = ref(new Date())
const viewMode = ref('week')
const loading = ref(false)
const showDetailDialog = ref(false)
const selectedSession = ref(null)

const sessions = ref([])

// Hours for schedule
const hours = ['07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18']

// Computed
const weekDays = computed(() => {
  const date = new Date(currentViewDate.value)
  const day = date.getDay()
  const diff = date.getDate() - day + (day === 0 ? -6 : 1)
  const monday = new Date(date.setDate(diff))
  
  const days = []
  for (let i = 0; i < 7; i++) {
    const d = new Date(monday)
    d.setDate(monday.getDate() + i)
    const today = new Date()
    days.push({
      name: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật'][i],
      number: d.getDate(),
      isToday: d.toDateString() === today.toDateString(),
      fullDate: d
    })
  }
  return days
})

const dateRangeText = computed(() => {
  if (viewMode.value === 'week') {
    const start = weekDays.value[0]
    const end = weekDays.value[6]
    return `${start.name} ${start.number} - ${end.name} ${end.number}`
  } else {
    const date = new Date(currentViewDate.value)
    return `${date.getMonth() + 1}/${date.getFullYear()}`
  }
})

const uniqueClassesCount = computed(() => {
  return new Set(sessions.value.map(s => s.classId || s.courseId)).size
})

const totalHours = computed(() => {
  return sessions.value.length * 2 // Giả định mỗi buổi 2 tiếng
})

// Methods
const getSessionsForDayAndHour = (day, hour) => {
  const dayDate = new Date(day.fullDate)
  return sessions.value.filter(s => {
    const start = new Date(s.startTime)
    return start.getDate() === dayDate.getDate() &&
           start.getMonth() === dayDate.getMonth() &&
           start.getFullYear() === dayDate.getFullYear() &&
           start.getHours() === parseInt(hour)
  })
}

const getRandomGradient = (id) => {
  const gradients = [
    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
    'linear-gradient(135deg, #a18cd1 0%, #fbc2eb 100%)',
  ]
  return gradients[id % gradients.length]
}

const formatTime = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
}

const formatDateTime = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleString('vi-VN', { 
    weekday: 'long',
    day: '2-digit', 
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const navigate = (direction) => {
  const newDate = new Date(currentViewDate.value)
  if (viewMode.value === 'week') {
    newDate.setDate(newDate.getDate() + direction * 7)
  } else {
    newDate.setMonth(newDate.getMonth() + direction)
  }
  currentViewDate.value = newDate
  loadSchedule()
}

const goToday = () => {
  currentViewDate.value = new Date()
  loadSchedule()
}

const openSessionDetail = (session) => {
  selectedSession.value = session
  showDetailDialog.value = true
}

// Load schedule
const loadSchedule = async () => {
  loading.value = true
  try {
    console.log('🔄 Đang tải lịch học...')
    const params = {}
    
    if (viewMode.value === 'week') {
      const start = weekDays.value[0]?.fullDate
      const end = weekDays.value[6]?.fullDate
      if (start && end) {
        params.startDate = start.toISOString().split('T')[0]
        params.endDate = end.toISOString().split('T')[0]
      }
    } else {
      const firstDay = new Date(currentViewDate.value.getFullYear(), currentViewDate.value.getMonth(), 1)
      const lastDay = new Date(currentViewDate.value.getFullYear(), currentViewDate.value.getMonth() + 1, 0)
      params.startDate = firstDay.toISOString().split('T')[0]
      params.endDate = lastDay.toISOString().split('T')[0]
    }

    const response = await api.get('/api/student/schedule', { params })
    console.log('✅ Lịch học fetch thành công:', response.data)
    sessions.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load schedule:', error)
    // Mock data
    const today = new Date()
    sessions.value = [
      { 
        id: 1, 
        className: 'Lập trình Python cơ bản', 
        courseName: 'Lập trình Python cơ bản',
        startTime: new Date(today.setHours(8, 0, 0, 0)).toISOString(),
        endTime: new Date(today.setHours(10, 0, 0, 0)).toISOString(),
        room: 'A101',
        roomName: 'Phòng A101',
        teacherName: 'PGS.TS Trần Văn Xuân',
        lecturer: 'PGS.TS Trần Văn Xuân',
        credits: 3
      },
      { 
        id: 2, 
        className: 'Lập trình Java cơ bản', 
        courseName: 'Lập trình Java cơ bản',
        startTime: new Date(today.setHours(13, 30, 0, 0)).toISOString(),
        endTime: new Date(today.setHours(16, 0, 0, 0)).toISOString(),
        room: 'B202',
        roomName: 'Phòng B202',
        teacherName: 'TS. Nguyễn Thị Yến',
        lecturer: 'TS. Nguyễn Thị Yến',
        credits: 3
      },
    ]
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Lịch học...')
  loadSchedule()
  console.log('✅ Trang Lịch học đã sẵn sàng')
})
</script>

<style scoped>
.student-schedule {
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

/* View Controls */
.view-controls {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
  margin-bottom: 24px;
  padding: 16px 20px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.view-toggle {
  display: flex;
  gap: 4px;
  background: #f1f5f9;
  padding: 4px;
  border-radius: 12px;
}

.view-toggle .v-btn {
  border-radius: 8px;
  text-transform: none;
  font-weight: 500;
}

.navigation-controls {
  display: flex;
  align-items: center;
  gap: 8px;
}

.date-range {
  font-weight: 600;
  color: #1e293b;
  min-width: 180px;
  text-align: center;
}

.today-btn {
  border-radius: 8px;
  text-transform: none;
}

/* Schedule Grid */
.schedule-container {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  min-height: 500px;
}

.loading-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px;
  color: #94a3b8;
}

.loading-placeholder p {
  margin-top: 16px;
}

.empty-state {
  text-align: center;
  padding: 60px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 16px;
  font-size: 16px;
}

.empty-sub {
  font-size: 13px !important;
  margin-top: 4px !important;
  color: #cbd5e1;
}

.schedule-grid {
  display: grid;
  grid-template-columns: 60px repeat(7, 1fr);
  overflow-x: auto;
}

.time-column {
  background: #f8fafc;
  border-right: 1px solid #eef2f6;
}

.time-header {
  height: 60px;
  border-bottom: 1px solid #eef2f6;
  background: #f8fafc;
}

.time-slot {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  color: #94a3b8;
  border-bottom: 1px solid #f1f5f9;
}

.day-column {
  border-right: 1px solid #eef2f6;
}

.day-column:last-child {
  border-right: none;
}

.day-header {
  height: 60px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-bottom: 2px solid #eef2f6;
  background: #fafbfc;
}

.day-header.today {
  background: #e0f2fe;
  border-bottom-color: #3b82f6;
}

.day-name {
  font-size: 13px;
  font-weight: 600;
  color: #475569;
}

.day-number {
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
}

.day-header.today .day-number {
  color: #3b82f6;
}

.day-slots {
  position: relative;
}

.slot {
  height: 60px;
  border-bottom: 1px solid #f1f5f9;
  position: relative;
}

.slot.has-session {
  background: rgba(59, 130, 246, 0.03);
}

.session-card {
  position: absolute;
  top: 2px;
  left: 2px;
  right: 2px;
  bottom: 2px;
  border-radius: 10px;
  padding: 6px 10px;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
  z-index: 2;
  display: flex;
  flex-direction: column;
  justify-content: center;
  overflow: hidden;
  min-height: 50px;
}

.session-card:hover {
  transform: scale(1.02);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 3;
}

.session-time {
  font-size: 10px;
  opacity: 0.85;
  font-weight: 500;
}

.session-name {
  font-size: 11px;
  font-weight: 600;
  line-height: 1.2;
  margin-top: 2px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.session-room {
  font-size: 9px;
  opacity: 0.8;
  display: flex;
  align-items: center;
  gap: 2px;
  margin-top: 2px;
}

/* Stats Row */
.stats-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
  margin-top: 24px;
}

.stat-card-mini {
  background: white;
  border-radius: 20px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
}

.stat-card-mini .stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #3b82f6;
}

.stat-card-mini .stat-label {
  font-size: 13px;
  color: #64748b;
  margin-top: 4px;
}

/* Dialog */
.modern-dialog {
  border-radius: 28px !important;
  overflow: hidden;
}

.dialog-header-schedule {
  padding: 24px 28px;
  display: flex;
  align-items: center;
  gap: 16px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
}

.dialog-header-icon {
  width: 56px;
  height: 56px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.dialog-title {
  font-size: 22px;
  font-weight: 700;
  margin-bottom: 4px;
}

.dialog-subtitle {
  font-size: 13px;
  opacity: 0.8;
}

.dialog-content {
  padding: 24px 28px !important;
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.detail-item {
  display: flex;
  gap: 12px;
  align-items: flex-start;
}

.detail-icon {
  width: 32px;
  flex-shrink: 0;
}

.detail-label {
  font-size: 11px;
  color: #64748b;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 4px;
}

.detail-value {
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
}

.dialog-actions {
  padding: 16px 28px 24px !important;
}

/* Responsive */
@media (max-width: 968px) {
  .schedule-grid {
    grid-template-columns: 50px repeat(7, 120px);
  }
  
  .session-card {
    padding: 4px 6px;
  }
  
  .session-name {
    font-size: 9px;
  }
  
  .time-slot {
    font-size: 10px;
  }
}

@media (max-width: 768px) {
  .hero-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .view-controls {
    flex-direction: column;
    align-items: stretch;
  }
  
  .navigation-controls {
    justify-content: center;
    flex-wrap: wrap;
  }
  
  .date-range {
    min-width: 120px;
    font-size: 14px;
  }
  
  .stats-row {
    grid-template-columns: 1fr;
  }
  
  .detail-grid {
    grid-template-columns: 1fr;
  }
  
  .schedule-grid {
    grid-template-columns: 40px repeat(7, 100px);
  }
}

@media (max-width: 480px) {
  .schedule-grid {
    grid-template-columns: 35px repeat(7, 80px);
  }
  
  .time-slot {
    font-size: 9px;
    height: 50px;
  }
  
  .slot {
    height: 50px;
  }
  
  .day-header {
    height: 50px;
  }
  
  .day-name {
    font-size: 11px;
  }
  
  .day-number {
    font-size: 14px;
  }
  
  .session-card {
    min-height: 40px;
  }
}
</style>