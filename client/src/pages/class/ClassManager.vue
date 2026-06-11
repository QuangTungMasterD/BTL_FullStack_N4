<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Quản lý lớp học</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Quản lý danh sách lớp học, lịch trình và giáo viên phụ trách
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/classes/trash" variant="primary">
          <span class="material-symbols-outlined">delete_sweep</span>
          Thùng rác
        </Link>
        <ImportExportButtons 
          :store="classStore" 
          :export-params="currentParams" 
        />
        <Button variant="primary" @click="openCreateModal">
          <span class="material-symbols-outlined">add</span>
          Mở lớp mới
        </Button>
      </div>
    </div>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="classStore.error"
      :error="classStore.error"
      :status-code="classStore.errorStatusCode"
      :validation-errors="classStore.validationErrors"
      :timestamp="classStore.timestamp"
      @close="classStore.clearErrors"
    />

    <!-- Filters -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Tên lớp..."
          icon="search"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.courseId"
          label="Khóa học"
          :options="courseOptions"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.status"
          label="Trạng thái"
          :options="classStatusOptions"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.teacherId"
          label="Giáo viên"
          :options="teacherOptions"
          @update:model-value="onFilterChange"
        />
      </div>
      <div class="flex justify-between items-center mt-4 pt-4 border-t border-outline-variant">
        <Button variant="outline" size="sm" @click="resetFilters">
          <span class="material-symbols-outlined text-sm">refresh</span>
          Đặt lại
        </Button>
        <div v-if="hasActiveFilters" class="text-label-md text-on-surface-variant">
          Đang lọc theo {{ activeFiltersCount }} tiêu chí
        </div>
      </div>
    </div>

    <!-- Loading -->
    <LoadingSpinner v-if="classStore.loading" />

    <!-- Data Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="displayClasses"
    >
      <template #className="{ row }">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-primary-container/20 flex items-center justify-center text-primary">
            <span class="material-symbols-outlined text-[18px]">class</span>
          </div>
          <span class="font-title-md">{{ row.className }}</span>
        </div>
      </template>

      <template #courseId="{ row }">
        {{ getCourseName(row.courseId) }}
      </template>

      <template #teacher="{ row }">
        <div class="flex items-center gap-2">
          <div class="w-6 h-6 rounded-full bg-secondary-container flex items-center justify-center text-on-secondary-container font-bold text-[10px]">
            {{ getTeacherInitials(row.id) }}
          </div>
          <span>{{ getTeacherName(row.id) }}</span>
        </div>
      </template>

      <template #maxStudent="{ row }">
        <div class="flex flex-col gap-1">
          <div class="w-full bg-surface-variant h-1.5 rounded-full overflow-hidden max-w-[80px]">
            <div
              class="bg-secondary h-full"
              :style="{ width: `${getOccupancyPercent(row.id)}%` }"
            ></div>
          </div>
          <span class="text-label-md">{{ getCurrentSlots(row.id) }}/{{ row.maxStudent }}</span>
        </div>
      </template>

      <template #schedule="{ row }">
        <div class="flex flex-col">
          <span class="text-body-md">{{ formatDateRange(row.startDate, row.endDate) }}</span>
          <span v-if="row.lesson" class="text-label-md text-outline">{{ row.lesson }} buổi</span>
        </div>
      </template>

      <template #actions="{ row }">
        <div class="flex items-center justify-center gap-2">
          <Link
            class="p-2 w-[38x] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error transition-all"
            title="Thông tin lớp"
            :to="`/classes/${row.id}`"
          >
            <span class="material-symbols-outlined text-[20px]">info</span>
          </Link>

          <button
            class="p-2 w-[38x] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-primary transition-all"
            title="Sửa lớp"
            @click="openEditModal(row)"
          >
            <span class="material-symbols-outlined text-[20px]">edit</span>
          </button>
          <button
            class="p-2 w-[38x] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error transition-all"
            title="Xóa lớp"
            @click="confirmDelete(row)"
          >
            <span class="material-symbols-outlined text-[20px]">delete</span>
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Empty State -->
    <EmptyState
      v-if="!classStore.loading && classStore.pagedData.data.length === 0"
      message="Không tìm thấy lớp học nào"
      action-text="Đặt lại bộ lọc"
      @action="resetFilters"
    />

    <!-- Pagination -->
    <div v-if="classStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="classStore.pagedData.totalRecords"
        :per-page="classStore.pagedData.pageSize"
        :current-page="classStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Modal Create/Edit -->
    <Modal
      v-model="showModal"
      :title="isEditing ? 'Sửa thông tin lớp' : 'Mở lớp mới'"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <Input
          v-model="formData.className"
          label="Tên lớp"
          required
          :error="validationErrors?.className?.join(', ')"
        />
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="formData.maxStudent"
            type="number"
            label="Sĩ số tối đa"
            required
            :min="1"
            :error="validationErrors?.maxStudent?.join(', ')"
          />
          <Select
            v-model="formData.status"
            label="Trạng thái"
            :options="classStatusOptionsForForm"
            required
            :error="validationErrors?.status?.join(', ')"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="formData.startDate"
            type="date"
            label="Ngày bắt đầu"
            required
            :error="validationErrors?.startDate?.join(', ')"
          />
          <Input
            v-model="formData.endDate"
            type="date"
            label="Ngày kết thúc"
            required
            :error="validationErrors?.endDate?.join(', ')"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="formData.lesson"
            type="number"
            label="số tiết"
            required
            :min="1"
            :error="validationErrors?.lesson?.join(', ')"
          />
          <Select
            v-model="formData.courseId"
            label="Khóa học"
            :options="courseOptionsForForm"
            required
            :error="validationErrors?.courseId?.join(', ')"
          />
        </div>
        <Select
          v-model="formData.teacherId"
          label="Giáo viên phụ trách"
          :options="teacherOptionsForForm"
          :error="validationErrors?.teacherId?.join(', ')"
        />
        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="classStore.loading">
            {{ isEditing ? 'Cập nhật' : 'Tạo lớp' }}
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa lớp '${selectedClass?.className}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteClass"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useClassStore, useCourseStore, useTeacherStore, useTeacherAssignmentStore } from '@/stores'
import { CLASS_STATUS_OPTIONS } from '@/constants'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Modal from '@/components/ui/Modal.vue'
import DataTable from '@/components/ui/DataTable.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import Pagination from '@/components/ui/Pagination.vue'
import ErrorAlert from '@/components/ui/ErrorAlert.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import Link from '@/components/ui/Link.vue'
import ImportExportButtons from '@/components/business/ImportExportButtons.vue'

