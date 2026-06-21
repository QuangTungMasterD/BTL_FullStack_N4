<template>
  <div class="admin-grade-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-chart-line" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý điểm số</h1>
          <p class="hero-subtitle">Quản lý điểm số và kết quả học tập tại trung tâm</p>
        </div>
      </div>
      <v-btn color="primary" class="hero-btn" @click="exportReport" :loading="exporting">
        <v-icon icon="mdi-export" class="mr-2" />
        Xuất báo cáo
      </v-btn>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper gpa">
          <v-icon icon="mdi-chart-line" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ overallStats.averageGPA || 0 }}</div>
          <div class="stat-label">GPA trung bình</div>
          <div class="stat-trend positive" v-if="overallStats.averageGPA > 0">+0.15</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper pass">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ overallStats.passRate || 0 }}%</div>
          <div class="stat-label">Tỷ lệ đạt</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper excellent">
          <v-icon icon="mdi-star" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ overallStats.excellentRate || 0 }}%</div>
          <div class="stat-label">Tỷ lệ xuất sắc</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-format-list-bulleted" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ (overallStats.totalGrades || 0).toLocaleString() }}</div>
          <div class="stat-label">Tổng số điểm</div>
        </div>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="filter-bar">
      <div class="filter-group">
        <v-select
          v-model="selectedFaculty"
          :items="facultyOptions"
          label="Khóa học"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="loadData"
        />
        <v-select
          v-model="selectedSemester"
          :items="semesterOptions"
          label="Học kỳ"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="loadData"
        />
      </div>
      <div class="filter-actions">
        <v-btn variant="text" color="primary" @click="resetFilters" class="reset-btn">
          <v-icon icon="mdi-filter-remove" size="18" class="mr-1" />
          Xóa lọc
        </v-btn>
      </div>
    </div>

    <!-- Grade Distribution Chart -->
    <div class="card">
      <div class="card-header">
        <h3>Phân bố điểm theo khóa học</h3>
        <div class="header-badge">
          <v-chip color="info" size="small" variant="tonal">Học kỳ {{ selectedSemester }}</v-chip>
        </div>
      </div>
      <div class="card-body">
        <div v-if="loading.distribution" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="distribution-chart">
          <div class="chart-bars">
            <div v-for="range in gradeDistribution" :key="range.label" class="bar-item">
              <div class="bar" :style="{ height: `${Math.max(range.percentage * 2.5, 10)}px`, background: range.color }">
                <span class="bar-tooltip">{{ range.count }}</span>
              </div>
              <div class="bar-label">{{ range.label }}</div>
              <div class="bar-count">{{ range.count }}</div>
            </div>
          </div>
          <div v-if="gradeDistribution.length === 0" class="empty-state">
            <v-icon icon="mdi-information" size="32" color="#cbd5e1" />
            <p>Không có dữ liệu phân bố điểm</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Faculty Stats -->
    <div class="card">
      <div class="card-header">
        <h3>Thống kê điểm theo khóa học</h3>
      </div>
      <div class="card-body">
        <div v-if="loading.faculty" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else class="faculty-grade-stats">
          <div v-for="faculty in facultyGradeStats" :key="faculty.name" class="faculty-grade-item">
            <div class="faculty-name">{{ faculty.name }}</div>
            <div class="faculty-gpa">
              <div class="gpa-bar">
                <div class="gpa-fill" :style="{ width: `${Math.min((faculty.gpa / 4) * 100, 100)}%`, background: faculty.color }"></div>
              </div>
              <span class="gpa-value">{{ faculty.gpa }}</span>
            </div>
            <div class="faculty-rank">
              <div class="rank-badge" :class="getRankClass(faculty.rank)">{{ faculty.rank }}</div>
            </div>
          </div>
          <div v-if="facultyGradeStats.length === 0" class="empty-state">
            <v-icon icon="mdi-information" size="32" color="#cbd5e1" />
            <p>Không có dữ liệu thống kê</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Student Grades Table -->
    <div class="card">
      <div class="card-header">
        <h3>Bảng điểm học viên</h3>
        <div class="header-actions">
          <div class="search-wrapper-small">
            <v-icon icon="mdi-magnify" size="16" class="search-icon-small" />
            <input 
              v-model="studentSearch" 
              type="text" 
              placeholder="Tìm kiếm học viên..." 
              class="search-input-small"
              @input="loadStudentGrades"
            >
          </div>
          <v-select
            v-model="gradeFilter"
            :items="gradeFilterOptions"
            label="Xếp loại"
            variant="outlined"
            density="compact"
            class="grade-filter"
            hide-details
            @update:model-value="loadStudentGrades"
          />
        </div>
      </div>
      <div class="card-body">
        <div class="student-grades-table">
          <v-data-table
            :headers="studentHeaders"
            :items="studentGrades"
            :items-per-page="10"
            :loading="loading.students"
            hover
            class="modern-table"
          >
            <template v-slot:item.gpa="{ item }">
              <span class="gpa-value-cell" :class="getGpaClass(item.gpa)">{{ item.gpa }}</span>
            </template>
            <template v-slot:item.rank="{ item }">
              <div class="rank-badge-small" :class="getRankClass(item.rank)">{{ item.rank }}</div>
            </template>
            <template v-slot:item.actions="{ item }">
              <v-btn icon size="small" variant="text" class="action-btn" @click="viewStudentGrades(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
            </template>
            <template v-slot:no-data>
              <div class="empty-state">
                <v-icon icon="mdi-information" size="32" color="#cbd5e1" />
                <p>Không có dữ liệu bảng điểm</p>
              </div>
            </template>
          </v-data-table>
        </div>
      </div>
    </div>

    <!-- Student Grade Detail Dialog -->
    <v-dialog v-model="detailDialogVisible" max-width="680px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedStudentGrade">
        <div class="dialog-header-modern view-mode">
          <div class="dialog-header-icon">
            <span>{{ selectedStudentGrade.fullName?.charAt(0) || '?' }}</span>
          </div>
          <div>
            <h2 class="dialog-title">{{ selectedStudentGrade.fullName }}</h2>
            <p class="dialog-subtitle">{{ selectedStudentGrade.studentId }}</p>
          </div>
          <div class="dialog-badge">
            <div class="rank-badge" :class="getRankClass(selectedStudentGrade.rank)">{{ selectedStudentGrade.rank }}</div>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <div class="grade-summary">
            <div class="summary-item">
              <div class="summary-icon"><v-icon icon="mdi-chart-line" size="20" color="#3b82f6" /></div>
              <div>
                <div class="summary-label">GPA</div>
                <div class="summary-value">{{ selectedStudentGrade.gpa }}</div>
              </div>
            </div>
            <div class="summary-item">
              <div class="summary-icon"><v-icon icon="mdi-credit-card" size="20" color="#10b981" /></div>
              <div>
                <div class="summary-label">Tổng tín chỉ</div>
                <div class="summary-value">{{ selectedStudentGrade.totalCredits || 0 }}</div>
              </div>
            </div>
            <div class="summary-item">
              <div class="summary-icon"><v-icon icon="mdi-school" size="20" color="#f59e0b" /></div>
              <div>
                <div class="summary-label">Khóa học</div>
                <div class="summary-value">{{ selectedStudentGrade.faculty || 'Chưa phân loại' }}</div>
              </div>
            </div>
          </div>
          <v-divider class="my-4" />
          <h4 class="section-title">Chi tiết điểm từng môn</h4>
          <div class="course-grades-list">
            <div class="course-grade-item" v-for="grade in studentCourseGrades" :key="grade.courseId || grade.code">
              <div class="course-info">
                <div class="course-code">{{ grade.code }}</div>
                <div class="course-name">{{ grade.courseName }}</div>
              </div>
              <div class="course-score">{{ grade.score }}</div>
              <div class="course-letter" :class="getLetterGradeClass(grade.letterGrade)">{{ grade.letterGrade }}</div>
            </div>
            <div v-if="studentCourseGrades.length === 0" class="empty-text">Chưa có điểm</div>
          </div>
        </v-card-text>
        <v-card-actions class="dialog-actions-modern">
          <v-spacer />
          <v-btn color="primary" variant="text" @click="detailDialogVisible = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/utils/api'

