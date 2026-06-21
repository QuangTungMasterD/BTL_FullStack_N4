<template>
  <div class="grade-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-chart-line" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý điểm số</h1>
          <p class="hero-subtitle">Nhập điểm và theo dõi kết quả học tập của học viên tại trung tâm</p>
        </div>
      </div>
      <v-btn color="success" class="hero-btn" @click="saveGrades" :loading="saving">
        <v-icon icon="mdi-content-save" class="mr-2" />
        Lưu điểm
      </v-btn>
    </div>

    <!-- Selector Card -->
    <div class="selector-card">
      <div class="selector-grid">
        <v-select 
          v-model="selectedCourse" 
          :items="courseOptions" 
          label="Chọn khóa học" 
          variant="outlined" 
          density="comfortable" 
          hide-details 
          @update:model-value="onCourseChange"
        >
          <template v-slot:item="{ item }">
            <div class="course-option">
              <span class="course-name">{{ item.title }}</span>
              <span class="course-code">{{ item.raw?.code || '' }}</span>
            </div>
          </template>
        </v-select>
        <v-select 
          v-model="selectedExam" 
          :items="examOptions" 
          label="Chọn loại điểm" 
          variant="outlined" 
          density="comfortable" 
          hide-details 
          @update:model-value="loadGrades"
        />
        <v-btn color="primary" class="refresh-btn" @click="loadGrades" :loading="loading.grades">
          <v-icon icon="mdi-refresh" class="mr-2" />Tải danh sách
        </v-btn>
      </div>
    </div>

    <!-- Exam Info -->
    <div class="exam-info-card" v-if="selectedExam && selectedExam !== 'all' && selectedCourse">
      <div class="info-row">
        <div class="info-item">
          <label>Loại điểm:</label>
          <span>{{ getExamName() }}</span>
        </div>
        <div class="info-item">
          <label>Hệ số:</label>
          <span>{{ getExamWeight() }}</span>
        </div>
        <div class="info-item">
          <label>Khóa học:</label>
          <span>{{ getCourseName() }}</span>
        </div>
        <div class="info-item">
          <label>Số học viên:</label>
          <span>{{ studentsGrades.length }}</span>
        </div>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid-modern" v-if="studentsGrades.length > 0">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper avg">
          <v-icon icon="mdi-chart-line" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ averageScore }}</div>
          <div class="stat-label">Điểm trung bình</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper highest">
          <v-icon icon="mdi-trending-up" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ highestScore }}</div>
          <div class="stat-label">Điểm cao nhất</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper lowest">
          <v-icon icon="mdi-trending-down" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ lowestScore }}</div>
          <div class="stat-label">Điểm thấp nhất</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper pass">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ passRate }}%</div>
          <div class="stat-label">Tỷ lệ đạt</div>
        </div>
      </div>
    </div>

    <!-- Grade Table Card -->
    <div class="workspace-card">
      <div class="card-header">
        <h3>Danh sách điểm học viên</h3>
        <div class="header-actions">
          <v-btn size="small" variant="text" class="export-btn" @click="exportGrades" :loading="exporting">
            <v-icon icon="mdi-export" size="16" class="mr-1" />Xuất Excel
          </v-btn>
          <v-btn size="small" variant="text" class="stats-btn" @click="showStatistics = true" :disabled="studentsGrades.length === 0">
            <v-icon icon="mdi-chart-bar" size="16" class="mr-1" />Thống kê
          </v-btn>
        </div>
      </div>
      <div class="card-body">
        <div v-if="loading.grades" class="loading-placeholder">
          <v-progress-circular indeterminate />
        </div>
        <div v-else-if="studentsGrades.length === 0" class="empty-state">
          <v-icon icon="mdi-account-off" size="56" color="#cbd5e1" />
          <p>Không có học viên trong khóa học này</p>
          <p class="empty-sub">Vui lòng chọn khóa học và loại điểm khác</p>
        </div>
        <div v-else class="grades-table">
          <v-data-table
            :headers="gradeHeaders"
            :items="studentsGrades"
            :items-per-page="10"
            hover
            class="modern-table"
          >
            <template v-slot:item.index="{ index }">
              <span>{{ index + 1 }}</span>
            </template>
            <template v-slot:item.score="{ item }">
              <v-text-field 
                v-model="item.score" 
                type="number" 
                step="0.1" 
                min="0" 
                max="10"
                variant="outlined" 
                density="compact" 
                style="width: 100px" 
                hide-details 
                @input="updateGrade(item)"
              />
            </template>
            <template v-slot:item.letterGrade="{ item }">
              <div class="letter-grade" :class="getLetterGradeClass(item.letterGrade)">
                {{ item.letterGrade || 'Chưa có' }}
              </div>
            </template>
            <template v-slot:item.rank="{ item }">
              <div class="rank-badge" :class="getRankClass(item.rank)">
                {{ item.rank || 'Chưa xếp loại' }}
              </div>
            </template>
            <template v-slot:item.note="{ item }">
              <v-text-field 
                v-model="item.note" 
                placeholder="Ghi chú..." 
                variant="outlined" 
                density="compact" 
                style="width: 150px" 
                hide-details 
              />
            </template>
          </v-data-table>
        </div>
        <div class="save-section" v-if="studentsGrades.length > 0">
          <v-btn color="success" size="large" class="save-btn" @click="saveGrades" :loading="saving">
            <v-icon icon="mdi-content-save" class="mr-2" />Lưu điểm
          </v-btn>
        </div>
      </div>
    </div>

    <!-- Distribution Chart Card -->
    <div class="workspace-card" v-if="studentsGrades.length > 0">
      <div class="card-header">
        <h3>Phân bố điểm</h3>
      </div>
      <div class="card-body">
        <div class="distribution-chart">
          <div class="chart-bars">
            <div v-for="range in gradeDistribution" :key="range.label" class="bar-item">
              <div class="bar" :style="{ height: `${Math.max(range.percentage * 3, 10)}px`, background: range.color }">
                <span class="bar-tooltip">{{ range.count }}</span>
              </div>
              <div class="bar-label">{{ range.label }}</div>
              <div class="bar-count">{{ range.count }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Statistics Dialog -->
    <v-dialog v-model="showStatistics" max-width="500px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-stats">
          <div class="dialog-header-icon">
            <v-icon icon="mdi-chart-bar" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">Thống kê điểm số</h2>
            <p class="dialog-subtitle">{{ getExamName() }} • {{ getExamWeight() }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content">
          <div class="statistics-list">
            <div class="statistic-item">
              <span>Tổng số học viên:</span>
              <strong>{{ studentsGrades.length }}</strong>
            </div>
            <div class="statistic-item">
              <span>Điểm trung bình:</span>
              <strong>{{ averageScore }}</strong>
            </div>
            <div class="statistic-item">
              <span>Điểm cao nhất:</span>
              <strong class="text-success">{{ highestScore }}</strong>
            </div>
            <div class="statistic-item">
              <span>Điểm thấp nhất:</span>
              <strong class="text-error">{{ lowestScore }}</strong>
            </div>
            <div class="statistic-item">
              <span>Tỷ lệ đạt (>=5):</span>
              <strong>{{ passRate }}%</strong>
            </div>
            <div class="statistic-item">
              <span>Tỷ lệ xuất sắc (>=8.5):</span>
              <strong>{{ excellentRate }}%</strong>
            </div>
          </div>
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-spacer />
          <v-btn color="primary" variant="text" @click="showStatistics = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, nextTick } from 'vue'
import api from '@/utils/api'

const selectedCourse = ref(null)
const selectedExam = ref('Midterm')
const saving = ref(false)
const exporting = ref(false)
const loading = ref({ grades: false })
const showStatistics = ref(false)

const courseOptions = ref([])
const examOptions = [
  { title: 'Giữa kỳ', value: 'Midterm' },
  { title: 'Cuối kỳ (Final)', value: 'Final' },
  { title: 'Bài tập lớn', value: 'Project' },
  { title: 'Quiz 1', value: 'Quiz1' },
  { title: 'Quiz 2', value: 'Quiz2' }
]

const studentsGrades = ref([])

// Computed - Kiểm tra an toàn
const averageScore = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  if (scores.length === 0) return '0'
  const sum = scores.reduce((acc, s) => acc + s, 0)
  return (sum / scores.length).toFixed(1)
})

