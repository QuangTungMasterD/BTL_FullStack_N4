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
          <p class="hero-subtitle">Điểm danh học viên và theo dõi tỷ lệ chuyên cần tại trung tâm</p>
        </div>
      </div>
      <v-btn 
        color="success" 
        class="hero-btn" 
        @click="saveAttendance" 
        :loading="saving"
        :disabled="!canTakeAttendance"
      >
        <v-icon icon="mdi-content-save" class="mr-2" />
        Lưu điểm danh
      </v-btn>
    </div>

    <!-- Course & Date Selector -->
    <div class="selector-card">
      <div class="selector-row">
        <v-select 
          v-model="selectedCourse" 
          :items="courseOptions" 
          label="Chọn khóa học" 
          variant="outlined" 
          density="comfortable" 
          class="course-select" 
          hide-details 
          @update:model-value="onCourseChange"
        >
          <template v-slot:item="{ item }">
            <div class="course-option">
              <span class="course-name">{{ item.title }}</span>
              <span class="course-schedule">{{ item.raw.schedule }}</span>
            </div>
          </template>
        </v-select>
        <v-text-field
          v-model="selectedDate"
          label="Ngày điểm danh"
          type="date"
          variant="outlined"
          density="comfortable"
          class="date-select"
          hide-details
          @update:model-value="onDateChange"
        />
        <v-btn color="primary" class="take-btn" @click="loadStudents" :loading="loading.students">
          <v-icon icon="mdi-refresh" class="mr-2" />Tải danh sách
        </v-btn>
      </div>
      
      <!-- Schedule Validation Info -->
      <div class="schedule-info" v-if="selectedCourse && scheduleInfo">
        <div class="schedule-detail">
          <v-icon icon="mdi-clock" size="16" color="#64748b" />
          <span class="schedule-text">Lịch dạy: {{ scheduleInfo }}</span>
        </div>
        
        <div class="schedule-validation">
          <v-chip 
            :color="isValidScheduleDate ? 'success' : 'error'" 
            size="small" 
            variant="tonal"
          >
            <v-icon :icon="isValidScheduleDate ? 'mdi-check-circle' : 'mdi-alert-circle'" size="14" class="mr-1" />
            {{ isValidScheduleDate ? 'Đúng ngày dạy' : '⚠️ Không trùng với lịch dạy' }}
          </v-chip>
        </div>
        
        <div class="time-validation" v-if="isValidScheduleDate">
          <v-chip 
            :color="isValidTimeRange ? 'success' : 'warning'" 
            size="small" 
            variant="tonal"
          >
            <v-icon :icon="isValidTimeRange ? 'mdi-clock-check' : 'mdi-clock-alert'" size="14" class="mr-1" />
            {{ isValidTimeRange ? 'Đúng giờ dạy' : '⚠️ Ngoài khung giờ dạy' }}
          </v-chip>
        </div>
      </div>
      
      <div v-if="!canTakeAttendance && selectedCourse" class="attendance-warning">
        <v-alert 
          type="warning" 
          variant="tonal" 
          density="compact"
          class="mt-2"
        >
          <v-icon icon="mdi-alert" size="18" class="mr-2" />
          <span v-if="!isValidScheduleDate">Ngày điểm danh không khớp với lịch dạy của khóa học!</span>
          <span v-else-if="!isValidTimeRange">Hiện tại không trong khung giờ dạy ({{ scheduleTimeRange }})!</span>
          <span v-else>Chưa thể điểm danh</span>
        </v-alert>
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
        <div class="header-actions">
          <v-btn 
            size="small" 
            variant="text" 
            class="mark-all-btn" 
            @click="markAllPresent"
            :disabled="!canTakeAttendance"
          >
            <v-icon icon="mdi-check-all" size="16" class="mr-1" />Tất cả có mặt
          </v-btn>
          <v-btn 
            size="small" 
            variant="text" 
            class="mark-all-btn" 
            @click="markAllAbsent"
            :disabled="!canTakeAttendance"
          >
            <v-icon icon="mdi-close" size="16" class="mr-1" />Tất cả vắng
          </v-btn>
          <v-btn 
            size="small" 
            variant="text" 
            class="mark-all-btn" 
            @click="markAllLate"
            :disabled="!canTakeAttendance"
          >
            <v-icon icon="mdi-clock-alert" size="16" class="mr-1" />Tất cả muộn
          </v-btn>
        </div>
      </div>
      <div class="card-body">
        <div v-if="loading.students" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else-if="studentsList.length === 0" class="empty-state">
          <v-icon icon="mdi-account-off" size="56" color="#cbd5e1" />
          <p>Không có học viên nào trong khóa học này</p>
          <p class="empty-sub">Vui lòng chọn khóa học khác</p>
        </div>
        <div v-else class="attendance-table">
          <v-data-table
            :headers="attendanceHeaders"
            :items="studentsList"
            :items-per-page="10"
            hover
            class="modern-table"
          >
            <template v-slot:item.index="{ index }">
              <span>{{ index + 1 }}</span>
            </template>
            <template v-slot:item.status="{ item }">
              <v-select 
                v-model="item.status" 
                :items="statusOptions" 
                variant="outlined" 
                density="compact" 
                hide-details 
                style="width: 140px" 
                :color="getStatusColor(item.status)"
                :disabled="!canTakeAttendance"
              />
            </template>
            <template v-slot:item.checkInTime="{ item }">
              <div class="checkin-wrapper">
                <v-text-field 
                  v-model="item.checkInTime" 
                  placeholder="HH:MM" 
                  variant="outlined" 
                  density="compact" 
                  hide-details 
                  style="width: 100px" 
                  :disabled="item.status === 'absent' || !canTakeAttendance"
                />
                <v-menu offset-y v-if="canTakeAttendance">
                  <template v-slot:activator="{ props }">
                    <v-btn 
                      icon 
                      size="x-small" 
                      variant="text" 
                      v-bind="props"
                      :disabled="item.status === 'absent'"
                      class="time-picker-btn"
                    >
                      <v-icon icon="mdi-clock" size="16" />
                    </v-btn>
                  </template>
                  <v-card class="time-picker-card">
                    <v-card-text>
                      <div class="quick-times">
                        <v-btn 
                          v-for="time in quickTimes" 
                          :key="time"
                          size="small" 
                          variant="tonal" 
                          @click="setCheckInTime(item, time)"
                          class="time-btn"
                        >
                          {{ time }}
                        </v-btn>
                      </div>
                    </v-card-text>
                  </v-card>
                </v-menu>
              </div>
            </template>
            <template v-slot:item.note="{ item }">
              <v-text-field 
                v-model="item.note" 
                placeholder="Ghi chú..." 
                variant="outlined" 
                density="compact" 
                hide-details 
                style="width: 150px" 
                :disabled="!canTakeAttendance"
              />
            </template>
          </v-data-table>
        </div>
      </div>
    </div>

    <!-- History Card -->
    <div class="workspace-card">
      <div class="card-header">
        <h3>Lịch sử điểm danh</h3>
        <v-btn size="small" variant="text" color="primary" @click="loadHistory">
          <v-icon icon="mdi-refresh" size="16" class="mr-1" />Làm mới
        </v-btn>
      </div>
      <div class="card-body">
        <div v-if="loading.history" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="history-table">
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
                  <div class="rate-fill" :style="{ width: `${Math.min(item.rate, 100)}%`, background: getRateColor(item.rate) }"></div>
                </div>
                <span>{{ item.rate }}%</span>
              </div>
            </template>
            <template v-slot:item.actions="{ item }">
              <v-btn icon size="small" variant="text" class="view-btn" @click="viewHistoryDetail(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
            </template>
            <template v-slot:no-data>
              <div class="empty-state">
                <v-icon icon="mdi-calendar-blank" size="32" color="#cbd5e1" />
                <p>Chưa có lịch sử điểm danh</p>
              </div>
            </template>
          </v-data-table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/utils/api'