const classStore = useClassStore()
const courseStore = useCourseStore()
const teacherStore = useTeacherStore()
const teacherAssignmentStore = useTeacherAssignmentStore()

const { validationErrors } = storeToRefs(classStore)

// Table columns
const columns = [
  { key: 'className', label: 'Tên lớp', align: 'left' },
  { key: 'courseId', label: 'Khóa học', align: 'left' },
  { key: 'teacher', label: 'Giáo viên', align: 'left' },
  { key: 'maxStudent', label: 'Sĩ số', align: 'left' },
  { key: 'schedule', label: 'Lịch học', align: 'left' },
  { key: 'actions', label: 'Hành động', align: 'center' }
]

// Filters
const filters = reactive({
  search: '',
  courseId: '',
  status: '',
  teacherId: ''
})

const currentParams = ref({
  page: 1,
  pageSize: 10,
  isDeleted: false
})

// Options
const classStatusOptions = [
  { value: '', label: 'Tất cả trạng thái' },
  ...CLASS_STATUS_OPTIONS
]

const classStatusOptionsForForm = CLASS_STATUS_OPTIONS

const courseOptions = computed(() => [
  { value: '', label: 'Tất cả khóa học' },
  ...courseStore.courses.map(c => ({ value: c.id, label: c.courseName }))
])

