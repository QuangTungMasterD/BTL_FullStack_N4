<template>
  <div class="my-students-container">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-account-group" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Danh sách sinh viên</h1>
          <p class="hero-subtitle">Quản lý sinh viên trong các lớp học của bạn</p>
        </div>
      </div>
      <v-btn color="primary" class="hero-btn" @click="exportList" :loading="exporting">
        <v-icon icon="mdi-export" class="mr-2" />
        Xuất danh sách
      </v-btn>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-account-group" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalStudents }}</div>
          <div class="stat-label">Tổng sinh viên</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper attendance">
          <v-icon icon="mdi-calendar-check" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ averageAttendance }}%</div>
          <div class="stat-label">Tỷ lệ chuyên cần TB</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper grade">
          <v-icon icon="mdi-chart-line" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ averageGrade }}</div>
          <div class="stat-label">Điểm trung bình TB</div>
        </div>
      </div>
    </div>

    <!-- Workspace Card -->
    <div class="workspace-card">
      <div class="workspace-toolbar">
        <div class="toolbar-left">
          <div class="search-wrapper">
            <v-icon icon="mdi-magnify" size="20" class="search-icon" />
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Tìm kiếm sinh viên..." 
              class="search-input-modern"
              @input="loadStudents"
            >
          </div>
          <div class="filter-group">
            <v-select
              v-model="selectedCourse"
              :items="courseOptions"
              label="Chọn lớp học"
              variant="plain"
              density="comfortable"
              class="filter-select-modern"
              hide-details
              @update:model-value="loadStudents"
            />
          </div>
        </div>
        <div class="toolbar-right">
          <v-btn variant="text" color="primary" @click="resetFilters" class="reset-btn">
            <v-icon icon="mdi-refresh" size="18" class="mr-1" />
            Làm mới
          </v-btn>
        </div>
      </div>

      <div class="table-container">
        <v-data-table
          :headers="headers"
          :items="studentsData"
          :items-per-page="10"
          :loading="loading"
          hover
          class="modern-table"
        >
          <template v-slot:item.attendance="{ item }">
            <div class="attendance-cell">
              <div class="attendance-bar">
                <div class="attendance-fill" :style="{ width: `${item.attendance}%`, background: getAttendanceColor(item.attendance) }"></div>
              </div>
              <span class="attendance-text">{{ item.attendance }}%</span>
            </div>
          </template>
          <template v-slot:item.averageGrade="{ item }">
            <span class="grade-value" :class="getGradeClass(item.averageGrade)">{{ item.averageGrade }}</span>
          </template>
          <template v-slot:item.rank="{ item }">
            <div class="rank-badge" :class="getRankClass(item.rank)">{{ item.rank }}</div>
          </template>
          <template v-slot:item.actions="{ item }">
            <div class="action-group">
              <v-btn icon size="small" variant="text" class="action-btn view" @click="viewDetails(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn attendance" @click="viewAttendance(item)">
                <v-icon icon="mdi-calendar-check" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn grade" @click="enterGrade(item)">
                <v-icon icon="mdi-chart-line" size="18" />
              </v-btn>
            </div>
          </template>
        </v-data-table>
      </div>
    </div>

    <!-- View Student Dialog -->
    <v-dialog v-model="showDetailDialog" max-width="560px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedStudent">
        <div class="dialog-header-view">
          <div class="dialog-avatar">
            <span>{{ selectedStudent.fullName?.charAt(0) || '?' }}</span>
          </div>
          <div>
            <h3>{{ selectedStudent.fullName }}</h3>
            <p class="student-code">{{ selectedStudent.studentId }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content">
          <div class="info-grid">
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-email" size="18" color="#64748b" /></div>
              <div><div class="info-label">Email</div><div class="info-value">{{ selectedStudent.email }}</div></div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-phone" size="18" color="#64748b" /></div>
              <div><div class="info-label">Điện thoại</div><div class="info-value">{{ selectedStudent.phone || 'Chưa cập nhật' }}</div></div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-school" size="18" color="#64748b" /></div>
              <div><div class="info-label">Lớp</div><div class="info-value">{{ selectedStudent.class || 'Chưa cập nhật' }}</div></div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-domain" size="18" color="#64748b" /></div>
              <div><div class="info-label">Khoa</div><div class="info-value">{{ selectedStudent.faculty || 'Chưa cập nhật' }}</div></div>
            </div>
          </div>
          <v-divider class="my-4" />
          <h4 class="section-title">Kết quả học tập các môn</h4>
          <div class="course-grade-list">
            <div class="course-grade-item" v-for="grade in studentCourses" :key="grade.courseId">
              <div class="course-grade-name">{{ grade.courseName }}</div>
              <div class="course-grade-score">{{ grade.score }}</div>
            </div>
          </div>
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-spacer />
          <v-btn color="primary" variant="text" @click="showDetailDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Enter Grade Dialog -->
    <v-dialog v-model="showGradeDialog" max-width="520px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-grade">
          <div class="dialog-header-icon">
            <v-icon icon="mdi-chart-line" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">Nhập điểm</h2>
            <p class="dialog-subtitle">{{ gradeStudent?.fullName }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content">
          <v-select v-model="selectedExam" :items="examTypes" label="Loại điểm" variant="outlined" density="comfortable" />
          <v-text-field v-model="gradeScore" label="Điểm số" type="number" step="0.1" min="0" max="10" variant="outlined" density="comfortable" />
          <v-text-field v-model="gradeMaxScore" label="Điểm tối đa" type="number" step="0.5" min="0" max="10" variant="outlined" density="comfortable" />
          <v-text-field v-model="gradeWeight" label="Trọng số (%)" type="number" step="5" min="0" max="100" variant="outlined" density="comfortable" />
        </v-card-text>
        <v-card-actions class="dialog-actions">
          <v-btn variant="text" @click="showGradeDialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="saveGrade" :loading="savingGrade">Lưu</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/utils/api'

const selectedCourse = ref('')
const searchQuery = ref('')
const loading = ref(false)
const exporting = ref(false)
const savingGrade = ref(false)
const showDetailDialog = ref(false)
const showGradeDialog = ref(false)
const selectedStudent = ref(null)
const gradeStudent = ref(null)

const courseOptions = ref([])
const studentsData = ref([])
const totalStudents = ref(0)
const averageAttendance = ref(0)
const averageGrade = ref(0)
const studentCourses = ref([])
const selectedExam = ref('Giữa kỳ')
const gradeScore = ref(0)
const gradeMaxScore = ref(10)
const gradeWeight = ref(25)

const examTypes = ['Quiz 1', 'Quiz 2', 'Giữa kỳ', 'Bài tập lớn', 'Cuối kỳ']

const headers = [
  { title: 'STT', key: 'index', sortable: false, width: 60 },
  { title: 'Mã sinh viên', key: 'studentId', sortable: true },
  { title: 'Họ tên', key: 'fullName', sortable: true },
  { title: 'Email', key: 'email' },
  { title: 'Chuyên cần', key: 'attendance', sortable: true },
  { title: 'Điểm TB', key: 'averageGrade', sortable: true },
  { title: 'Xếp loại', key: 'rank', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false, width: 120 },
]

const loadCourses = async () => {
  try {
    const response = await api.get('/studentattendance/api/lecturer/courses')
    courseOptions.value = response.data.map(c => ({ title: c.courseName, value: c.id }))
    if (courseOptions.value.length > 0 && !selectedCourse.value) {
      selectedCourse.value = courseOptions.value[0].value
      loadStudents()
    }
  } catch (error) {
    console.error('Failed to load courses:', error)
  }
}

const loadStudents = async () => {
  if (!selectedCourse.value) return
  loading.value = true
  try {
    const params = { courseId: selectedCourse.value, search: searchQuery.value || undefined }
    const response = await api.get('/studentattendance/api/lecturer/students', { params })
    studentsData.value = response.data.items.map((s, idx) => ({ ...s, index: idx + 1 }))
    totalStudents.value = response.data.total || studentsData.value.length
    if (studentsData.value.length > 0) {
      const avgAtt = studentsData.value.reduce((sum, s) => sum + s.attendance, 0) / studentsData.value.length
      const avgGr = studentsData.value.reduce((sum, s) => sum + s.averageGrade, 0) / studentsData.value.length
      averageAttendance.value = Math.round(avgAtt)
      averageGrade.value = avgGr.toFixed(1)
    }
  } catch (error) {
    console.error('Failed to load students:', error)
  } finally {
    loading.value = false
  }
}

const resetFilters = () => {
  searchQuery.value = ''
  loadStudents()
}

const getAttendanceColor = (attendance) => {
  if (attendance >= 85) return '#10b981'
  if (attendance >= 70) return '#f59e0b'
  return '#ef4444'
}

const getGradeClass = (grade) => {
  if (grade >= 8.5) return 'excellent'
  if (grade >= 7) return 'good'
  if (grade >= 5) return 'average'
  return 'poor'
}

const getRankClass = (rank) => {
  const classes = { 'Xuất sắc': 'excellent', 'Giỏi': 'good', 'Khá': 'average', 'Trung bình': 'poor', 'Yếu': 'fail' }
  return classes[rank] || 'default'
}

const viewDetails = async (student) => {
  selectedStudent.value = student
  try {
    const response = await api.get(`/studentattendance/api/lecturer/students/${student.id}/grades`)
    studentCourses.value = response.data
  } catch (error) {
    console.error('Failed to load student grades:', error)
    studentCourses.value = []
  }
  showDetailDialog.value = true
}

const viewAttendance = (student) => {
  // router.push(`/lecturer-attendance?student=${student.id}`)
  console.log('View attendance:', student)
}

const enterGrade = (student) => {
  gradeStudent.value = student
  gradeScore.value = 0
  selectedExam.value = 'Giữa kỳ'
  gradeMaxScore.value = 10
  gradeWeight.value = 25
  showGradeDialog.value = true
}

const saveGrade = async () => {
  savingGrade.value = true
  try {
    await api.post('/studentattendance/api/lecturer/grades', {
      studentId: gradeStudent.value.id,
      courseId: selectedCourse.value,
      examType: selectedExam.value,
      score: parseFloat(gradeScore.value),
      maxScore: parseFloat(gradeMaxScore.value),
      weight: parseFloat(gradeWeight.value) / 100
    })
    await loadStudents()
    showGradeDialog.value = false
  } catch (error) {
    console.error('Failed to save grade:', error)
    alert('Lưu điểm thất bại')
  } finally {
    savingGrade.value = false
  }
}

const exportList = async () => {
  exporting.value = true
  try {
    const response = await api.get('/studentattendance/api/lecturer/students/export', {
      params: { courseId: selectedCourse.value },
      responseType: 'blob'
    })
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `students_${selectedCourse.value}.xlsx`)
    document.body.appendChild(link); link.click(); link.remove()
  } catch (error) {
    console.error('Export failed:', error)
    alert('Xuất danh sách thất bại')
  } finally {
    exporting.value = false
  }
}

onMounted(() => {
  loadCourses()
})
</script>

<style scoped>
.my-students-container {
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
}

/* Stats Cards */
.stats-grid-modern {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
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
.stat-icon-wrapper.attendance { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.grade { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }

.stat-info-modern .stat-value { font-size: 28px; font-weight: 700; color: #1e293b; }
.stat-info-modern .stat-label { font-size: 13px; color: #64748b; margin-top: 4px; }

/* Workspace Card */
.workspace-card {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
}

.workspace-toolbar {
  padding: 20px 24px;
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
  gap: 20px;
  flex-wrap: wrap;
  flex: 1;
}

.search-wrapper {
  position: relative;
  min-width: 280px;
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
  padding: 12px 16px 12px 44px;
  border: 1.5px solid #e2e8f0;
  border-radius: 14px;
  font-size: 14px;
  transition: all 0.2s ease;
  background: white;
}

.search-input-modern:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.filter-select-modern {
  min-width: 200px;
}

.reset-btn {
  border-radius: 12px;
  text-transform: none;
}

.table-container {
  overflow-x: auto;
}

/* Table */
.modern-table :deep(.v-data-table-header th) {
  background: #f8fafc;
  font-weight: 600;
  color: #475569;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 16px;
}

.modern-table :deep(td) {
  padding: 16px;
}

.attendance-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.attendance-bar {
  width: 70px;
  height: 6px;
  background: #e2e8f0;
  border-radius: 10px;
  overflow: hidden;
}

.attendance-fill {
  height: 100%;
  border-radius: 10px;
}

.attendance-text {
  font-size: 13px;
  font-weight: 500;
}

.grade-value {
  font-weight: 700;
}

.grade-value.excellent { color: #10b981; }
.grade-value.good { color: #3b82f6; }
.grade-value.average { color: #f59e0b; }
.grade-value.poor { color: #ef4444; }

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

.action-group {
  display: flex;
  gap: 4px;
}

.action-btn.view:hover { background: #dbeafe; color: #3b82f6; }
.action-btn.attendance:hover { background: #dcfce7; color: #10b981; }
.action-btn.grade:hover { background: #fef3c7; color: #f59e0b; }

/* Dialog */
.modern-dialog {
  border-radius: 28px !important;
  overflow: hidden;
}

.dialog-header-view, .dialog-header-grade {
  padding: 24px 28px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.dialog-header-view {
  background: linear-gradient(135deg, #f8fafc, #f1f5f9);
}

.dialog-header-grade {
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

.dialog-avatar {
  width: 56px;
  height: 56px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 50%;
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
  margin-bottom: 2px;
}

.dialog-subtitle {
  font-size: 13px;
  opacity: 0.8;
}

.student-code {
  font-size: 12px;
  color: #64748b;
  margin-top: 4px;
}

.dialog-content {
  padding: 24px 28px !important;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.info-item {
  display: flex;
  gap: 12px;
  align-items: flex-start;
}

.info-icon {
  width: 32px;
  flex-shrink: 0;
}

.info-label {
  font-size: 11px;
  color: #64748b;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 4px;
}

.info-value {
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.course-grade-list {
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
  border-radius: 14px;
}

.course-grade-name {
  font-weight: 500;
  color: #1e293b;
}

.course-grade-score {
  font-weight: 700;
  color: #3b82f6;
}

.dialog-actions {
  padding: 16px 28px 24px !important;
  gap: 12px;
}

@media (max-width: 768px) {
  .hero-header { flex-direction: column; align-items: flex-start; }
  .stats-grid-modern { grid-template-columns: 1fr; }
  .toolbar-left { flex-direction: column; width: 100%; }
  .search-wrapper { width: 100%; }
  .filter-select-modern { width: 100%; }
  .info-grid { grid-template-columns: 1fr; }
}
</style>