const highestScore = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  if (scores.length === 0) return '0'
  return Math.max(...scores).toFixed(1)
})

const lowestScore = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  if (scores.length === 0) return '0'
  return Math.min(...scores).toFixed(1)
})

const passRate = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  if (scores.length === 0) return 0
  const passed = scores.filter(s => s >= 5).length
  return Math.round((passed / scores.length) * 100)
})

const excellentRate = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  if (scores.length === 0) return 0
  const excellent = scores.filter(s => s >= 8.5).length
  return Math.round((excellent / scores.length) * 100)
})

const gradeDistribution = computed(() => {
  const scores = studentsGrades.value
    .filter(s => s.score !== undefined && s.score !== null && !isNaN(s.score))
    .map(s => parseFloat(s.score))
    .filter(s => !isNaN(s))
  
  const ranges = [
    { label: '0-4', min: 0, max: 4, color: '#ef4444', count: 0 },
    { label: '4-5', min: 4, max: 5, color: '#f59e0b', count: 0 },
    { label: '5-6.5', min: 5, max: 6.5, color: '#eab308', count: 0 },
    { label: '6.5-8', min: 6.5, max: 8, color: '#3b82f6', count: 0 },
    { label: '8-10', min: 8, max: 10, color: '#10b981', count: 0 }
  ]
  
  scores.forEach(score => {
    const range = ranges.find(r => score >= r.min && score < r.max)
    if (range) range.count++
  })
  
  const maxCount = Math.max(...ranges.map(r => r.count), 1)
  ranges.forEach(range => {
    range.percentage = (range.count / maxCount) * 100
  })
  
  return ranges
})

