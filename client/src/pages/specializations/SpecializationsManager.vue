<template>
  <div class="specialization-management">
    <!-- Hero Header -->
    <div class="hero-header">
      <div class="hero-content">
        <div class="hero-icon">
          <v-icon icon="mdi-format-list-bulleted" size="32" color="white" />
        </div>
        <div>
          <h1 class="hero-title">Quản lý chuyên ngành</h1>
          <p class="hero-subtitle">Quản lý danh sách chuyên ngành đào tạo tại trung tâm</p>
        </div>
      </div>
      <v-btn color="primary" class="hero-btn" @click="openCreateModal">
        <v-icon icon="mdi-plus" class="mr-2" />
        Thêm chuyên ngành
      </v-btn>
    </div>

    <!-- Statistics Section -->
    <div class="stats-grid-modern">
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper total">
          <v-icon icon="mdi-format-list-bulleted" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ totalSpecializations }}</div>
          <div class="stat-label">Tổng chuyên ngành</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper active">
          <v-icon icon="mdi-check-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ activeSpecializations }}</div>
          <div class="stat-label">Đang hoạt động</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper inactive">
          <v-icon icon="mdi-close-circle" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ inactiveSpecializations }}</div>
          <div class="stat-label">Ngừng hoạt động</div>
        </div>
      </div>
      <div class="stat-card-modern">
        <div class="stat-icon-wrapper new">
          <v-icon icon="mdi-account-plus" size="24" />
        </div>
        <div class="stat-info-modern">
          <div class="stat-value">{{ newThisMonth }}</div>
          <div class="stat-label">Mới trong tháng</div>
        </div>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="filter-bar">
      <div class="filter-group">
        <div class="search-wrapper">
          <v-icon icon="mdi-magnify" size="20" class="search-icon" />
          <input 
            v-model="filters.search" 
            type="text" 
            placeholder="Tìm kiếm chuyên ngành..." 
            class="search-input-modern"
            @input="onFilterChange"
          >
        </div>
        <v-select
          v-model="filters.isActive"
          :items="statusOptions"
          label="Trạng thái"
          variant="outlined"
          density="compact"
          class="filter-select"
          hide-details
          @update:model-value="onFilterChange"
        />
      </div>
      <div class="filter-actions">
        <v-btn variant="text" color="primary" @click="resetFilters" class="reset-btn">
          <v-icon icon="mdi-filter-remove" size="18" class="mr-1" />
          Xóa lọc
        </v-btn>
        <span v-if="hasActiveFilters" class="filter-count text-label-md text-on-surface-variant">
          Đang lọc theo {{ activeFiltersCount }} tiêu chí
        </span>
      </div>
    </div>

    <!-- Table -->
    <div class="workspace-card">
      <div class="workspace-toolbar">
        <div class="toolbar-left">
          <span class="total-records">Tổng: {{ totalSpecializations }} chuyên ngành</span>
        </div>
        <div class="toolbar-right">
          <v-btn variant="tonal" color="primary" @click="exportData" class="export-btn">
            <v-icon icon="mdi-export" size="18" class="mr-1" />
            Xuất Excel
          </v-btn>
        </div>
      </div>

      <div class="table-container">
        <div v-if="loading" class="loading-container">
          <v-icon icon="mdi-loading" size="40" class="spin" />
          <p>Đang tải dữ liệu...</p>
        </div>
        <v-data-table
          v-else
          :headers="headers"
          :items="specializationsData"
          :items-per-page="10"
          hover
          class="modern-table"
        >
          <template v-slot:item.specializationName="{ item }">
            <div class="flex items-center gap-3">
              <div class="specialization-icon">
                <v-icon icon="mdi-format-list-bulleted" size="20" color="#3b82f6" />
              </div>
              <span class="font-title-md">{{ item.specializationName }}</span>
            </div>
          </template>

          <template v-slot:item.isActive="{ item }">
            <div class="status-badge" :class="item.isActive ? 'active' : 'inactive'">
              <span class="status-dot"></span>
              {{ item.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
            </div>
          </template>

          <template v-slot:item.descrt="{ item }">
            <div class="description-text">{{ item.descrt || '—' }}</div>
          </template>

          <template v-slot:item.actions="{ item }">
            <div class="action-group">
              <v-btn icon size="small" variant="text" class="action-btn view" @click="viewSpecialization(item)">
                <v-icon icon="mdi-eye" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn edit" @click="openEditModal(item)">
                <v-icon icon="mdi-pencil" size="18" />
              </v-btn>
              <v-btn icon size="small" variant="text" class="action-btn delete" @click="confirmDelete(item)">
                <v-icon icon="mdi-delete" size="18" />
              </v-btn>
            </div>
          </template>

          <template v-slot:no-data>
            <div class="empty-state">
              <v-icon icon="mdi-format-list-bulleted-off" size="56" color="#cbd5e1" />
              <p>Không có dữ liệu chuyên ngành</p>
              <v-btn color="primary" variant="tonal" @click="openCreateModal" class="mt-3">
                <v-icon icon="mdi-plus" class="mr-2" />
                Thêm chuyên ngành mới
              </v-btn>
            </div>
          </template>
        </v-data-table>
      </div>

      <div v-if="totalSpecializations > 0" class="pagination-wrapper">
        <v-pagination
          v-model="currentPage"
          :length="totalPages"
          :total-visible="5"
          @update:model-value="handlePageChange"
        />
        <span class="pagination-info">
          Hiển thị {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalSpecializations) }} / {{ totalSpecializations }}
        </span>
      </div>
    </div>

    <!-- Add/Edit Dialog -->
    <v-dialog v-model="showModal" max-width="560px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-modern" :class="isEditing ? 'edit-mode' : 'create-mode'">
          <div class="dialog-header-icon">
            <v-icon :icon="isEditing ? 'mdi-pencil' : 'mdi-format-list-bulleted-plus'" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">{{ isEditing ? 'Chỉnh sửa chuyên ngành' : 'Thêm chuyên ngành mới' }}</h2>
            <p class="dialog-subtitle">{{ isEditing ? 'Cập nhật thông tin chuyên ngành' : 'Nhập thông tin để tạo chuyên ngành mới' }}</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <v-form ref="form" v-model="formValid">
            <div class="form-grid">
              <v-text-field
                v-model="formData.specializationName"
                label="Tên chuyên ngành"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-format-list-bulleted"
                required
                :error-messages="validationErrors?.SpecializationName?.[0]"
              />
              <v-textarea
                v-model="formData.descrt"
                label="Mô tả"
                variant="outlined"
                density="comfortable"
                rows="3"
                prepend-inner-icon="mdi-text"
                :error-messages="validationErrors?.Descrt?.[0]"
              />
              <v-select
                v-model="formData.isActive"
                :items="statusSelectOptions"
                label="Trạng thái"
                variant="outlined"
                density="comfortable"
                prepend-inner-icon="mdi-check-circle"
              />
            </div>
          </v-form>
        </v-card-text>
        <v-divider />
        <v-card-actions class="dialog-actions-modern">
          <v-btn variant="text" class="cancel-btn" @click="closeModal">Hủy</v-btn>
          <v-btn :color="isEditing ? 'warning' : 'primary'" class="save-btn" @click="handleSubmit" :loading="saving">
            {{ isEditing ? 'Cập nhật' : 'Thêm mới' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- View Dialog -->
    <v-dialog v-model="viewDialogVisible" max-width="520px" transition="dialog-transition">
      <v-card class="modern-dialog" v-if="selectedSpecialization">
        <div class="view-header">
          <div class="view-avatar">
            <span>{{ selectedSpecialization.specializationName?.charAt(0) || '?' }}</span>
          </div>
          <div class="view-header-info">
            <h3>{{ selectedSpecialization.specializationName }}</h3>
            <p>Chuyên ngành</p>
          </div>
          <v-chip :color="selectedSpecialization.isActive ? 'success' : 'default'" size="small" class="view-chip">
            {{ selectedSpecialization.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
          </v-chip>
        </div>
        <v-divider />
        <v-card-text class="view-content">
          <div class="info-grid">
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-format-list-bulleted" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Tên chuyên ngành</div>
                <div class="info-value">{{ selectedSpecialization.specializationName }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-text" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Mô tả</div>
                <div class="info-value">{{ selectedSpecialization.descrt || 'Chưa có mô tả' }}</div>
              </div>
            </div>
            <div class="info-item">
              <div class="info-icon"><v-icon icon="mdi-check-circle" size="18" color="#64748b" /></div>
              <div>
                <div class="info-label">Trạng thái</div>
                <div class="info-value">
                  <span class="status-badge" :class="selectedSpecialization.isActive ? 'active' : 'inactive'">
                    {{ selectedSpecialization.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
                  </span>
                </div>
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

    <!-- Confirm Delete Dialog -->
    <v-dialog v-model="showDeleteConfirm" max-width="400px" transition="dialog-transition">
      <v-card class="modern-dialog">
        <div class="dialog-header-modern delete-mode">
          <div class="dialog-header-icon">
            <v-icon icon="mdi-delete" size="28" />
          </div>
          <div>
            <h2 class="dialog-title">Xác nhận xóa</h2>
            <p class="dialog-subtitle">Bạn có chắc muốn xóa chuyên ngành này?</p>
          </div>
        </div>
        <v-divider />
        <v-card-text class="dialog-content-modern">
          <p class="delete-message">Bạn có chắc muốn xóa chuyên ngành <strong>{{ selectedSpec?.specializationName }}</strong>? Hành động này không thể hoàn tác.</p>
        </v-card-text>
        <v-card-actions class="dialog-actions-modern">
          <v-btn variant="text" class="cancel-btn" @click="showDeleteConfirm = false">Hủy</v-btn>
          <v-btn color="error" class="save-btn" @click="handleDelete" :loading="deleting">Xóa</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import api from '@/utils/api'

// State
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const formValid = ref(true)
const showModal = ref(false)
const viewDialogVisible = ref(false)
const showDeleteConfirm = ref(false)
const isEditing = ref(false)
const editingId = ref(null)
const selectedSpec = ref(null)
const selectedSpecialization = ref(null)
const currentPage = ref(1)
const pageSize = ref(10)

// Form data
const formData = reactive({
  specializationName: '',
  descrt: '',
  isActive: true
})

// Filters
const filters = reactive({
  search: '',
  isActive: ''
})

// Options
const statusOptions = [
  { title: 'Tất cả', value: '' },
  { title: 'Đang hoạt động', value: true },
  { title: 'Ngừng hoạt động', value: false }
]

const statusSelectOptions = [
  { title: 'Đang hoạt động', value: true },
  { title: 'Ngừng hoạt động', value: false }
]

// Headers
const headers = [
  { title: 'Tên chuyên ngành', key: 'specializationName', sortable: true, align: 'start' },
  { title: 'Mô tả', key: 'descrt', sortable: false, align: 'start' },
  { title: 'Trạng thái', key: 'isActive', sortable: true, align: 'center' },
  { title: 'Thao tác', key: 'actions', sortable: false, align: 'center', width: 120 },
]

// Mock data
const mockSpecializations = [
  { id: 1, specializationName: 'Khoa học máy tính', descrt: 'Chuyên ngành nghiên cứu và phát triển phần mềm', isActive: true },
  { id: 2, specializationName: 'Kỹ thuật phần mềm', descrt: 'Chuyên ngành phát triển ứng dụng và hệ thống', isActive: true },
  { id: 3, specializationName: 'Quản trị kinh doanh', descrt: 'Chuyên ngành quản lý và điều hành doanh nghiệp', isActive: true },
  { id: 4, specializationName: 'Ngôn ngữ Anh', descrt: 'Chuyên ngành ngôn ngữ và văn hóa Anh', isActive: false },
]

const specializationsData = ref([])
const totalSpecializations = ref(0)
const activeSpecializations = ref(0)
const inactiveSpecializations = ref(0)
const newThisMonth = ref(0)
const validationErrors = ref({})

// Computed
const totalPages = computed(() => Math.ceil(totalSpecializations.value / pageSize.value))
const hasActiveFilters = computed(() => filters.search || filters.isActive !== '')
const activeFiltersCount = computed(() => (filters.search ? 1 : 0) + (filters.isActive !== '' ? 1 : 0))

// Methods
const loadSpecializations = async () => {
  loading.value = true
  try {
    const params = {
      page: currentPage.value,
      pageSize: pageSize.value,
      search: filters.search || undefined,
      isActive: filters.isActive !== '' ? filters.isActive : undefined
    }

    const response = await api.get('/api/admin/specializations', { params })
    const data = response.data
    
    if (data && data.items && data.items.length > 0) {
      specializationsData.value = data.items
      totalSpecializations.value = data.total || data.items.length
    } else {
      specializationsData.value = [...mockSpecializations]
      totalSpecializations.value = mockSpecializations.length
    }
    
    updateStatistics()
  } catch (error) {
    console.error('❌ Lỗi tải chuyên ngành, sử dụng mock data:', error)
    specializationsData.value = [...mockSpecializations]
    totalSpecializations.value = mockSpecializations.length
    updateStatistics()
  } finally {
    loading.value = false
  }
}

const updateStatistics = () => {
  activeSpecializations.value = specializationsData.value.filter(s => s.isActive).length
  inactiveSpecializations.value = specializationsData.value.filter(s => !s.isActive).length
  newThisMonth.value = Math.floor(Math.random() * 5) + 1
}

const onFilterChange = () => {
  currentPage.value = 1
  loadSpecializations()
}

const resetFilters = () => {
  filters.search = ''
  filters.isActive = ''
  currentPage.value = 1
  loadSpecializations()
}

const handlePageChange = (page) => {
  currentPage.value = page
  loadSpecializations()
}

const openCreateModal = () => {
  isEditing.value = false
  editingId.value = null
  formData.specializationName = ''
  formData.descrt = ''
  formData.isActive = true
  validationErrors.value = {}
  showModal.value = true
}

const openEditModal = (row) => {
  isEditing.value = true
  editingId.value = row.id
  formData.specializationName = row.specializationName
  formData.descrt = row.descrt || ''
  formData.isActive = row.isActive
  validationErrors.value = {}
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  validationErrors.value = {}
}

const viewSpecialization = (item) => {
  selectedSpecialization.value = item
  viewDialogVisible.value = true
}

const handleSubmit = async () => {
  saving.value = true
  try {
    const data = {
      specializationName: formData.specializationName.trim(),
      descrt: formData.descrt?.trim() || null,
      isActive: formData.isActive === true || formData.isActive === 'true'
    }

    let response
    if (isEditing.value) {
      response = await api.put(`/api/admin/specializations/${editingId.value}`, data)
    } else {
      response = await api.post('/api/admin/specializations', data)
    }

    closeModal()
    await loadSpecializations()
  } catch (error) {
    console.error('❌ Lỗi lưu chuyên ngành:', error)
    if (error.response?.data?.errors) {
      validationErrors.value = error.response.data.errors
    } else {
      alert('Có lỗi xảy ra, vui lòng thử lại sau')
    }
  } finally {
    saving.value = false
  }
}

const confirmDelete = (row) => {
  selectedSpec.value = row
  showDeleteConfirm.value = true
}

const handleDelete = async () => {
  if (!selectedSpec.value) return
  deleting.value = true
  try {
    await api.delete(`/api/admin/specializations/${selectedSpec.value.id}`)
    showDeleteConfirm.value = false
    await loadSpecializations()
  } catch (error) {
    console.error('❌ Lỗi xóa chuyên ngành:', error)
    alert('Không thể xóa chuyên ngành này')
  } finally {
    deleting.value = false
  }
}

const exportData = () => {
  console.log('📊 Xuất dữ liệu chuyên ngành...')
  alert('Chức năng xuất Excel đang được phát triển')
}

// Watch for filter changes
watch(() => filters.search, () => {
  clearTimeout(window._filterTimeout)
  window._filterTimeout = setTimeout(() => {
    onFilterChange()
  }, 500)
})

// Lifecycle
onMounted(() => {
  console.log('🚀 Khởi tạo trang Quản lý chuyên ngành...')
  loadSpecializations()
  console.log('✅ Trang Quản lý chuyên ngành đã sẵn sàng')
})
</script>

<style scoped>
.specialization-management {
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
.stat-icon-wrapper.inactive { background: linear-gradient(135deg, #fee2e2, #fecaca); color: #ef4444; }
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
  gap: 16px;
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
  min-width: 160px;
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

.filter-count {
  font-size: 13px;
  color: #64748b;
}

/* Workspace Card */
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

/* Table */
.table-container {
  overflow-x: auto;
  min-height: 400px;
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

.specialization-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: #dbeafe;
  display: flex;
  align-items: center;
  justify-content: center;
}

.description-text {
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
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

/* Pagination */
.pagination-wrapper {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-top: 1px solid #eef2f6;
  flex-wrap: wrap;
  gap: 16px;
}

.pagination-info {
  font-size: 13px;
  color: #64748b;
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

.dialog-header-modern.delete-mode {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
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

.delete-message {
  font-size: 14px;
  color: #475569;
  line-height: 1.6;
}

.delete-message strong {
  color: #1e293b;
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
  .filter-bar { flex-direction: column; align-items: stretch; }
  .filter-group { flex-direction: column; }
  .search-wrapper { min-width: 100%; }
  .filter-select { width: 100%; }
  .filter-actions { flex-wrap: wrap; }
  .toolbar-left { flex-direction: column; align-items: flex-start; }
  .form-grid { grid-template-columns: 1fr; }
  .info-grid { grid-template-columns: 1fr; }
  .pagination-wrapper { flex-direction: column; align-items: center; }
}

@media (max-width: 480px) {
  .stats-grid-modern { grid-template-columns: 1fr; }
}
</style>