const route = useRoute()
const selectedCourse = ref('')
const selectedDate = ref(new Date().toISOString().split('T')[0])
const saving = ref(false)
const loading = ref({ students: false, history: false, schedule: false })

const courseOptions = ref([])
const statusOptions = [
  { title: 'Có mặt', value: 'present' },
  { title: 'Vắng mặt', value: 'absent' },
  { title: 'Đi muộn', value: 'late' }
]

const studentsList = ref([])
const attendanceHistory = ref([])
const scheduleInfo = ref('')
const scheduleTimeRange = ref('')
const isValidScheduleDate = ref(true)
const isValidTimeRange = ref(true)
const quickTimes = ['07:30', '08:00', '08:30', '09:00', '09:30', '10:00', '13:30', '14:00', '14:30', '15:00', '16:00', '17:00']

// Computed
const presentCount = computed(() => studentsList.value.filter(s => s.status === 'present').length)
const absentCount = computed(() => studentsList.value.filter(s => s.status === 'absent').length)
const lateCount = computed(() => studentsList.value.filter(s => s.status === 'late').length)
const totalStudents = computed(() => studentsList.value.length)

// Check if can take attendance
const canTakeAttendance = computed(() => {
  if (!selectedCourse.value) return false
  return isValidScheduleDate.value && isValidTimeRange.value
})

