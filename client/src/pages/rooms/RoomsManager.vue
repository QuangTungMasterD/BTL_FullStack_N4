<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Quản lý phòng học</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Quản lý danh sách phòng học, loại phòng và trạng thái sử dụng
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/rooms/trash" variant="primary">
          <span class="material-symbols-outlined">delete_sweep</span>
          Thùng rác
        </Link>
        <ImportExportButtons :store="roomStore" :export-params="currentParams" />
        <Button variant="primary" @click="openCreateModal">
          <span class="material-symbols-outlined">add</span>
          Thêm phòng
        </Button>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Tên phòng..."
          icon="search"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.roomType"
          label="Loại phòng"
          :options="roomTypeOptions"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.status"
          label="Trạng thái"
          :options="roomStatusOptions"
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
    <div v-if="roomStore.loading" class="overflow-x-auto rounded-xl border border-outline-variant bg-surface-container-lowest">
      <table class="w-full min-w-[640px] border-collapse">
        <thead class="bg-surface-container-low">
          <tr class="border-b border-outline-variant">
            <th v-for="col in columns" :key="col.key" class="px-6 py-4 text-left font-label-md text-on-surface-variant">
              {{ col.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <SkeletonTableRow v-for="i in 5" :key="i" :columns="columns.length" />
        </tbody>
      </table>
    </div>

    <!-- Data Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="roomStore.pagedData.data"
    >
      <!-- Cột Tên phòng -->
      <template #roomName="{ row }">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-primary-container/20 flex items-center justify-center text-primary">
            <span class="material-symbols-outlined text-[18px]">meeting_room</span>
          </div>
          <span class="font-title-md">{{ row.roomName }}</span>
        </div>
      </template>

      <!-- Cột Loại phòng -->
      <template #roomType="{ row }">
        <Badge :variant="getRoomTypeVariant(row.roomType)">
          {{ roomTypeText(row.roomType) }}
        </Badge>
      </template>

      <!-- Cột Trạng thái -->
      <template #status="{ row }">
        <Badge :variant="getRoomStatusVariant(row.status)">
          {{ roomStatusText(row.status) }}
        </Badge>
      </template>

      <!-- Cột Mô tả -->
      <template #descrt="{ row }">
        <div class="max-w-xs truncate">{{ row.descrt || '—' }}</div>
      </template>

      <!-- Cột Hành động -->
      <template #actions="{ row }">
        <div class="flex items-center justify-center gap-2">
          <button
            class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-primary transition-all"
            title="Sửa"
            @click="openEditModal(row)"
          >
            <span class="material-symbols-outlined text-[20px]">edit</span>
          </button>
          <button
            class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error transition-all"
            title="Xóa"
            @click="confirmDelete(row)"
          >
            <span class="material-symbols-outlined text-[20px]">delete</span>
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Empty State -->
    <EmptyState
      v-if="!roomStore.loading && roomStore.pagedData.data.length === 0"
      message="Không tìm thấy phòng nào"
      action-text="Đặt lại bộ lọc"
      @action="resetFilters"
    />

    <!-- Pagination -->
    <div v-if="roomStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="roomStore.pagedData.totalRecords"
        :per-page="roomStore.pagedData.pageSize"
        :current-page="roomStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Modal Create/Edit -->
    <Modal
      v-model="showModal"
      :title="isEditing ? 'Sửa phòng' : 'Thêm phòng mới'"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <Input
          v-model="formData.roomName"
          label="Tên phòng"
          required
          :error="validationErrors?.roomName?.join(', ')"
        />
        <Select
          v-model="formData.roomType"
          label="Loại phòng"
          :options="roomTypeOptionsForForm"
          required
          :error="validationErrors?.roomType?.join(', ')"
        />
        <Select
          v-model="formData.status"
          label="Trạng thái"
          :options="roomStatusOptionsForForm"
          required
          :error="validationErrors?.status?.join(', ')"
        />
        <Input
          v-model="formData.descrt"
          label="Mô tả"
          type="textarea"
          rows="3"
          :error="validationErrors?.descrt?.join(', ')"
        />
        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="roomStore.loading">
            {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa phòng '${selectedRoom?.roomName}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteRoom"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRoomStore } from '@/stores'
import { ROOM_TYPE_OPTIONS, ROOM_STATUS_OPTIONS } from '@/constants'
import { roomTypeText, roomStatusText } from '@/composables/useFormat'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Modal from '@/components/ui/Modal.vue'
import DataTable from '@/components/ui/DataTable.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import Pagination from '@/components/ui/Pagination.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import Link from '@/components/ui/Link.vue'
import ImportExportButtons from '@/components/business/ImportExportButtons.vue'
import Badge from '@/components/ui/Badge.vue'
import SkeletonTableRow from '@/components/skeleton/SkeletonTableRow.vue';

const roomStore = useRoomStore()
const { validationErrors } = storeToRefs(roomStore)

// Table columns
const columns = [
  { key: 'roomName', label: 'Tên phòng', align: 'center' },
  { key: 'roomType', label: 'Loại phòng', align: 'center' },
  { key: 'status', label: 'Trạng thái', align: 'center' },
  { key: 'descrt', label: 'Mô tả', align: 'center' },
  { key: 'actions', label: 'Hành động', align: 'center' }
]

// Filters
const filters = reactive({
  search: '',
  roomType: '',
  status: '',
})

const currentParams = ref({
  page: 1,
  pageSize: 12,
})

// Options for filters and forms
const roomTypeOptions = [
  { value: '', label: 'Tất cả loại phòng' },
  ...ROOM_TYPE_OPTIONS
]
const roomStatusOptions = [
  { value: '', label: 'Tất cả trạng thái' },
  ...ROOM_STATUS_OPTIONS
]
const roomTypeOptionsForForm = ROOM_TYPE_OPTIONS
const roomStatusOptionsForForm = ROOM_STATUS_OPTIONS

// Helper functions for badge variants
const getRoomTypeVariant = (type) => {
  const map = { 1: 'info', 2: 'success', 3: 'warning', 4: 'default', 5: 'secondary' }
  return map[type] || 'default'
}
const getRoomStatusVariant = (status) => {
  const map = { 1: 'success', 2: 'info', 3: 'warning', 4: 'error' }
  return map[status] || 'default'
}

// Modal state
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref(null)
const showDeleteConfirm = ref(false)
const selectedRoom = ref(null)

const formData = reactive({
  roomName: '',
  roomType: 1,
  status: 1,
  descrt: '',
})

// Methods
const loadRooms = async () => {
  const params = {
    page: currentParams.value.page,
    pageSize: currentParams.value.pageSize,
    search: filters.search || undefined,
    roomType: filters.roomType || undefined,
    status: filters.status || undefined,
    isDeleted: false,
  }
  await roomStore.fetchPaged(params)
}

const handleFilterChange = () => {
  currentParams.value.page = 1
  loadRooms()
}

let debounceTimer = null
const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    handleFilterChange()
  }, 500)
}

