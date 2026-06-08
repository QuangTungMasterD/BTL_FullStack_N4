<template>
  <div
    class="flex items-center justify-center rounded-full bg-surface-variant text-on-surface-variant font-bold"
    :class="[sizeClass, { 'border-2 border-primary': bordered }]"
  >
    <img v-if="src" :src="src" alt="avatar" class="w-full h-full rounded-full object-cover" />
    <span v-else>{{ initials }}</span>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  src: String,
  name: String,
  size: { type: String, default: 'md' }, // sm, md, lg
  bordered: Boolean,
});

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'w-8 h-8 text-xs';
    case 'lg': return 'w-12 h-12 text-lg';
    default: return 'w-10 h-10 text-sm';
  }
});

const initials = computed(() => {
  if (!props.name) return '?';
  return props.name.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase();
});
</script>