const courseOptionsForForm = computed(() => [
  { value: '', label: '-- Chọn khóa học --' },
  ...courseStore.courses.map(c => ({ value: c.id, label: c.courseName }))
])

const teacherOptions = computed(() => [
  { value: '', label: 'Tất cả giáo viên' },
  ...teacherStore.teachers.map(t => ({ value: t.id, label: t.fullName }))
])

const teacherOptionsForForm = computed(() => [
  { value: '', label: '-- Chưa phân công --' },
  ...teacherStore.teachers.map(t => ({ value: t.id, label: t.fullName }))
])

// Helper maps
const courseMap = computed(() => {
  const map = {}
  courseStore.courses.forEach(c => { map[c.id] = c.courseName })
  return map
})

const teacherMap = computed(() => {
  const map = {}
  teacherStore.teachers.forEach(t => { map[t.id] = t.fullName })
  return map
})

const assignmentMap = ref({})
const currentSlotsMap = ref({}) // tạm thời random để minh họa, sau có thể thay bằng API thực

// Helper functions
const getCourseName = (courseId) => courseMap.value[courseId] || 'N/A'
const getTeacherName = (classId) => {
  const teacherId = assignmentMap.value[classId]
  return teacherId ? teacherMap.value[teacherId] || 'Chưa có tên' : 'Chưa phân công'
}
const getTeacherInitials = (classId) => {
  const teacherId = assignmentMap.value[classId]
  if (!teacherId) return '?'
  const name = teacherMap.value[teacherId]
  return name ? name.split(' ').slice(-1)[0].charAt(0).toUpperCase() : '?'
}
const getOccupancyPercent = (classId) => {
  const cls = classStore.pagedData.data.find(c => c.id === classId)
  const max = cls?.maxStudent || 1
  const current = currentSlotsMap.value[classId] || 0
  return Math.min(100, (current / max) * 100)
}
const getCurrentSlots = (classId) => currentSlotsMap.value[classId] || 0
const formatDateRange = (startDate, endDate) => {
  if (!startDate && !endDate) return 'Chưa có lịch'
  const start = startDate ? new Date(startDate).toLocaleDateString('vi-VN') : ''
  const end = endDate ? new Date(endDate).toLocaleDateString('vi-VN') : ''
  if (start && end) return `${start} → ${end}`
  return start || end || 'Chưa có lịch'
}

// Client-side filter by teacherId (since API doesn't support teacherId filter)
const displayClasses = computed(() => {
  if (!filters.teacherId) return classStore.pagedData.data
  return classStore.pagedData.data.filter(c => assignmentMap.value[c.id] === Number(filters.teacherId))
})

// Data loading
const loadCourses = async () => {
  await courseStore.fetchAll()
}
const loadTeachers = async () => {
  await teacherStore.fetchAll()
}
const loadAssignments = async () => {
  await teacherAssignmentStore.fetchAll()
  const map = {}
  teacherAssignmentStore.assignments.forEach(a => {
    // Nếu một class có nhiều teacher, lấy teacher đầu tiên (có thể cải tiến sau)
    if (!map[a.classId]) map[a.classId] = a.teacherId
  })
  assignmentMap.value = map
}
const loadCurrentSlots = () => {
  // Demo: random sĩ số hiện tại (thực tế cần gọi API lấy số học sinh đăng ký)
  classStore.pagedData.data.forEach(c => {
    if (!currentSlotsMap.value[c.id]) {
      currentSlotsMap.value[c.id] = Math.floor(Math.random() * (c.maxStudent + 1))
    }
  })
}
const loadClasses = async () => {
  const params = {
    page: currentParams.value.page,
    pageSize: currentParams.value.pageSize,
    search: filters.search || undefined,
    courseId: filters.courseId || undefined,
    status: filters.status || undefined,
    isDeleted: false
  }
  await classStore.fetchPaged(params)
  await loadAssignments()
  loadCurrentSlots()
}

