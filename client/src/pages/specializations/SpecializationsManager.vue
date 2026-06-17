<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="font-headline-lg text-headline-lg">Quản lý chuyên ngành</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Quản lý danh sách chuyên ngành đào tạo</p>
      </div>
      <div class="flex gap-3">
        <ImportExportButtons :store="specializationStore" :export-params="currentParams" />
        <Button variant="primary" @click="openCreateModal">
          <span class="material-symbols-outlined">add</span>
          Thêm chuyên ngành
        </Button>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Tên chuyên ngành..."
          icon="search"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.isActive"
          label="Trạng thái"
          :options="statusOptions"
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

    <div v-if="specializationStore.loading" class="overflow-x-auto rounded-xl border border-outline-variant bg-surface-container-lowest">
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
      :data="specializationStore.pagedData.data"
    >
      <template #specializationName="{ row }">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-primary-container/20 flex items-center justify-center text-primary">
            <span class="material-symbols-outlined text-[18px]">category</span>
          </div>
          <span class="font-title-md">{{ row.specializationName }}</span>
        </div>
      </template>

      <template #isActive="{ row }">
        <Badge :variant="row.isActive ? 'success' : 'default'">
          {{ row.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
        </Badge>
      </template>

      <template #descrt="{ row }">
        <div class="max-w-xs truncate">{{ row.descrt || '—' }}</div>
      </template>

      <template #actions="{ row }">
        <div class="flex items-center justify-center gap-2">
          <button class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-primary" @click="openEditModal(row)">
            <span class="material-symbols-outlined text-[20px]">edit</span>
          </button>
          <button class="p-2 w-[38px] h-[38px] hover:bg-surface-container-highest rounded-full text-on-surface-variant hover:text-error" @click="confirmDelete(row)">
            <span class="material-symbols-outlined text-[20px]">delete</span>
          </button>
        </div>
      </template>
    </DataTable>

    <EmptyState
      v-if="!specializationStore.loading && specializationStore.pagedData.data.length === 0"
      message="Không tìm thấy chuyên ngành nào"
      action-text="Đặt lại bộ lọc"
      @action="resetFilters"
    />

    <div v-if="specializationStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="specializationStore.pagedData.totalRecords"
        :per-page="specializationStore.pagedData.pageSize"
        :current-page="specializationStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Modal -->
    <Modal v-model="showModal" :title="isEditing ? 'Sửa chuyên ngành' : 'Thêm chuyên ngành'">
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <Input 
          v-model="formData.specializationName" 
          label="Tên chuyên ngành" 
          required 
          :error="validationErrors?.SpecializationName?.[0]"
        />
        <Input 
          v-model="formData.descrt" 
          label="Mô tả" 
          type="textarea" 
          rows="3" 
          :error="validationErrors?.Descrt?.[0]"
        />
        <Select v-model="formData.isActive" label="Trạng thái" :options="statusOptionsForForm" required />
        <div class="flex justify-end gap-3">
          <Button variant="outline" @click="closeModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="specializationStore.loading">Lưu</Button>
        </div>
      </form>
    </Modal>

    <ConfirmDialog v-model="showDeleteConfirm" title="Xác nhận xóa" :message="`Xóa chuyên ngành '${selectedSpec?.specializationName}'?`" confirm-text="Xóa" @confirm="handleDelete" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useSpecializationStore } from '@/stores'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Modal from '@/components/ui/Modal.vue'
import DataTable from '@/components/ui/DataTable.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import Pagination from '@/components/ui/Pagination.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import ImportExportButtons from '@/components/business/ImportExportButtons.vue'
import Badge from '@/components/ui/Badge.vue'
import SkeletonTableRow from '@/components/skeleton/SkeletonTableRow.vue';

const specializationStore = useSpecializationStore()
const { validationErrors } = storeToRefs(specializationStore)

const columns = [
  { key: 'specializationName', label: 'Tên chuyên ngành', align: 'left' },
  { key: 'descrt', label: 'Mô tả', align: 'left' },
  { key: 'isActive', label: 'Trạng thái', align: 'left' },
  { key: 'actions', label: 'Hành động', align: 'center' }
]

const filters = reactive({ search: '', isActive: '' })
const currentParams = ref({ page: 1, pageSize: 12 })

const statusOptions = [
  { value: '', label: 'Tất cả' },
  { value: true, label: 'Đang hoạt động' },
  { value: false, label: 'Ngừng hoạt động' }
]
const statusOptionsForForm = [
  { value: true, label: 'Đang hoạt động' },
  { value: false, label: 'Ngừng hoạt động' }
]

const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref(null)
const showDeleteConfirm = ref(false)
const selectedSpec = ref(null)

const formData = reactive({ specializationName: '', descrt: '', isActive: true })

const loadData = async () => {
  await specializationStore.fetchPaged({
    page: currentParams.value.page,
    pageSize: currentParams.value.pageSize,
    search: filters.search || undefined,
    isActive: filters.isActive !== '' ? filters.isActive : undefined
  })
}

let debounce = null
const onFilterChange = () => {
  if (debounce) clearTimeout(debounce)
  debounce = setTimeout(() => {
    currentParams.value.page = 1
    loadData()
  }, 500)
}
const resetFilters = () => {
  filters.search = ''
  filters.isActive = ''
  currentParams.value.page = 1
  loadData()
}
const handlePageChange = (page) => {
  currentParams.value.page = page
  loadData()
}
const hasActiveFilters = computed(() => filters.search || filters.isActive !== '')
const activeFiltersCount = computed(() => (filters.search ? 1 : 0) + (filters.isActive !== '' ? 1 : 0))

const openCreateModal = () => {
  isEditing.value = false
  editingId.value = null
  formData.specializationName = ''
  formData.descrt = ''
  formData.isActive = true
  showModal.value = true
}
const openEditModal = (row) => {
  isEditing.value = true
  editingId.value = row.id
  formData.specializationName = row.specializationName
  formData.descrt = row.descrt || ''
  formData.isActive = row.isActive
  showModal.value = true
}
const closeModal = () => { showModal.value = false }

const handleSubmit = async () => {
  const data = {
    specializationName: formData.specializationName.trim(),
    descrt: formData.descrt?.trim() || null,
    isActive: formData.isActive === true || formData.isActive === 'true'
  }
  if (isEditing.value) {
    await specializationStore.update(editingId.value, data)
  } else {
    await specializationStore.create(data)
  }
  closeModal()
  await loadData()
}

const confirmDelete = (row) => {
  selectedSpec.value = row
  showDeleteConfirm.value = true
}
const handleDelete = async () => {
  if (selectedSpec.value) {
    await specializationStore.deletePermanent(selectedSpec.value.id)
    showDeleteConfirm.value = false
    await loadData()
  }
}

onMounted(() => { loadData() })
</script>