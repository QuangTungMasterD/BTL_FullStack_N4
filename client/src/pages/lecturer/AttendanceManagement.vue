<template>
  <div class="attendance-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-calendar-check" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý điểm danh</h1>
          <p class="hero-subtitle">Điểm danh sinh viên và theo dõi tỷ lệ chuyên cần</p>
        </div>
      </div>
    </div>

    <!-- Course Selector -->
    <div class="selector-card">
      <div class="selector-row">
        <v-select v-model="selectedCourse" :items="courseOptions" label="Chọn lớp học" variant="outlined" density="comfortable" class="course-select" hide-details @update:model-value="loadStudents" />
        <v-select v-model="selectedDate" :items="dateOptions" label="Chọn buổi học" variant="outlined" density="comfortable" class="date-select" hide-details />
        <v-btn color="primary" class="take-btn" @click="takeAttendance" :loading="takingAttendance">
          <v-icon icon="mdi-check-all" class="mr-2" />Điểm danh
        </v-btn>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper present">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ presentCount }}</div>
          <div class="stat-label">Có mặt</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper absent">
          <v-icon icon="mdi-close-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ absentCount }}</div>
          <div class="stat-label">Vắng mặt</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper late">
          <v-icon icon="mdi-clock-alert" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ lateCount }}</div>
          <div class="stat-label">Đi muộn</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-account-group" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalStudents }}</div>
          <div class="stat-label">Tổng số</div>
        </div>
      </div>
    </div>

    <!-- Attendance Table Card -->
    <div class="workspace-card">
      <div class="card-header">
        <h3>Danh sách điểm danh</h3>
        <v-btn size="small" variant="text" class="mark-all-btn" @click="markAllPresent">
          <v-icon icon="mdi-check-all" size="16" class="mr-1" />Tất cả có mặt
        </v-btn>
      </div>
      <div class="card-body">
        <div v-if="loading.students" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="attendance-table">
          <v-data-table
            :headers="attendanceHeaders"
            :items="studentsList"
            :items-per-page="10"
            hover
            class="modern-table"
          >
            <template v-slot:item.status="{ item }">
              <v-select v-model="item.status" :items="statusOptions" variant="outlined" density="compact" hide-details style="width: 140px" :color="getStatusColor(item.status)" />
            </template>
            <template v-slot:item.checkInTime="{ item }">
              <v-text-field v-model="item.checkInTime" placeholder="HH:MM" variant="outlined" density="compact" hide-details style="width: 100px" :disabled="item.status === 'absent'" />
            </template>
            <template v-slot:item.note="{ item }">
              <v-text-field v-model="item.note" placeholder="Ghi chú..." variant="outlined" density="compact" hide-details style="width: 150px" />
            </template>
          </v-data-table>
        </div>
        <div class="save-section">
          <v-btn color="success" size="large" class="save-btn" @click="saveAttendance" :loading="saving">
            <v-icon icon="mdi-content-save" class="mr-2" />Lưu điểm danh
          </v-btn>
        </div>
      </div>
    </div>

    <!-- History Card -->
    <div class="workspace-card">
      <div class="card-header">
        <h3>Lịch sử điểm danh</h3>
      </div>
      <div class="card-body">
        <div class="history-table">
          <v-data-table
            :headers="historyHeaders"
            :items="attendanceHistory"
            :items-per-page="5"
            hover
            class="modern-table"
          >
            <template v-slot:item.rate="{ item }">
              <div class="rate-cell">
                <div class="rate-bar">
                  <div class="rate-fill" :style="{ width: `${item.rate}%`, background: getRateColor(item.rate) }"></div>
                </div>
                <span>{{ item.rate }}%</span>
              </div>
            </template>
            <template v-slot:item.actions="{ item }">
              <v-btn icon size="small" variant="text" class="view-btn" @click="viewHistoryDetail(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
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

const selectedCourse = ref('')
const selectedDate = ref('today')
const takingAttendance = ref(false)
const saving = ref(false)
const loading = ref({ students: true, history: true })

const courseOptions = ref([])
const dateOptions = [{ title: 'Hôm nay', value: 'today' }, { title: 'Buổi 1 - Tuần 12', value: 'week12-1' }, { title: 'Buổi 2 - Tuần 12', value: 'week12-2' }]
const statusOptions = [{ title: 'Có mặt', value: 'present' }, { title: 'Vắng mặt', value: 'absent' }, { title: 'Đi muộn', value: 'late' }]

const studentsList = ref([])
const presentCount = computed(() => studentsList.value.filter(s => s.status === 'present').length)
const absentCount = computed(() => studentsList.value.filter(s => s.status === 'absent').length)
const lateCount = computed(() => studentsList.value.filter(s => s.status === 'late').length)
const totalStudents = computed(() => studentsList.value.length)

const attendanceHistory = ref([])

const attendanceHeaders = [
  { title: 'STT', key: 'index', sortable: false, width: 60 },
  { title: 'Mã sinh viên', key: 'studentId' },
  { title: 'Họ tên', key: 'fullName' },
  { title: 'Trạng thái', key: 'status' },
  { title: 'Giờ điểm danh', key: 'checkInTime' },
  { title: 'Ghi chú', key: 'note' },
]

