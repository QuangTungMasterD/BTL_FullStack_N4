<template>
  <div class="student-enrollments">
    <div class="page-header">
      <h1 class="page-title">Đăng ký khóa học</h1>
      <p class="page-subtitle">Học kỳ Fall 2024 • Thời gian đăng ký: 01/12 - 15/12/2024</p>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <div class="search-box">
        <v-icon icon="mdi-magnify" />
        <input type="text" placeholder="Tìm kiếm khóa học..." v-model="searchQuery" @input="loadAvailableCourses">
      </div>
      <div class="filter-options">
        <v-select
          v-model="selectedFaculty"
          :items="faculties"
          label="Khoa"
          variant="outlined"
          density="compact"
          hide-details
          style="width: 200px"
          @update:model-value="loadAvailableCourses"
        />
        <v-select
          v-model="selectedCredits"
          :items="creditOptions"
          label="Số tín chỉ"
          variant="outlined"
          density="compact"
          hide-details
          style="width: 150px"
          @update:model-value="loadAvailableCourses"
        />
      </div>
    </div>

    <div class="enrollments-grid">
      <!-- Available Courses -->
      <div class="courses-section">
        <div class="section-header">
          <h2>Khóa học có sẵn</h2>
          <span class="course-count">{{ availableCourses.length }} khóa học</span>
        </div>
        <div v-if="loading.available" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="courses-list">
          <div class="course-card" v-for="course in availableCourses" :key="course.id">
            <div class="course-code">{{ course.code }}</div>
            <h3 class="course-name">{{ course.name }}</h3>
            <div class="course-details">
              <div class="detail"><v-icon icon="mdi-credit-card" size="16" /><span>{{ course.credits }} tín chỉ</span></div>
              <div class="detail"><v-icon icon="mdi-account-group" size="16" /><span>{{ course.enrolled }}/{{ course.maxStudents }}</span></div>
              <div class="detail"><v-icon icon="mdi-clock" size="16" /><span>{{ course.schedule }}</span></div>
              <div class="detail"><v-icon icon="mdi-map-marker" size="16" /><span>{{ course.room }}</span></div>
            </div>
            <div class="lecturer-info"><v-icon icon="mdi-school" size="16" /><span>{{ course.lecturer }}</span></div>
            <button class="register-btn" @click="registerCourse(course)" :disabled="isRegistered(course.id)" :loading="registeringCourse === course.id">
              {{ isRegistered(course.id) ? 'Đã đăng ký' : 'Đăng ký' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Registered Courses -->
      <div class="registered-section">
        <div class="section-header">
          <h2>Khóa học đã đăng ký</h2>
          <span class="course-count">{{ registeredCourses.length }} khóa học</span>
        </div>
        <div v-if="loading.registered" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="registered-list">
          <div class="registered-card" v-for="course in registeredCourses" :key="course.id">
            <div class="registered-header">
              <div class="course-code">{{ course.code }}</div>
              <button class="remove-btn" @click="removeCourse(course.id)">
                <v-icon icon="mdi-close" size="16" />
              </button>
            </div>
            <h4 class="course-name">{{ course.name }}</h4>
            <div class="course-details">
              <span>{{ course.credits }} tín chỉ</span>
              <span>{{ course.schedule }}</span>
            </div>
          </div>
          <div v-if="registeredCourses.length === 0" class="empty-state">
            <v-icon icon="mdi-book-open" size="48" color="#cbd5e1" />
            <p>Chưa đăng ký khóa học nào</p>
          </div>
        </div>
        
        <!-- Timetable Preview -->
        <div class="timetable-preview">
          <h3>Thời khóa biểu</h3>
          <div class="timetable-list">
            <div class="timetable-item" v-for="item in timetable" :key="item.id">
              <div class="timetable-day">{{ item.day }}</div>
              <div class="timetable-info">
                <div class="timetable-course">{{ item.courseName }}</div>
                <div class="timetable-time">{{ item.time }} • {{ item.room }}</div>
              </div>
            </div>
            <div v-if="timetable.length === 0" class="empty-state">
              <p>Chưa có lịch học</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/utils/api'

const searchQuery = ref('')
const selectedFaculty = ref('all')
const selectedCredits = ref('all')
const registeringCourse = ref(null)

const loading = ref({ available: true, registered: true })

const faculties = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Công nghệ thông tin', value: 'CNTT' },
  { title: 'Quản trị kinh doanh', value: 'QTKD' },
  { title: 'Ngôn ngữ Anh', value: 'NNKT' }
]
const creditOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: '2 tín chỉ', value: 2 },
  { title: '3 tín chỉ', value: 3 },
  { title: '4 tín chỉ', value: 4 }
]

