<template>
  <span :class="['inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-label-md font-label-md', colorClass]">
    <span v-if="dot" class="w-1.5 h-1.5 rounded-full" :class="dotColorClass" />
    <slot />
  </span>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  variant: { type: String, default: 'default' }, // default, success, warning, error, info
  dot: Boolean,
});

const colorClass = computed(() => {
  switch (props.variant) {
    case 'success': return 'bg-secondary-container/20 text-on-secondary-container';
    case 'warning': return 'bg-amber-100 text-amber-800';
    case 'error':   return 'bg-error-container text-on-error-container';
    case 'info':    return 'bg-primary-container/20 text-primary';
    default:        return 'bg-surface-container-high text-on-surface-variant';
  }
});

const dotColorClass = computed(() => {
  switch (props.variant) {
    case 'success': return 'bg-secondary';
    case 'warning': return 'bg-amber-500';
    case 'error':   return 'bg-error';
    case 'info':    return 'bg-primary';
    default:        return 'bg-outline';
  }
});
</script>