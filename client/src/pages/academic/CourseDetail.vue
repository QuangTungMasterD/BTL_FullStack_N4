<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <div class="flex items-center gap-3">
          <Link to="/courses" variant="muted" icon-left="arrow_back">
            Quay lại
          </Link>
          <h1 class="font-headline-lg text-headline-lg">Chi tiết khóa học</h1>
        </div>
      </div>
      <div class="flex gap-3">
        <Button variant="outline" @click="openEditModal">
          <span class="material-symbols-outlined">edit</span>
          Sửa khóa học
        </Button>
        <Button variant="error" @click="confirmDelete" :disabled="courseStore.loading">
          <span class="material-symbols-outlined">delete</span>
          Xóa khóa học
        </Button>
      </div>
    </div>

    <!-- Loading -->
    <LoadingSpinner v-if="courseStore.loading" />

    <div v-else class="space-y-6">
      <!-- Thông tin khóa học -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <h2 class="font-headline-md text-headline-md mb-4">Thông tin chung</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p class="text-label-md text-on-surface-variant">Tên khóa học</p>
            <p class="font-body-md text-title-md">{{ course?.courseName }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Mã khóa học</p>
            <p class="font-body-md">{{ course?.id }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Học phí</p>
            <p class="font-body-md text-primary font-bold">{{ formatVND(course?.tuitionFee) }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">số tiết</p>
            <p class="font-body-md">{{ course?.lesson }} buổi</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Trình độ</p>
            <Badge :variant="courseLevelBadgeVariant(course?.level)">
              {{ courseLevelText(course?.level) }}
            </Badge>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Trạng thái</p>
            <Badge :variant="course?.isActive ? 'success' : 'default'">
              {{ course?.isActive ? 'Đang mở' : 'Ngừng mở' }}
            </Badge>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Chuyên môn</p>
            <p class="font-body-md">{{ specializationName || 'Chưa có' }}</p>
          </div>
          <div class="md:col-span-2">
            <p class="text-label-md text-on-surface-variant">Mô tả</p>
            <p class="font-body-md">{{ course?.desct || 'Chưa có mô tả' }}</p>
          </div>
        </div>
      </div>

      <!-- Danh sách lớp học -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="font-headline-md text-headline-md">Danh sách lớp học</h2>
          <!-- <Button size="sm" variant="primary" @click="goToCreateClass">
            <span class="material-symbols-outlined">add</span>
            Tạo lớp mới
          </Button> -->
        </div>

        <LoadingSpinner v-if="loadingClasses" />

        <div v-else-if="classes.length === 0" class="text-center py-8 text-on-surface-variant">
          Chưa có lớp học nào cho khóa học này
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
            <div class="grid grid-cols-2 md:grid-cols-4 gap-3 mt-3 text-label-sm text-on-surface-variant">
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">person</span>
                Giáo viên: {{ classItem.teacherName || 'Chưa phân công' }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">group</span>
                Sĩ số: {{ classItem.currentSlots || 0 }}/{{ classItem.maxSlots }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">meeting_room</span>
                Phòng: {{ classItem.roomName || 'Chưa có' }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">schedule</span>
                Lịch: {{ classItem.schedule || 'Chưa có' }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Edit -->
    <Modal v-model="showEditModal" title="Sửa khóa học">
      <form @submit.prevent="handleUpdate" class="space-y-4">
        <Input
          v-model="editForm.courseName"
          label="Tên khóa học"
          required
          :error="validationErrors?.courseName?.join(', ')"
        />
        
        <Input
          v-model="editForm.desct"
          label="Mô tả"
          type="textarea"
          :error="validationErrors?.desct?.join(', ')"
        />
        
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.tuitionFee"
            type="number"
            label="Học phí"
            required
            :error="validationErrors?.tuitionFee?.join(', ')"
          />
          
          <Input
            v-model="editForm.lesson"
            type="number"
            label="số tiết"
            required
            :error="validationErrors?.lesson?.join(', ')"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <Select
            v-model="editForm.level"
            label="Trình độ"
            :options="LEVEL_OPTIONS"
            required
            :error="validationErrors?.level?.join(', ')"
          />
          
          <Select
            v-model="editForm.isActive"
            label="Trạng thái"
            :options="STATUS_OPTIONS"
            required
          />
        </div>

        <Select
          v-model="editForm.specializationId"
          label="Chuyên ngành"
          :options="specializationOptions"
          :error="validationErrors?.specializationId?.join(', ')"
        />

        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeEditModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="courseStore.loading">
            Cập nhật
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa khóa học '${course?.courseName}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteCourse"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useCourseStore, useSpecializationStore, useClassStore } from '@/stores';
import Link from '@/components/ui/Link.vue';
import Button from '@/components/ui/Button.vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Modal from '@/components/ui/Modal.vue';
import Badge from '@/components/ui/Badge.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue';
import { formatVND, courseLevelText, courseLevelBadgeVariant, classStatusText } from '@/composables/useFormat';
import { LEVEL_OPTIONS, STATUS_OPTIONS } from '@/constants';

const router = useRouter();
const route = useRoute();
const courseStore = useCourseStore();
const specializationStore = useSpecializationStore();
const classStore = useClassStore();
const { validationErrors } = storeToRefs(courseStore);

// State
const course = ref(null);
const classes = ref([]);
const loadingClasses = ref(false);
const showEditModal = ref(false);
const showDeleteConfirm = ref(false);
const specializationName = ref('');

// Edit form
const editForm = reactive({
  courseName: '',
  desct: '',
  tuitionFee: null,
  lesson: null,
  level: null,
  isActive: true,
  specializationId: null,
});

const specializationOptions = computed(() => [
  { value: null, label: '-- Chọn chuyên ngành --' },
  ...specializationStore.specializations.map(item => ({
    value: item.id,
    label: item.specializationName
  }))
]);

// Lấy tên chuyên môn từ ID
const getSpecializationName = (id) => {
  if (!id) return 'Chưa có';
  const spec = specializationStore.specializations.find(s => s.id === id);
  return spec?.specializationName || 'Chưa có';
};

// Class status helpers
const getClassStatusVariant = (status) => {
  const map = { 1: 'info', 2: 'success', 3: 'default', 4: 'error', 5: 'warning' };
  return map[status] || 'default';
};

// Load data
const loadCourseDetail = async () => {
  const id = route.params.id;
  if (!id) {
    router.push('/courses');
    return;
  }
  
  try {
    course.value = await courseStore.fetchById(id);
    // Lấy tên chuyên môn sau khi có course
    specializationName.value = getSpecializationName(course.value.specializationId);
  } catch (err) {
    console.error('Failed to load course:', err);
    router.push('/courses');
  }
};

const loadClasses = async () => {
  const id = route.params.id;
  if (!id) return;
  
  loadingClasses.value = true;
  try {
    classes.value = await classStore.fetchByCourseId(id);
  } catch (err) {
    console.error('Failed to load classes:', err);
  } finally {
    loadingClasses.value = false;
  }
};

// Edit modal
const openEditModal = () => {
  editForm.courseName = course.value.courseName;
  editForm.desct = course.value.desct || '';
  editForm.tuitionFee = course.value.tuitionFee;
  editForm.lesson = course.value.lesson;
  editForm.level = course.value.level;
  editForm.isActive = course.value.isActive;
  editForm.specializationId = course.value.specializationId || null;
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
};

// Update course
const handleUpdate = async () => {
  const submitData = {
    courseName: editForm.courseName,
    desct: editForm.desct,
    tuitionFee: Number(editForm.tuitionFee),
    lesson: Number(editForm.lesson),
    level: Number(editForm.level),
    isActive: editForm.isActive === true || editForm.isActive === 'true',
    specializationId: editForm.specializationId ? Number(editForm.specializationId) : null,
  };
  
  try {
    await courseStore.update(course.value.id, submitData);
    closeEditModal();
    await loadCourseDetail();
    await loadClasses();
  } catch (err) {
    console.error('Update failed:', err);
  }
};

// Navigation
const goToClassDetail = (classId) => {
  router.push(`/classes/${classId}`);
};

// Delete
const confirmDelete = () => {
  showDeleteConfirm.value = true;
};

const handleDeleteCourse = async () => {
  await courseStore.delete(course.value.id);
  showDeleteConfirm.value = false;
  router.push('/courses');
};

onMounted(async () => {
  await specializationStore.fetchAll();
  await loadCourseDetail();
  await loadClasses();
});
</script>