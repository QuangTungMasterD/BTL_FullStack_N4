<template>
  <div class="group relative bg-surface-container-lowest rounded-xl border border-outline-variant hover:shadow-lg transition-all duration-200">
    <!-- Action buttons - hiện khi hover -->
    <div class="absolute top-3 right-3 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity duration-200">
      <template v-if="isTrash">
        <button 
          @click="$emit('restore', course)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-success-container/20 text-on-surface-variant hover:text-success transition-colors"
          title="Khôi phục khóa học"
        >
          <span class="material-symbols-outlined text-sm">restore_from_trash</span>
        </button>
        <button 
          @click="$emit('delete-permanent', course)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-error-container/20 text-on-surface-variant hover:text-error transition-colors"
          title="Xóa vĩnh viễn"
        >
          <span class="material-symbols-outlined text-sm">delete_forever</span>
        </button>
      </template>
      <template v-else>
        <button 
          @click="$emit('edit', course.id)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-primary-container/20 text-on-surface-variant hover:text-primary transition-colors"
          title="Sửa khóa học"
        >
          <span class="material-symbols-outlined text-sm">edit</span>
        </button>
        <button 
          @click="$emit('delete', course)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-error-container/20 text-on-surface-variant hover:text-error transition-colors"
          title="Xóa khóa học"
        >
          <span class="material-symbols-outlined text-sm">delete</span>
        </button>
      </template>
    </div>

    <div class="p-4">
      <div class="flex items-start gap-3">
        <!-- Icon -->
        <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-primary-container/30 to-primary-container/10 flex items-center justify-center text-primary shrink-0">
          <span class="material-symbols-outlined text-2xl">menu_book</span>
        </div>
        
        <!-- Nội dung -->
        <div class="flex-1 min-w-0">
          <h4 class="font-title-md font-semibold group-hover:text-primary transition-colors truncate">
            {{ course.courseName }}
          </h4>
          <!-- <p class="text-label-sm text-on-surface-variant mt-0.5">Mã: {{ course.id }}</p> -->
          
          <!-- Mô tả -->
          <p class="text-body-sm line-clamp-2 mt-2 text-on-surface-variant/80">
            {{ course.desct || 'Chưa có mô tả' }}
          </p>
          
          <!-- Badge & Giá -->
          <div class="mt-3 flex items-center justify-between flex-wrap gap-2">
            <Badge :variant="course.isActive ? 'success' : 'default'" size="sm">
              {{ course.isActive ? 'Đang mở' : 'Ngừng mở' }}
            </Badge>
            <span class="text-title-md font-bold text-primary">{{ formatVND(course.tuitionFee) }}</span>
          </div>
          
          <!-- Thông tin thêm -->
          <div class="mt-2 flex items-center gap-3 text-label-sm text-on-surface-variant">
            <span class="flex items-center gap-1">
              <span class="material-symbols-outlined text-sm">signal_cellular_alt</span>
              {{ courseLevelText(course.level) }}
            </span>
            <span class="flex items-center gap-1">
              <span class="material-symbols-outlined text-sm">schedule</span>
              {{ course.lesson }} buổi
            </span>
          </div>
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
  isTrash: {
    type: Boolean,
    default: false,
  },

});

defineEmits(['edit', 'delete', 'restore', 'delete-permanent']);
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>