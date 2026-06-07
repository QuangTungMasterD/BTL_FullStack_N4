<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Quản lý khóa học</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Quản lý danh sách khóa học và chương trình đào tạo của hệ thống
        </p>
      </div>
      <Button variant="primary" @click="openCreateForm">
        <span class="material-symbols-outlined">add</span>
        Thêm khóa học
      </Button>
    </div>

    <!-- Error Alert -->
    <ErrorAlert
      v-if="courseStore.error"
      :error="courseStore.error"
      :status-code="courseStore.errorStatusCode"
      :validation-errors="courseStore.validationErrors"
      :timestamp="courseStore.timestamp"
      @close="courseStore.clearErrors"
    />

    <!-- Filters -->
    <CourseFilters @filter-change="handleFilterChange" />

    <!-- Loading -->
    <LoadingSpinner v-if="courseStore.loading" />

    <!-- Course Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <CourseCard
        v-for="course in courseStore.pagedData.data"
        :key="course.id"
        :course="course"
        @edit="openEditModal"
        @delete="confirmDelete"
      />
    </div>

    <!-- Empty State -->
    <EmptyState
      v-if="!courseStore.loading && courseStore.pagedData.data.length === 0"
      message="Không tìm thấy khóa học nào"
      action-text="Đặt lại bộ lọc"
      @action="resetAndRefresh"
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

    <!-- Create/Edit Modal -->
    <Modal
      v-model="showModal"
      :title="isEditing ? 'Sửa khóa học' : 'Thêm khóa học'"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <Input
          v-model="formData.courseName"
          label="Tên khóa học"
          required
          :error="validationErrors?.courseName?.join(', ')"
        />
        
        <Input
          v-model="formData.desct"
          label="Mô tả"
          type="textarea"
          :error="validationErrors?.desct?.join(', ')"
        />
        
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="formData.tuitionFee"
            type="number"
            label="Học phí"
            required
            :error="validationErrors?.tuitionFee?.join(', ')"
          />
          
          <Input
            v-model="formData.lesson"
            type="number"
            label="Số buổi"
            required
            :error="validationErrors?.lesson?.join(', ')"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <Select
            v-model="formData.level"
            label="Trình độ"
            :options="levelOptions"
            required
            :error="validationErrors?.level?.join(', ')"
          />
          
          <Select
            v-model="formData.isActive"
            label="Trạng thái"
            :options="statusOptions"
            required
          />
        </div>

        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="courseStore.loading">
            {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
          </Button>
        </div>
      </form>
    </Modal>

    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa khóa học '${selectedCourse?.courseName}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteCourse"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useCourseStore } from '@/stores';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Modal from '@/components/ui/Modal.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import Pagination from '@/components/ui/Pagination.vue';
import ErrorAlert from '@/components/ui/ErrorAlert.vue';
import CourseCard from '@/components/business/CourseCard.vue';
import CourseFilters from '@/components/business/CourseFilters.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';

const courseStore = useCourseStore();
const { pagedData, loading, validationErrors } = storeToRefs(courseStore);

// Modal state
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref(null);
const showDeleteConfirm = ref(false);
const selectedCourse = ref(null);

// Form data
const formData = reactive({
  courseName: '',
  desct: '',
  tuitionFee: null,
  lesson: null,
  level: null,
  isActive: true,
});

// Options
const levelOptions = [
  { value: 1, label: 'Sơ cấp' },
  { value: 2, label: 'Căn bản' },
  { value: 3, label: 'Trung cấp' },
  { value: 4, label: 'Cao cấp' },
  { value: 5, label: 'Chuyên gia' },
];

const statusOptions = [
  { value: true, label: 'Đang mở' },
  { value: false, label: 'Ngừng mở' },
];

// Current query params
const currentParams = ref({
  page: 1,
  pageSize: 12,
});

// Methods
const loadCourses = async () => {
  await courseStore.fetchPaged(currentParams.value);
};

const handleFilterChange = (params) => {
  currentParams.value = { pageSize: 12, ...params, page: 1 };
  
  loadCourses();
};

// Delete handlers
const confirmDelete = (course) => {
  selectedCourse.value = course;
  showDeleteConfirm.value = true;
};

const handleDeleteCourse = async () => {
  if (selectedCourse.value) {
    await courseStore.delete(selectedCourse.value.id);
    showDeleteConfirm.value = false;
    await loadCourses();
  }
};

// Open edit modal
const openEditModal = (course) => {
  selectedCourse.value = course;
  showModal.value = true;
};

const handlePageChange = (page) => {
  currentParams.value.page = page;
  loadCourses();
};

const resetAndRefresh = () => {
  currentParams.value = { page: 1, pageSize: 12 };
  loadCourses();
};

const openCreateForm = () => {
  isEditing.value = false;
  editingId.value = null;
  resetForm();
  showModal.value = true;
};

const openEditForm = async (id) => {
  isEditing.value = true;
  editingId.value = id;
  try {
    const course = await courseStore.fetchById(id);
    formData.courseName = course.courseName;
    formData.desct = course.desct;
    formData.tuitionFee = course.tuitionFee;
    formData.lesson = course.lesson;
    formData.level = course.level;
    formData.isActive = course.isActive;
    showModal.value = true;
  } catch (err) {
    console.error('Failed to load course:', err);
  }
};

const resetForm = () => {
  formData.courseName = '';
  formData.desct = '';
  formData.tuitionFee = null;
  formData.lesson = null;
  formData.level = null;
  formData.isActive = true;
};

const closeModal = () => {
  showModal.value = false;
  resetForm();
};

const handleSubmit = async () => {
  try {
    if (isEditing.value) {
      await courseStore.update(editingId.value, { ...formData });
    } else {
      await courseStore.create({ ...formData });
    }
    closeModal();
    await loadCourses();
  } catch (err) {
    // Error already handled in store
    console.error('Submit failed:', err);
  }
};

onMounted(() => {
  loadCourses();
});
</script>