// Headers
const gradeHeaders = [
  { title: 'STT', key: 'index', sortable: false, width: 60, align: 'center' },
  { title: 'Mã HV', key: 'studentId', sortable: true, align: 'start' },
  { title: 'Họ tên', key: 'fullName', sortable: true, align: 'start' },
  { title: 'Điểm', key: 'score', align: 'center' },
  { title: 'Điểm chữ', key: 'letterGrade', align: 'center' },
  { title: 'Xếp loại', key: 'rank', align: 'center' },
  { title: 'Ghi chú', key: 'note', align: 'center' },
]

// Load courses
const loadCourses = async () => {
  try {
    console.log('🔄 Đang tải danh sách khóa học...')
    const response = await api.get('/api/lecturer/courses')
    console.log('✅ Danh sách khóa học fetch thành công:', response.data)
    
    // Đảm bảo courseOptions luôn là array
    const data = Array.isArray(response.data) ? response.data : []
    courseOptions.value = data.map(c => ({ 
      title: c.courseName || 'Không có tên', 
      value: c.id,
      code: c.code || '',
      schedule: c.schedule || '' 
    }))
    
    // Chọn khóa học đầu tiên nếu có
    if (courseOptions.value.length > 0 && !selectedCourse.value) {
      selectedCourse.value = courseOptions.value[0].value
      await nextTick()
      await loadGrades()
    }
  } catch (error) {
    console.error('❌ Failed to load courses:', error)
    // Mock data nếu API lỗi
    courseOptions.value = [
      { title: 'Lập trình Python cơ bản', value: 1, code: 'PY101', schedule: 'Thứ 2, 13:30-16:30' },
      { title: 'Lập trình Java cơ bản', value: 2, code: 'JA101', schedule: 'Thứ 3, 08:00-11:00' },
    ]
    if (!selectedCourse.value && courseOptions.value.length > 0) {
      selectedCourse.value = courseOptions.value[0].value
      await nextTick()
      await loadGrades()
    }
  }
}

// On course change
const onCourseChange = async () => {
  await loadGrades()
}

// Load grades
const loadGrades = async () => {
  if (!selectedCourse.value) {
    console.warn('⚠️ Chưa chọn khóa học')
    studentsGrades.value = []
    return
  }
  
  loading.value.grades = true
  try {
    console.log(`🔄 Đang tải điểm loại ${selectedExam.value} cho khóa ${selectedCourse.value}...`)
    const response = await api.get('/api/lecturer/grades', { 
      params: { 
        courseId: selectedCourse.value, 
        examType: selectedExam.value 
      } 
    })
    console.log('✅ Điểm fetch thành công:', response.data)
    
    const data = Array.isArray(response.data) ? response.data : []
    studentsGrades.value = data.map((s, idx) => ({
      ...s,
      score: s.score || 0,
      letterGrade: s.letterGrade || getLetterGrade(s.score || 0),
      rank: s.rank || getRank(s.score || 0),
      index: idx + 1
    }))
  } catch (error) {
    console.error('❌ Failed to load grades:', error)
    // Mock data
    const mockData = [
      { id: 1, studentId: 'HV001', fullName: 'Nguyễn Văn An', score: 8.5, note: '' },
      { id: 2, studentId: 'HV002', fullName: 'Trần Thị Ngọc Bích', score: 7.5, note: '' },
      { id: 3, studentId: 'HV003', fullName: 'Lê Văn Cường', score: 6.0, note: '' },
      { id: 4, studentId: 'HV004', fullName: 'Phạm Thị Phương Dung', score: 4.5, note: '' },
      { id: 5, studentId: 'HV005', fullName: 'Hoàng Văn Minh Em', score: 9.0, note: '' },
    ]
    studentsGrades.value = mockData.map((s, idx) => ({
      ...s,
      letterGrade: getLetterGrade(s.score),
      rank: getRank(s.score),
      index: idx + 1
    }))
  } finally {
    loading.value.grades = false
  }
}

