<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Thùng rác - Giáo viên</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Các giáo viên đã bị xóa tạm thời, có thể khôi phục hoặc xóa vĩnh viễn
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/teachers" variant="outline">
          <span class="material-symbols-outlined">arrow_back</span>
          Quay lại
        </Link>
        <Button 
          variant="error" 
          @click="confirmEmptyTrash" 
          :disabled="teacherStore.pagedData.data.length === 0"
        >
          <span class="material-symbols-outlined">delete_forever</span>
          Xóa tất cả
        </Button>
      </div>
    </div>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="teacherStore.error"
      :error="teacherStore.error"
      :status-code="teacherStore.errorStatusCode"
      :validation-errors="teacherStore.validationErrors"
      :timestamp="teacherStore.timestamp"
      @close="teacherStore.clearErrors"
    />

    <!-- Filters -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Họ tên, email..."
          icon="search"
          @update:model-value="onFilterChange"
        />
        <Select
          v-model="filters.gender"
          label="Giới tính"
          :options="genderOptions"
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
    <LoadingSpinner v-if="teacherStore.loading" />

    <!-- Teacher Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <TeacherCard
        v-for="teacher in teacherStore.pagedData.data"
        :key="teacher.id"
        :teacher="teacher"
        :is-trash="true"
        @restore="confirmRestore"
        @delete-permanent="confirmDeletePermanent"
      />
    </div>

    <!-- Empty State -->
    <EmptyState
      v-if="!teacherStore.loading && teacherStore.pagedData.data.length === 0"
      message="Thùng rác trống"
      action-text="Quay lại danh sách giáo viên"
      @action="goBack"
    />

    <!-- Pagination -->
    <div v-if="teacherStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="teacherStore.pagedData.totalRecords"
        :per-page="teacherStore.pagedData.pageSize"
        :current-page="teacherStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Confirm Dialogs -->
    <ConfirmDialog
      v-model="showRestoreConfirm"
      title="Xác nhận khôi phục"
      :message="`Bạn có chắc chắn muốn khôi phục giáo viên '${selectedTeacher?.fullName}' không?`"
      confirm-text="Khôi phục"
      @confirm="handleRestoreTeacher"
      type="secondary"
    />

    <ConfirmDialog
      v-model="showDeletePermanentConfirm"
      title="Xác nhận xóa vĩnh viễn"
      :message="`Bạn có chắc chắn muốn xóa vĩnh viễn giáo viên '${selectedTeacher?.fullName}' không? Hành động này không thể hoàn tác.`"
      confirm-text="Xóa vĩnh viễn"
      @confirm="handleDeletePermanentTeacher"
    />

    <ConfirmDialog
      v-model="showEmptyTrashConfirm"
      title="Xác nhận xóa tất cả"
      message="Bạn có chắc chắn muốn xóa vĩnh viễn TẤT CẢ giáo viên trong thùng rác? Hành động này không thể hoàn tác."
      confirm-text="Xóa tất cả"
      @confirm="handleEmptyTrash"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useTeacherStore } from '@/stores';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Link from '@/components/ui/Link.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import Pagination from '@/components/ui/Pagination.vue';
import ErrorAlert from '@/components/ui/ErrorAlert.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import TeacherCard from '@/components/business/TeacherCard.vue';

const router = useRouter();
const teacherStore = useTeacherStore();

// Filter state
const filters = reactive({
  search: '',
  gender: '',
});

const genderOptions = [
  { value: '', label: 'Tất cả' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];

// Modal state
const showRestoreConfirm = ref(false);
const showDeletePermanentConfirm = ref(false);
const showEmptyTrashConfirm = ref(false);
const selectedTeacher = ref(null);

// Current query params
const currentParams = ref({
  page: 1,
  pageSize: 12,
  isDeleted: true,  // Lấy các giáo viên đã xóa
});

const hasActiveFilters = computed(() => {
  return filters.search || filters.gender !== '';
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (filters.search) count++;
  if (filters.gender !== '') count++;
  return count;
});

let debounceTimer = null;

// Methods
const loadTeachers = async () => {
  const params = {
    ...currentParams.value,
    search: filters.search || undefined,
    gender: filters.gender !== '' ? filters.gender : undefined,
  };
  await teacherStore.fetchPaged(params);
};

const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    currentParams.value.page = 1;
    loadTeachers();
  }, 500);
};

const resetFilters = () => {
  filters.search = '';
  filters.gender = '';
  currentParams.value.page = 1;
  loadTeachers();
};

const handlePageChange = (page) => {
  currentParams.value.page = page;
  loadTeachers();
};

const goBack = () => {
  router.push('/teachers');
};

// Restore handlers
const confirmRestore = (teacher) => {
  selectedTeacher.value = teacher;
  showRestoreConfirm.value = true;
};

const handleRestoreTeacher = async () => {
  if (selectedTeacher.value) {
    await teacherStore.restore(selectedTeacher.value.id);
    showRestoreConfirm.value = false;
    await loadTeachers();
  }
};

// Delete permanent handlers
const confirmDeletePermanent = (teacher) => {
  selectedTeacher.value = teacher;
  showDeletePermanentConfirm.value = true;
};

const handleDeletePermanentTeacher = async () => {
  if (selectedTeacher.value) {
    await teacherStore.deletePermanent(selectedTeacher.value.id);
    showDeletePermanentConfirm.value = false;
    await loadTeachers();
  }
};

// Empty trash
const confirmEmptyTrash = () => {
  if (teacherStore.pagedData.data.length === 0) return;
  showEmptyTrashConfirm.value = true;
};

const handleEmptyTrash = async () => {
  const teachers = [...teacherStore.pagedData.data];
  for (const teacher of teachers) {
    await teacherStore.deletePermanent(teacher.id);
  }
  showEmptyTrashConfirm.value = false;
  await loadTeachers();
};

onMounted(() => {
  loadTeachers();
});
</script>