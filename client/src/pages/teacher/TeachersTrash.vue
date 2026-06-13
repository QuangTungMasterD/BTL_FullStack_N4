<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Thùng rác</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Các giáo viên đã bị xóa tạm thời, có thể khôi phục hoặc xóa vĩnh viễn
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/teachers" variant="outline" class="px-4 py-2 rounded-lg border">
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

        <Select
          v-model="filters.specializationId"
          label="Chuyên ngành"
          :options="specializationOptions"
          @update:model-value="onFilterChange"
        />
        <!-- Có thể thêm filter năm sinh -->
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
    <div v-if="teacherStore.loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <SkeletonCard v-for="i in 6" :key="i" />
    </div>

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
import { storeToRefs } from 'pinia';
import { useTeacherStore, useSpecializationStore } from '@/stores';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Link from '@/components/ui/Link.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import Pagination from '@/components/ui/Pagination.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import TeacherCard from '@/components/business/TeacherCard.vue';
import SkeletonCard from '@/components/skeleton/SkeletonCard.vue';

const router = useRouter();
const teacherStore = useTeacherStore();
const specializationStore = useSpecializationStore();
const { pagedData, loading } = storeToRefs(teacherStore);

const filters = reactive({
  search: '',
  gender: '',
  specializationId: '',
});

const genderOptions = [
  { value: '', label: 'Tất cả' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];

const currentParams = ref({
  page: 1,
  pageSize: 12,
  isDeleted: true,
});

const specializationOptions = computed(() => [
  { value: '', label: 'Tất cả chuyên ngành' },
  ...specializationStore.specializations.map(spec => ({
    value: spec.id,
    label: spec.specializationName
  }))
]);

const hasActiveFilters = computed(() => filters.search || filters.gender !== '');
const activeFiltersCount = computed(() => (filters.search ? 1 : 0) + (filters.gender !== '' ? 1 : 0));

let debounceTimer = null;

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
const showRestoreConfirm = ref(false);
const showDeletePermanentConfirm = ref(false);
const showEmptyTrashConfirm = ref(false);
const selectedTeacher = ref(null);

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
  specializationStore.fetchAll();
});
</script>