<template>
  <label class="flex items-center gap-2 cursor-pointer">
    <input
      type="checkbox"
      :checked="modelValue"
      class="w-4 h-4 rounded border-outline-variant text-primary focus:ring-primary/20"
      @change="$emit('update:modelValue', $event.target.checked)"
    />
    <span class="text-body-md">{{ label }}</span>
  </label>
</template>

<script setup>
defineProps({ modelValue: Boolean, label: String });
defineEmits(['update:modelValue']);
</script>