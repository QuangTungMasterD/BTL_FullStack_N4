<template>
  <div class="student-attendance">
    <div class="page-header">
      <h1 class="page-title">Điểm danh</h1>
      <p class="page-subtitle">Theo dõi lịch sử điểm danh và tỷ lệ chuyên cần</p>
    </div>

    <!-- Overview Statistics -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon attendance">
          <v-icon icon="mdi-calendar-check" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ overallStats.percentage }}%</div>
          <div class="stat-title">Tỷ lệ chuyên cần</div>
          <div class="stat-detail">{{ overallStats.present }}/{{ overallStats.total }} buổi</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon present">
          <v-icon icon="mdi-check-circle" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ overallStats.present }}</div>
          <div class="stat-title">Có mặt</div>
          <div class="stat-detail">Buổi học</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon absent">
          <v-icon icon="mdi-close-circle" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ overallStats.absent }}</div>
          <div class="stat-title">Vắng mặt</div>
          <div class="stat-detail">Buổi học</div>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon late">
          <v-icon icon="mdi-clock-alert" size="28" />
        </div>
        <div class="stat-content">
          <div class="stat-value">{{ overallStats.late }}</div>
          <div class="stat-title">Đi muộn</div>
          <div class="stat-detail">Buổi học</div>
        </div>
      </div>
    </div>

    <!-- Course Attendance Summary -->
    <div class="card">
      <div class="card-header">
        <h3>Tổng hợp điểm danh theo môn học</h3>
        <v-select
          v-model="selectedSemester"
          :items="semesters"
          label="Học kỳ"
          variant="outlined"
          density="compact"
          style="width: 150px"
          hide-details
          @update:model-value="loadAttendanceSummary"
        />
      </div>
      <div class="card-body">
        <div v-if="loading.summary" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="course-summary-grid">
          <div v-for="course in attendanceSummary" :key="course.courseId" class="course-summary-card">
            <div class="course-header">
              <h4>{{ course.courseName }}</h4>
              <div class="course-percentage" :style="{ color: course.color }">
                {{ course.percentage }}%
              </div>
            </div>
            <div class="progress-bar-container">
              <div class="progress-bar">
                <div class="progress-fill" :style="{ width: `${course.percentage}%`, background: course.color }"></div>
              </div>
            </div>
            <div class="course-stats">
              <div class="stat-item"><span class="label">Có mặt:</span><span class="value">{{ course.present }}</span></div>
              <div class="stat-item"><span class="label">Vắng:</span><span class="value">{{ course.absent }}</span></div>
              <div class="stat-item"><span class="label">Muộn:</span><span class="value">{{ course.late }}</span></div>
              <div class="stat-item"><span class="label">Tổng:</span><span class="value">{{ course.totalSessions }}</span></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detailed Attendance Records -->
    <div class="card">
      <div class="card-header">
        <h3>Lịch sử điểm danh chi tiết</h3>
        <div class="filters">
          <v-select
            v-model="selectedCourse"
            :items="courseOptions"
            label="Chọn môn học"
            variant="outlined"
            density="compact"
            style="width: 200px"
            hide-details
            @update:model-value="loadAttendanceRecords"
          />
          <v-select
            v-model="selectedStatus"
            :items="statusOptions"
            label="Trạng thái"
            variant="outlined"
            density="compact"
            style="width: 150px"
            hide-details
            @update:model-value="loadAttendanceRecords"
          />
        </div>
      </div>
      <div class="card-body">
        <div v-if="loading.records" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="attendance-table">
          <v-data-table
            :headers="recordHeaders"
            :items="filteredAttendance"
            :items-per-page="10"
            hover
            class="elevation-0"
          >
            <template v-slot:item.status="{ item }">
              <v-chip :color="getStatusColor(item.status)" size="small">
                {{ getStatusText(item.status) }}
              </v-chip>
            </template>
          </v-data-table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '@/utils/api'

const selectedSemester = ref('Fall 2024')
const selectedCourse = ref('all')
const selectedStatus = ref('all')
const loading = ref({ summary: true, records: true })

