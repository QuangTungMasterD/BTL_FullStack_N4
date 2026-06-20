<template>
  <div class="w-full">
    <div class="flex justify-between text-label-md mb-1">
      <span>{{ label }}</span>
      <span>{{ value }}%</span>
    </div>
    <div class="h-2 bg-surface-container rounded-full overflow-hidden">
      <div class="h-full rounded-full" :class="[colorClass, value > 100 ? 'bg-error' : '']" :style="{ width: `${Math.min(value, 100)}%` }" />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  label: String,
  value: { type: Number, required: true },
  color: { type: String, default: 'primary' }, // primary, secondary, error
});

const colorClass = computed(() => {
  switch (props.color) {
    case 'secondary': return 'bg-secondary';
    case 'error': return 'bg-error';
    default: return 'bg-primary';
  }
});
</script>