// Debounced filter
let debounceTimer = null
const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    currentParams.value.page = 1
    loadClasses()
  }, 500)
}
const resetFilters = () => {
  filters.search = ''
  filters.courseId = ''
  filters.status = ''
  filters.teacherId = ''
  currentParams.value.page = 1
  loadClasses()
}
const handlePageChange = (page) => {
  currentParams.value.page = page
  loadClasses()
}

const hasActiveFilters = computed(() => {
  return filters.search || filters.courseId || filters.status || filters.teacherId
})
const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.search) count++
  if (filters.courseId) count++
  if (filters.status) count++
  if (filters.teacherId) count++
  return count
})

// Modal state
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref(null)
const showDeleteConfirm = ref(false)
const selectedClass = ref(null)

const formData = reactive({
  className: '',
  maxStudent: 20,
  status: 1,
  startDate: '',
  endDate: '',
  lesson: 0,
  courseId: '',
  teacherId: ''
})

const openCreateModal = () => {
  isEditing.value = false
  editingId.value = null
  resetForm()
  showModal.value = true
}
const openEditModal = (classItem) => {
  isEditing.value = true
  editingId.value = classItem.id
  formData.className = classItem.className
  formData.maxStudent = classItem.maxStudent
  formData.status = classItem.status
  formData.startDate = classItem.startDate?.split('T')[0] || ''
  formData.endDate = classItem.endDate?.split('T')[0] || ''
  formData.lesson = classItem.lesson
  formData.courseId = classItem.courseId
  formData.teacherId = assignmentMap.value[classItem.id] || ''
  showModal.value = true
}
const resetForm = () => {
  formData.className = ''
  formData.maxStudent = 20
  formData.status = 1
  formData.startDate = ''
  formData.endDate = ''
  formData.lesson = 0
  formData.courseId = ''
  formData.teacherId = ''
}
const closeModal = () => {
  showModal.value = false
  resetForm()
}
const handleSubmit = async () => {
  const submitData = {
    className: formData.className,
    maxStudent: Number(formData.maxStudent),
    status: Number(formData.status),
    startDate: formData.startDate,
    endDate: formData.endDate,
    lesson: Number(formData.lesson),
    courseId: Number(formData.courseId)
  }
  try {
    let savedClass
    if (isEditing.value) {
      savedClass = await classStore.update(editingId.value, submitData)
    } else {
      savedClass = await classStore.create(submitData)
    }
    // Xử lý teacher assignment
    const newTeacherId = formData.teacherId ? Number(formData.teacherId) : null
    const oldTeacherId = assignmentMap.value[savedClass.id]
    if (oldTeacherId !== newTeacherId) {
      if (oldTeacherId) {
        const assignment = teacherAssignmentStore.assignments.find(a => a.classId === savedClass.id && a.teacherId === oldTeacherId)
        if (assignment) await teacherAssignmentStore.delete(assignment.id)
      }
      if (newTeacherId) {
        await teacherAssignmentStore.create({ teacherId: newTeacherId, classId: savedClass.id })
      }
      await loadAssignments()
    }
    closeModal()
    await loadClasses()
  } catch (err) {
    console.error('Submit failed:', err)
  }
}

// Delete
const confirmDelete = (classItem) => {
  selectedClass.value = classItem
  showDeleteConfirm.value = true
}
const handleDeleteClass = async () => {
  if (selectedClass.value) {
    await classStore.delete(selectedClass.value.id)
    showDeleteConfirm.value = false
    await loadClasses()
  }
}

// Watch teacherId filter: reload classes when teacherId changes (client-side filter)
watch(() => filters.teacherId, () => {
  // No need to reload, computed will handle display
})

onMounted(async () => {
  await Promise.all([loadCourses(), loadTeachers()])
  await loadClasses()
})
</script>