const resetFilters = () => {
  filters.search = ''
  filters.roomType = ''
  filters.status = ''
  handleFilterChange()
}

const handlePageChange = (page) => {
  currentParams.value.page = page
  loadRooms()
}

const hasActiveFilters = computed(() => {
  return filters.search || filters.roomType !== '' || filters.status !== ''
})
const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.search) count++
  if (filters.roomType !== '') count++
  if (filters.status !== '') count++
  return count
})

const openCreateModal = () => {
  isEditing.value = false
  editingId.value = null
  resetForm()
  showModal.value = true
}

const openEditModal = async (room) => {
  isEditing.value = true
  editingId.value = room.id
  formData.roomName = room.roomName
  formData.roomType = room.roomType
  formData.status = room.status
  formData.descrt = room.descrt || ''
  showModal.value = true
}

const resetForm = () => {
  formData.roomName = ''
  formData.roomType = 1
  formData.status = 1
  formData.descrt = ''
}

const closeModal = () => {
  showModal.value = false
  resetForm()
}

const handleSubmit = async () => {
  const submitData = {
    roomName: formData.roomName,
    roomType: Number(formData.roomType),
    status: Number(formData.status),
    descrt: formData.descrt || null,
  }
  try {
    if (isEditing.value) {
      await roomStore.update(editingId.value, submitData)
    } else {
      await roomStore.create(submitData)
    }
    closeModal()
    await loadRooms()
  } catch (err) {
    console.error('Submit failed:', err)
  }
}

const confirmDelete = (room) => {
  selectedRoom.value = room
  showDeleteConfirm.value = true
}

const handleDeleteRoom = async () => {
  if (selectedRoom.value) {
    await roomStore.delete(selectedRoom.value.id)
    showDeleteConfirm.value = false
    await loadRooms()
  }
}

onMounted(() => {
  loadRooms()
})
</script>