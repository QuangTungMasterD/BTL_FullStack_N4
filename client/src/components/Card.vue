<template>
  <div :class="['bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant', hover && 'hover:shadow-md transition-shadow', paddingClass]">
    <slot />
  </div>
</template>

<script setup>
defineProps({
  padding: { type: Boolean, default: true },
  hover: { type: Boolean, default: false },
});
const paddingClass = ({ padding: true }) ? 'p-6' : '';
</script>