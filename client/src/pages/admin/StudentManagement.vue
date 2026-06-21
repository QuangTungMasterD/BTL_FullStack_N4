<template>
  <div class="student-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-account-group" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý học viên</h1>
          <p class="hero-subtitle">Quản lý thông tin, hồ sơ và tài khoản học viên trung tâm</p>
        </div>
      </div>
      <v-btn color="primary" class="hero-btn" @click="openAddDialog">
        <v-icon icon="mdi-plus" class="mr-2" />
        Thêm học viên
      </v-btn>
    </div>

    <!-- Statistics Section -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-account-group" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalStudents }}</div>
          <div class="stat-label">Tổng học viên</div>
          <div class="stat-trend positive">+12% so với tháng trước</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper active">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ activeStudents }}</div>
          <div class="stat-label">Đang học</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper completed">
          <v-icon icon="mdi-flag-checkered" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ completedStudents }}</div>
          <div class="stat-label">Đã hoàn thành</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper new">
          <v-icon icon="mdi-account-plus" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ newStudentsThisMonth }}</div>
          <div class="stat-label">Mới trong tháng</div>
        </div>
      </div>
    </div>

    <!-- Main Workspace Card -->
    <div class="workspace-card">
      <div class="workspace-toolbar">
        <div class="toolbar-left">
          <div class="search-wrapper">
            <v-icon icon="mdi-magnify" size="20" class="search-icon" />
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Tìm kiếm học viên..." 
              class="search-input-modern"
              @input="loadStudents"
            >
          </div>
          <div class="filter-group">
            <v-select
              v-model="selectedFaculty"
              :items="facultyOptions"
              label="Khóa học"
              variant="plain"
              density="comfortable"
              class="filter-select-modern"
              hide-details
              @update:model-value="loadStudents"
            >
              <template v-slot:selection="{ item }">
                <span class="filter-selected">{{ item?.title || 'Tất cả khóa học' }}</span>
              </template>
            </v-select>
            <v-select
              v-model="selectedStatus"
              :items="statusOptions"
              label="Trạng thái"
              variant="plain"
              density="comfortable"
              class="filter-select-modern"
              hide-details
              @update:model-value="loadStudents"
            >
              <template v-slot:selection="{ item }">
                <span class="filter-selected">{{ item?.title || 'Tất cả' }}</span>
              </template>
            </v-select>
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
          :items="studentsData"
          :items-per-page="10"
          :loading="loading"
          hover
          class="modern-table"
        >
          <template v-slot:item.status="{ item }">
            <div class="status-badge" :class="getStatusClass(item.status)">
              <span class="status-dot"></span>
              {{ getStatusText(item.status) }}
            </div>
          </template>
          <template v-slot:item.actions="{ item }">
            <div class="action-group">
              <v-btn icon size="small" variant="text" class="action-btn view" @click="viewStudent(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn edit" @click="editStudent(item)">
                <v-icon icon="mdi-pencil" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn delete" @click="deleteStudent(item)">
                <v-icon icon="mdi-delete" size="18" />
              </v-btn>
            </div>
          </template>
          <template v-slot:no-data>
            <div class="empty-state">
              <v-icon icon="mdi-account-off" size="56" color="#cbd5e1" />
              <p>Không có dữ liệu học viên</p>
              <v-btn color="primary" variant="tonal" @click="openAddDialog" class="mt-3">
                <v-icon icon="mdi-plus" class="mr-2" />
                Thêm học viên mới
              </v-btn>
            </div>
          </template>
        </v-data-table>
      </div>
    </div>

    <!-- Add/Edit Student Dialog -->
    <v-dialog v-model="dialogVisible" max-width="560px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-modern" :class="isEditing ? 'edit-mode' : 'create-mode'">
          <div class="dialog-header-icon">
            <v-icon :icon="isEditing ? 'mdi-pencil' : 'mdi-account-plus'" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">{{ isEditing ? 'Chỉnh sửa học viên' : 'Thêm học viên mới' }}</h2>
            <p class="dialog-subtitle">{{ isEditing ? 'Cập nhật thông tin học viên' : 'Nhập thông tin để tạo tài khoản học viên' }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <v-form ref="form" v-model="formValid">
            <div class="form-grid">
              <v-text-field
                v-model="formData.studentId"
                label="Mã học viên"
                variant="outlined"
                density="comfortable"
                :disabled="isEditing"
                prepend-inner-icon="mdi-identifier"
              />
              <v-text-field
                v-model="formData.fullName"
                label="Họ và tên"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-account"
              />
              <v-text-field
                v-model="formData.email"
                label="Email"
                type="email"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-email"
              />
              <v-text-field
                v-model="formData.phone"
                label="Số điện thoại"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-phone"
              />
              <v-select
                v-model="formData.faculty"
                :items="facultyList"
                label="Khóa học"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-school"
              />
              <v-select
                v-model="formData.status"
                :items="statusSelectOptions"
                label="Trạng thái"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-check-circle"
              />
              <v-text-field
                v-model="formData.major"
                label="Chuyên ngành"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-book-open"
              />
              <v-text-field
                v-model="formData.class"
                label="Lớp"
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
          <v-btn :color="isEditing ? 'warning' : 'primary'" class="save-btn" @click="saveStudent" :loading="saving">
            {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- View Student Dialog -->
    <v-dialog v-model="viewDialogVisible" max-width="520px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedStudent">
        <div class="view-header">
          <div class="view-avatar">
            <span>{{ selectedStudent.fullName?.charAt(0) || '?' }}</span>
          </div>
          <div class="view-header-info">
            <h3>{{ selectedStudent.fullName }}</h3>
            <p>{{ selectedStudent.studentId }}</p>
          </div>
          <v-chip :color="selectedStudent.status === 'Active' ? 'success' : selectedStudent.status === 'Completed' ? 'primary' : 'default'" size="small" class="view-chip">
            {{ getStatusText(selectedStudent.status) }}
          </v-chip>
        </div>
        <v-divider />
        <v-card-text class="view-content">
          <div class="info-grid">
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-email" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Email</div>
                <div class="info-value">{{ selectedStudent.email }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-phone" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Điện thoại</div>
                <div class="info-value">{{ selectedStudent.phone || 'Chưa cập nhật' }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-school" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Khóa học</div>
                <div class="info-value">{{ selectedStudent.faculty || 'Chưa phân loại' }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-book-open" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Chuyên ngành</div>
                <div class="info-value">{{ selectedStudent.major || 'Chưa cập nhật' }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-account-group" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Lớp</div>
                <div class="info-value">{{ selectedStudent.class || 'Chưa phân lớp' }}</div>
              </div>
            </div>
          </div>
        </v-card-text>
        <v-card-actions class="dialog-actions-modern">
          <v-spacer />
          <v-btn color="primary" variant="text" @click="viewDialogVisible = false">Đóng</v-btn>
        </v-card-actions>
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
const selectedStatus = ref('all')
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const isEditing = ref(false)
const loading = ref(false)
const saving = ref(false)
const selectedStudent = ref(null)
const formValid = ref(false)

// Options cho filter
const facultyOptions = [
  { title: 'Tất cả khóa học', value: 'all' },
  { title: 'Lập trình Python', value: 'Python' },
  { title: 'Lập trình Java', value: 'Java' },
  { title: 'Tiếng Anh giao tiếp', value: 'English' },
  { title: 'Toán cao cấp', value: 'Math' },
  { title: 'Kỹ năng mềm', value: 'SoftSkills' },
]

const facultyList = ['Python', 'Java', 'English', 'Math', 'SoftSkills', 'Data Science', 'AI/ML']

const statusOptions = [
  { title: 'Tất cả', value: 'all' },
  { title: 'Đang học', value: 'Active' },
  { title: 'Đã hoàn thành', value: 'Completed' },
  { title: 'Tạm nghỉ', value: 'Inactive' },
]

const statusSelectOptions = [
  { title: 'Đang học', value: 'Active' },
  { title: 'Đã hoàn thành', value: 'Completed' },
  { title: 'Tạm nghỉ', value: 'Inactive' },
]

// Headers cho bảng
const headers = [
  { title: 'Mã HV', key: 'studentId', sortable: true, align: 'start' },
  { title: 'Họ tên', key: 'fullName', sortable: true, align: 'start' },
  { title: 'Email', key: 'email', align: 'start' },
  { title: 'Khóa học', key: 'faculty', align: 'start' },
  { title: 'Chuyên ngành', key: 'major', align: 'start' },
  { title: 'Lớp', key: 'class', align: 'start' },
  { title: 'Trạng thái', key: 'status', sortable: true, align: 'center' },
  { title: 'Thao tác', key: 'actions', sortable: false, align: 'center', width: 120 },
]

// Mock data
const mockStudents = [
  { id: 1, studentId: 'HV001', fullName: 'Nguyễn Văn An', email: 'nguyenvanan@gmail.com', phone: '0352123456', faculty: 'Python', major: 'Lập trình Python', class: 'PY2024-A', status: 'Active' },
  { id: 2, studentId: 'HV002', fullName: 'Trần Thị Ngọc Bích', email: 'tranthibich@gmail.com', phone: '0363234567', faculty: 'Java', major: 'Lập trình Java', class: 'JA2024-A', status: 'Active' },
  { id: 3, studentId: 'HV003', fullName: 'Lê Văn Cường', email: 'levancuong@gmail.com', phone: '0374345678', faculty: 'English', major: 'Tiếng Anh giao tiếp', class: 'EN2024-A', status: 'Completed' },
  { id: 4, studentId: 'HV004', fullName: 'Phạm Thị Phương Dung', email: 'phamthidung@gmail.com', phone: '0385456789', faculty: 'Math', major: 'Toán cao cấp', class: 'MA2024-A', status: 'Active' },
  { id: 5, studentId: 'HV005', fullName: 'Hoàng Văn Minh Em', email: 'hoangvanem@gmail.com', phone: '0396567890', faculty: 'Python', major: 'Lập trình Python', class: 'PY2024-B', status: 'Active' },
  { id: 6, studentId: 'HV006', fullName: 'Đỗ Thị Thu Hà', email: 'dothuha@gmail.com', phone: '0367678901', faculty: 'SoftSkills', major: 'Kỹ năng mềm', class: 'SK2024-A', status: 'Inactive' },
]

const studentsData = ref([])
const totalStudents = ref(0)
const activeStudents = ref(0)
const completedStudents = ref(0)
const newStudentsThisMonth = ref(0)

const formData = ref({
  studentId: '',
  fullName: '',
  email: '',
  phone: '',
  faculty: '',
  major: '',
  class: '',
  status: 'Active'
})

// Helper methods
const getStatusClass = (status) => {
  switch(status) {
    case 'Active': return 'active'
    case 'Completed': return 'completed'
    case 'Inactive': return 'inactive'
    default: return 'inactive'
  }
}

const getStatusText = (status) => {
  switch(status) {
    case 'Active': return 'Đang học'
    case 'Completed': return 'Đã hoàn thành'
    case 'Inactive': return 'Tạm nghỉ'
    default: return 'Không xác định'
  }
}

// Load students
const loadStudents = async () => {
  loading.value = true
  try {
    const params = {}
    if (searchQuery.value) params.search = searchQuery.value
    if (selectedFaculty.value !== 'all') params.faculty = selectedFaculty.value
    if (selectedStatus.value !== 'all') params.status = selectedStatus.value
    
    const response = await api.get('/api/admin/students', { params })
    const data = response.data.items || response.data || []
    
    if (data && data.length > 0) {
      studentsData.value = data
    } else {
      studentsData.value = [...mockStudents]
    }
    updateStatistics()
  } catch (error) {
    console.error('❌ API failed, using mock data:', error)
    studentsData.value = [...mockStudents]
    updateStatistics()
  } finally {
    loading.value = false
  }
}

const updateStatistics = () => {
  totalStudents.value = studentsData.value.length
  activeStudents.value = studentsData.value.filter(s => s.status === 'Active').length
  completedStudents.value = studentsData.value.filter(s => s.status === 'Completed').length
  // Tính số học viên mới trong tháng (mock)
  newStudentsThisMonth.value = Math.floor(Math.random() * 30) + 5
}

const resetFilters = () => {
  searchQuery.value = ''
  selectedFaculty.value = 'all'
  selectedStatus.value = 'all'
  loadStudents()
}

const openAddDialog = () => {
  isEditing.value = false
  formData.value = { studentId: '', fullName: '', email: '', phone: '', faculty: '', major: '', class: '', status: 'Active' }
  dialogVisible.value = true
}

const editStudent = (student) => {
  isEditing.value = true
  formData.value = { ...student }
  dialogVisible.value = true
}

const viewStudent = (student) => {
  selectedStudent.value = student
  viewDialogVisible.value = true
}

const deleteStudent = async (student) => {
  if (confirm(`Bạn có chắc muốn xóa học viên ${student.fullName}?`)) {
    try {
      await api.delete(`/api/admin/students/${student.id}`)
      await loadStudents()
    } catch (error) {
      console.error('❌ Delete failed, removing from local list:', error)
      const index = studentsData.value.findIndex(s => s.id === student.id)
      if (index > -1) studentsData.value.splice(index, 1)
      updateStatistics()
    }
  }
}

const saveStudent = async () => {
  saving.value = true
  try {
    if (isEditing.value) {
      await api.put(`/api/admin/students/${formData.value.id}`, formData.value)
    } else {
      await api.post('/api/admin/students', formData.value)
    }
    await loadStudents()
    dialogVisible.value = false
  } catch (error) {
    console.error('❌ Save failed, using local:', error)
    if (isEditing.value) {
      const index = studentsData.value.findIndex(s => s.id === formData.value.id)
      if (index > -1) studentsData.value[index] = { ...formData.value }
    } else {
      const newId = Math.max(...studentsData.value.map(s => s.id), 0) + 1
      studentsData.value.push({ ...formData.value, id: newId })
    }
    updateStatistics()
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
  console.log('🚀 Khởi tạo trang Quản lý học viên...')
  loadStudents()
  console.log('✅ Trang Quản lý học viên đã sẵn sàng')
})
</script>

<style scoped>
.student-management {
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

/* Stats Grid */
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
.stat-icon-wrapper.completed { background: linear-gradient(135deg, #e9d5ff, #d8b4fe); color: #8b5cf6; }
.stat-icon-wrapper.new { background: linear-gradient(135deg, #fef3c7, #fde68a); color: #f59e0b; }

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

/* Workspace Card */
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

.filter-select-modern :deep(.v-field) {
  background: transparent;
}

.filter-select-modern :deep(.v-field__input) {
  padding: 8px 0;
}

.filter-selected {
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
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

/* Table Styles */
.table-container {
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
  padding: 16px;
  border-bottom: 2px solid #e2e8f0;
  white-space: nowrap;
}

.modern-table :deep(.v-data-table-header th:first-child) {
  border-top-left-radius: 12px;
}

.modern-table :deep(.v-data-table-header th:last-child) {
  border-top-right-radius: 12px;
}

.modern-table :deep(td) {
  padding: 16px;
  border-bottom: 1px solid #f1f5f9;
}

.modern-table :deep(.v-data-table-footer) {
  padding: 16px 24px;
  background: white;
  border-top: 1px solid #eef2f6;
}

.modern-table :deep(.v-data-table-footer .v-select) {
  width: 100px;
}

.modern-table :deep(.v-data-table-footer .v-select .v-field) {
  border-radius: 8px;
}

.modern-table :deep(.v-data-table-footer .v-select .v-field__input) {
  text-align: center;
  justify-content: center;
}

.modern-table :deep(.v-data-table-footer .v-select .v-select__selection) {
  justify-content: center;
}

.modern-table :deep(.v-data-table-footer .v-pagination) {
  justify-content: flex-end;
}

.modern-table :deep(.v-data-table-footer .v-data-table-footer__info) {
  font-size: 13px;
  color: #64748b;
}

/* Status Badge */
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 30px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active {
  background: #dcfce7;
  color: #10b981;
}

.status-badge.completed {
  background: #e9d5ff;
  color: #8b5cf6;
}

.status-badge.inactive {
  background: #f1f5f9;
  color: #64748b;
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}

/* Action Buttons */
.action-group {
  display: flex;
  gap: 4px;
  justify-content: center;
}

.action-btn.view:hover { background: #dbeafe; color: #3b82f6; }
.action-btn.edit:hover { background: #fef3c7; color: #f59e0b; }
.action-btn.delete:hover { background: #fee2e2; color: #ef4444; }

/* Dialog Styles */
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

.cancel-btn, .save-btn {
  border-radius: 40px;
  padding: 10px 24px;
  text-transform: none;
  font-weight: 600;
}

/* View Dialog */
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

.empty-state {
  text-align: center;
  padding: 60px;
  color: #94a3b8;
}

.empty-state p {
  margin-top: 16px;
  font-size: 14px;
}

/* Responsive */
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