const semesters = ['Fall 2024', 'Spring 2024', 'Fall 2023']
const courseOptions = ref([{ title: 'Tất cả', value: 'all' }])
const statusOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Có mặt', value: 'Present' },
  { title: 'Vắng mặt', value: 'Absent' },
  { title: 'Đi muộn', value: 'Late' }
]

const recordHeaders = [
  { title: 'Ngày', key: 'date', sortable: true },
  { title: 'Môn học', key: 'courseName' },
  { title: 'Giờ vào', key: 'checkInTime' },
  { title: 'Giờ ra', key: 'checkOutTime' },
  { title: 'Trạng thái', key: 'status', sortable: true },
  { title: 'Tuần', key: 'week', sortable: true }
]

const overallStats = ref({ total: 0, present: 0, absent: 0, late: 0, percentage: 0 })
const attendanceSummary = ref([])
const attendanceRecords = ref([])

const filteredAttendance = computed(() => {
  let records = attendanceRecords.value
  if (selectedStatus.value !== 'all') {
    records = records.filter(r => r.status === selectedStatus.value)
  }
  return records.sort((a, b) => new Date(b.date) - new Date(a.date))
})

const loadOverallStats = async () => {
  try {
    const response = await api.get('/studentattendance/api/student/attendance/overall')
    overallStats.value = response.data
  } catch (error) {
    console.error('Failed to load overall stats:', error)
  }
}

const loadAttendanceSummary = async () => {
  loading.value.summary = true
  try {
    const response = await api.get('/studentattendance/api/student/attendance/summary', {
      params: { semester: selectedSemester.value }
    })
    attendanceSummary.value = response.data
    courseOptions.value = [
      { title: 'Tất cả', value: 'all' },
      ...attendanceSummary.value.map(c => ({ title: c.courseName, value: c.courseId }))
    ]
  } catch (error) {
    console.error('Failed to load attendance summary:', error)
  } finally {
    loading.value.summary = false
  }
}

const loadAttendanceRecords = async () => {
  loading.value.records = true
  try {
    const params = { courseId: selectedCourse.value !== 'all' ? selectedCourse.value : undefined }
    const response = await api.get('/studentattendance/api/student/attendance/records', { params })
    attendanceRecords.value = response.data
  } catch (error) {
    console.error('Failed to load attendance records:', error)
  } finally {
    loading.value.records = false
  }
}

const getStatusColor = (status) => {
  const colors = { Present: 'success', Absent: 'error', Late: 'warning' }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = { Present: 'Có mặt', Absent: 'Vắng mặt', Late: 'Đi muộn' }
  return texts[status] || status
}

onMounted(() => {
  loadOverallStats()
  loadAttendanceSummary()
  loadAttendanceRecords()
})
</script>

<style scoped>
.student-attendance {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 24px;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 8px;
}

.page-subtitle {
  color: #64748b;
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

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.stat-icon.attendance { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
.stat-icon.present { background: linear-gradient(135deg, #10b981 0%, #34d399 100%); }
.stat-icon.absent { background: linear-gradient(135deg, #ef4444 0%, #f87171 100%); }
.stat-icon.late { background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%); }

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
}

.stat-title {
  font-size: 14px;
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
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  margin-bottom: 24px;
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

.filters {
  display: flex;
  gap: 12px;
}

.card-body {
  padding: 24px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  padding: 40px;
}

.course-summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.course-summary-card {
  padding: 16px;
  background: #f8fafc;
  border-radius: 16px;
  transition: all 0.3s ease;
}

.course-summary-card:hover {
  background: #f1f5f9;
  transform: translateY(-2px);
}

.course-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.course-header h4 {
  font-size: 16px;
  font-weight: 600;
  color: #1e293b;
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
  margin-top: 12px;
}

.stat-item {
  text-align: center;
}

.stat-item .label {
  font-size: 11px;
  color: #64748b;
  display: block;
}

.stat-item .value {
  font-size: 14px;
  font-weight: 600;
  color: #1e293b;
}

.attendance-table {
  overflow-x: auto;
}
</style>