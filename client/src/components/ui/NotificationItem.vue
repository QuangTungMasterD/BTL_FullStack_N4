<template>
  <div class="flex items-start gap-4 p-4 rounded-lg hover:bg-surface-container-low transition-colors group cursor-pointer">
    <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="iconBg">
      <span class="material-symbols-outlined">{{ icon }}</span>
    </div>
    <div class="flex-1">
      <p class="font-body-md font-medium">{{ title }}</p>
      <p class="text-label-md text-on-surface-variant">{{ time }}</p>
    </div>
    <button v-if="action" class="text-primary opacity-0 group-hover:opacity-100 transition-opacity" @click.stop="handleAction">
      {{ action }}
    </button>
  </div>
</template>

<script setup>
const props = defineProps({
  title: String,
  time: String,
  icon: String,
  iconBg: { type: String, default: 'bg-primary-container/20 text-primary' },
  action: String,
});
const emit = defineEmits(['action']);
const handleAction = () => emit('action');
</script>