// Headers
const attendanceHeaders = [
  { title: 'STT', key: 'index', sortable: false, width: 60, align: 'center' },
  { title: 'Mã HV', key: 'studentId', sortable: true, align: 'start' },
  { title: 'Họ tên', key: 'fullName', sortable: true, align: 'start' },
  { title: 'Trạng thái', key: 'status', align: 'center' },
  { title: 'Giờ điểm danh', key: 'checkInTime', align: 'center' },
  { title: 'Ghi chú', key: 'note', align: 'center' },
]

const historyHeaders = [
  { title: 'Ngày', key: 'date', sortable: true, align: 'start' },
  { title: 'Buổi học', key: 'session', align: 'start' },
  { title: 'Sĩ số', key: 'total', align: 'center' },
  { title: 'Có mặt', key: 'present', align: 'center' },
  { title: 'Vắng mặt', key: 'absent', align: 'center' },
  { title: 'Muộn', key: 'late', align: 'center' },
  { title: 'Tỷ lệ', key: 'rate', align: 'center' },
  { title: 'Thao tác', key: 'actions', sortable: false, align: 'center', width: 80 },
]

// Load courses
const loadCourses = async () => {
  try {
    console.log('🔄 Đang tải danh sách khóa học...')
    const response = await api.get('/api/lecturer/courses')
    console.log('✅ Danh sách khóa học fetch thành công:', response.data)
    courseOptions.value = response.data.map(c => ({ 
      title: c.courseName, 
      value: c.id,
      schedule: c.schedule,
      code: c.code
    }))
    
    if (route.query.course) {
      selectedCourse.value = parseInt(route.query.course)
    } else if (courseOptions.value.length > 0) {
      selectedCourse.value = courseOptions.value[0].value
    }
    
    if (selectedCourse.value) {
      await checkSchedule()
      await loadStudents()
      await loadHistory()
    }
  } catch (error) {
    console.error('❌ Failed to load courses:', error)
    courseOptions.value = [
      { title: 'Lập trình Python cơ bản', value: 1, schedule: 'Thứ 2, 13:30-16:30', code: 'PY101' },
      { title: 'Lập trình Java cơ bản', value: 2, schedule: 'Thứ 3, 08:00-11:00', code: 'JA101' },
    ]
    selectedCourse.value = 1
    await checkSchedule()
  }
}

// Check schedule
const checkSchedule = async () => {
  const selected = courseOptions.value.find(c => c.value === selectedCourse.value)
  if (selected && selected.schedule) {
    scheduleInfo.value = selected.schedule
    scheduleTimeRange.value = selected.schedule.split(',')[1]?.trim() || ''
    
    // Kiểm tra ngày hiện tại có khớp với lịch không
    const selectedDateObj = new Date(selectedDate.value)
    const dayMap = { 'Thứ 2': 1, 'Thứ 3': 2, 'Thứ 4': 3, 'Thứ 5': 4, 'Thứ 6': 5, 'Thứ 7': 6, 'Chủ nhật': 0 }
    const scheduleDay = selected.schedule.split(',')[0]?.trim()
    const dayNumber = dayMap[scheduleDay]
    
    if (dayNumber !== undefined) {
      isValidScheduleDate.value = selectedDateObj.getDay() === dayNumber
    } else {
      isValidScheduleDate.value = true
    }
    
    // Kiểm tra giờ hiện tại có trong khung giờ dạy không
    if (isValidScheduleDate.value && scheduleTimeRange.value) {
      const now = new Date()
      const currentHour = now.getHours()
      const currentMinute = now.getMinutes()
      const currentTime = currentHour * 60 + currentMinute
      
      // Parse time range (e.g., "13:30-16:30")
      const timeParts = scheduleTimeRange.value.split('-')
      if (timeParts.length === 2) {
        const startParts = timeParts[0].trim().split(':')
        const endParts = timeParts[1].trim().split(':')
        const startTime = parseInt(startParts[0]) * 60 + parseInt(startParts[1])
        const endTime = parseInt(endParts[0]) * 60 + parseInt(endParts[1])
        
        // Cho phép điểm danh trong khoảng 30 phút trước giờ học và 15 phút sau giờ học
        const startBuffer = 30 // 30 phút trước
        const endBuffer = 15 // 15 phút sau
        isValidTimeRange.value = currentTime >= (startTime - startBuffer) && currentTime <= (endTime + endBuffer)
      } else {
        isValidTimeRange.value = true
      }
    } else {
      isValidTimeRange.value = true
    }
  }
}

