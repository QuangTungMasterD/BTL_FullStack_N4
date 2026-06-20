<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Thùng rác</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Các khóa học đã bị xóa tạm thời, có thể khôi phục hoặc xóa vĩnh viễn
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/admin-courses" variant="outline" class="px-4 py-2 rounded-lg border">
          <span class="material-symbols-outlined">arrow_back</span>
          Quay lại
        </Link>
        <Button 
          variant="error" 
          @click="confirmEmptyTrash" 
          :disabled="courseStore.pagedData.data.length === 0"
        >
          <span class="material-symbols-outlined">delete_forever</span>
          Xóa tất cả
        </Button>
      </div>
    </div>

    <!-- Filters (ẩn filter trạng thái vì đã xóa hết rồi) -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <!-- Tìm kiếm -->
        <Input
          v-model="filters.search"
          label="Tìm kiếm"
          placeholder="Tên khóa học..."
          icon="search"
          @update:model-value="onFilterChange"
        />

        <!-- Trình độ -->
        <Select
          v-model="filters.level"
          label="Trình độ"
          :options="levelOptions"
          @update:model-value="onFilterChange"
        />

        <!-- Chuyên môn -->
        <Select
          v-model="filters.specializationId"
          label="Chuyên môn"
          :options="specializationOptions"
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
    <div v-if="courseStore.loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <SkeletonCard v-for="i in 6" :key="i" />
    </div>

    <!-- Course Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <CourseCard
        v-for="course in courseStore.pagedData.data"
        :key="course.id"
        :course="course"
        :is-trash="true"
        @restore="confirmRestore"
        @delete-permanent="confirmDeletePermanent"
      />
    </div>

    <!-- Empty State -->
    <EmptyState
      v-if="!courseStore.loading && courseStore.pagedData.data.length === 0"
      message="Thùng rác trống"
      action-text="Quay lại danh sách khóa học"
      @action="goBack"
    />

    <!-- Pagination -->
    <div v-if="courseStore.pagedData.totalRecords > 0" class="mt-6">
      <Pagination
        :total="courseStore.pagedData.totalRecords"
        :per-page="courseStore.pagedData.pageSize"
        :current-page="courseStore.pagedData.page"
        @update:current-page="handlePageChange"
      />
    </div>

    <!-- Confirm Dialogs -->
    <ConfirmDialog
      v-model="showRestoreConfirm"
      title="Xác nhận khôi phục"
      :message="`Bạn có chắc chắn muốn khôi phục khóa học '${selectedCourse?.courseName}' không?`"
      confirm-text="Khôi phục"
      @confirm="handleRestoreCourse"
      type="secondary"
    />

    <ConfirmDialog
      v-model="showDeletePermanentConfirm"
      title="Xác nhận xóa vĩnh viễn"
      :message="`Bạn có chắc chắn muốn xóa vĩnh viễn khóa học '${selectedCourse?.courseName}' không? Hành động này không thể hoàn tác.`"
      confirm-text="Xóa vĩnh viễn"
      @confirm="handleDeletePermanentCourse"
    />

    <ConfirmDialog
      v-model="showEmptyTrashConfirm"
      title="Xác nhận xóa tất cả"
      message="Bạn có chắc chắn muốn xóa vĩnh viễn TẤT CẢ khóa học trong thùng rác? Hành động này không thể hoàn tác."
      confirm-text="Xóa tất cả"
      @confirm="handleEmptyTrash"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useCourseStore, useSpecializationStore } from '@/stores';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Link from '@/components/ui/Link.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import Pagination from '@/components/ui/Pagination.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import CourseCard from '@/components/business/CourseCard.vue';
import SkeletonCard from '@/components/skeleton/SkeletonCard.vue';

const router = useRouter();
const courseStore = useCourseStore();
const specializationStore = useSpecializationStore();
const { pagedData, loading, validationErrors } = storeToRefs(courseStore);

// Filter state
const filters = reactive({
  search: '',
  level: '',
  specializationId: '',
});

// Options
const levelOptions = [
  { value: '', label: 'Tất cả trình độ' },
  { value: 1, label: 'Sơ cấp' },
  { value: 2, label: 'Căn bản' },
  { value: 3, label: 'Trung cấp' },
  { value: 4, label: 'Cao cấp' },
  { value: 5, label: 'Chuyên gia' },
];

const specializationOptions = computed(() => [
  { value: '', label: 'Tất cả chuyên môn' },
  ...specializationStore.specializations.map(item => ({
    value: item.id,
    label: item.specializationName
  }))
]);

// Modal state
const showRestoreConfirm = ref(false);
const showDeletePermanentConfirm = ref(false);
const showEmptyTrashConfirm = ref(false);
const selectedCourse = ref(null);

// Current query params
const currentParams = ref({
  page: 1,
  pageSize: 12,
  isDeleted: true,  // ← Lấy các khóa đã xóa
});

// Filter computed
const hasActiveFilters = computed(() => {
  return filters.search || 
         filters.level !== '' || 
         filters.specializationId !== '';
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (filters.search) count++;
  if (filters.level !== '') count++;
  if (filters.specializationId !== '') count++;
  return count;
});

let debounceTimer = null;

// Methods
const loadCourses = async () => {
  const params = {
    ...currentParams.value,
    search: filters.search || undefined,
    level: filters.level !== '' ? Number(filters.level) : undefined,
    specializationId: filters.specializationId !== '' ? Number(filters.specializationId) : undefined,
  };
  await courseStore.fetchPaged(params);
};

const onFilterChange = async () => {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(async () => {
    currentParams.value.page = 1;
    await loadCourses();
  }, 500);
};

const resetFilters = async () => {
  filters.search = '';
  filters.level = '';
  filters.specializationId = '';
  currentParams.value.page = 1;
  await loadCourses();
};

const handlePageChange = async (page) => {
  currentParams.value.page = page;
  await loadCourses();
};

const goBack = () => {
  router.push('/admin-courses');
};

// Restore handlers
const confirmRestore = (course) => {
  selectedCourse.value = course;
  showRestoreConfirm.value = true;
};

const handleRestoreCourse = async () => {
  if (selectedCourse.value) {
    await courseStore.restore(selectedCourse.value.id);
    showRestoreConfirm.value = false;
    await loadCourses();
  }
};

// Delete permanent handlers
const confirmDeletePermanent = (course) => {
  selectedCourse.value = course;
  showDeletePermanentConfirm.value = true;
};

const handleDeletePermanentCourse = async () => {
  if (selectedCourse.value) {
    await courseStore.delete(selectedCourse.value.id, true);
    showDeletePermanentConfirm.value = false;
    await loadCourses();
  }
};

// Empty trash
const confirmEmptyTrash = () => {
  if (courseStore.pagedData.data.length === 0) return;
  showEmptyTrashConfirm.value = true;
};

const handleEmptyTrash = async () => {
  const courses = [...courseStore.pagedData.data];
  for (const course of courses) {
    await courseStore.delete(course.id, true);
  }
  showEmptyTrashConfirm.value = false;
  await loadCourses();
};

onMounted(() => {
  specializationStore.fetchAll();
  loadCourses();
});
</script>