<template>
  <div class="space-y-2">
    <label v-for="opt in options" :key="opt.value" class="flex items-center gap-2 cursor-pointer">
      <input
        type="radio"
        :value="opt.value"
        :checked="modelValue === opt.value"
        class="w-4 h-4 border-outline-variant text-primary focus:ring-primary/20"
        @change="$emit('update:modelValue', opt.value)"
      />
      <span class="text-body-md">{{ opt.label }}</span>
    </label>
  </div>
</template>

<script setup>
defineProps({
  modelValue: [String, Number],
  options: { type: Array, required: true }, // [{ value, label }]
});
defineEmits(['update:modelValue']);
</script>