const availableCourses = ref([])
const registeredCourses = ref([])
const timetable = ref([])

const loadAvailableCourses = async () => {
  loading.value.available = true
  try {
    const params = {
      search: searchQuery.value || undefined,
      faculty: selectedFaculty.value !== 'all' ? selectedFaculty.value : undefined,
      credits: selectedCredits.value !== 'all' ? selectedCredits.value : undefined
    }
    const response = await api.get('/studentattendance/api/student/courses/available', { params })
    availableCourses.value = response.data
  } catch (error) {
    console.error('Failed to load available courses:', error)
  } finally {
    loading.value.available = false
  }
}

const loadRegisteredCourses = async () => {
  loading.value.registered = true
  try {
    const response = await api.get('/studentattendance/api/student/courses/registered')
    registeredCourses.value = response.data
  } catch (error) {
    console.error('Failed to load registered courses:', error)
  } finally {
    loading.value.registered = false
  }
}

const loadTimetable = async () => {
  try {
    const response = await api.get('/studentattendance/api/student/timetable')
    timetable.value = response.data
  } catch (error) {
    console.error('Failed to load timetable:', error)
  }
}

const isRegistered = (courseId) => {
  return registeredCourses.value.some(c => c.id === courseId)
}

const registerCourse = async (course) => {
  registeringCourse.value = course.id
  try {
    await api.post('/studentattendance/api/student/courses/register', { courseId: course.id })
    await Promise.all([loadRegisteredCourses(), loadTimetable()])
    await loadAvailableCourses()
  } catch (error) {
    console.error('Failed to register course:', error)
    alert(error.response?.data?.message || 'Đăng ký thất bại')
  } finally {
    registeringCourse.value = null
  }
}

const removeCourse = async (courseId) => {
  if (!confirm('Bạn có chắc muốn hủy đăng ký khóa học này?')) return
  try {
    await api.delete(`/studentattendance/api/student/courses/register/${courseId}`)
    await Promise.all([loadRegisteredCourses(), loadTimetable()])
    await loadAvailableCourses()
  } catch (error) {
    console.error('Failed to remove course:', error)
    alert(error.response?.data?.message || 'Hủy đăng ký thất bại')
  }
}

onMounted(() => {
  loadAvailableCourses()
  loadRegisteredCourses()
  loadTimetable()
})
</script>

<style scoped>
.student-enrollments {
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

.search-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 24px;
  margin-bottom: 32px;
  padding: 20px;
  background: white;
  border-radius: 20px;
}

.search-box {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px;
  background: #f8fafc;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
}

.search-box input {
  flex: 1;
  border: none;
  background: none;
  outline: none;
}

.filter-options {
  display: flex;
  gap: 12px;
}

.enrollments-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 32px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h2 {
  font-size: 20px;
  font-weight: 600;
  color: #1e293b;
}

.course-count {
  color: #64748b;
  font-size: 14px;
}

.loading-placeholder {
  display: flex;
  justify-content: center;
  padding: 40px;
}

.courses-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.course-card {
  background: white;
  border-radius: 16px;
  padding: 20px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.course-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
}

.course-code {
  display: inline-block;
  padding: 4px 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 20px;
  font-size: 12px;
  margin-bottom: 12px;
}

.course-name {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 12px;
  color: #1e293b;
}

.course-details {
  display: flex;
  gap: 16px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.detail {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #64748b;
}

.lecturer-info {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #64748b;
  margin-bottom: 16px;
}

.register-btn {
  width: 100%;
  padding: 10px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.register-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.register-btn:disabled {
  background: #cbd5e1;
  cursor: not-allowed;
}

.registered-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 24px;
}

.registered-card {
  background: #f8fafc;
  border-radius: 12px;
  padding: 16px;
}

.registered-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.remove-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #ef4444;
}

.timetable-preview {
  background: white;
  border-radius: 16px;
  padding: 20px;
  margin-top: 24px;
}

.timetable-preview h3 {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
}

.timetable-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.timetable-item {
  display: flex;
  gap: 12px;
  padding: 10px;
  border-radius: 8px;
  background: #f8fafc;
}

.timetable-day {
  min-width: 80px;
  font-weight: 600;
  color: #3b82f6;
}

.timetable-course {
  font-weight: 500;
  margin-bottom: 4px;
}

.timetable-time {
  font-size: 12px;
  color: #64748b;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 12px;
}

@media (max-width: 968px) {
  .search-section {
    flex-direction: column;
  }
  
  .filter-options {
    width: 100%;
  }
  
  .enrollments-grid {
    grid-template-columns: 1fr;
  }
}
</style>