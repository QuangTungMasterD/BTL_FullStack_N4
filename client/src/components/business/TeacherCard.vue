<template>
  <div class="group relative bg-surface-container-lowest rounded-xl border border-outline-variant hover:shadow-lg transition-all duration-200">
    <!-- Action buttons -->
    <div class="absolute top-3 right-3 flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity duration-200">
      <template v-if="isTrash">
        <button 
          @click="$emit('restore', teacher)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-success-container/20 text-on-surface-variant hover:text-success transition-colors"
          title="Khôi phục"
        >
          <span class="material-symbols-outlined text-sm">restore_from_trash</span>
        </button>
        <button 
          @click="$emit('delete-permanent', teacher)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-error-container/20 text-on-surface-variant hover:text-error transition-colors"
          title="Xóa vĩnh viễn"
        >
          <span class="material-symbols-outlined text-sm">delete_forever</span>
        </button>
      </template>
      <template v-else>
        <Link :to="`/teachers/${teacher.id}`" variant="primary" class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-primary-container/20 text-on-surface-variant hover:text-primary transition-colors">
          <span class="material-symbols-outlined">info</span>
        </Link>
        <button 
          @click="$emit('edit', teacher.id)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-primary-container/20 text-on-surface-variant hover:text-primary transition-colors"
          title="Sửa"
        >
          <span class="material-symbols-outlined text-sm">edit</span>
        </button>
        <button 
          @click="$emit('delete', teacher)"
          class="w-[36px] h-[36px] p-1.5 rounded-xl bg-surface-container hover:bg-error-container/20 text-on-surface-variant hover:text-error transition-colors"
          title="Xóa"
        >
          <span class="material-symbols-outlined text-sm">delete</span>
        </button>
      </template>
    </div>

    <div class="p-4">
      <div class="flex items-start gap-3">
        <!-- Avatar -->
        <Avatar :name="teacher.fullName" size="lg" />

        <!-- Nội dung -->
        <div class="flex-1 min-w-0">
          <h4 class="font-title-md font-semibold group-hover:text-primary transition-colors truncate">
            {{ teacher.fullName }}
          </h4>
          <p class="text-label-sm text-on-surface-variant mt-0.5">ID: {{ teacher.id }}</p>
          
          <!-- Email & Phone -->
          <div class="mt-2 space-y-1">
            <div class="flex items-center gap-1 text-label-sm text-on-surface-variant">
              <span class="material-symbols-outlined text-sm">mail</span>
              <span class="truncate">{{ teacher.email || 'Chưa có email' }}</span>
            </div>
            <div class="flex items-center gap-1 text-label-sm text-on-surface-variant">
              <span class="material-symbols-outlined text-sm">phone</span>
              <span>{{ teacher.phone || 'Chưa có số điện thoại' }}</span>
            </div>
          </div>
          
          <!-- Thông tin khác -->
          <div class="mt-2 flex items-center gap-3 text-label-sm text-on-surface-variant">
            <span class="flex items-center gap-1">
              <span class="material-symbols-outlined text-sm">cake</span>
              {{ teacher.yoB || 'Chưa có' }}
            </span>
            <span class="flex items-center gap-1">
              <span class="material-symbols-outlined text-sm">{{ teacher.gender === true ? 'male' : 'female' }}</span>
              {{ teacher.gender === true ? 'Nam' : teacher.gender === false ? 'Nữ' : 'Khác' }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import Link from '@/components/ui/Link.vue';
import Avatar from '@/components/ui/Avatar.vue';

defineProps({
  teacher: {
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