const historyHeaders = [
  { title: 'Ngày', key: 'date', sortable: true },
  { title: 'Buổi học', key: 'session' },
  { title: 'Sĩ số', key: 'total' },
  { title: 'Có mặt', key: 'present' },
  { title: 'Vắng mặt', key: 'absent' },
  { title: 'Muộn', key: 'late' },
  { title: 'Tỷ lệ', key: 'rate' },
  { title: 'Thao tác', key: 'actions', sortable: false },
]

const loadCourses = async () => {
  try {
    const response = await api.get('/studentattendance/api/lecturer/courses')
    courseOptions.value = response.data.map(c => ({ title: c.courseName, value: c.id }))
    if (courseOptions.value.length > 0 && !selectedCourse.value) {
      selectedCourse.value = courseOptions.value[0].value
      loadStudents()
      loadHistory()
    }
  } catch (error) {
    console.error('Failed to load courses:', error)
  }
}

const loadStudents = async () => {
  if (!selectedCourse.value) return
  loading.value.students = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/attendance/students', {
      params: { courseId: selectedCourse.value, date: selectedDate.value }
    })
    studentsList.value = response.data.map((s, idx) => ({ ...s, index: idx + 1 }))
  } catch (error) {
    console.error('Failed to load students:', error)
  } finally {
    loading.value.students = false
  }
}

const loadHistory = async () => {
  if (!selectedCourse.value) return
  loading.value.history = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/attendance/history', {
      params: { courseId: selectedCourse.value }
    })
    attendanceHistory.value = response.data
  } catch (error) {
    console.error('Failed to load history:', error)
  } finally {
    loading.value.history = false
  }
}

const getStatusColor = (status) => {
  const colors = { present: 'success', absent: 'error', late: 'warning' }
  return colors[status] || 'default'
}

const getRateColor = (rate) => {
  if (rate >= 90) return '#10b981'
  if (rate >= 75) return '#3b82f6'
  if (rate >= 60) return '#f59e0b'
  return '#ef4444'
}

const takeAttendance = async () => {
  takingAttendance.value = true
  try {
    await api.post('/studentattendance/api/lecturer/attendance/take', {
      courseId: selectedCourse.value,
      date: selectedDate.value
    })
    await loadStudents()
  } catch (error) {
    console.error('Failed to take attendance:', error)
  } finally {
    takingAttendance.value = false
  }
}

const markAllPresent = () => {
  studentsList.value.forEach(s => {
    s.status = 'present'
    s.checkInTime = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  })
}

const saveAttendance = async () => {
  saving.value = true
  try {
    await api.post('/studentattendance/api/lecturer/attendance/save', {
      courseId: selectedCourse.value,
      date: selectedDate.value,
      records: studentsList.value
    })
    alert('Đã lưu điểm danh thành công!')
    await loadHistory()
  } catch (error) {
    console.error('Failed to save attendance:', error)
    alert('Lưu điểm danh thất bại')
  } finally {
    saving.value = false
  }
}

const viewHistoryDetail = (record) => {
  console.log('View detail:', record)
}

onMounted(() => {
  loadCourses()
})
</script>

<style scoped>
.attendance-management {
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

/* Selector Card */
.selector-card {
  background: white;
  border-radius: 24px;
  padding: 20px 24px;
  margin-bottom: 24px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.selector-row {
  display: flex;
  gap: 16px;
  align-items: center;
  flex-wrap: wrap;
}

.course-select {
  width: 280px;
}

.date-select {
  width: 200px;
}

.take-btn {
  padding: 10px 20px;
  border-radius: 12px;
  text-transform: none;
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

.stat-icon-wrapper.present { background: #dcfce7; color: #10b981; }
.stat-icon-wrapper.absent { background: #fee2e2; color: #ef4444; }
.stat-icon-wrapper.late { background: #fef3c7; color: #f59e0b; }
.stat-icon-wrapper.total { background: #dbeafe; color: #3b82f6; }

.stat-info-modern .stat-value { font-size: 28px; font-weight: 700; color: #1e293b; }
.stat-info-modern .stat-label { font-size: 13px; color: #64748b; margin-top: 4px; }

/* Workspace Card */
.workspace-card {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  margin-bottom: 24px;
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

.mark-all-btn {
  border-radius: 10px;
  text-transform: none;
}

.card-body {
  padding: 24px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  padding: 40px;
}

/* Table */
.modern-table :deep(.v-data-table-header th) {
  background: #f8fafc;
  font-weight: 600;
  color: #475569;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 14px 16px;
}

.modern-table :deep(td) {
  padding: 14px 16px;
}

.rate-cell {
  display: flex;
  align-items: center;
  gap: 10px;
}

.rate-bar {
  width: 80px;
  height: 6px;
  background: #e2e8f0;
  border-radius: 10px;
  overflow: hidden;
}

.rate-fill {
  height: 100%;
  border-radius: 10px;
}

.save-section {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
}

.save-btn {
  padding: 12px 28px;
  border-radius: 40px;
  text-transform: none;
}

.view-btn:hover {
  background: #dbeafe;
  color: #3b82f6;
}

@media (max-width: 968px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
  .selector-row { flex-direction: column; align-items: stretch; }
  .course-select, .date-select { width: 100%; }
}
</style>