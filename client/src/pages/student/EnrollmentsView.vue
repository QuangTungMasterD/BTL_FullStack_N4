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
          style="width: 180px"
          @update:model-value="loadAvailableCourses"
        />
        <v-select
          v-model="selectedCredits"
          :items="creditOptions"
          label="Số tín chỉ"
          variant="outlined"
          density="compact"
          hide-details
          style="width: 140px"
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
        <div v-else-if="availableCourses.length === 0" class="empty-state">
          <v-icon icon="mdi-book-open" size="48" color="#cbd5e1" />
          <p>Không tìm thấy khóa học nào</p>
        </div>
        <div v-else class="courses-list">
          <div class="course-card" v-for="course in availableCourses" :key="course.id">
            <div class="course-code">{{ course.code }}</div>
            <h3 class="course-name">{{ course.name }}</h3>
            <div class="course-details">
              <div class="detail">
                <v-icon icon="mdi-credit-card" size="16" />
                <span>{{ course.credits }} tín chỉ</span>
              </div>
              <div class="detail">
                <v-icon icon="mdi-account-group" size="16" />
                <span>{{ course.enrolled || 0 }}/{{ course.maxStudents || 40 }}</span>
              </div>
              <div class="detail">
                <v-icon icon="mdi-clock" size="16" />
                <span>{{ course.schedule || 'Chưa cập nhật' }}</span>
              </div>
              <div class="detail">
                <v-icon icon="mdi-map-marker" size="16" />
                <span>{{ course.room || 'Chưa cập nhật' }}</span>
              </div>
            </div>
            <div class="lecturer-info">
              <v-icon icon="mdi-school" size="16" />
              <span>{{ course.lecturer || 'Chưa cập nhật' }}</span>
            </div>
            <button 
              class="register-btn" 
              @click="registerCourse(course)" 
              :disabled="isRegistered(course.id) || registeringCourse === course.id"
            >
              <v-icon v-if="registeringCourse === course.id" icon="mdi-loading" spin size="20" />
              <span v-else>{{ isRegistered(course.id) ? 'Đã đăng ký' : 'Đăng ký' }}</span>
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
              <div>
                <span class="course-code">{{ course.code }}</span>
                <h4 class="course-name">{{ course.name }}</h4>
              </div>
              <button class="remove-btn" @click="removeCourse(course.id)" title="Hủy đăng ký">
                <v-icon icon="mdi-close" size="18" />
              </button>
            </div>
            <div class="course-details">
              <span>{{ course.credits }} tín chỉ</span>
              <span>{{ course.schedule || 'Chưa cập nhật' }}</span>
              <span>{{ course.room || 'Chưa cập nhật' }}</span>
            </div>
            <div class="lecturer-info">
              <v-icon icon="mdi-school" size="14" />
              <span>{{ course.lecturer || 'Chưa cập nhật' }}</span>
            </div>
          </div>
          <div v-if="registeredCourses.length === 0" class="empty-state">
            <v-icon icon="mdi-book-open" size="48" color="#cbd5e1" />
            <p>Chưa đăng ký khóa học nào</p>
            <p style="font-size: 13px; color: #94a3b8;">Hãy đăng ký khóa học từ danh sách bên trái</p>
          </div>
        </div>
        
        <!-- Timetable Preview -->
        <div class="timetable-preview">
          <h3>Thời khóa biểu</h3>
          <div v-if="loading.timetable" class="loading-placeholder" style="padding: 20px;">
            <v-progress-circular indeterminate size="32" />
          </div>
          <div v-else class="timetable-list">
            <div class="timetable-item" v-for="item in timetable" :key="item.id">
              <div class="timetable-day">{{ item.day }}</div>
              <div class="timetable-info">
                <div class="timetable-course">{{ item.courseName }}</div>
                <div class="timetable-time">{{ item.time }} • {{ item.room }}</div>
              </div>
            </div>
            <div v-if="timetable.length === 0" class="empty-state" style="padding: 20px;">
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

const loading = ref({ 
  available: true, 
  registered: true,
  timetable: true 
})

const faculties = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Công nghệ thông tin', value: 'CNTT' },
  { title: 'Quản trị kinh doanh', value: 'QTKD' },
  { title: 'Ngôn ngữ Anh', value: 'NNA' }
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
    const params = {}
    if (searchQuery.value) params.search = searchQuery.value
    if (selectedFaculty.value !== 'all') params.faculty = selectedFaculty.value
    if (selectedCredits.value !== 'all') params.credits = selectedCredits.value
    
    console.log('🔄 Đang tải khóa học có sẵn với params:', params)
    const response = await api.get('/api/student/courses/available', { params })
    console.log('✅ Khóa học có sẵn:', response.data)
    availableCourses.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load available courses:', error)
    // Fallback data
    availableCourses.value = [
      { id: 1, code: 'CS301', name: 'Cơ sở dữ liệu', credits: 3, 
        schedule: 'Thứ 5, 08:00-09:30', room: 'Phòng A104', 
        maxStudents: 40, enrolled: 25, lecturer: 'TS. Nguyễn Văn A' },
      { id: 4, code: 'NET101', name: 'Lập trình mạng cơ bản', credits: 3, 
        schedule: 'Thứ 6, 10:00-11:30', room: 'Phòng B201', 
        maxStudents: 35, enrolled: 18, lecturer: 'ThS. Trần Thị B' },
      { id: 5, code: 'AI101', name: 'Trí tuệ nhân tạo', credits: 4, 
        schedule: 'Thứ 7, 13:00-15:00', room: 'Phòng A205', 
        maxStudents: 30, enrolled: 28, lecturer: 'PGS. Lê Văn C' }
    ]
  } finally {
    loading.value.available = false
  }
}

