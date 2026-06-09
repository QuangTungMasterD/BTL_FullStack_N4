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
          Sửa thông tin
        </Button>
        <Button variant="error" @click="confirmDelete" :disabled="teacherStore.loading">
          <span class="material-symbols-outlined">delete</span>
          Xóa giáo viên
        </Button>
      </div>
    </div>

    <!-- Loading -->
    <LoadingSpinner v-if="teacherStore.loading" />

    <div v-else class="space-y-6">
      <!-- Thông tin giáo viên -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex items-center gap-4 mb-6">
          <Avatar :name="teacher?.fullName" size="lg" />
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
            <p class="text-label-md text-on-surface-variant">Năm sinh</p>
            <p class="font-body-md">{{ teacher?.yoB ? formatDate(teacher.yoB) : 'Chưa có' }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Giới tính</p>
            <p class="font-body-md">{{ teacher?.gender === true ? 'Nam' : teacher?.gender === false ? 'Nữ' : 'Khác' }}</p>
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
          <div>
            <p class="text-label-md text-on-surface-variant">Cập nhật lần cuối</p>
            <p class="font-body-md">{{ formatDateTime(teacher?.updatedAt) }}</p>
          </div>
          <div class="md:col-span-2">
            <p class="text-label-md text-on-surface-variant">Chuyên ngành</p>
            <div class="flex flex-wrap gap-2 mt-1">
              <Badge v-for="spec in teacherSpecializations" :key="spec.id" variant="info">
                {{ spec.specializationName }}
              </Badge>
              <span v-if="teacherSpecializations.length === 0" class="text-on-surface-variant">Chưa có chuyên ngành</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Danh sách lớp được phân công (nếu có) -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <h2 class="font-headline-md text-headline-md mb-4">Lớp học giảng dạy</h2>
        
        <LoadingSpinner v-if="loadingClasses" />
        
        <div v-else-if="classes.length === 0" class="text-center py-8 text-on-surface-variant">
          Giáo viên chưa được phân công lớp nào
        </div>
        
        <div v-else class="space-y-3">
          <div 
            v-for="classItem in classes" 
            :key="classItem.id"
            class="p-4 rounded-lg border border-outline-variant hover:shadow-sm transition cursor-pointer"
            @click="goToClassDetail(classItem.id)"
          >
            <div class="flex justify-between items-start">
              <div>
                <h4 class="font-title-md">{{ classItem.className }}</h4>
                <p class="text-label-sm text-on-surface-variant mt-1">
                  Mã lớp: {{ classItem.id }}
                </p>
              </div>
              <Badge :variant="getClassStatusVariant(classItem.status)">
                {{ classStatusText(classItem.status) }}
              </Badge>
            </div>
            <div class="grid grid-cols-2 md:grid-cols-3 gap-3 mt-3 text-label-sm text-on-surface-variant">
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">school</span>
                Khóa học: {{ classItem.courseName || 'N/A' }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">group</span>
                Sĩ số: {{ classItem.currentSlots || 0 }}/{{ classItem.maxSlots }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">meeting_room</span>
                Phòng: {{ classItem.roomName || 'Chưa có' }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Edit (tương tự như trong TeachersManager, có thể tái sử dụng component) -->
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
          <Input
            v-model="editForm.yoB"
            type="date"
            label="Năm sinh"
            :error="validationErrors?.yoB?.join(', ')"
          />
          <Select
            v-model="editForm.gender"
            label="Giới tính"
            :options="genderOptions"
            :error="validationErrors?.gender?.join(', ')"
          />
        </div>
        <Select
          v-model="editForm.isActive"
          label="Trạng thái"
          :options="statusOptions"
        />

        <SpecializationSelector
          v-model="editForm.specializationIds"
          :options="specializationOptions"
          label="Chuyên ngành"
          :error="validationErrors?.specializationIds?.join(', ')"
        />

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
import { useRouter, useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useTeacherStore, useSpecializationStore, useClassStore } from '@/stores';
import Link from '@/components/ui/Link.vue';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Modal from '@/components/ui/Modal.vue';
import Badge from '@/components/ui/Badge.vue';
import Avatar from '@/components/ui/Avatar.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import { formatDate, formatDateTime, classStatusText } from '@/composables/useFormat';
import SpecializationSelector from '@/components/business/SpecializationSelector.vue';

const router = useRouter();
const route = useRoute();
const teacherStore = useTeacherStore();
const specializationStore = useSpecializationStore();
const classStore = useClassStore();
const { validationErrors } = storeToRefs(teacherStore);

const teacher = ref(null);
// SỬA: dùng computed thay vì ref để reactive
const teacherSpecializations = computed(() => {
  if (!teacher.value?.specialization) return [];
  return teacher.value.specialization
    .map(id => specializationStore.specializations.find(s => s.id === id))
    .filter(Boolean);
});

const classes = ref([]);
const loadingClasses = ref(false);
const showEditModal = ref(false);
const showDeleteConfirm = ref(false);

// Edit form
const editForm = reactive({
  fullName: '',
  email: '',
  phone: '',
  yoB: '',
  gender: null,
  isActive: true,
  specializationIds: [],
});

const genderOptions = [
  { value: null, label: '-- Chọn giới tính --' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];
const statusOptions = [
  { value: true, label: 'Đang hoạt động' },
  { value: false, label: 'Ngừng hoạt động' },
];
const specializationOptions = computed(() =>
  specializationStore.specializations.map(spec => ({
    value: spec.id,
    label: spec.specializationName
  }))
);

// Bỏ hàm loadTeacherSpecializations vì đã dùng computed

const loadClasses = async () => {
  const teacherId = route.params.id;
  if (!teacherId) return;
  loadingClasses.value = true;
  try {
    // Giả sử có API lấy lớp theo teacherId
    // classes.value = await classStore.fetchByTeacherId(teacherId);
    classes.value = []; // tạm thời
  } catch (err) {
    console.error('Failed to load classes:', err);
  } finally {
    loadingClasses.value = false;
  }
};

const loadTeacherDetail = async () => {
  const id = route.params.id;
  if (!id) {
    router.push('/teachers');
    return;
  }
  try {
    teacher.value = await teacherStore.fetchById(id);
    // Không cần gán teacherSpecializations thủ công nữa
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
  editForm.yoB = teacher.value.yoB ? teacher.value.yoB.split('T')[0] : '';
  editForm.gender = teacher.value.gender;
  editForm.isActive = teacher.value.isActive;
  editForm.specializationIds = teacher.value.specialization ? [...teacher.value.specialization] : [];
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
};

const handleUpdate = async () => {
  const submitData = {
    fullName: editForm.fullName,
    email: editForm.email,
    phone: editForm.phone,
    yoB: editForm.yoB || null,
    gender: editForm.gender,
    isActive: editForm.isActive === true || editForm.isActive === 'true',
    specializationIds: editForm.specializationIds,
  };
  try {
    await teacherStore.update(teacher.value.id, submitData);
    closeEditModal();
    await loadTeacherDetail();
    await loadClasses();
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

// Class status helper
const getClassStatusVariant = (status) => {
  const map = { 1: 'info', 2: 'success', 3: 'default', 4: 'error', 5: 'warning' };
  return map[status] || 'default';
};

const goToClassDetail = (classId) => {
  router.push(`/classes/${classId}`);
};

onMounted(async () => {
  await specializationStore.fetchAll(); // Đảm bảo chuyên ngành được tải trước
  await loadTeacherDetail();
  await loadClasses();
});
</script>