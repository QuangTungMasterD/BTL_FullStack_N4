<template>
  <button
    class="w-10 h-10 rounded-full flex items-center justify-center text-on-surface-variant hover:bg-surface-container-high transition-all active:opacity-70"
    @click="$emit('click')"
  >
    <span class="material-symbols-outlined">{{ icon }}</span>
  </button>
</template>

<script setup>
defineProps({
  icon: { type: String, required: true }
})
defineEmits(['click'])
</script>