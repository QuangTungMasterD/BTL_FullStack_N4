<template>
  <div class="student-attendance">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-calendar-check" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Điểm danh của tôi</h1>
          <p class="hero-subtitle">Theo dõi lịch sử điểm danh và tỷ lệ chuyên cần tại trung tâm</p>
        </div>
      </div>
      <div class="date-badge">
        <v-icon icon="mdi-calendar" size="18" />
        <span>{{ currentDate }}</span>
      </div>
    </div>

    <!-- Tabs: Tổng quan / Chi tiết -->
    <v-tabs v-model="activeTab" color="primary" class="mb-4">
      <v-tab value="overview">
        <v-icon icon="mdi-view-dashboard" class="mr-2" size="18" />
        Tổng quan
      </v-tab>
      <v-tab value="detail">
        <v-icon icon="mdi-format-list-bulleted" class="mr-2" size="18" />
        Chi tiết theo môn
      </v-tab>
    </v-tabs>

    <!-- Tab Content: Tổng quan -->
    <v-window v-model="activeTab">
      <v-window-item value="overview">
        <!-- Statistics Cards -->
        <div class="stats-grid">
          <div class="stat-card" :class="{ 'active': true }">
            <div class="stat-icon attendance">
              <v-icon icon="mdi-calendar-check" size="28" />
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overallStats.percentage || 0 }}%</div>
              <div class="stat-title">Tỷ lệ chuyên cần</div>
              <div class="stat-detail">{{ overallStats.present || 0 }}/{{ overallStats.total || 0 }} buổi</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon present">
              <v-icon icon="mdi-check-circle" size="28" />
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overallStats.present || 0 }}</div>
              <div class="stat-title">Có mặt</div>
              <div class="stat-detail">Buổi học</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon absent">
              <v-icon icon="mdi-close-circle" size="28" />
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overallStats.absent || 0 }}</div>
              <div class="stat-title">Vắng mặt</div>
              <div class="stat-detail">Buổi học</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon late">
              <v-icon icon="mdi-clock-alert" size="28" />
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ overallStats.late || 0 }}</div>
              <div class="stat-title">Đi muộn</div>
              <div class="stat-detail">Buổi học</div>
            </div>
          </div>
        </div>

        <!-- Course Attendance Summary -->
        <div class="card">
          <div class="card-header">
            <h3>Tổng hợp điểm danh theo môn học</h3> <!-- Đã sửa từ "khóa học" thành "môn học" -->
          </div>
          <div class="card-body">
            <div v-if="loading.summary" class="loading-placeholder">
              <v-progress-circular indeterminate />
              <p style="margin-top: 12px; color: #94a3b8; font-size: 14px;">Đang tải dữ liệu điểm danh theo môn học...
              </p>
            </div>
            <div v-else-if="attendanceSummary.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-blank" size="48" color="#cbd5e1" />
              <p>Chưa có dữ liệu điểm danh</p>
            </div>
            <div v-else class="course-summary-grid">
              <div v-for="course in attendanceSummary" :key="course.courseId" class="course-summary-card"
                @click="viewCourseDetail(course.courseId)">
                <div class="course-header">
                  <div class="course-info">
                    <h4>{{ course.courseName }}</h4>
                    <span class="course-code">{{ course.courseCode || '' }}</span>
                  </div>
                  <div class="course-percentage" :style="{ color: course.color }">
                    {{ course.percentage }}%
                  </div>
                </div>
                <div class="progress-bar-container">
                  <div class="progress-bar">
                    <div class="progress-fill"
                      :style="{ width: `${Math.min(course.percentage, 100)}%`, background: course.color }"></div>
                  </div>
                </div>
                <div class="course-stats">
                  <div class="stat-item">
                    <span class="label">Có mặt</span>
                    <span class="value present">{{ course.present }}</span>
                  </div>
                  <div class="stat-item">
                    <span class="label">Vắng</span>
                    <span class="value absent">{{ course.absent }}</span>
                  </div>
                  <div class="stat-item">
                    <span class="label">Muộn</span>
                    <span class="value late">{{ course.late }}</span>
                  </div>
                  <div class="stat-item">
                    <span class="label">Tổng</span>
                    <span class="value total">{{ course.totalSessions }}</span>
                  </div>
                </div>
                <div class="course-status" :class="getCourseStatusClass(course.percentage)">
                  <v-icon :icon="getCourseStatusIcon(course.percentage)" size="16" />
                  {{ getCourseStatusText(course.percentage) }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Attendance Records -->
        <div class="card">
          <div class="card-header">
            <h3>Điểm danh gần đây</h3>
            <v-btn variant="text" size="small" color="primary" @click="activeTab = 'detail'">
              Xem tất cả
              <v-icon icon="mdi-chevron-right" size="16" />
            </v-btn>
          </div>
          <div class="card-body">
            <div v-if="loading.records" class="loading-placeholder">
              <v-progress-circular indeterminate />
            </div>
            <div v-else-if="recentRecords.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-blank" size="32" color="#cbd5e1" />
              <p>Chưa có lịch sử điểm danh</p>
            </div>
            <div v-else class="recent-list">
              <div class="record-item" v-for="record in recentRecords" :key="record.id">
                <div class="record-status">
                  <v-chip :color="getAttendanceStatusColor(record.status)" size="small">
                    {{ getAttendanceStatusText(record.status) }}
                  </v-chip>
                </div>
                <div class="record-info">
                  <div class="record-course">{{ record.courseName }}</div>
                  <div class="record-meta">
                    <span><v-icon icon="mdi-calendar" size="12" /> {{ formatDate(record.date) }}</span>
                    <span><v-icon icon="mdi-clock" size="12" /> {{ record.checkInTime || '--:--' }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </v-window-item>

      <!-- Tab Content: Chi tiết theo môn -->
      <v-window-item value="detail">
        <div class="card">
          <div class="card-header">
            <h3>Lịch sử điểm danh chi tiết</h3>
            <div class="filters">
              <v-select v-model="selectedCourse" :items="courseOptions" label="Chọn khóa học" variant="outlined"
                density="compact" style="width: 200px" hide-details @update:model-value="loadAttendanceRecords" />
              <v-select v-model="selectedStatus" :items="statusOptions" label="Trạng thái" variant="outlined"
                density="compact" style="width: 150px" hide-details @update:model-value="loadAttendanceRecords" />
            </div>
          </div>
          <div class="card-body">
            <div v-if="loading.records" class="loading-placeholder">
              <v-progress-circular indeterminate />
            </div>
            <div v-else-if="attendanceRecords.length === 0" class="empty-state">
              <v-icon icon="mdi-calendar-blank" size="48" color="#cbd5e1" />
              <p>Không có dữ liệu điểm danh</p>
            </div>
            <div v-else class="attendance-table">
              <v-data-table :headers="recordHeaders" :items="attendanceRecords" :items-per-page="10" hover
                class="modern-table">
                <template v-slot:item.date="{ item }">
                  {{ formatDate(item.date) }}
                </template>
                <template v-slot:item.status="{ item }">
                  <v-chip :color="getAttendanceStatusColor(item.status)" size="small">
                    {{ getAttendanceStatusText(item.status) }}
                  </v-chip>
                </template>
                <template v-slot:item.checkInTime="{ item }">
                  {{ item.checkInTime || '--:--' }}
                </template>
              </v-data-table>
            </div>
          </div>
        </div>
      </v-window-item>
    </v-window>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/utils/api'

const router = useRouter()
const currentDate = ref(new Date().toLocaleDateString('vi-VN'))
const activeTab = ref('overview')
// Bỏ selectedSemester vì không cần filter nữa
const selectedCourse = ref('all')
const selectedStatus = ref('all')

const loading = ref({ summary: true, records: true })

// Bỏ semesters array
const courseOptions = ref([{ title: 'Tất cả', value: 'all' }])
const statusOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Có mặt', value: 'Present' },
  { title: 'Vắng mặt', value: 'Absent' },
  { title: 'Đi muộn', value: 'Late' }
]

const recordHeaders = [
  { title: 'Ngày', key: 'date', sortable: true, align: 'start' },
  { title: 'Môn học', key: 'courseName', sortable: true, align: 'start' },
  { title: 'Giờ vào', key: 'checkInTime', align: 'center' },
  { title: 'Trạng thái', key: 'status', sortable: true, align: 'center' },
  { title: 'Tuần', key: 'week', sortable: true, align: 'center' }
]

const overallStats = ref({ total: 0, present: 0, absent: 0, late: 0, percentage: 0 })
const attendanceSummary = ref([])
const attendanceRecords = ref([])

// Recent records (5 records)
const recentRecords = computed(() => {
  return attendanceRecords.value.slice(0, 5)
})

// Helper functions
const getAttendanceStatusColor = (status) => {
  const colors = { Present: 'success', Absent: 'error', Late: 'warning' }
  return colors[status] || 'default'
}

const getAttendanceStatusText = (status) => {
  const texts = { Present: 'Có mặt', Absent: 'Vắng mặt', Late: 'Đi muộn' }
  return texts[status] || status
}

const getCourseStatusClass = (percentage) => {
  if (percentage >= 90) return 'excellent'
  if (percentage >= 75) return 'good'
  if (percentage >= 60) return 'average'
  return 'poor'
}

const getCourseStatusIcon = (percentage) => {
  if (percentage >= 90) return 'mdi-check-circle'
  if (percentage >= 75) return 'mdi-check'
  if (percentage >= 60) return 'mdi-alert'
  return 'mdi-close-circle'
}

const getCourseStatusText = (percentage) => {
  if (percentage >= 90) return 'Xuất sắc'
  if (percentage >= 75) return 'Tốt'
  if (percentage >= 60) return 'Trung bình'
  return 'Cần cải thiện'
}

// Load overall stats
const loadOverallStats = async () => {
  try {
    console.log('🔄 Đang tải thống kê tổng quan điểm danh...')
    const response = await api.get('/api/student/attendance/overall')
    console.log('✅ Thống kê tổng quan fetch thành công:', response.data)
    overallStats.value = response.data
  } catch (error) {
    console.error('❌ Failed to load overall stats:', error)
    overallStats.value = { total: 30, present: 25, absent: 3, late: 2, percentage: 83 }
  }
}

// Load attendance summary by course - Bỏ filter semester
const loadAttendanceSummary = async () => {
  loading.value.summary = true
  try {
    console.log(`🔄 Đang tải tổng hợp điểm danh theo môn học...`)
    // Bỏ params semester
    const response = await api.get('/api/student/attendance/summary')
    console.log('✅ Tổng hợp điểm danh fetch thành công:', response.data)

    attendanceSummary.value = response.data || []

    // Cập nhật course options
    courseOptions.value = [
      { title: 'Tất cả', value: 'all' },
      ...attendanceSummary.value.map(c => ({
        title: c.courseName,
        value: c.courseId
      }))
    ]

    console.log(`📊 Đã cập nhật ${courseOptions.value.length} môn học vào dropdown`)

  } catch (error) {
    console.error('❌ Failed to load attendance summary:', error)
    // Fallback data
    attendanceSummary.value = [
      { courseId: 1, courseName: 'Lập trình Python cơ bản', courseCode: 'PY101', totalSessions: 12, present: 10, absent: 1, late: 1, percentage: 83, color: '#10b981' },
      { courseId: 2, courseName: 'Lập trình Java cơ bản', courseCode: 'JA101', totalSessions: 10, present: 8, absent: 1, late: 1, percentage: 80, color: '#3b82f6' },
      { courseId: 3, courseName: 'Tiếng Anh giao tiếp', courseCode: 'EN101', totalSessions: 8, present: 7, absent: 1, late: 0, percentage: 88, color: '#f59e0b' },
    ]
    courseOptions.value = [
      { title: 'Tất cả', value: 'all' },
      ...attendanceSummary.value.map(c => ({ title: c.courseName, value: c.courseId }))
    ]
  } finally {
    loading.value.summary = false
  }
}

// Load attendance records
const loadAttendanceRecords = async () => {
  loading.value.records = true
  try {
    const params = {}
    if (selectedCourse.value !== 'all') params.courseId = selectedCourse.value
    if (selectedStatus.value !== 'all') params.status = selectedStatus.value

    console.log(`🔄 Đang tải lịch sử điểm danh với params:`, params)
    const response = await api.get('/api/student/attendance/records', { params })
    console.log('✅ Lịch sử điểm danh fetch thành công:', response.data)
    attendanceRecords.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load attendance records:', error)
    attendanceRecords.value = [
      { id: 1, date: '2024-06-15', courseName: 'Lập trình Python cơ bản', checkInTime: '08:00', status: 'Present', week: 12 },
      { id: 2, date: '2024-06-08', courseName: 'Lập trình Java cơ bản', checkInTime: '08:15', status: 'Late', week: 11 },
      { id: 3, date: '2024-06-01', courseName: 'Tiếng Anh giao tiếp', checkInTime: '08:00', status: 'Present', week: 10 },
    ]
  } finally {
    loading.value.records = false
  }
}

// View course detail
const viewCourseDetail = (courseId) => {
  selectedCourse.value = courseId
  activeTab.value = 'detail'
  loadAttendanceRecords()
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

// Watch for tab changes
watch(activeTab, (newVal) => {
  if (newVal === 'detail') {
    loadAttendanceRecords()
  }
})

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Điểm danh của tôi...')
  loadOverallStats()
  loadAttendanceSummary()
  loadAttendanceRecords()
  console.log('✅ Trang Điểm danh của tôi đã sẵn sàng')
})
</script>

