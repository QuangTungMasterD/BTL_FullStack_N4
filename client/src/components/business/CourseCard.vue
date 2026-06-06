<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 hover:shadow-md transition-all group">
    <div class="flex items-start gap-3">
      <div class="w-12 h-12 rounded-lg bg-primary-container/20 flex items-center justify-center text-primary shrink-0">
        <span class="material-symbols-outlined">menu_book</span>
      </div>
      <div class="flex-1 min-w-0">
        <h4 class="font-title-md group-hover:text-primary transition-colors truncate">{{ course.courseName }}</h4>
        <p class="text-label-md text-on-surface-variant">Mã: {{ course.id }}</p>
        <p class="text-body-md line-clamp-2 mt-1">{{ course.desct }}</p>
        <div class="mt-2 flex items-center justify-between flex-wrap gap-2">
          <Badge :variant="course.isActive ? 'success' : 'default'">
            {{ course.isActive ? 'Đang mở' : 'Ngừng mở' }}
          </Badge>
          <span class="text-title-md font-bold text-primary">{{ formatVND(course.tuitionFee) }}</span>
        </div>
        <div class="mt-2 text-label-md text-on-surface-variant">
          Trình độ: {{ courseLevelText(course.level) }} | Số buổi: {{ course.lesson }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import Badge from '@/components/ui/Badge.vue';
import { formatVND, courseLevelText } from '@/composables/useFormat.js';

defineProps({
  course: {
    type: Object,
    required: true,
  },
});
</script>