<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Thùng rác</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Các lớp học đã bị xóa tạm thời, có thể khôi phục hoặc xóa vĩnh viễn
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/classes" variant="outline" class="px-4 py-2 rounded-lg border">
          <span class="material-symbols-outlined">arrow_back</span>
          Quay lại
        </Link>
        <Button 
          variant="error" 
          @click="confirmEmptyTrash" 
          :disabled="classStore.pagedData.data.length === 0"
        >
          <span class="material-symbols-outlined">delete_forever</span>
          Xóa tất cả
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
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <!-- Tìm kiếm -->
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Tên lớp, mã lớp..."
          icon="search"
          @update:model-value="onFilterChange"
        />
        <!-- Lọc theo khóa học -->
        <Select
          v-model="filters.courseId"
          label="Khóa học"
          :options="courseOptions"
          @update:model-value="onFilterChange"
        />
        <!-- Lọc theo trạng thái (của lớp khi chưa xóa) -->
        <Select
          v-model="filters.status"
          label="Trạng thái cũ"
          :options="classStatusOptions"
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
      :data="classStore.pagedData.data"
    >
      <!-- Cột Tên lớp -->
      <template #className="{ row }">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-primary-container/20 flex items-center justify-center text-primary">
            <span class="material-symbols-outlined text-[18px]">class</span>
          </div>
          <span class="font-title-md">{{ row.className }}</span>
        </div>
      </template>

      <!-- Cột Khóa học -->
      <template #courseId="{ row }">
        {{ getCourseName(row.courseId) }}
      </template>

      <!-- Cột Giáo viên (lấy từ assignment) -->
      <template #teacher="{ row }">
        <div class="flex items-center gap-2">
          <div class="w-6 h-6 rounded-full bg-secondary-container flex items-center justify-center text-on-secondary-container font-bold text-[10px]">
            {{ getTeacherInitials(row.id) }}
          </div>
          <span>{{ getTeacherName(row.id) }}</span>
        </div>
      </template>

      <!-- Cột Sĩ số -->
      <template #maxStudent="{ row }">
        <span>{{ row.maxStudent }}</span>
      </template>

      <!-- Cột Lịch học -->
      <template #schedule="{ row }">
        <div class="flex flex-col">
          <span class="text-body-md">{{ formatDateRange(row.startDate, row.endDate) }}</span>
          <span v-if="row.lesson" class="text-label-md text-outline">{{ row.lesson }} buổi</span>
        </div>
      </template>

      <!-- Cột Hành động -->
      <template #actions="{ row }">
        <div class="flex items-center justify-center gap-2">
          <button
            class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-success transition-all"
            title="Khôi phục"
            @click="confirmRestore(row)"
          >
            <span class="material-symbols-outlined text-[20px]">restore_from_trash</span>
          </button>
          <button
            class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error transition-all"
            title="Xóa vĩnh viễn"
            @click="confirmDeletePermanent(row)"
          >
            <span class="material-symbols-outlined text-[20px]">delete_forever</span>
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Empty State -->
    <EmptyState
      v-if="!classStore.loading && classStore.pagedData.data.length === 0"
      message="Thùng rác trống"
      action-text="Quay lại danh sách lớp học"
      @action="goBack"
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

    <!-- Confirm Dialogs -->
    <ConfirmDialog
      v-model="showRestoreConfirm"
      title="Xác nhận khôi phục"
      :message="`Bạn có chắc chắn muốn khôi phục lớp '${selectedClass?.className}' không?`"
      confirm-text="Khôi phục"
      @confirm="handleRestoreClass"
      type="secondary"
    />

    <ConfirmDialog
      v-model="showDeletePermanentConfirm"
      title="Xác nhận xóa vĩnh viễn"
      :message="`Bạn có chắc chắn muốn xóa vĩnh viễn lớp '${selectedClass?.className}' không? Hành động này không thể hoàn tác.`"
      confirm-text="Xóa vĩnh viễn"
      @confirm="handleDeletePermanentClass"
    />

    <ConfirmDialog
      v-model="showEmptyTrashConfirm"
      title="Xác nhận xóa tất cả"
      message="Bạn có chắc chắn muốn xóa vĩnh viễn TẤT CẢ lớp học trong thùng rác? Hành động này không thể hoàn tác."
      confirm-text="Xóa tất cả"
      @confirm="handleEmptyTrash"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useClassStore, useCourseStore, useTeacherStore, useTeacherAssignmentStore } from '@/stores'
