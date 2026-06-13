<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Thùng rác - Phòng học</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Các phòng đã bị xóa tạm thời, có thể khôi phục hoặc xóa vĩnh viễn
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/rooms" variant="outline" class="px-4 py-2 rounded-lg border">
          <span class="material-symbols-outlined">arrow_back</span>
          Quay lại
        </Link>
        <Button variant="error" @click="confirmEmptyTrash" :disabled="roomStore.pagedData.data.length === 0">
          <span class="material-symbols-outlined">delete_forever</span>
          Xóa tất cả
        </Button>
      </div>
    </div>

    <!-- Filters (đơn giản hơn) -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
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

    <DataTable
      v-else
      :columns="columns"
      :data="roomStore.pagedData.data"
    >
      <template #roomName="{ row }">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-primary-container/20 flex items-center justify-center text-primary">
            <span class="material-symbols-outlined text-[18px]">meeting_room</span>
          </div>
          <span class="font-title-md">{{ row.roomName }}</span>
        </div>
      </template>

      <template #roomType="{ row }">
        <Badge :variant="getRoomTypeVariant(row.roomType)">{{ roomTypeText(row.roomType) }}</Badge>
      </template>

      <template #status="{ row }">
        <Badge :variant="getRoomStatusVariant(row.status)">{{ roomStatusText(row.status) }}</Badge>
      </template>

      <template #actions="{ row }">
        <div class="flex items-center justify-center gap-2">
          <button
            class="p-2 h-[38px] w-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-success transition-all"
            title="Khôi phục"
            @click="confirmRestore(row)"
          >
            <span class="material-symbols-outlined text-[20px]">restore_from_trash</span>
          </button>
          <button
            class="p-2 h-[38px] w-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error transition-all"
            title="Xóa vĩnh viễn"
            @click="confirmDeletePermanent(row)"
          >
            <span class="material-symbols-outlined text-[20px]">delete_forever</span>
          </button>
        </div>
      </template>
    </DataTable>

    <EmptyState
      v-if="!roomStore.loading && roomStore.pagedData.data.length === 0"
      message="Thùng rác trống"
      action-text="Quay lại danh sách phòng"
      @action="goBack"
    />

    <div v-if="roomStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="roomStore.pagedData.totalRecords"
        :per-page="roomStore.pagedData.pageSize"
        :current-page="roomStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Confirm Dialogs -->
    <ConfirmDialog
      v-model="showRestoreConfirm"
      title="Xác nhận khôi phục"
      :message="`Khôi phục phòng '${selectedRoom?.roomName}'?`"
      confirm-text="Khôi phục"
      @confirm="handleRestoreRoom"
      type="secondary"
    />

    <ConfirmDialog
      v-model="showDeletePermanentConfirm"
      title="Xác nhận xóa vĩnh viễn"
      :message="`Xóa vĩnh viễn phòng '${selectedRoom?.roomName}'? Hành động này không thể hoàn tác.`"
      confirm-text="Xóa vĩnh viễn"
      @confirm="handleDeletePermanentRoom"
    />

    <ConfirmDialog
      v-model="showEmptyTrashConfirm"
      title="Xác nhận xóa tất cả"
      message="Bạn có chắc chắn muốn xóa vĩnh viễn TẤT CẢ phòng trong thùng rác?"
      confirm-text="Xóa tất cả"
      @confirm="handleEmptyTrash"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useRoomStore } from '@/stores'
import { ROOM_TYPE_OPTIONS } from '@/constants'
import { roomTypeText, roomStatusText } from '@/composables/useFormat'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Link from '@/components/ui/Link.vue'
import DataTable from '@/components/ui/DataTable.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import Pagination from '@/components/ui/Pagination.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import Badge from '@/components/ui/Badge.vue'
import SkeletonTableRow from '@/components/skeleton/SkeletonTableRow.vue';

const router = useRouter()
const roomStore = useRoomStore()

const columns = [
  { key: 'roomName', label: 'Tên phòng', align: 'left' },
  { key: 'roomType', label: 'Loại phòng', align: 'left' },
  { key: 'status', label: 'Trạng thái', align: 'left' },
  { key: 'actions', label: 'Hành động', align: 'center' }
]

const filters = reactive({
  search: '',
  roomType: '',
})

const currentParams = ref({
  page: 1,
  pageSize: 12,
  isDeleted: true,
})

const roomTypeOptions = [
  { value: '', label: 'Tất cả loại phòng' },
  ...ROOM_TYPE_OPTIONS
]

const getRoomTypeVariant = (type) => {
  const map = { 1: 'info', 2: 'success', 3: 'warning', 4: 'default', 5: 'secondary' }
  return map[type] || 'default'
}
const getRoomStatusVariant = (status) => {
  const map = { 1: 'success', 2: 'info', 3: 'warning', 4: 'error' }
  return map[status] || 'default'
}

const loadRooms = async () => {
  const params = {
    page: currentParams.value.page,
    pageSize: currentParams.value.pageSize,
    search: filters.search || undefined,
    roomType: filters.roomType || undefined,
    isDeleted: true,
  }
  await roomStore.fetchPaged(params)
}

let debounceTimer = null
const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    currentParams.value.page = 1
    loadRooms()
  }, 500)
}
const resetFilters = () => {
  filters.search = ''
  filters.roomType = ''
  currentParams.value.page = 1
  loadRooms()
}
const handlePageChange = (page) => {
  currentParams.value.page = page
  loadRooms()
}
const hasActiveFilters = computed(() => filters.search || filters.roomType !== '')
const activeFiltersCount = computed(() => (filters.search ? 1 : 0) + (filters.roomType !== '' ? 1 : 0))
const goBack = () => router.push('/rooms')

// Confirm dialogs
const showRestoreConfirm = ref(false)
const showDeletePermanentConfirm = ref(false)
const showEmptyTrashConfirm = ref(false)
const selectedRoom = ref(null)

const confirmRestore = (room) => {
  selectedRoom.value = room
  showRestoreConfirm.value = true
}
const handleRestoreRoom = async () => {
  if (selectedRoom.value) {
    await roomStore.restore(selectedRoom.value.id)
    showRestoreConfirm.value = false
    await loadRooms()
  }
}

const confirmDeletePermanent = (room) => {
  selectedRoom.value = room
  showDeletePermanentConfirm.value = true
}
const handleDeletePermanentRoom = async () => {
  if (selectedRoom.value) {
    await roomStore.deletePermanent(selectedRoom.value.id)
    showDeletePermanentConfirm.value = false
    await loadRooms()
  }
}

const confirmEmptyTrash = () => {
  if (roomStore.pagedData.data.length === 0) return
  showEmptyTrashConfirm.value = true
}
const handleEmptyTrash = async () => {
  const rooms = [...roomStore.pagedData.data]
  for (const room of rooms) {
    await roomStore.deletePermanent(room.id)
  }
  showEmptyTrashConfirm.value = false
  await loadRooms()
}

onMounted(() => {
  loadRooms()
})
</script>