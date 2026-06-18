<template>
  <tr class="animate-pulse border-b border-outline-variant">
    <td v-for="i in columns" :key="i" class="px-6 py-4">
      <div class="h-4 bg-surface-variant rounded"></div>
    </td>
  </tr>
</template>

<script setup>
defineProps({
  columns: { type: Number, required: true }
});
</script>