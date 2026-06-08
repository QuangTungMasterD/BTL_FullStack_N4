<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <div class="flex items-center gap-3">
          <Link to="/teachers" variant="muted" icon-left="arrow_back">
            Quay lại
          </Link>
          <h1 class="font-headline-lg text-headline-lg">Chi tiết giáo viên</h1>
        </div>
      </div>
      <div class="flex gap-3">
        <Button variant="outline" @click="openEditModal">
          <span class="material-symbols-outlined">edit</span>
          Sửa
        </Button>
        <Button variant="error" @click="confirmDelete" :disabled="teacherStore.loading">
          <span class="material-symbols-outlined">delete</span>
          Xóa
        </Button>
      </div>
    </div>

    <!-- Loading -->
    <LoadingSpinner v-if="teacherStore.loading" />

    <div v-else class="space-y-6">
      <!-- Thông tin giáo viên -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex items-center gap-4 mb-6">
          <Avatar :name="teacher?.fullName" size="lg" bordered />
          <div>
            <h2 class="font-headline-md text-headline-md">{{ teacher?.fullName }}</h2>
            <p class="text-label-md text-on-surface-variant">ID: {{ teacher?.id }}</p>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p class="text-label-md text-on-surface-variant">Email</p>
            <p class="font-body-md">{{ teacher?.email || 'Chưa có' }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Số điện thoại</p>
            <p class="font-body-md">{{ teacher?.phone || 'Chưa có' }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Giới tính</p>
            <Badge :variant="teacher?.gender === true ? 'info' : teacher?.gender === false ? 'info' : 'default'">
              {{ teacher?.gender === true ? 'Nam' : teacher?.gender === false ? 'Nữ' : 'Chưa có' }}
            </Badge>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Năm sinh</p>
            <p class="font-body-md">{{ teacher?.yoB || 'Chưa có' }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Trạng thái</p>
            <Badge :variant="teacher?.isActive ? 'success' : 'default'">
              {{ teacher?.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
            </Badge>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Ngày tạo</p>
            <p class="font-body-md">{{ formatDateTime(teacher?.createdAt) }}</p>
          </div>
          <div class="md:col-span-2">
            <p class="text-label-md text-on-surface-variant">Ngày cập nhật</p>
            <p class="font-body-md">{{ formatDateTime(teacher?.updatedAt) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Edit -->
    <Modal v-model="showEditModal" title="Sửa giáo viên">
      <form @submit.prevent="handleUpdate" class="space-y-4">
        <Input
          v-model="editForm.fullName"
          label="Họ tên"
          required
          :error="validationErrors?.fullName?.join(', ')"
        />
        
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.email"
            type="email"
            label="Email"
            :error="validationErrors?.email?.join(', ')"
          />
          <Input
            v-model="editForm.phone"
            label="Số điện thoại"
            :error="validationErrors?.phone?.join(', ')"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <Select
            v-model="editForm.gender"
            label="Giới tính"
            :options="genderOptions"
          />
          <Input
            v-model="editForm.yoB"
            type="number"
            label="Năm sinh"
            placeholder="VD: 1990"
          />
        </div>

        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeEditModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="teacherStore.loading">
            Cập nhật
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa giáo viên '${teacher?.fullName}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteTeacher"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useTeacherStore } from '@/stores';
import Link from '@/components/ui/Link.vue';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Modal from '@/components/ui/Modal.vue';
import Badge from '@/components/ui/Badge.vue';
import Avatar from '@/components/ui/Avatar.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import { formatDateTime } from '@/composables/useFormat';

const route = useRoute();
const router = useRouter();
const teacherStore = useTeacherStore();

// State
const teacher = ref(null);
const showEditModal = ref(false);
const showDeleteConfirm = ref(false);

// Edit form
const editForm = reactive({
  fullName: '',
  email: '',
  phone: '',
  gender: null,
  yoB: null,
});

const genderOptions = [
  { value: null, label: '-- Chọn giới tính --' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];

const validationErrors = computed(() => teacherStore.validationErrors);

// Load data
const loadTeacherDetail = async () => {
  const id = route.params.id;
  if (!id) {
    router.push('/teachers');
    return;
  }
  
  try {
    teacher.value = await teacherStore.fetchById(id);
  } catch (err) {
    console.error('Failed to load teacher:', err);
    router.push('/teachers');
  }
};

// Edit modal
const openEditModal = () => {
  editForm.fullName = teacher.value.fullName;
  editForm.email = teacher.value.email || '';
  editForm.phone = teacher.value.phone || '';
  editForm.gender = teacher.value.gender;
  editForm.yoB = teacher.value.yoB || null;
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
};

// Update teacher
const handleUpdate = async () => {
  const submitData = {
    fullName: editForm.fullName,
    email: editForm.email || null,
    phone: editForm.phone || null,
    gender: editForm.gender,
    yoB: editForm.yoB ? String(editForm.yoB) : null,
  };
  
  try {
    await teacherStore.update(teacher.value.id, submitData);
    closeEditModal();
    await loadTeacherDetail();
  } catch (err) {
    console.error('Update failed:', err);
  }
};

// Delete
const confirmDelete = () => {
  showDeleteConfirm.value = true;
};

const handleDeleteTeacher = async () => {
  await teacherStore.delete(teacher.value.id);
  showDeleteConfirm.value = false;
  router.push('/teachers');
};

onMounted(() => {
  loadTeacherDetail();
});
</script>