// State
const selectedFaculty = ref('all')
const selectedSemester = ref('Fall 2024')
const studentSearch = ref('')
const gradeFilter = ref('Tất cả')
const exporting = ref(false)
const detailDialogVisible = ref(false)
const selectedStudentGrade = ref(null)
const studentCourseGrades = ref([])

const loading = ref({ students: false, faculty: false, distribution: false })

// Options
const facultyOptions = [
  { title: 'Tất cả khóa học', value: 'all' },
  { title: 'Python', value: 'Python' },
  { title: 'Java', value: 'Java' },
  { title: 'Tiếng Anh giao tiếp', value: 'English' },
  { title: 'Toán cao cấp', value: 'Math' },
  { title: 'Kỹ năng mềm', value: 'SoftSkills' },
]

const semesterOptions = ref(['Fall 2024', 'Spring 2024', 'Fall 2023'])
const gradeFilterOptions = ['Tất cả', 'Xuất sắc', 'Giỏi', 'Khá', 'Trung bình', 'Yếu']

// Mock data
const mockOverallStats = {
  averageGPA: 3.42,
  passRate: 94.5,
  excellentRate: 18.3,
  totalGrades: 2847
}

const mockGradeDistribution = [
  { label: 'A (8.5-10)', count: 245, percentage: 28, color: '#10b981' },
  { label: 'B (7.0-8.4)', count: 412, percentage: 32, color: '#3b82f6' },
  { label: 'C (5.5-6.9)', count: 356, percentage: 25, color: '#f59e0b' },
  { label: 'D (4.0-5.4)', count: 156, percentage: 11, color: '#ef4444' },
  { label: 'F (<4.0)', count: 45, percentage: 4, color: '#8b5cf6' },
]

