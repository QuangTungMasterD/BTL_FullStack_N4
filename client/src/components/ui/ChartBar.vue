<template>
  <div class="w-full h-48 flex items-end gap-2">
    <div v-for="item in data" :key="item.label" class="flex-1 flex flex-col items-center">
      <div
        class="w-full rounded-t-lg transition-all"
        :style="{ height: `${item.percent}%`, backgroundColor: color }"
      />
      <span class="text-label-md text-on-surface-variant mt-2">{{ item.label }}</span>
    </div>
  </div>
</template>

<script setup>
defineProps({
  data: { type: Array, required: true }, // [{ label, percent }]
  color: { type: String, default: '#00288e' },
});
</script>