import { CLASS_STATUS_OPTIONS } from '@/constants'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Link from '@/components/ui/Link.vue'
import DataTable from '@/components/ui/DataTable.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import Pagination from '@/components/ui/Pagination.vue'
import ErrorAlert from '@/components/ui/ErrorAlert.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'

const router = useRouter()
const classStore = useClassStore()
const courseStore = useCourseStore()
const teacherStore = useTeacherStore()
const teacherAssignmentStore = useTeacherAssignmentStore()

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
  status: ''
})

const currentParams = ref({
  page: 1,
  pageSize: 10,
  isDeleted: true   // Chỉ lấy các lớp đã xóa mềm
})

// Options
const classStatusOptions = [
  { value: '', label: 'Tất cả trạng thái' },
  ...CLASS_STATUS_OPTIONS
]

const courseOptions = computed(() => [
  { value: '', label: 'Tất cả khóa học' },
  ...courseStore.courses.map(c => ({ value: c.id, label: c.courseName }))
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
const formatDateRange = (startDate, endDate) => {
  if (!startDate && !endDate) return 'Chưa có lịch'
  const start = startDate ? new Date(startDate).toLocaleDateString('vi-VN') : ''
  const end = endDate ? new Date(endDate).toLocaleDateString('vi-VN') : ''
  if (start && end) return `${start} → ${end}`
  return start || end || 'Chưa có lịch'
}

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
    if (!map[a.classId]) map[a.classId] = a.teacherId
  })
  assignmentMap.value = map
}

const loadClasses = async () => {
  const params = {
    page: currentParams.value.page,
    pageSize: currentParams.value.pageSize,
    search: filters.search || undefined,
    courseId: filters.courseId || undefined,
    status: filters.status || undefined,
    isDeleted: true
  }
  await classStore.fetchPaged(params)
  await loadAssignments()
}

// Filters debounce
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
  currentParams.value.page = 1
  loadClasses()
}
const handlePageChange = (page) => {
  currentParams.value.page = page
  loadClasses()
}

const hasActiveFilters = computed(() => {
  return filters.search || filters.courseId || filters.status
})
const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.search) count++
  if (filters.courseId) count++
  if (filters.status) count++
  return count
})

// Navigation
const goBack = () => {
  router.push('/classes')
}

// Confirm dialogs state
const showRestoreConfirm = ref(false)
const showDeletePermanentConfirm = ref(false)
const showEmptyTrashConfirm = ref(false)
const selectedClass = ref(null)

// Restore
const confirmRestore = (classItem) => {
  selectedClass.value = classItem
  showRestoreConfirm.value = true
}
const handleRestoreClass = async () => {
  if (selectedClass.value) {
    await classStore.restore(selectedClass.value.id)
    showRestoreConfirm.value = false
    await loadClasses()
  }
}

// Delete permanent
const confirmDeletePermanent = (classItem) => {
  selectedClass.value = classItem
  showDeletePermanentConfirm.value = true
}
const handleDeletePermanentClass = async () => {
  if (selectedClass.value) {
    await classStore.delete(selectedClass.value.id, true) // permanent = true
    showDeletePermanentConfirm.value = false
    await loadClasses()
  }
}

// Empty trash
const confirmEmptyTrash = () => {
  if (classStore.pagedData.data.length === 0) return
  showEmptyTrashConfirm.value = true
}
const handleEmptyTrash = async () => {
  const classes = [...classStore.pagedData.data]
  for (const classItem of classes) {
    await classStore.delete(classItem.id, true)
  }
  showEmptyTrashConfirm.value = false
  await loadClasses()
}

onMounted(async () => {
  await Promise.all([loadCourses(), loadTeachers()])
  await loadClasses()
})
</script>