<style scoped>
/* Giữ nguyên style từ file trước */
.student-attendance {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 4px;
}

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

:deep(.v-tabs) {
  background: white;
  border-radius: 16px;
  padding: 4px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

:deep(.v-tab) {
  border-radius: 12px;
  text-transform: none;
  font-weight: 500;
  min-height: 44px;
}

:deep(.v-tab--selected) {
  background: #e0f2fe;
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
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.08);
}

.stat-card.active {
  border-color: #667eea;
  background: linear-gradient(135deg, #f8f9ff, #f0f2ff);
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

.stat-icon.attendance {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.present {
  background: linear-gradient(135deg, #10b981 0%, #34d399 100%);
}

.stat-icon.absent {
  background: linear-gradient(135deg, #ef4444 0%, #f87171 100%);
}

.stat-icon.late {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
}

.stat-title {
  font-size: 13px;
  color: #64748b;
}

.stat-detail {
  font-size: 12px;
  color: #94a3b8;
}

.card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  border: 1px solid #f1f5f9;
  margin-bottom: 24px;
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
  flex-wrap: wrap;
  gap: 12px;
}

.card-header h3 {
  font-size: 17px;
  font-weight: 600;
  color: #0f172a;
  margin: 0;
}

.filters {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.card-body {
  padding: 24px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  padding: 40px;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 12px;
}

.course-summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

.course-summary-card {
  padding: 16px 20px;
  background: #f8fafc;
  border-radius: 16px;
  transition: all 0.3s ease;
  cursor: pointer;
  border: 1px solid transparent;
}

.course-summary-card:hover {
  background: #f1f5f9;
  transform: translateY(-2px);
  border-color: #e2e8f0;
}

.course-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.course-info h4 {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
  margin: 0;
}

.course-code {
  font-size: 12px;
  color: #94a3b8;
}

.course-percentage {
  font-size: 20px;
  font-weight: 700;
}

.progress-bar-container {
  margin-bottom: 12px;
}

.progress-bar {
  height: 8px;
  background: #e2e8f0;
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  border-radius: 4px;
  transition: width 1s ease;
}

.course-stats {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 8px;
}

.stat-item {
  text-align: center;
}

.stat-item .label {
  font-size: 10px;
  color: #94a3b8;
  display: block;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-item .value {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
}

.stat-item .value.present {
  color: #10b981;
}

.stat-item .value.absent {
  color: #ef4444;
}

.stat-item .value.late {
  color: #f59e0b;
}

.stat-item .value.total {
  color: #64748b;
}

.course-status {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 500;
}

.course-status.excellent {
  color: #10b981;
}

.course-status.good {
  color: #3b82f6;
}

.course-status.average {
  color: #f59e0b;
}

.course-status.poor {
  color: #ef4444;
}

.recent-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.record-item {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 16px;
  border-radius: 12px;
  transition: all 0.3s ease;
}

.record-item:hover {
  background: #f8fafc;
}

.record-status {
  flex-shrink: 0;
}

.record-info {
  flex: 1;
}

.record-course {
  font-weight: 500;
  color: #0f172a;
}

.record-meta {
  display: flex;
  gap: 16px;
  font-size: 12px;
  color: #94a3b8;
}

.record-meta span {
  display: flex;
  align-items: center;
  gap: 4px;
}

.attendance-table {
  overflow-x: auto;
}

.modern-table :deep(.v-data-table-header) {
  background-color: #f8fafc !important;
}

.modern-table :deep(.v-data-table-header th) {
  background: #f8fafc !important;
  font-weight: 600;
  color: #1e293b;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 14px 16px;
  border-bottom: 2px solid #e2e8f0;
}

.modern-table :deep(td) {
  padding: 14px 16px;
  border-bottom: 1px solid #f1f5f9;
}

.modern-table :deep(.v-data-table-footer) {
  padding: 16px 24px;
  background: white;
  border-top: 1px solid #f1f5f9;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .hero-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .stats-grid {
    grid-template-columns: 1fr 1fr;
  }

  .card-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .filters {
    flex-direction: column;
    width: 100%;
  }

  .filters .v-select {
    width: 100% !important;
  }

  .course-summary-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>