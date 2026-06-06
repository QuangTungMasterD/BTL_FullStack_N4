<template>
  <div class="flex flex-col items-center justify-center py-12 text-center">
    <span class="material-symbols-outlined text-6xl text-outline">inbox</span>
    <p class="mt-4 text-on-surface-variant">{{ message }}</p>
    <Button v-if="actionText" variant="outline" class="mt-4" @click="$emit('action')">
      {{ actionText }}
    </Button>
  </div>
</template>

<script setup>
defineProps({
  message: { type: String, default: 'Không có dữ liệu' },
  actionText: String,
});
defineEmits(['action']);
</script>