const loadRegisteredCourses = async () => {
  loading.value.registered = true
  try {
    console.log('🔄 Đang tải khóa học đã đăng ký...')
    const response = await api.get('/api/student/courses/registered')
    console.log('✅ Khóa học đã đăng ký:', response.data)
    registeredCourses.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load registered courses:', error)
    registeredCourses.value = []
  } finally {
    loading.value.registered = false
  }
}

const loadTimetable = async () => {
  loading.value.timetable = true
  try {
    console.log('🔄 Đang tải thời khóa biểu...')
    const response = await api.get('/api/student/timetable')
    console.log('✅ Thời khóa biểu:', response.data)
    timetable.value = response.data || []
  } catch (error) {
    console.error('❌ Failed to load timetable:', error)
    timetable.value = []
  } finally {
    loading.value.timetable = false
  }
}

const isRegistered = (courseId) => {
  return registeredCourses.value.some(c => c.id === courseId || c.courseId === courseId)
}

const registerCourse = async (course) => {
  if (isRegistered(course.id)) {
    alert('Bạn đã đăng ký khóa học này rồi!')
    return
  }
  
  registeringCourse.value = course.id
  try {
    console.log(`📊 Đang đăng ký khóa học ${course.id}...`)
    await api.post('/api/student/courses/register', { courseId: course.id })
    console.log('✅ Đăng ký thành công!')
    
    // Reload all data
    await Promise.all([
      loadRegisteredCourses(),
      loadTimetable(),
      loadAvailableCourses()
    ])
    
    alert('Đăng ký khóa học thành công!')
  } catch (error) {
    console.error('❌ Failed to register course:', error)
    const message = error.response?.data?.message || 'Đăng ký thất bại. Vui lòng thử lại.'
    alert(message)
  } finally {
    registeringCourse.value = null
  }
}

const removeCourse = async (courseId) => {
  if (!confirm('Bạn có chắc muốn hủy đăng ký khóa học này?')) return
  
  try {
    console.log(`📊 Đang hủy đăng ký khóa học ${courseId}...`)
    await api.delete(`/api/student/courses/register/${courseId}`)
    console.log('✅ Hủy đăng ký thành công!')
    
    // Reload all data
    await Promise.all([
      loadRegisteredCourses(),
      loadTimetable(),
      loadAvailableCourses()
    ])
    
    alert('Hủy đăng ký thành công!')
  } catch (error) {
    console.error('❌ Failed to remove course:', error)
    const message = error.response?.data?.message || 'Hủy đăng ký thất bại. Vui lòng thử lại.'
    alert(message)
  }
}

onMounted(() => {
  console.log('🚀 Khởi tạo trang Đăng ký khóa học...')
  loadAvailableCourses()
  loadRegisteredCourses()
  loadTimetable()
  console.log('✅ Trang Đăng ký khóa học đã sẵn sàng')
})
</script>

<style scoped>
.student-enrollments {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 4px;
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
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
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
  font-size: 14px;
}

.search-box input::placeholder {
  color: #94a3b8;
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

.courses-section,
.registered-section {
  display: flex;
  flex-direction: column;
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

.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #94a3b8;
  background: #f8fafc;
  border-radius: 16px;
}

.empty-state p {
  margin-top: 12px;
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
  border: 1px solid #f1f5f9;
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
  font-weight: 500;
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
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
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
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
}

.registered-card:hover {
  background: #f1f5f9;
}

.registered-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
}

.registered-header .course-code {
  font-size: 11px;
  padding: 2px 10px;
  margin-bottom: 6px;
}

.registered-header .course-name {
  font-size: 16px;
  margin-bottom: 0;
}

.remove-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #ef4444;
  padding: 4px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.remove-btn:hover {
  background: #fee2e2;
}

.timetable-preview {
  background: white;
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  border: 1px solid #f1f5f9;
}

.timetable-preview h3 {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
  color: #1e293b;
}

.timetable-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.timetable-item {
  display: flex;
  gap: 12px;
  padding: 10px 14px;
  border-radius: 8px;
  background: #f8fafc;
  border-left: 4px solid #667eea;
}

.timetable-day {
  min-width: 80px;
  font-weight: 600;
  color: #3b82f6;
}

.timetable-info {
  flex: 1;
}

.timetable-course {
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 4px;
}

.timetable-time {
  font-size: 12px;
  color: #64748b;
}

@media (max-width: 968px) {
  .search-section {
    flex-direction: column;
  }
  
  .filter-options {
    width: 100%;
    flex-wrap: wrap;
  }
  
  .filter-options .v-select {
    flex: 1;
    min-width: 120px;
  }
  
  .enrollments-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .page-title {
    font-size: 24px;
  }
  
  .course-details {
    flex-direction: column;
    gap: 8px;
  }
  
  .filter-options {
    flex-direction: column;
  }
  
  .filter-options .v-select {
    width: 100% !important;
  }
}
</style>