// Helper functions
const getExamName = () => {
  const exam = examOptions.find(e => e.value === selectedExam.value)
  return exam ? exam.title : 'Không xác định'
}

const getExamWeight = () => {
  const weights = { 
    Midterm: '25%', 
    Final: '40%', 
    Project: '20%', 
    Quiz1: '10%', 
    Quiz2: '10%' 
  }
  return weights[selectedExam.value] || '0%'
}

const getCourseName = () => {
  if (!Array.isArray(courseOptions.value) || courseOptions.value.length === 0) {
    return 'Không có khóa học'
  }
  const course = courseOptions.value.find(c => c.value === selectedCourse.value)
  return course ? course.title : 'Không xác định'
}

const getLetterGrade = (score) => {
  const num = parseFloat(score) || 0
  if (num >= 9.0) return 'A'
  if (num >= 8.0) return 'B+'
  if (num >= 7.0) return 'B'
  if (num >= 6.0) return 'C+'
  if (num >= 5.0) return 'C'
  if (num >= 4.0) return 'D'
  return 'F'
}

const getRank = (score) => {
  const num = parseFloat(score) || 0
  if (num >= 9.0) return 'Xuất sắc'
  if (num >= 8.0) return 'Giỏi'
  if (num >= 7.0) return 'Khá'
  if (num >= 5.0) return 'Trung bình'
  return 'Yếu'
}

const getLetterGradeClass = (grade) => {
  const classes = { A: 'excellent', 'B+': 'good', B: 'good', 'C+': 'average', C: 'average', D: 'poor', F: 'fail' }
  return classes[grade] || 'default'
}

const getRankClass = (rank) => {
  const classes = { 'Xuất sắc': 'excellent', 'Giỏi': 'good', 'Khá': 'average', 'Trung bình': 'poor', 'Yếu': 'fail' }
  return classes[rank] || 'default'
}

const updateGrade = (student) => {
  const score = parseFloat(student.score) || 0
  student.letterGrade = getLetterGrade(score)
  student.rank = getRank(score)
}

// Save grades
const saveGrades = async () => {
  if (!selectedCourse.value) {
    alert('Vui lòng chọn khóa học')
    return
  }
  
  if (studentsGrades.value.length === 0) {
    alert('Không có học viên để lưu điểm')
    return
  }
  
  saving.value = true
  try {
    console.log('📊 Đang lưu điểm...')
    const grades = studentsGrades.value.map(s => ({
      studentId: s.id,
      score: parseFloat(s.score) || 0,
      note: s.note || null
    }))
    
    await api.post('/api/lecturer/grades/save', {
      courseId: selectedCourse.value,
      examType: selectedExam.value,
      grades: grades
    })
    console.log('✅ Lưu điểm thành công')
    alert('Đã lưu điểm thành công!')
  } catch (error) {
    console.error('❌ Failed to save grades:', error)
    alert('Lưu điểm thất bại: ' + (error.response?.data?.message || error.message))
  } finally {
    saving.value = false
  }
}