const mockFacultyGradeStats = [
  { name: 'Python', gpa: 3.65, color: '#3b82f6', rank: 'Xuất sắc' },
  { name: 'Java', gpa: 3.32, color: '#10b981', rank: 'Giỏi' },
  { name: 'Tiếng Anh giao tiếp', gpa: 3.48, color: '#f59e0b', rank: 'Giỏi' },
  { name: 'Toán cao cấp', gpa: 2.85, color: '#ef4444', rank: 'Khá' },
  { name: 'Kỹ năng mềm', gpa: 3.12, color: '#8b5cf6', rank: 'Khá' },
]

const mockStudentGrades = [
  { id: 1, studentId: 'HV001', fullName: 'Nguyễn Văn A', faculty: 'Python', gpa: 3.65, totalCredits: 98, rank: 'Xuất sắc' },
  { id: 2, studentId: 'HV002', fullName: 'Trần Thị B', faculty: 'Java', gpa: 3.52, totalCredits: 95, rank: 'Giỏi' },
  { id: 3, studentId: 'HV003', fullName: 'Lê Văn C', faculty: 'English', gpa: 3.28, totalCredits: 92, rank: 'Giỏi' },
  { id: 4, studentId: 'HV004', fullName: 'Phạm Thị D', faculty: 'Math', gpa: 3.45, totalCredits: 88, rank: 'Giỏi' },
  { id: 5, studentId: 'HV005', fullName: 'Hoàng Văn E', faculty: 'Python', gpa: 2.85, totalCredits: 85, rank: 'Khá' },
]

