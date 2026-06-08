<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <h1 class="font-headline-lg text-headline-lg">Quản lý giáo viên</h1>
        <p class="text-body-md text-on-surface-variant mt-1">
          Quản lý danh sách giáo viên của hệ thống
        </p>
      </div>
      <div class="flex gap-3">
        <Link to="/teachers/trash" variant="primary">
          <span class="material-symbols-outlined">delete_sweep</span>
          Thùng rác
        </Link>
        <ImportExportButtons 
          :store="teacherStore" 
          :export-params="currentParams" 
        />
        <Button variant="primary" @click="openCreateForm">
          <span class="material-symbols-outlined">add</span>
          Thêm giáo viên
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
    <TeacherFilters @filter-change="handleFilterChange" />

    <!-- Loading -->
    <LoadingSpinner v-if="teacherStore.loading" />

    <!-- Teacher Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <TeacherCard
        v-for="teacher in teacherStore.pagedData.data"
        :key="teacher.id"
        :teacher="teacher"
        @edit="openEditModal"
        @delete="confirmDelete"
      />
    </div>

    <!-- Empty State -->
    <EmptyState
      v-if="!teacherStore.loading && teacherStore.pagedData.data.length === 0"
      message="Không tìm thấy giáo viên nào"
      action-text="Đặt lại bộ lọc"
      @action="resetAndRefresh"
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

    <!-- Create/Edit Modal -->
    <Modal
      v-model="showModal"
      :title="isEditing ? 'Sửa giáo viên' : 'Thêm giáo viên'"
    >
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <Input
          v-model="formData.fullName"
          label="Họ tên"
          required
          :error="validationErrors?.fullName?.join(', ')"
        />
        
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="formData.email"
            type="email"
            label="Email"
            :error="validationErrors?.email?.join(', ')"
          />
          <Input
            v-model="formData.phone"
            label="Số điện thoại"
            :error="validationErrors?.phone?.join(', ')"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <Select
            v-model="formData.gender"
            label="Giới tính"
            :options="genderOptions"
          />
          <Input
            v-model="formData.yoB"
            type="number"
            label="Năm sinh"
            placeholder="VD: 1990"
          />
        </div>

        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="teacherStore.loading">
            {{ isEditing ? 'Cập nhật' : 'Tạo mới' }}
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa giáo viên '${selectedTeacher?.fullName}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteTeacher"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useTeacherStore } from '@/stores';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Modal from '@/components/ui/Modal.vue';
import Link from '@/components/ui/Link.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import Pagination from '@/components/ui/Pagination.vue';
import ErrorAlert from '@/components/ui/ErrorAlert.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import TeacherCard from '@/components/business/TeacherCard.vue';
import TeacherFilters from '@/components/business/TeacherFilters.vue';
import ImportExportButtons from '@/components/business/ImportExportButtons.vue';

const teacherStore = useTeacherStore();

// Modal state
const showModal = ref(false);
const isEditing = ref(false);
const editingId = ref(null);
const showDeleteConfirm = ref(false);
const selectedTeacher = ref(null);

// Form data
const formData = reactive({
  fullName: '',
  email: '',
  phone: '',
  gender: null,
  yoB: null,
});

// Options
const genderOptions = [
  { value: null, label: '-- Chọn giới tính --' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];

// Current query params
const currentParams = ref({
  page: 1,
  pageSize: 12,
});

const validationErrors = computed(() => teacherStore.validationErrors);

// Methods
const loadTeachers = async () => {
  await teacherStore.fetchPaged(currentParams.value);
};

const handleFilterChange = (params) => {
  currentParams.value = { pageSize: 12, ...params, page: 1 };
  loadTeachers();
};

const handlePageChange = (page) => {
  currentParams.value.page = page;
  loadTeachers();
};

const resetAndRefresh = () => {
  currentParams.value = { page: 1, pageSize: 12 };
  loadTeachers();
};

const openCreateForm = () => {
  isEditing.value = false;
  editingId.value = null;
  resetForm();
  showModal.value = true;
};

const openEditModal = async (id) => {
  isEditing.value = true;
  editingId.value = id;
  try {
    const teacher = await teacherStore.fetchById(id);
    formData.fullName = teacher.fullName;
    formData.email = teacher.email || '';
    formData.phone = teacher.phone || '';
    formData.gender = teacher.gender;
    formData.yoB = teacher.yoB || null;
    showModal.value = true;
  } catch (err) {
    console.error('Failed to load teacher:', err);
  }
};

const resetForm = () => {
  formData.fullName = '';
  formData.email = '';
  formData.phone = '';
  formData.gender = null;
  formData.yoB = null;
};

const closeModal = () => {
  showModal.value = false;
  resetForm();
};

const handleSubmit = async () => {
  const submitData = {
    fullName: formData.fullName,
    email: formData.email || null,
    phone: formData.phone || null,
    gender: formData.gender,
    yoB: formData.yoB ? String(formData.yoB) : null,
  };
  
  try {
    if (isEditing.value) {
      await teacherStore.update(editingId.value, submitData);
    } else {
      await teacherStore.create(submitData);
    }
    closeModal();
    await loadTeachers();
  } catch (err) {
    console.error('Submit failed:', err);
  }
};

const confirmDelete = (teacher) => {
  selectedTeacher.value = teacher;
  showDeleteConfirm.value = true;
};

const handleDeleteTeacher = async () => {
  if (selectedTeacher.value) {
    await teacherStore.delete(selectedTeacher.value.id);
    showDeleteConfirm.value = false;
    await loadTeachers();
  }
};

onMounted(() => {
  loadTeachers();
});
</script>