// On course change
const onCourseChange = async () => {
  await checkSchedule()
  await loadStudents()
  await loadHistory()
}

// On date change
const onDateChange = async () => {
  await checkSchedule()
  await loadStudents()
}

// Load students for attendance
const loadStudents = async () => {
  if (!selectedCourse.value) {
    console.warn('⚠️ Chưa chọn khóa học')
    return
  }
  
  loading.value.students = true
  try {
    console.log(`🔄 Đang tải danh sách học viên cho khóa ${selectedCourse.value}...`)
    const response = await api.get('/api/lecturer/attendance/students', {
      params: { 
        courseId: selectedCourse.value,
        date: selectedDate.value 
      }
    })
    console.log('✅ Danh sách học viên fetch thành công:', response.data)
    
    const data = response.data || []
    studentsList.value = data.map((s, idx) => ({
      ...s,
      index: idx + 1,
      status: s.status || 'present',
      checkInTime: s.checkInTime || '',
      note: s.note || ''
    }))
  } catch (error) {
    console.error('❌ Failed to load students:', error)
    studentsList.value = [
      { id: 1, studentId: 'HV001', fullName: 'Nguyễn Văn An', status: 'present', checkInTime: '08:00', note: '' },
      { id: 2, studentId: 'HV002', fullName: 'Trần Thị Ngọc Bích', status: 'present', checkInTime: '08:15', note: '' },
      { id: 3, studentId: 'HV003', fullName: 'Lê Văn Cường', status: 'late', checkInTime: '08:30', note: 'Đi muộn 15 phút' },
      { id: 4, studentId: 'HV004', fullName: 'Phạm Thị Phương Dung', status: 'absent', checkInTime: '', note: '' },
      { id: 5, studentId: 'HV005', fullName: 'Hoàng Văn Minh Em', status: 'present', checkInTime: '08:00', note: '' },
    ]
  } finally {
    loading.value.students = false
  }
}

// Load attendance history
const loadHistory = async () => {
  if (!selectedCourse.value) return
  
  loading.value.history = true
  try {
    console.log(`🔄 Đang tải lịch sử điểm danh cho khóa ${selectedCourse.value}...`)
    const response = await api.get('/api/lecturer/attendance/history', {
      params: { courseId: selectedCourse.value }
    })
    console.log('✅ Lịch sử điểm danh fetch thành công:', response.data)
    attendanceHistory.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load history:', error)
    attendanceHistory.value = [
      { id: 1, date: '15/06/2024', session: 'Buổi 1 - Tuần 12', total: 25, present: 20, absent: 3, late: 2, rate: 80 },
      { id: 2, date: '08/06/2024', session: 'Buổi 2 - Tuần 11', total: 25, present: 22, absent: 2, late: 1, rate: 88 },
      { id: 3, date: '01/06/2024', session: 'Buổi 1 - Tuần 11', total: 25, present: 18, absent: 5, late: 2, rate: 72 },
    ]
  } finally {
    loading.value.history = false
  }
}

// Helper functions
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

// Set check-in time
const setCheckInTime = (item, time) => {
  item.checkInTime = time
}

// Mark all as present
const markAllPresent = () => {
  const now = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  studentsList.value.forEach(s => {
    s.status = 'present'
    s.checkInTime = now
  })
}

// Mark all as absent
const markAllAbsent = () => {
  studentsList.value.forEach(s => {
    s.status = 'absent'
    s.checkInTime = ''
  })
}

// Mark all as late
const markAllLate = () => {
  const now = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  studentsList.value.forEach(s => {
    s.status = 'late'
    s.checkInTime = now
  })
}