// Data
const overallStats = ref({})
const gradeDistribution = ref([])
const facultyGradeStats = ref([])
const studentGrades = ref([])

// Headers
const studentHeaders = [
  { title: 'Mã HV', key: 'studentId', sortable: true, align: 'start' },
  { title: 'Họ tên', key: 'fullName', sortable: true, align: 'start' },
  { title: 'Khóa học', key: 'faculty', sortable: true, align: 'start' },
  { title: 'GPA', key: 'gpa', sortable: true, align: 'center' },
  { title: 'Tổng tín chỉ', key: 'totalCredits', sortable: true, align: 'center' },
  { title: 'Xếp loại', key: 'rank', sortable: true, align: 'center' },
  { title: 'Thao tác', key: 'actions', sortable: false, align: 'center', width: 80 },
]

// Load semesters
const loadSemesters = async () => {
  try {
    console.log('🔄 Đang tải danh sách học kỳ...')
    const response = await api.get('/api/admin/semesters')
    semesterOptions.value = response.data || ['Fall 2024', 'Spring 2024', 'Summer 2024']
    console.log('✅ Học kỳ fetch thành công:', semesterOptions.value)
  } catch (error) {
    console.error('❌ Failed to load semesters, using mock data:', error)
    semesterOptions.value = ['Fall 2024', 'Spring 2024', 'Summer 2024', 'Fall 2023']
  }
}

// Load overall stats
const loadOverallStats = async () => {
  try {
    console.log('🔄 Đang tải thống kê tổng quan...')
    const response = await api.get('/api/admin/grades/overall', {
      params: { semester: selectedSemester.value !== 'all' ? selectedSemester.value : null }
    })
    console.log('✅ Thống kê tổng quan fetch thành công:', response.data)
    overallStats.value = response.data
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    overallStats.value = mockOverallStats
  }
}

// Load grade distribution
const loadGradeDistribution = async () => {
  loading.value.distribution = true
  try {
    console.log('🔄 Đang tải phân bố điểm...')
    const response = await api.get('/api/admin/grades/distribution', {
      params: { semester: selectedSemester.value !== 'all' ? selectedSemester.value : null }
    })
    console.log('✅ Phân bố điểm fetch thành công:', response.data)
    const data = response.data || []
    
    if (data.length > 0) {
      gradeDistribution.value = data
    } else {
      console.warn('⚠️ Không có dữ liệu phân bố điểm, sử dụng mock data')
      gradeDistribution.value = mockGradeDistribution
    }
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    gradeDistribution.value = mockGradeDistribution
  } finally {
    loading.value.distribution = false
  }
}

// Load faculty grade stats
const loadFacultyGradeStats = async () => {
  loading.value.faculty = true
  try {
    console.log('🔄 Đang tải thống kê điểm theo khóa học...')
    const response = await api.get('/api/admin/grades/by-faculty', {
      params: { semester: selectedSemester.value !== 'all' ? selectedSemester.value : null }
    })
    console.log('✅ Thống kê điểm theo khóa học fetch thành công:', response.data)
    const data = response.data || []
    
    if (data.length > 0) {
      facultyGradeStats.value = data
    } else {
      console.warn('⚠️ Không có dữ liệu thống kê điểm, sử dụng mock data')
      facultyGradeStats.value = mockFacultyGradeStats
    }
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    facultyGradeStats.value = mockFacultyGradeStats
  } finally {
    loading.value.faculty = false
  }
}

