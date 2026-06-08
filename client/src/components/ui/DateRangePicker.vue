<template>
  <div class="flex gap-2">
    <Input type="date" :model-value="start" placeholder="Từ ngày" @update:model-value="$emit('update:start', $event)" />
    <Input type="date" :model-value="end" placeholder="Đến ngày" @update:model-value="$emit('update:end', $event)" />
  </div>
</template>

<script setup>
import Input from './Input.vue';
defineProps({ start: String, end: String });
defineEmits(['update:start', 'update:end']);
</script>