// Save attendance
const saveAttendance = async () => {
  if (!canTakeAttendance.value) {
    alert('Không thể điểm danh vì không đúng ngày/giờ dạy!')
    return
  }
  
  if (!selectedCourse.value) {
    alert('Vui lòng chọn khóa học')
    return
  }
  
  if (studentsList.value.length === 0) {
    alert('Không có học viên để điểm danh')
    return
  }
  
  saving.value = true
  try {
    console.log('📊 Đang lưu điểm danh...')
    const records = studentsList.value.map(s => ({
      id: s.id,
      status: s.status,
      checkInTime: s.checkInTime || null,
      note: s.note || null
    }))
    
    await api.post('/api/lecturer/attendance/save', {
      courseId: selectedCourse.value,
      date: selectedDate.value,
      records: records
    })
    console.log('✅ Lưu điểm danh thành công')
    alert('Đã lưu điểm danh thành công!')
    await loadHistory()
  } catch (error) {
    console.error('❌ Failed to save attendance:', error)
    alert('Lưu điểm danh thất bại: ' + (error.response?.data?.message || error.message))
  } finally {
    saving.value = false
  }
}

// View history detail
const viewHistoryDetail = (record) => {
  console.log('View detail:', record)
  // TODO: Implement detail view
}

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Quản lý điểm danh...')
  loadCourses()
  console.log('✅ Trang Quản lý điểm danh đã sẵn sàng')
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

.hero-btn {
  padding: 10px 24px;
  border-radius: 40px;
  font-weight: 600;
  text-transform: none;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.hero-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
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
  width: 350px;
}

.course-option {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.course-name {
  font-weight: 500;
}

.course-schedule {
  font-size: 12px;
  color: #64748b;
}

.date-select {
  width: 200px;
}

.take-btn {
  padding: 10px 20px;
  border-radius: 12px;
  text-transform: none;
}

/* Schedule Info */
.schedule-info {
  display: flex;
  gap: 16px;
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #eef2f6;
  flex-wrap: wrap;
  align-items: center;
}

.schedule-detail {
  display: flex;
  align-items: center;
  gap: 6px;
}

.schedule-text {
  font-size: 14px;
  color: #475569;
  font-weight: 500;
}

.schedule-validation, .time-validation {
  display: flex;
  align-items: center;
}

.attendance-warning {
  margin-top: 12px;
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

.stat-icon-wrapper.present { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.absent { background: linear-gradient(135deg, #fee2e2, #fecaca); color: #ef4444; }
.stat-icon-wrapper.late { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }
.stat-icon-wrapper.total { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }

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
  flex-wrap: wrap;
  gap: 12px;
}

.card-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
}

.header-actions {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.mark-all-btn {
  border-radius: 10px;
  text-transform: none;
}

.mark-all-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.card-body {
  padding: 24px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 40px;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 16px;
  font-size: 14px;
}

.empty-sub {
  font-size: 12px !important;
  margin-top: 4px !important;
  color: #cbd5e1;
}

/* Table */
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
  border-top: 1px solid #eef2f6;
}

/* Check-in Wrapper */
.checkin-wrapper {
  display: flex;
  align-items: center;
  gap: 4px;
}

.time-picker-btn {
  transition: all 0.2s ease;
}

.time-picker-btn:hover {
  background: #dbeafe;
  color: #3b82f6;
}

.time-picker-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.time-picker-card {
  border-radius: 12px !important;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

.quick-times {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
  padding: 8px;
}

.time-btn {
  border-radius: 8px !important;
  font-size: 12px !important;
  min-width: 60px;
  transition: all 0.2s ease;
}

.time-btn:hover {
  background: #3b82f6 !important;
  color: white !important;
}

/* Rate */
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
  transition: width 0.5s ease;
}

.view-btn {
  transition: all 0.2s ease;
}
.view-btn:hover {
  background: #dbeafe;
  color: #3b82f6;
}

/* Responsive */
@media (max-width: 968px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
  .selector-row { flex-direction: column; align-items: stretch; }
  .course-select, .date-select { width: 100%; }
  .hero-header { flex-direction: column; align-items: flex-start; }
  .card-header { flex-direction: column; align-items: flex-start; }
  .schedule-info { flex-direction: column; align-items: flex-start; }
}

@media (max-width: 480px) {
  .stats-grid-modern { grid-template-columns: 1fr; }
  .quick-times { grid-template-columns: repeat(2, 1fr); }
}
</style>