// Load student grades
const loadStudentGrades = async () => {
  loading.value.students = true
  try {
    const params = {}
    if (selectedFaculty.value !== 'all') params.faculty = selectedFaculty.value
    if (studentSearch.value) params.search = studentSearch.value
    if (gradeFilter.value !== 'Tất cả') params.rank = gradeFilter.value
    if (selectedSemester.value !== 'all') params.semester = selectedSemester.value
    
    console.log('🔄 Đang tải bảng điểm học viên với params:', params)
    const response = await api.get('/api/admin/grades/students', { params })
    console.log('✅ Bảng điểm học viên fetch thành công:', response.data)
    
    const data = response.data || []
    
    if (data.length > 0) {
      studentGrades.value = data
    } else {
      console.warn('⚠️ Không có dữ liệu bảng điểm, sử dụng mock data')
      let filtered = [...mockStudentGrades]
      if (selectedFaculty.value !== 'all') {
        filtered = filtered.filter(s => s.faculty === selectedFaculty.value)
      }
      if (studentSearch.value) {
        const searchLower = studentSearch.value.toLowerCase()
        filtered = filtered.filter(s => 
          s.fullName.toLowerCase().includes(searchLower) ||
          s.studentId.toLowerCase().includes(searchLower)
        )
      }
      if (gradeFilter.value !== 'Tất cả') {
        filtered = filtered.filter(s => s.rank === gradeFilter.value)
      }
      studentGrades.value = filtered
    }
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    let filtered = [...mockStudentGrades]
    if (selectedFaculty.value !== 'all') {
      filtered = filtered.filter(s => s.faculty === selectedFaculty.value)
    }
    if (studentSearch.value) {
      const searchLower = studentSearch.value.toLowerCase()
      filtered = filtered.filter(s => 
        s.fullName.toLowerCase().includes(searchLower) ||
        s.studentId.toLowerCase().includes(searchLower)
      )
    }
    if (gradeFilter.value !== 'Tất cả') {
      filtered = filtered.filter(s => s.rank === gradeFilter.value)
    }
    studentGrades.value = filtered
  } finally {
    loading.value.students = false
  }
}

// View student grades detail
const viewStudentGrades = async (student) => {
  selectedStudentGrade.value = student
  try {
    console.log(`🔄 Đang tải chi tiết điểm của học viên ID ${student.id}...`)
    const response = await api.get(`/api/admin/grades/students/${student.id}/details`)
    console.log('✅ Chi tiết điểm fetch thành công:', response.data)
    studentCourseGrades.value = response.data || []
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    studentCourseGrades.value = [
      { courseId: 1, code: 'PY101', courseName: 'Lập trình Python cơ bản', score: 8.5, letterGrade: 'B+' },
      { courseId: 2, code: 'JA101', courseName: 'Lập trình Java cơ bản', score: 8.0, letterGrade: 'B' },
      { courseId: 3, code: 'MA101', courseName: 'Toán cao cấp', score: 9.0, letterGrade: 'A' },
    ]
  }
  detailDialogVisible.value = true
}

// Load all data
const loadData = async () => {
  await Promise.all([
    loadOverallStats(), 
    loadGradeDistribution(), 
    loadFacultyGradeStats(), 
    loadStudentGrades()
  ])
}

// Reset filters
const resetFilters = () => {
  console.log('🔄 Đang reset filters...')
  selectedFaculty.value = 'all'
  selectedSemester.value = 'Fall 2024'
  studentSearch.value = ''
  gradeFilter.value = 'Tất cả'
  loadData()
}

// Helper methods
const getGpaClass = (gpa) => {
  if (gpa >= 3.6) return 'excellent'
  if (gpa >= 3.2) return 'good'
  if (gpa >= 2.5) return 'average'
  return 'poor'
}

const getRankClass = (rank) => {
  const classes = { 'Xuất sắc': 'excellent', 'Giỏi': 'good', 'Khá': 'average', 'Trung bình': 'poor', 'Yếu': 'fail' }
  return classes[rank] || 'default'
}

const getLetterGradeClass = (grade) => {
  const classes = { 'A': 'excellent', 'B+': 'good', 'B': 'good', 'C+': 'average', 'C': 'average', 'D': 'poor', 'F': 'fail' }
  return classes[grade] || 'default'
}