// Export grades
const exportGrades = async () => {
  if (!selectedCourse.value) {
    alert('Vui lòng chọn khóa học')
    return
  }
  
  exporting.value = true
  try {
    console.log('📊 Đang xuất điểm...')
    const response = await api.get('/api/lecturer/grades/export', {
      params: { 
        courseId: selectedCourse.value, 
        examType: selectedExam.value 
      },
      responseType: 'blob'
    })
    
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `grades_${selectedCourse.value}_${selectedExam.value}.csv`)
    document.body.appendChild(link)
    link.click()
    link.remove()
    console.log('✅ Xuất điểm thành công')
    alert('Xuất điểm thành công!')
  } catch (error) {
    console.error('❌ Export failed:', error)
    alert('Xuất điểm thất bại')
  } finally {
    exporting.value = false
  }
}

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Quản lý điểm số...')
  loadCourses()
  console.log('✅ Trang Quản lý điểm số đã sẵn sàng')
})
</script>

<style scoped>
.grade-management {
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

/* Selector Card */
.selector-card {
  background: white;
  border-radius: 24px;
  padding: 20px 24px;
  margin-bottom: 24px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.selector-grid {
  display: grid;
  grid-template-columns: 1fr 1fr auto;
  gap: 16px;
  align-items: center;
}

.course-option {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.course-name {
  font-weight: 500;
}

.course-code {
  font-size: 12px;
  color: #64748b;
}

.refresh-btn {
  padding: 10px 20px;
  border-radius: 12px;
  text-transform: none;
}

/* Exam Info */
.exam-info-card {
  background: #f8fafc;
  border-radius: 16px;
  padding: 16px 20px;
  margin-bottom: 24px;
}

.info-row {
  display: flex;
  gap: 32px;
  flex-wrap: wrap;
}

.info-item {
  display: flex;
  gap: 8px;
  align-items: center;
}

.info-item label {
  font-weight: 600;
  color: #475569;
}

.info-item span {
  color: #1e293b;
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

.stat-icon-wrapper.avg { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }
.stat-icon-wrapper.highest { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.lowest { background: linear-gradient(135deg, #fee2e2, #fecaca); color: #ef4444; }
.stat-icon-wrapper.pass { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }

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
  gap: 12px;
}

.export-btn, .stats-btn {
  border-radius: 10px;
  text-transform: none;
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
.grades-table {
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
  border-top: 1px solid #eef2f6;
}

.letter-grade {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.letter-grade.excellent { background: #dcfce7; color: #10b981; }
.letter-grade.good { background: #dbeafe; color: #3b82f6; }
.letter-grade.average { background: #fef3c7; color: #f59e0b; }
.letter-grade.poor { background: #fee2e2; color: #ef4444; }
.letter-grade.fail { background: #f1f5f9; color: #64748b; }

.rank-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 500;
}

.rank-badge.excellent { background: #dcfce7; color: #10b981; }
.rank-badge.good { background: #dbeafe; color: #3b82f6; }
.rank-badge.average { background: #fef3c7; color: #f59e0b; }
.rank-badge.poor { background: #fee2e2; color: #ef4444; }
.rank-badge.fail { background: #f1f5f9; color: #64748b; }

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

/* Distribution Chart */
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
  height: 200px;
}

.bar-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  width: 70px;
}

.bar {
  position: relative;
  width: 40px;
  border-radius: 10px 10px 6px 6px;
  transition: height 0.5s ease;
  cursor: pointer;
  min-height: 10px;
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

.bar:hover .bar-tooltip { opacity: 1; }

.bar-label { font-size: 12px; color: #64748b; }
.bar-count { font-size: 13px; font-weight: 600; color: #1e293b; }

/* Dialog */
.modern-dialog {
  border-radius: 28px !important;
  overflow: hidden;
}

.dialog-header-stats {
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

.dialog-title { font-size: 22px; font-weight: 700; margin-bottom: 4px; }
.dialog-subtitle { font-size: 13px; opacity: 0.8; }

.dialog-content { padding: 24px 28px !important; }
.dialog-actions { padding: 16px 28px 24px !important; gap: 12px; }

.statistics-list { display: flex; flex-direction: column; gap: 12px; }
.statistic-item { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eef2f6; }
.statistic-item strong { font-weight: 700; color: #1e293b; }

.text-success { color: #10b981; }
.text-error { color: #ef4444; }

/* Responsive */
@media (max-width: 968px) {
  .selector-grid { grid-template-columns: 1fr; }
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
  .chart-bars { gap: 20px; }
  .bar-item { width: 50px; }
  .hero-header { flex-direction: column; align-items: flex-start; }
  .card-header { flex-direction: column; align-items: flex-start; }
}

@media (max-width: 480px) {
  .stats-grid-modern { grid-template-columns: 1fr; }
}
</style>