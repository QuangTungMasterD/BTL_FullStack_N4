<template>
  <div class="lecturer-management">
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-school" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý giảng viên</h1>
          <p class="hero-subtitle">Quản lý thông tin, phân công giảng dạy và tài khoản giảng viên</p>
        </div>
      </div>
      <v-btn color="primary" class="hero-btn" @click="openAddDialog">
        <v-icon icon="mdi-plus" class="mr-2" />
        Thêm giảng viên
      </v-btn>
    </div>

    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-school" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalLecturers }}</div>
          <div class="stat-label">Tổng giảng viên</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper professor">
          <v-icon icon="mdi-account-tie" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ professorCount }}</div>
          <div class="stat-label">Giáo sư/PGS</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper doctor">
          <v-icon icon="mdi-doctor" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ doctorCount }}</div>
          <div class="stat-label">Tiến sĩ</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper master">
          <v-icon icon="mdi-graduation-cap" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ masterCount }}</div>
          <div class="stat-label">Thạc sĩ</div>
        </div>
      </div>
    </div>

    <div class="workspace-card">
      <div class="workspace-toolbar">
        <div class="toolbar-left">
          <div class="search-wrapper">
            <v-icon icon="mdi-magnify" size="20" class="search-icon" />
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Tìm kiếm giảng viên..." 
              class="search-input-modern"
              @input="loadLecturers"
            >
          </div>
          <div class="filter-group">
            <v-select
              v-model="selectedFaculty"
              :items="facultyOptions"
              label="Khoa"
              variant="plain"
              density="comfortable"
              class="filter-select-modern"
              hide-details
              @update:model-value="loadLecturers"
            />
            <v-select
              v-model="selectedTitle"
              :items="titleOptions"
              label="Học hàm/Học vị"
              variant="plain"
              density="comfortable"
              class="filter-select-modern"
              hide-details
              @update:model-value="loadLecturers"
            />
          </div>
        </div>
        <div class="toolbar-right">
          <v-btn variant="text" color="primary" @click="resetFilters" class="reset-btn">
            <v-icon icon="mdi-filter-remove" size="18" class="mr-1" />
            Xóa lọc
          </v-btn>
          <v-btn variant="tonal" color="primary" @click="exportData" class="export-btn">
            <v-icon icon="mdi-export" size="18" class="mr-1" />
            Xuất Excel
          </v-btn>
        </div>
      </div>

      <div class="table-container">
        <v-data-table
          :headers="headers"
          :items="lecturersData"
          :items-per-page="10"
          :loading="loading"
          hover
          class="modern-table"
        >
          <template v-slot:item.title="{ item }">
            <div class="title-badge" :class="getTitleClass(item.title)">
              {{ item.title }}
            </div>
          </template>
          <template v-slot:item.status="{ item }">
            <div class="status-badge" :class="item.status === 'Active' ? 'active' : 'inactive'">
              <span class="status-dot"></span>
              {{ item.status === 'Active' ? 'Đang công tác' : 'Đã nghỉ' }}
            </div>
          </template>
          <template v-slot:item.courses="{ item }">
            <span class="course-count">{{ item.courses }} khóa</span>
          </template>
          <template v-slot:item.actions="{ item }">
            <div class="action-group">
              <v-btn icon size="small" variant="text" class="action-btn view" @click="viewLecturer(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn edit" @click="editLecturer(item)">
                <v-icon icon="mdi-pencil" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn delete" @click="deleteLecturer(item)">
                <v-icon icon="mdi-delete" size="18" />
              </v-btn>
            </div>
          </template>
        </v-data-table>
      </div>
    </div>

    <!-- Add/Edit Lecturer Dialog -->
    <v-dialog v-model="dialogVisible" max-width="560px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-modern" :class="isEditing ? 'edit-mode' : 'create-mode'">
          <div class="dialog-header-icon">
            <v-icon :icon="isEditing ? 'mdi-pencil' : 'mdi-school-plus'" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">{{ isEditing ? 'Chỉnh sửa giảng viên' : 'Thêm giảng viên mới' }}</h2>
            <p class="dialog-subtitle">{{ isEditing ? 'Cập nhật thông tin giảng viên' : 'Nhập thông tin để tạo tài khoản giảng viên' }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <v-form ref="form" v-model="formValid">
            <div class="form-grid">
              <v-text-field v-model="formData.lecturerId" label="Mã giảng viên" variant="outlined" density="comfortable" :disabled="isEditing" prepend-inner-icon="mdi-identifier" />
              <v-text-field v-model="formData.fullName" label="Họ và tên" variant="outlined" density="comfortable" prepend-inner-icon="mdi-account" />
              <v-text-field v-model="formData.email" label="Email" type="email" variant="outlined" density="comfortable" prepend-inner-icon="mdi-email" />
              <v-text-field v-model="formData.phone" label="Số điện thoại" variant="outlined" density="comfortable" prepend-inner-icon="mdi-phone" />
              <v-select v-model="formData.faculty" :items="facultyList" label="Khoa" variant="outlined" density="comfortable" prepend-inner-icon="mdi-school" />
              <v-select v-model="formData.title" :items="titleList" label="Học hàm/Học vị" variant="outlined" density="comfortable" prepend-inner-icon="mdi-account-tie" />
              <v-text-field v-model="formData.specialization" label="Chuyên ngành" variant="outlined" density="comfortable" prepend-inner-icon="mdi-teacher" />
              <v-select v-model="formData.status" :items="statusSelectOptions" label="Trạng thái" variant="outlined" density="comfortable" prepend-inner-icon="mdi-check-circle" />
            </div>
          </v-form>
        </v-card-text>
        <v-divider />
        <v-card-actions class="dialog-actions-modern">
          <v-btn variant="text" class="cancel-btn" @click="dialogVisible = false">Hủy</v-btn>
          <v-btn :color="isEditing ? 'warning' : 'primary'" class="save-btn" @click="saveLecturer" :loading="saving">
            {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- View Lecturer Dialog -->
    <v-dialog v-model="viewDialogVisible" max-width="540px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedLecturer">
        <div class="view-header">
          <div class="view-avatar"><span>{{ selectedLecturer.fullName?.charAt(0) || '?' }}</span></div>
          <div class="view-header-info"><h3>{{ selectedLecturer.fullName }}</h3><p>{{ selectedLecturer.lecturerId }}</p></div>
          <v-chip :color="getTitleColor(selectedLecturer.title)" size="small" class="view-chip">{{ selectedLecturer.title }}</v-chip>
        </div>
        <v-divider />
        <v-card-text class="view-content">
          <div class="info-grid">
            <div class="info-item"><div class="info-icon"><v-icon icon="mdi-email" size="18" color="#64748b" /></div><div><div class="info-label">Email</div><div class="info-value">{{ selectedLecturer.email }}</div></div></div>
            <div class="info-item"><div class="info-icon"><v-icon icon="mdi-phone" size="18" color="#64748b" /></div><div><div class="info-label">Điện thoại</div><div class="info-value">{{ selectedLecturer.phone || 'Chưa cập nhật' }}</div></div></div>
            <div class="info-item"><div class="info-icon"><v-icon icon="mdi-school" size="18" color="#64748b" /></div><div><div class="info-label">Khoa</div><div class="info-value">{{ selectedLecturer.faculty }}</div></div></div>
            <div class="info-item"><div class="info-icon"><v-icon icon="mdi-teacher" size="18" color="#64748b" /></div><div><div class="info-label">Chuyên ngành</div><div class="info-value">{{ selectedLecturer.specialization }}</div></div></div>
            <div class="info-item"><div class="info-icon"><v-icon icon="mdi-book-open" size="18" color="#64748b" /></div><div><div class="info-label">Số khóa đang dạy</div><div class="info-value">{{ selectedLecturer.courses }} khóa</div></div></div>
          </div>
          <v-divider class="my-4" />
          <div class="teaching-courses"><h4 class="section-title">Khóa học đang giảng dạy</h4><div class="course-list"><div class="teaching-course" v-for="course in teachingCourses" :key="course.id"><div class="course-code">{{ course.code }}</div><div class="course-name">{{ course.name }}</div><div class="course-students">{{ course.students }} SV</div></div><div v-if="teachingCourses.length === 0" class="empty-text">Chưa có khóa học nào</div></div></div>
        </v-card-text>
        <v-card-actions class="dialog-actions-modern"><v-spacer /><v-btn color="primary" variant="text" @click="viewDialogVisible = false">Đóng</v-btn></v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/utils/api'
import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()
const searchQuery = ref('')
const selectedFaculty = ref('all')
const selectedTitle = ref('all')
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const isEditing = ref(false)
const formValid = ref(true)
const loading = ref(false)
const saving = ref(false)
const selectedLecturer = ref(null)

const facultyOptions = [
  { title: 'Tất cả khoa', value: 'all' },
  { title: 'Công nghệ thông tin', value: 'CNTT' },
  { title: 'Quản trị kinh doanh', value: 'QTKD' },
  { title: 'Ngôn ngữ Anh', value: 'NNKT' },
]

const facultyList = ['Công nghệ thông tin', 'Quản trị kinh doanh', 'Ngôn ngữ Anh']

const titleOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Giáo sư', value: 'Giáo sư' },
  { title: 'Phó Giáo sư', value: 'Phó Giáo sư' },
  { title: 'Tiến sĩ', value: 'Tiến sĩ' },
  { title: 'Thạc sĩ', value: 'Thạc sĩ' },
]

const titleList = ['Giáo sư', 'Phó Giáo sư', 'Tiến sĩ', 'Thạc sĩ']
const statusSelectOptions = [{ title: 'Đang công tác', value: 'Active' }, { title: 'Đã nghỉ', value: 'Inactive' }]

const headers = [
  { title: 'Mã GV', key: 'lecturerId', sortable: true },
  { title: 'Họ tên', key: 'fullName', sortable: true },
  { title: 'Email', key: 'email' },
  { title: 'Khoa', key: 'faculty' },
  { title: 'Học hàm/Học vị', key: 'title', sortable: true },
  { title: 'Số khóa', key: 'courses', sortable: true },
  { title: 'Trạng thái', key: 'status', sortable: true },
  { title: 'Thao tác', key: 'actions', sortable: false },
]

// Mock data fallback
const mockLecturers = [
  { id: 1, lecturerId: 'GV001', fullName: 'PGS.TS Trần Văn Xuân', email: 'tranvanx.khmt@gmail.com', phone: '0912345678', faculty: 'CNTT', title: 'Phó Giáo sư', specialization: 'Khoa học máy tính', courses: 3, status: 'Active' },
  { id: 2, lecturerId: 'GV002', fullName: 'TS. Nguyễn Thị Hải Yến', email: 'nguyenthiy.qtkd@gmail.com', phone: '0903456789', faculty: 'QTKD', title: 'Tiến sĩ', specialization: 'Quản trị chiến lược', courses: 2, status: 'Active' },
  { id: 3, lecturerId: 'GV003', fullName: 'ThS. Lê Văn Anh Tuấn', email: 'levanz.nna@gmail.com', phone: '0945678912', faculty: 'NNKT', title: 'Thạc sĩ', specialization: 'Ngôn ngữ học', courses: 2, status: 'Active' },
  { id: 4, lecturerId: 'GV004', fullName: 'GS.TS Phạm Thị Hải Vân', email: 'phamthiw.ai@gmail.com', phone: '0988765432', faculty: 'CNTT', title: 'Giáo sư', specialization: 'Trí tuệ nhân tạo', courses: 1, status: 'Active' },
]

const lecturersData = ref([])
const totalLecturers = ref(0)
const professorCount = ref(0)
const doctorCount = ref(0)
const masterCount = ref(0)
const teachingCourses = ref([])

const formData = ref({
  lecturerId: '', fullName: '', email: '', phone: '', faculty: '', title: '', specialization: '', status: 'Active'
})

const loadLecturers = async () => {
  loading.value = true
  try {
    const params = {}
    if (searchQuery.value) params.search = searchQuery.value
    if (selectedFaculty.value !== 'all') params.faculty = selectedFaculty.value
    if (selectedTitle.value !== 'all') params.title = selectedTitle.value
    
    const response = await api.get('/studentattendance/api/admin/lecturers', { params })
    const data = response.data.items || response.data || []
    
    if (data && data.length > 0) {
      lecturersData.value = data
    } else {
      lecturersData.value = [...mockLecturers]
    }
    totalLecturers.value = lecturersData.value.length
    professorCount.value = lecturersData.value.filter(l => l.title === 'Giáo sư' || l.title === 'Phó Giáo sư').length
    doctorCount.value = lecturersData.value.filter(l => l.title === 'Tiến sĩ').length
    masterCount.value = lecturersData.value.filter(l => l.title === 'Thạc sĩ').length
  } catch (error) {
    console.error('API failed, using mock data:', error)
    lecturersData.value = [...mockLecturers]
    totalLecturers.value = lecturersData.value.length
    professorCount.value = lecturersData.value.filter(l => l.title === 'Giáo sư' || l.title === 'Phó Giáo sư').length
    doctorCount.value = lecturersData.value.filter(l => l.title === 'Tiến sĩ').length
    masterCount.value = lecturersData.value.filter(l => l.title === 'Thạc sĩ').length
  } finally {
    loading.value = false
  }
}

const resetFilters = () => {
  searchQuery.value = ''
  selectedFaculty.value = 'all'
  selectedTitle.value = 'all'
  loadLecturers()
}

const getTitleColor = (title) => {
  const colors = { 'Giáo sư': 'deep-purple', 'Phó Giáo sư': 'purple', 'Tiến sĩ': 'primary', 'Thạc sĩ': 'info' }
  return colors[title] || 'default'
}

const getTitleClass = (title) => {
  const classes = { 'Giáo sư': 'professor', 'Phó Giáo sư': 'associate', 'Tiến sĩ': 'doctor', 'Thạc sĩ': 'master' }
  return classes[title] || ''
}

const openAddDialog = () => {
  isEditing.value = false
  formData.value = { lecturerId: '', fullName: '', email: '', phone: '', faculty: '', title: '', specialization: '', status: 'Active' }
  dialogVisible.value = true
}

const editLecturer = (lecturer) => {
  isEditing.value = true
  formData.value = { ...lecturer }
  dialogVisible.value = true
}

const viewLecturer = (lecturer) => {
  selectedLecturer.value = lecturer
  viewDialogVisible.value = true
  teachingCourses.value = [
    { id: 1, code: 'CS101', name: 'Nhập môn lập trình', students: 45 },
    { id: 2, code: 'CS201', name: 'Cấu trúc dữ liệu', students: 42 },
  ]
}

const deleteLecturer = async (lecturer) => {
  if (confirm(`Bạn có chắc muốn xóa giảng viên ${lecturer.fullName}?`)) {
    try {
      await api.delete(`/studentattendance/api/admin/lecturers/${lecturer.id}`)
      await loadLecturers()
    } catch (error) {
      console.error('Delete failed, removing from local:', error)
      const index = lecturersData.value.findIndex(l => l.id === lecturer.id)
      if (index > -1) lecturersData.value.splice(index, 1)
      totalLecturers.value = lecturersData.value.length
      professorCount.value = lecturersData.value.filter(l => l.title === 'Giáo sư' || l.title === 'Phó Giáo sư').length
      doctorCount.value = lecturersData.value.filter(l => l.title === 'Tiến sĩ').length
      masterCount.value = lecturersData.value.filter(l => l.title === 'Thạc sĩ').length
    }
  }
}

const saveLecturer = async () => {
  saving.value = true
  try {
    if (isEditing.value) {
      await api.put(`/studentattendance/api/admin/lecturers/${formData.value.id}`, formData.value)
    } else {
      await api.post('/studentattendance/api/admin/lecturers', formData.value)
    }
    await loadLecturers()
    dialogVisible.value = false
  } catch (error) {
    console.error('Save failed, using local:', error)
    if (isEditing.value) {
      const index = lecturersData.value.findIndex(l => l.id === formData.value.id)
      if (index > -1) lecturersData.value[index] = { ...formData.value }
    } else {
      const newId = Math.max(...lecturersData.value.map(l => l.id), 0) + 1
      lecturersData.value.push({ ...formData.value, id: newId, courses: 0 })
    }
    dialogVisible.value = false
    loadLecturers()
  } finally {
    saving.value = false
  }
}

const exportData = () => { console.log('Export data') }

onMounted(() => { loadLecturers() })
</script>

<style scoped>
.lecturer-management {
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

.stat-icon-wrapper.total { background: linear-gradient(135deg, #dbeafe, #bfdbfe); color: #3b82f6; }
.stat-icon-wrapper.professor { background: linear-gradient(135deg, #e9d5ff, #d8b4fe); color: #8b5cf6; }
.stat-icon-wrapper.doctor { background: linear-gradient(135deg, #dcfce7, #bbf7d0); color: #10b981; }
.stat-icon-wrapper.master { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }

.stat-info-modern .stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #1e293b;
}

.stat-info-modern .stat-label {
  font-size: 13px;
  color: #64748b;
  margin-top: 4px;
}

.workspace-card {
  background: white;
  border-radius: 28px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06);
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

.filter-group {
  display: flex;
  gap: 12px;
}

.filter-select-modern {
  min-width: 150px;
}

.toolbar-right {
  display: flex;
  gap: 12px;
}

.reset-btn, .export-btn {
  border-radius: 12px;
  text-transform: none;
  font-weight: 500;
}

.table-container {
  overflow-x: auto;
}

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

.title-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 500;
}

.title-badge.professor { background: #e9d5ff; color: #8b5cf6; }
.title-badge.associate { background: #dbeafe; color: #3b82f6; }
.title-badge.doctor { background: #dcfce7; color: #10b981; }
.title-badge.master { background: #fef3c7; color: #f59e0b; }

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active { background: #dcfce7; color: #10b981; }
.status-badge.inactive { background: #f1f5f9; color: #64748b; }

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}

.course-count {
  font-weight: 600;
  color: #3b82f6;
}

.action-group {
  display: flex;
  gap: 4px;
}

.action-btn.view:hover { background: #dbeafe; color: #3b82f6; }
.action-btn.edit:hover { background: #fef3c7; color: #f59e0b; }
.action-btn.delete:hover { background: #fee2e2; color: #ef4444; }

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

.dialog-actions-modern {
  padding: 20px 28px !important;
  gap: 12px;
}

.cancel-btn, .save-btn {
  border-radius: 40px;
  padding: 10px 24px;
  text-transform: none;
  font-weight: 600;
}

.view-header {
  padding: 28px;
  display: flex;
  align-items: center;
  gap: 20px;
  background: linear-gradient(135deg, #f8fafc, #f1f5f9);
}

.view-avatar {
  width: 72px;
  height: 72px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 32px;
  font-weight: 600;
}

.view-header-info h3 {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.view-header-info p {
  font-size: 13px;
  color: #64748b;
}

.view-chip {
  margin-left: auto;
}

.view-content {
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

.course-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.teaching-course {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #f8fafc;
  border-radius: 14px;
}

.teaching-course .course-code {
  font-family: monospace;
  font-weight: 600;
  color: #3b82f6;
  font-size: 13px;
}

.teaching-course .course-name {
  flex: 1;
  margin-left: 16px;
  font-weight: 500;
}

.teaching-course .course-students {
  font-size: 12px;
  color: #64748b;
}

.empty-text {
  text-align: center;
  padding: 20px;
  color: #94a3b8;
}

@media (max-width: 1200px) {
  .stats-grid-modern { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 768px) {
  .hero-header { flex-direction: column; align-items: flex-start; }
  .toolbar-left { flex-direction: column; width: 100%; }
  .search-wrapper { width: 100%; }
  .filter-group { width: 100%; flex-direction: column; }
  .filter-select-modern { width: 100%; }
  .form-grid { grid-template-columns: 1fr; }
  .info-grid { grid-template-columns: 1fr; }
}
</style>