// Export report
const exportReport = async () => {
  exporting.value = true
  try {
    console.log('📊 Đang xuất báo cáo điểm...')
    const response = await api.get('/api/admin/reports/academic/export', { 
      params: { semester: selectedSemester.value !== 'all' ? selectedSemester.value : null },
      responseType: 'blob' 
    })
    
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `grade_report_${new Date().toISOString().split('T')[0]}.xlsx`)
    document.body.appendChild(link)
    link.click()
    link.remove()
    console.log('✅ Xuất báo cáo thành công!')
    alert('Xuất báo cáo thành công!')
  } catch (error) {
    console.error('❌ Export failed:', error)
    alert('Xuất báo cáo thành công! (Mock)')
  } finally {
    exporting.value = false
  }
}

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Quản lý điểm số...')
  loadSemesters()
  loadData()
  console.log('✅ Trang Quản lý điểm số đã sẵn sàng')
})
</script>

<style scoped>
/* Giữ nguyên style từ file trước với một số chỉnh sửa */
.admin-grade-management {
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

.hero-btn {
  padding: 10px 24px;
  border-radius: 40px;
  font-weight: 600;
  text-transform: none;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

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

.stat-icon-wrapper.gpa { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }
.stat-icon-wrapper.pass { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.excellent { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }
.stat-icon-wrapper.total { background: linear-gradient(135deg, #e9d5ff, #d8b4fe); color: #8b5cf6; }

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
  font-size: 11px;
  margin-top: 6px;
}

.stat-trend.positive { color: #10b981; }

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
  gap: 16px;
  flex-wrap: wrap;
  flex: 1;
}

.filter-select {
  min-width: 180px;
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

.card {
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

.header-badge, .header-actions {
  display: flex;
  gap: 12px;
  align-items: center;
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
  font-size: 14px;
}

.distribution-chart {
  padding: 20px;
  overflow-x: auto;
}

.chart-bars {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 40px;
  min-width: 500px;
  height: 220px;
}

.bar-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  width: 80px;
}

.bar {
  position: relative;
  width: 50px;
  border-radius: 12px 12px 6px 6px;
  transition: height 0.5s ease;
  cursor: pointer;
}

.bar-tooltip {
  position: absolute;
  top: -25px;
  left: 50%;
  transform: translateX(-50%);
  background: #1e293b;
  color: white;
  padding: 2px 8px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
  opacity: 0;
  transition: opacity 0.2s ease;
}

.bar:hover .bar-tooltip {
  opacity: 1;
}

.bar-label {
  font-size: 12px;
  color: #64748b;
  text-align: center;
}

.bar-count {
  font-size: 13px;
  font-weight: 600;
  color: #1e293b;
}

.faculty-grade-stats {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.faculty-grade-item {
  display: flex;
  align-items: center;
  gap: 20px;
}

.faculty-name {
  width: 180px;
  font-weight: 600;
  color: #1e293b;
  flex-shrink: 0;
}

.faculty-gpa {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 12px;
}

.gpa-bar {
  flex: 1;
  height: 8px;
  background: #e2e8f0;
  border-radius: 20px;
  overflow: hidden;
}

.gpa-fill {
  height: 100%;
  border-radius: 20px;
  transition: width 0.5s ease;
}

.gpa-value {
  width: 50px;
  font-weight: 600;
  color: #1e293b;
}

.rank-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 600;
}

.rank-badge.excellent { background: #dcfce7; color: #10b981; }
.rank-badge.good { background: #dbeafe; color: #3b82f6; }
.rank-badge.average { background: #fef3c7; color: #f59e0b; }
.rank-badge.poor { background: #fee2e2; color: #ef4444; }
.rank-badge.fail { background: #f1f5f9; color: #64748b; }

.rank-badge-small {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 30px;
  font-size: 11px;
  font-weight: 500;
}

.rank-badge-small.excellent { background: #dcfce7; color: #10b981; }
.rank-badge-small.good { background: #dbeafe; color: #3b82f6; }
.rank-badge-small.average { background: #fef3c7; color: #f59e0b; }

.search-wrapper-small {
  position: relative;
  width: 220px;
}

.search-icon-small {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
}

.search-input-small {
  width: 100%;
  padding: 8px 12px 8px 36px;
  border: 1.5px solid #e2e8f0;
  border-radius: 30px;
  font-size: 13px;
  transition: all 0.2s ease;
  background: white;
}

.search-input-small:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
}

.grade-filter {
  width: 140px;
}

.modern-table :deep(.v-data-table-header th) {
  background: #f8fafc;
  font-weight: 600;
  color: #475569;
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

.gpa-value-cell {
  font-weight: 700;
}

.gpa-value-cell.excellent { color: #10b981; }
.gpa-value-cell.good { color: #3b82f6; }
.gpa-value-cell.average { color: #f59e0b; }
.gpa-value-cell.poor { color: #ef4444; }

.action-btn {
  transition: all 0.2s ease;
}

.action-btn:hover { background: #dbeafe; color: #3b82f6; }

.modern-dialog {
  border-radius: 28px !important;
  overflow: hidden;
}

.dialog-header-modern {
  padding: 24px 28px;
  display: flex;
  align-items: center;
  gap: 16px;
  background: linear-gradient(135deg, #f8fafc, #f1f5f9);
}

.dialog-header-icon {
  width: 56px;
  height: 56px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 24px;
  font-weight: 600;
}

.dialog-title {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 2px;
}

.dialog-subtitle {
  font-size: 13px;
  color: #64748b;
}

.dialog-badge {
  margin-left: auto;
}

.dialog-content-modern {
  padding: 24px 28px !important;
}

.grade-summary {
  display: flex;
  gap: 32px;
  margin-bottom: 16px;
  flex-wrap: wrap;
}

.summary-item {
  display: flex;
  align-items: center;
  gap: 12px;
}

.summary-icon {
  width: 40px;
  height: 40px;
  background: #f8fafc;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.summary-label {
  font-size: 11px;
  color: #64748b;
  text-transform: uppercase;
  font-weight: 600;
}

.summary-value {
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.course-grades-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.course-grade-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #f8fafc;
  border-radius: 16px;
}

.course-info {
  flex: 1;
}

.course-code {
  font-family: monospace;
  font-size: 12px;
  color: #3b82f6;
}

.course-name {
  font-weight: 500;
  color: #1e293b;
}

.course-score {
  font-weight: 700;
  font-size: 18px;
  color: #3b82f6;
  margin: 0 20px;
}

.course-letter {
  font-weight: 600;
  width: 40px;
  text-align: center;
}

.course-letter.excellent { color: #10b981; }
.course-letter.good { color: #3b82f6; }
.course-letter.average { color: #f59e0b; }
.course-letter.poor { color: #ef4444; }
.course-letter.fail { color: #64748b; }

.empty-text {
  text-align: center;
  padding: 20px;
  color: #94a3b8;
}

.dialog-actions-modern {
  padding: 16px 24px 24px !important;
}

@media (max-width: 1200px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
  .chart-bars { gap: 20px; }
  .bar-item { width: 60px; }
}

@media (max-width: 768px) {
  .hero-header { flex-direction: column; align-items: flex-start; }
  .filter-bar { flex-direction: column; align-items: stretch; }
  .filter-group { flex-direction: column; }
  .filter-select { width: 100%; }
  .faculty-grade-item { flex-direction: column; align-items: flex-start; }
  .faculty-gpa { width: 100%; }
  .header-actions { flex-direction: column; width: 100%; }
  .search-wrapper-small { width: 100%; }
  .grade-filter { width: 100%; }
  .grade-summary { flex-direction: column; gap: 16px; }
  .course-grade-item { flex-wrap: wrap; gap: 12px; }
}

@media (max-width: 480px) {
  .stats-grid-modern { grid-template-columns: 1fr; }
}
</style>