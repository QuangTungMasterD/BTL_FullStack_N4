<template>
  <div class="course-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-book-open-variant" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý khóa học</h1>
          <p class="hero-subtitle">Quản lý danh sách khóa học và phân công giảng viên tại trung tâm</p>
        </div>
      </div>
      <div class="hero-actions">
        <v-btn color="primary" class="hero-btn" @click="openAddDialog">
          <v-icon icon="mdi-plus" class="mr-2" />
          Thêm khóa học
        </v-btn>
        <v-btn variant="tonal" class="hero-btn-outline" @click="goToTrash">
          <v-icon icon="mdi-delete" class="mr-2" />
          Thùng rác
        </v-btn>
      </div>
    </div>

    <!-- Statistics Section -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-book-open-variant" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalCourses }}</div>
          <div class="stat-label">Tổng khóa học</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper active">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ activeCourses }}</div>
          <div class="stat-label">Đang mở</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper credits">
          <v-icon icon="mdi-credit-card" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalCredits }}</div>
          <div class="stat-label">Tổng tín chỉ</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper enrollment">
          <v-icon icon="mdi-account-group" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalEnrollments.toLocaleString() }}</div>
          <div class="stat-label">Lượt đăng ký</div>
          <div class="stat-trend" v-if="popularCourse">🔥 {{ popularCourse }}</div>
        </div>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="filter-bar">
      <div class="filter-group">
        <div class="search-wrapper">
          <v-icon icon="mdi-magnify" size="20" class="search-icon" />
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="Tìm kiếm khóa học..." 
            class="search-input-modern"
            @input="loadCourses"
          />
        </div>
        <v-select
          v-model="selectedFaculty"
          :items="facultyOptions"
          label="Khóa học"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="loadCourses"
        />
        <v-select
          v-model="selectedSemester"
          :items="semesterOptions"
          label="Học kỳ"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="loadCourses"
        />
        <v-select
          v-model="selectedStatus"
          :items="statusOptions"
          label="Trạng thái"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="loadCourses"
        />
      </div>
      <div class="filter-actions">
        <v-btn variant="text" color="primary" @click="resetFilters" class="reset-btn">
          <v-icon icon="mdi-filter-remove" size="18" class="mr-1" />
          Xóa lọc
        </v-btn>
      </div>
    </div>

    <!-- Course Cards Grid -->
    <div class="workspace-card">
      <div class="workspace-toolbar">
        <div class="toolbar-left">
          <span class="total-records">Tổng: {{ totalCourses }} khóa học</span>
        </div>
        <div class="toolbar-right">
          <v-btn variant="tonal" color="primary" @click="exportData" class="export-btn">
            <v-icon icon="mdi-export" size="18" class="mr-1" />
            Xuất Excel
          </v-btn>
        </div>
      </div>

      <div v-if="loading" class="loading-container">
        <v-icon icon="mdi-loading" size="40" class="spin" />
        <p>Đang tải dữ liệu...</p>
      </div>

      <div v-else class="courses-grid">
        <div class="course-card-modern" v-for="course in coursesData" :key="course.id">
          <div class="course-card-header" :style="{ background: getCourseGradient(course.id) }">
            <div class="course-code-badge">{{ course.code }}</div>
            <div class="course-actions">
              <v-btn icon size="small" variant="text" class="card-action-btn" @click="editCourse(course)">
                <v-icon icon="mdi-pencil" size="18" color="white" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="card-action-btn" @click="deleteCourse(course)">
                <v-icon icon="mdi-delete" size="18" color="white" />
              </v-btn>
            </div>
          </div>
          <div class="course-card-body">
            <h3 class="course-name-modern">{{ course.name }}</h3>
            <div class="course-info-grid">
              <div class="course-info-item">
                <v-icon icon="mdi-credit-card" size="14" class="info-icon" />
                <span>{{ course.credits }} tín chỉ</span>
              </div>
              <div class="course-info-item">
                <v-icon icon="mdi-school" size="14" class="info-icon" />
                <span>{{ course.lecturer || 'Chưa phân công' }}</span>
              </div>
              <div class="course-info-item">
                <v-icon icon="mdi-domain" size="14" class="info-icon" />
                <span>{{ course.faculty }}</span>
              </div>
              <div class="course-info-item">
                <v-icon icon="mdi-calendar" size="14" class="info-icon" />
                <span>{{ course.semester }}</span>
              </div>
            </div>
            <div class="course-schedule-info" v-if="course.schedule">
              <v-icon icon="mdi-clock" size="14" class="info-icon" />
              <span>{{ course.schedule }}</span>
            </div>
            <div class="course-room-info" v-if="course.room">
              <v-icon icon="mdi-map-marker" size="14" class="info-icon" />
              <span>{{ course.room }}</span>
            </div>
            <div class="course-enrollment-progress">
              <div class="progress-bar-container">
                <div class="progress-bar-bg">
                  <div class="progress-fill" :style="{ width: `${Math.min((course.enrolled / course.maxStudents) * 100, 100)}%` }"></div>
                </div>
                <span class="progress-text">{{ course.enrolled || 0 }}/{{ course.maxStudents }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="!loading && coursesData.length === 0" class="empty-state">
        <v-icon icon="mdi-book-open-off" size="56" color="#cbd5e1" />
        <p>Không có dữ liệu khóa học</p>
        <v-btn color="primary" variant="tonal" @click="openAddDialog" class="mt-3">
          <v-icon icon="mdi-plus" class="mr-2" />
          Thêm khóa học mới
        </v-btn>
      </div>
    </div>

    <!-- Add/Edit Course Dialog -->
    <v-dialog v-model="dialogVisible" max-width="560px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-modern" :class="isEditing ? 'edit-mode' : 'create-mode'">
          <div class="dialog-header-icon">
            <v-icon :icon="isEditing ? 'mdi-pencil' : 'mdi-book-plus'" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">{{ isEditing ? 'Chỉnh sửa khóa học' : 'Thêm khóa học mới' }}</h2>
            <p class="dialog-subtitle">{{ isEditing ? 'Cập nhật thông tin khóa học' : 'Nhập thông tin để tạo khóa học mới' }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <v-form ref="form" v-model="formValid">
            <div class="form-grid">
              <v-text-field 
                v-model="formData.code" 
                label="Mã khóa học" 
                variant="outlined" 
                density="comfortable" 
                :disabled="isEditing" 
                prepend-inner-icon="mdi-identifier" 
              />
              <v-text-field 
                v-model="formData.name" 
                label="Tên khóa học" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-book-open" 
              />
              <v-text-field 
                v-model="formData.credits" 
                label="Số tín chỉ" 
                type="number" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-credit-card" 
              />
              <v-select 
                v-model="formData.faculty" 
                :items="facultyList" 
                label="Khóa học" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-domain" 
              />
              <v-select 
                v-model="formData.lecturerId" 
                :items="lecturerList" 
                label="Giảng viên" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-school" 
                item-title="title"
                item-value="value"
              />
              <v-text-field 
                v-model="formData.schedule" 
                label="Lịch học" 
                placeholder="Thứ 2, 13:30-16:30" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-clock" 
              />
              <v-text-field 
                v-model="formData.room" 
                label="Phòng học" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-map-marker" 
              />
              <v-select 
                v-model="formData.semester" 
                :items="semesterOptions" 
                label="Học kỳ" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-calendar" 
              />
              <v-text-field 
                v-model="formData.maxStudents" 
                label="Sĩ số tối đa" 
                type="number" 
                variant="outlined" 
                density="comfortable" 
                prepend-inner-icon="mdi-account-group" 
              />
            </div>
          </v-form>
        </v-card-text>
        <v-divider />
        <v-card-actions class="dialog-actions-modern">
          <v-btn variant="text" class="cancel-btn" @click="dialogVisible = false">Hủy</v-btn>
          <v-btn :color="isEditing ? 'warning' : 'primary'" class="save-btn" @click="saveCourse" :loading="saving">
            {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/utils/api'

const router = useRouter()
const searchQuery = ref('')
const selectedFaculty = ref('all')
const selectedSemester = ref('all')
const selectedStatus = ref('all')
const dialogVisible = ref(false)
const isEditing = ref(false)
const loading = ref(false)
const saving = ref(false)
const formValid = ref(true)

// Statistics data
const totalCourses = ref(0)
const totalCredits = ref(0)
const activeCourses = ref(0)
const totalEnrollments = ref(0)
const popularCourse = ref('')

// Options
const facultyOptions = [
  { title: 'Tất cả khóa học', value: 'all' },
  { title: 'Python', value: 'Python' },
  { title: 'Java', value: 'Java' },
  { title: 'Tiếng Anh giao tiếp', value: 'English' },
  { title: 'Toán cao cấp', value: 'Math' },
  { title: 'Kỹ năng mềm', value: 'SoftSkills' },
]

const facultyList = ['Python', 'Java', 'English', 'Math', 'SoftSkills', 'Data Science', 'AI/ML']

const statusOptions = [
  { title: 'Tất cả trạng thái', value: 'all' },
  { title: 'Đang mở', value: 'active' },
  { title: 'Đã đóng', value: 'closed' },
  { title: 'Sắp khai giảng', value: 'upcoming' },
]

const semesterOptions = ref(['Fall 2024', 'Spring 2025', 'Summer 2025'])
const lecturerList = ref([])

const coursesData = ref([])

const formData = ref({
  code: '',
  name: '',
  credits: 3,
  faculty: '',
  lecturerId: null,
  schedule: '',
  room: '',
  semester: 'Fall 2024',
  maxStudents: 60
})

// Mock data
const mockStats = {
  totalCourses: 156,
  totalCredits: 486,
  activeCourses: 42,
  totalEnrollments: 2847,
  popularCourseName: 'Nhập môn lập trình',
  popularCourseEnrollment: 156
}

// Load course statistics
const loadCourseStatistics = async () => {
  try {
    console.log('🔄 Đang tải thống kê khóa học...')
    const response = await api.get('/api/admin/courses/stats')
    console.log('✅ Thống kê khóa học fetch thành công:', response.data)
    const data = response.data
    totalCourses.value = data.totalCourses || 0
    totalCredits.value = data.totalCredits || 0
    activeCourses.value = data.activeCourses || 0
    totalEnrollments.value = data.totalEnrollments || 0
    popularCourse.value = `${data.popularCourseName || 'N/A'} (${data.popularCourseEnrollment || 0} HV)`
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    totalCourses.value = mockStats.totalCourses
    totalCredits.value = mockStats.totalCredits
    activeCourses.value = mockStats.activeCourses
    totalEnrollments.value = mockStats.totalEnrollments
    popularCourse.value = `${mockStats.popularCourseName} (${mockStats.popularCourseEnrollment} HV)`
  }
}

// Load semesters
const loadSemesters = async () => {
  try {
    const response = await api.get('/api/admin/semesters')
    semesterOptions.value = response.data || ['Fall 2024', 'Spring 2024', 'Summer 2024']
  } catch (error) {
    console.error('❌ Failed to load semesters, using mock data:', error)
    semesterOptions.value = ['Fall 2024', 'Spring 2024', 'Summer 2024', 'Fall 2023']
  }
}

// Load lecturers
const loadLecturers = async () => {
  try {
    const response = await api.get('/api/admin/lecturers')
    const items = response.data.items || response.data || []
    lecturerList.value = items.map(l => ({ title: l.fullName, value: l.id }))
  } catch (error) {
    console.error('❌ Failed to load lecturers, using mock data:', error)
    lecturerList.value = [
      { title: 'PGS.TS Trần Văn X', value: 1 },
      { title: 'TS. Nguyễn Thị Y', value: 2 },
      { title: 'ThS. Lê Văn Z', value: 3 }
    ]
  }
}

// Load courses
const loadCourses = async () => {
  loading.value = true
  try {
    const params = {}
    if (searchQuery.value) params.search = searchQuery.value
    if (selectedFaculty.value !== 'all') params.faculty = selectedFaculty.value
    if (selectedSemester.value !== 'all') params.semester = selectedSemester.value
    if (selectedStatus.value !== 'all') params.status = selectedStatus.value

    console.log('🔄 Đang tải danh sách khóa học với params:', params)
    const response = await api.get('/api/admin/courses', { params })
    console.log('✅ Danh sách khóa học fetch thành công:', response.data)
    
    const items = response.data.items || response.data || []
    
    if (items && items.length > 0) {
      coursesData.value = items
    } else {
      console.warn('⚠️ Không có dữ liệu khóa học, sử dụng mock data')
      // Mock data fallback
      coursesData.value = [
        { id: 1, code: 'PY101', name: 'Lập trình Python cơ bản', credits: 3, faculty: 'Python', semester: 'Fall 2024', lecturer: 'PGS.TS Trần Văn X', schedule: 'Thứ 2, 13:30-16:30', room: 'A101', enrolled: 45, maxStudents: 60 },
        { id: 2, code: 'JA101', name: 'Lập trình Java cơ bản', credits: 4, faculty: 'Java', semester: 'Fall 2024', lecturer: 'TS. Nguyễn Thị Y', schedule: 'Thứ 3, 08:00-11:00', room: 'B202', enrolled: 42, maxStudents: 60 },
        { id: 3, code: 'EN101', name: 'Tiếng Anh giao tiếp', credits: 2, faculty: 'English', semester: 'Fall 2024', lecturer: 'ThS. Lê Văn Z', schedule: 'Thứ 4, 13:30-16:30', room: 'C303', enrolled: 50, maxStudents: 60 },
        { id: 4, code: 'MA101', name: 'Toán cao cấp', credits: 3, faculty: 'Math', semester: 'Fall 2024', lecturer: 'PGS.TS Trần Văn X', schedule: 'Thứ 5, 08:00-11:00', room: 'D404', enrolled: 38, maxStudents: 60 },
        { id: 5, code: 'SK101', name: 'Kỹ năng mềm', credits: 2, faculty: 'SoftSkills', semester: 'Fall 2024', lecturer: 'ThS. Lê Văn Z', schedule: 'Thứ 6, 13:30-16:30', room: 'E505', enrolled: 55, maxStudents: 60 },
      ]
    }
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    coursesData.value = [
      { id: 1, code: 'PY101', name: 'Lập trình Python cơ bản', credits: 3, faculty: 'Python', semester: 'Fall 2024', lecturer: 'PGS.TS Trần Văn X', schedule: 'Thứ 2, 13:30-16:30', room: 'A101', enrolled: 45, maxStudents: 60 },
      { id: 2, code: 'JA101', name: 'Lập trình Java cơ bản', credits: 4, faculty: 'Java', semester: 'Fall 2024', lecturer: 'TS. Nguyễn Thị Y', schedule: 'Thứ 3, 08:00-11:00', room: 'B202', enrolled: 42, maxStudents: 60 },
      { id: 3, code: 'EN101', name: 'Tiếng Anh giao tiếp', credits: 2, faculty: 'English', semester: 'Fall 2024', lecturer: 'ThS. Lê Văn Z', schedule: 'Thứ 4, 13:30-16:30', room: 'C303', enrolled: 50, maxStudents: 60 },
      { id: 4, code: 'MA101', name: 'Toán cao cấp', credits: 3, faculty: 'Math', semester: 'Fall 2024', lecturer: 'PGS.TS Trần Văn X', schedule: 'Thứ 5, 08:00-11:00', room: 'D404', enrolled: 38, maxStudents: 60 },
      { id: 5, code: 'SK101', name: 'Kỹ năng mềm', credits: 2, faculty: 'SoftSkills', semester: 'Fall 2024', lecturer: 'ThS. Lê Văn Z', schedule: 'Thứ 6, 13:30-16:30', room: 'E505', enrolled: 55, maxStudents: 60 },
    ]
  } finally {
    loading.value = false
  }
}

const goToTrash = () => {
  router.push('/courses/trash')
}

const resetFilters = () => {
  searchQuery.value = ''
  selectedFaculty.value = 'all'
  selectedSemester.value = 'all'
  selectedStatus.value = 'all'
  loadCourses()
}

const getCourseGradient = (id) => {
  const gradients = [
    'linear-gradient(135deg, #3b82f6, #2563eb)',
    'linear-gradient(135deg, #10b981, #059669)',
    'linear-gradient(135deg, #8b5cf6, #7c3aed)',
    'linear-gradient(135deg, #f59e0b, #d97706)',
    'linear-gradient(135deg, #ef4444, #dc2626)',
    'linear-gradient(135deg, #14b8a6, #0d9488)',
  ]
  return gradients[(id - 1) % gradients.length]
}

const openAddDialog = () => {
  isEditing.value = false
  formData.value = { 
    code: '', 
    name: '', 
    credits: 3, 
    faculty: '', 
    lecturerId: null, 
    schedule: '', 
    room: '', 
    semester: 'Fall 2024', 
    maxStudents: 60 
  }
  dialogVisible.value = true
}

const editCourse = (course) => {
  isEditing.value = true
  formData.value = {
    id: course.id,
    code: course.code,
    name: course.name,
    credits: course.credits,
    faculty: course.faculty,
    lecturerId: null,
    schedule: course.schedule || '',
    room: course.room || '',
    semester: course.semester,
    maxStudents: course.maxStudents
  }
  dialogVisible.value = true
}

const deleteCourse = async (course) => {
  if (confirm(`Bạn có chắc muốn xóa khóa học ${course.name}?`)) {
    try {
      await api.delete(`/api/admin/courses/${course.id}`)
      await loadCourses()
      await loadCourseStatistics()
    } catch (error) {
      console.error('❌ Delete failed, using local:', error)
      const index = coursesData.value.findIndex(c => c.id === course.id)
      if (index > -1) coursesData.value.splice(index, 1)
    }
  }
}

const saveCourse = async () => {
  saving.value = true
  try {
    if (isEditing.value) {
      await api.put(`/api/admin/courses/${formData.value.id}`, formData.value)
    } else {
      await api.post('/api/admin/courses', formData.value)
    }
    await loadCourses()
    await loadCourseStatistics()
    dialogVisible.value = false
  } catch (error) {
    console.error('❌ Save failed, using local:', error)
    if (isEditing.value) {
      const index = coursesData.value.findIndex(c => c.id === formData.value.id)
      if (index > -1) coursesData.value[index] = { ...formData.value, enrolled: coursesData.value[index].enrolled }
    } else {
      const newId = Math.max(...coursesData.value.map(c => c.id), 0) + 1
      coursesData.value.push({ ...formData.value, id: newId, enrolled: 0 })
    }
    dialogVisible.value = false
  } finally {
    saving.value = false
  }
}

const exportData = () => { 
  console.log('📊 Export data...')
  alert('Chức năng xuất Excel đang được phát triển')
}

onMounted(() => {
  console.log('🚀 Khởi tạo trang Quản lý khóa học...')
  loadCourseStatistics()
  loadCourses()
  loadLecturers()
  loadSemesters()
  console.log('✅ Trang Quản lý khóa học đã sẵn sàng')
})
</script>

<style scoped>
/* Giữ nguyên style như cũ */
.course-management {
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

.hero-actions {
  display: flex;
  gap: 12px;
}

.hero-btn {
  padding: 10px 24px;
  border-radius: 40px;
  font-weight: 600;
  text-transform: none;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.hero-btn-outline {
  padding: 10px 24px;
  border-radius: 40px;
  font-weight: 600;
  text-transform: none;
  color: white !important;
  border-color: rgba(255, 255, 255, 0.3) !important;
}

/* Statistics */
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

.stat-icon-wrapper.total { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }
.stat-icon-wrapper.active { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.credits { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }
.stat-icon-wrapper.enrollment { background: linear-gradient(135deg, #e9d5ff, #d8b4fe); color: #8b5cf6; }

.stat-info-modern .stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #1e293b;
  line-height: 1.2;
}

.stat-info-modern .stat-label {
  font-size: 13px;
  color: #64748b;
  margin-top: 4px;
}

.stat-trend {
  font-size: 12px;
  color: #f59e0b;
  margin-top: 4px;
}

/* Filter Bar */
.filter-bar {
  background: white;
  border-radius: 20px;
  padding: 16px 24px;
  margin-bottom: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.filter-group {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  flex: 1;
}

.search-wrapper {
  position: relative;
  min-width: 250px;
  flex: 1;
}

.search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
}

.search-input-modern {
  width: 100%;
  padding: 10px 16px 10px 44px;
  border: 1.5px solid #e2e8f0;
  border-radius: 12px;
  font-size: 14px;
  transition: all 0.2s ease;
  background: white;
}

.search-input-modern:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.filter-select {
  min-width: 150px;
}

.filter-select :deep(.v-field) {
  border-radius: 12px;
  background: #f8fafc;
}

.filter-actions {
  display: flex;
  align-items: center;
  gap: 16px;
}

.reset-btn {
  border-radius: 12px;
  text-transform: none;
  font-weight: 500;
}

/* Workspace */
.workspace-card {
  background: white;
  border-radius: 28px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
}

.workspace-toolbar {
  padding: 16px 24px;
  border-bottom: 1px solid #eef2f6;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.total-records {
  font-size: 14px;
  font-weight: 500;
  color: #475569;
}

.toolbar-right {
  display: flex;
  gap: 12px;
}

.export-btn {
  border-radius: 12px;
  text-transform: none;
  font-weight: 500;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px;
  color: #94a3b8;
}

.loading-container .spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Course Grid */
.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(380px, 1fr));
  gap: 24px;
  padding: 24px;
}

.course-card-modern {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  transition: all 0.3s ease;
}

.course-card-modern:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 30px -12px rgba(0, 0, 0, 0.15);
}

.course-card-header {
  padding: 20px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.course-code-badge {
  background: rgba(255, 255, 255, 0.2);
  padding: 6px 14px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 600;
  color: white;
  backdrop-filter: blur(8px);
}

.card-action-btn {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  transition: all 0.2s ease;
}

.card-action-btn:hover {
  background: rgba(255, 255, 255, 0.25);
}

.course-card-body {
  padding: 24px;
}

.course-name-modern {
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 16px;
}

.course-info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
  margin-bottom: 16px;
}

.course-info-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #64748b;
}

.info-icon {
  color: #94a3b8;
}

.course-schedule-info,
.course-room-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #64748b;
  margin-bottom: 8px;
}

.course-enrollment-progress {
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #eef2f6;
}

.progress-bar-container {
  display: flex;
  align-items: center;
  gap: 12px;
}

.progress-bar-bg {
  flex: 1;
  height: 8px;
  background: #e2e8f0;
  border-radius: 20px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: #3b82f6;
  border-radius: 20px;
  transition: width 0.5s ease;
}

.progress-text {
  font-size: 12px;
  font-weight: 600;
  color: #3b82f6;
}

.empty-state {
  text-align: center;
  padding: 60px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 16px;
  font-size: 14px;
}

/* Dialog */
.modern-dialog {
  border-radius: 28px !important;
  overflow: hidden;
}

.dialog-header-modern {
  padding: 24px 28px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.dialog-header-modern.create-mode {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.dialog-header-modern.edit-mode {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
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

.dialog-content-modern {
  padding: 28px !important;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-grid :deep(.v-field) {
  border-radius: 14px;
}

.dialog-actions-modern {
  padding: 20px 28px !important;
  gap: 12px;
}

.cancel-btn,
.save-btn {
  border-radius: 40px;
  padding: 10px 24px;
  text-transform: none;
  font-weight: 600;
}

@media (max-width: 1200px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 768px) {
  .hero-header { flex-direction: column; align-items: flex-start; }
  .hero-actions { width: 100%; flex-direction: column; }
  .hero-actions .hero-btn { width: 100%; justify-content: center; }
  .filter-bar { flex-direction: column; align-items: stretch; }
  .filter-group { flex-direction: column; }
  .search-wrapper { min-width: 100%; }
  .filter-select { width: 100%; }
  .toolbar-left { flex-direction: column; align-items: flex-start; }
  .form-grid { grid-template-columns: 1fr; }
  .courses-grid { grid-template-columns: 1fr; padding: 16px; }
}

@media (max-width: 480px) {
  .stats-grid-modern { grid-template-columns: 1fr; }
}
</style>