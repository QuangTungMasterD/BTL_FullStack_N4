<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <div class="flex items-center gap-3">
          <Link to="/admin-courses" variant="muted" icon-left="arrow_back">
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
    <SkeletonDetail v-if="courseStore.loading" />

    <div v-else class="space-y-6">
      <!-- Thông tin khóa học với ảnh -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex flex-col md:flex-row gap-6">
          <!-- Ảnh khóa học -->
          <div class="w-full md:w-48 h-48 rounded-lg overflow-hidden bg-gradient-to-br from-primary/20 to-purple-500/20 flex items-center justify-center flex-shrink-0">
            <img 
              v-if="course?.imageUrl" 
              :src="course.imageUrl" 
              alt="Course image" 
              class="w-full h-full object-cover" 
            />
            <span v-else class="material-symbols-outlined text-6xl text-primary/40">menu_book</span>
          </div>
          
          <!-- Thông tin -->
          <div class="flex-1">
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
              <div class="md:col-span-2">
                <p class="text-label-md text-on-surface-variant">Mô tả</p>
                <p class="font-body-md">{{ course?.desct || 'Chưa có mô tả' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Danh sách lớp học -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="font-headline-md text-headline-md">Danh sách lớp học</h2>
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
          :error="validationErrors?.CourseName?.[0]"
        />
        
        <Input
          v-model="editForm.desct"
          label="Mô tả"
          type="textarea"
          :error="validationErrors?.desct?.[0]"
        />
        
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.tuitionFee"
            type="number"
            label="Học phí"
            required
            :error="validationErrors?.tuitionFee?.[0]"
          />
          
          <Input
            v-model="editForm.lesson"
            type="number"
            label="số tiết"
            required
            :error="validationErrors?.lesson?.[0]"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <Select
            v-model="editForm.level"
            label="Trình độ"
            :options="LEVEL_OPTIONS"
            required
            :error="validationErrors?.level?.[0]"
          />
          
          <Select
            v-model="editForm.isActive"
            label="Trạng thái"
            :options="STATUS_OPTIONS"
            required
          />
        </div>

        <!-- Upload ảnh -->
        <div class="mt-4">
          <label class="block text-sm font-medium text-gray-700">Ảnh khóa học</label>
          <div class="mt-2 flex items-center gap-4">
            <!-- Ảnh preview -->
            <div v-if="editForm.imageUrl || imagePreview" class="relative w-24 h-24 rounded-lg overflow-hidden border border-gray-200">
              <img :src="imagePreview || editForm.imageUrl" alt="Course image" class="w-full h-full object-cover" />
              <button v-if="editForm.imageUrl || imagePreview" type="button" @click="removeImage" class="absolute top-1 right-1 bg-red-500 text-white rounded-full p-1 hover:bg-red-600">
                <span class="material-symbols-outlined text-sm">close</span>
              </button>
            </div>
            <!-- Nút chọn ảnh -->
            <label class="cursor-pointer px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-lg border border-gray-300">
              <span class="material-symbols-outlined text-sm mr-1">upload</span>
              Chọn ảnh
              <input type="file" accept="image/*" class="hidden" @change="handleImageUpload" />
            </label>
            <span v-if="uploadingImage" class="text-sm text-gray-500">Đang tải lên...</span>
          </div>
          <p class="text-xs text-gray-500 mt-1">Hỗ trợ JPG, PNG, GIF, WebP. Tối đa 2MB.</p>
        </div>

        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeEditModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="courseStore.loading || uploadingImage">
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
import { useCourseStore, useClassStore } from '@/stores';
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
import SkeletonDetail from '@/components/skeleton/SkeletonDetail.vue';
import { useToast } from '@/composables/useToast';

const router = useRouter();
const route = useRoute();
const courseStore = useCourseStore();
const classStore = useClassStore();
const { validationErrors } = storeToRefs(courseStore);
const toast = useToast();

// State
const course = ref(null);
const classes = ref([]);
const loadingClasses = ref(false);
const showEditModal = ref(false);
const showDeleteConfirm = ref(false);

// Image state
const uploadingImage = ref(false);
const imageFile = ref(null);
const imagePreview = ref(null);

// Edit form
const editForm = reactive({
  courseName: '',
  desct: '',
  tuitionFee: 0,
  lesson: 1,
  level: 1,
  isActive: true,
  imageUrl: '',
});

// Class status helpers
const getClassStatusVariant = (status) => {
  const map = { 1: 'info', 2: 'success', 3: 'default', 4: 'error', 5: 'warning' };
  return map[status] || 'default';
};

// ===================== IMAGE HANDLERS =====================
const handleImageUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;

  // Kiểm tra kích thước (2MB)
  if (file.size > 2 * 1024 * 1024) {
    toast.error('File quá lớn, tối đa 2MB');
    event.target.value = '';
    return;
  }

  // Kiểm tra định dạng
  const allowed = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
  if (!allowed.includes(file.type)) {
    toast.error('Chỉ hỗ trợ JPG, PNG, GIF, WebP');
    event.target.value = '';
    return;
  }

  imageFile.value = file;
  const reader = new FileReader();
  reader.onload = (e) => {
    imagePreview.value = e.target.result;
  };
  reader.readAsDataURL(file);
};

const removeImage = () => {
  editForm.imageUrl = '';
  imageFile.value = null;
  imagePreview.value = null;
};

// ===================== DATA LOADING =====================
const loadCourseDetail = async () => {
  const id = route.params.id;
  if (!id) {
    router.push('/admin-courses');
    return;
  }
  
  try {
    course.value = await courseStore.fetchById(id);
  } catch (err) {
    console.error('Failed to load course:', err);
    router.push('/admin-courses');
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

// ===================== EDIT MODAL =====================
const openEditModal = () => {
  editForm.courseName = course.value.courseName;
  editForm.desct = course.value.desct || '';
  editForm.tuitionFee = course.value.tuitionFee;
  editForm.lesson = course.value.lesson;
  editForm.level = course.value.level;
  editForm.isActive = course.value.isActive;
  editForm.imageUrl = course.value.imageUrl || '';
  imageFile.value = null;
  imagePreview.value = null;
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
};

// ===================== UPDATE COURSE =====================
const handleUpdate = async () => {
  let finalImageUrl = editForm.imageUrl;

  // Nếu có ảnh mới, upload trước
  if (imageFile.value) {
    uploadingImage.value = true;
    try {
      const uploadResult = await courseStore.uploadImage(course.value.id, imageFile.value);
      finalImageUrl = uploadResult;
    } catch (err) {
      toast.error(err.message || 'Upload ảnh thất bại');
      uploadingImage.value = false;
      return;
    } finally {
      uploadingImage.value = false;
    }
  }

  const submitData = {
    courseName: editForm.courseName,
    desct: editForm.desct,
    tuitionFee: Number(editForm.tuitionFee),
    lesson: Number(editForm.lesson),
    level: Number(editForm.level),
    isActive: editForm.isActive === true || editForm.isActive === 'true',
    imageUrl: finalImageUrl,
  };
  
  try {
    await courseStore.update(course.value.id, submitData);
    toast.success('Cập nhật thành công!');
    closeEditModal();
    loadCourseDetail();
    loadClasses();
  } catch (err) {
    toast.error(err.message || 'Cập nhật thất bại');
    console.error('Update failed:', err);
  }
};

// ===================== DELETE =====================
const confirmDelete = () => {
  showDeleteConfirm.value = true;
};

const handleDeleteCourse = async () => {
  await courseStore.delete(course.value.id);
  showDeleteConfirm.value = false;
  router.push('/admin-courses');
};

// ===================== NAVIGATION =====================
const goToClassDetail = (classId) => {
  router.push(`/classes/${classId}`);
};

// ===================== LIFECYCLE =====================
onMounted(() => {
  loadCourseDetail();
  loadClasses();
});
</script>