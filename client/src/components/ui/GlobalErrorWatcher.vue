<template>
  <div class="none"></div>
</template>

<script setup>
import { watch } from 'vue';
import { useToast } from '@/composables/useToast';
import {
  useCourseStore,
  useTeacherStore,
  useClassStore,
  useRoomStore,
  useSpecializationStore,
  useScheduleChangeRequestStore,
  useClassSessionStore,
  useTeacherAssignmentStore,
  useTeacherSpecializationStore,
} from '@/stores';

const toast = useToast();

// Lấy tất cả store
const courseStore = useCourseStore();
const teacherStore = useTeacherStore();
const classStore = useClassStore();
const roomStore = useRoomStore();
const specializationStore = useSpecializationStore();
const scheduleRequestStore = useScheduleChangeRequestStore();
const classSessionStore = useClassSessionStore();
const teacherAssignmentStore = useTeacherAssignmentStore();
const teacherSpecializationStore = useTeacherSpecializationStore();

// Gom thành mảng để dễ xử lý
const stores = [
  courseStore,
  teacherStore,
  classStore,
  roomStore,
  specializationStore,
  scheduleRequestStore,
  classSessionStore,
  teacherAssignmentStore,
  teacherSpecializationStore,
];

// Theo dõi lỗi của từng store
stores.forEach((store) => {
  watch(
    () => store.error,
    (newError) => {
      if (newError) {
        // Hiển thị toast lỗi, kèm theo validationErrors nếu có
        toast.error(newError, 5000, store.validationErrors);
        // Xóa lỗi trong store để tránh hiển thị lại
        store.error = null;
        store.errorStatusCode = null;
        store.timestamp = null;
      }
    }
  );
});
</script>