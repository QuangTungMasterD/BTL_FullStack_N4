<template>
  <div class="space-y-1">
    <label v-if="label" :for="id" class="font-label-md text-label-md text-on-surface-variant ml-1">
      {{ label }}
      <span v-if="required" class="text-error">*</span>
    </label>
    <div class="relative">
      <span v-if="icon" class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-outline">
        {{ icon }}
      </span>
      <input
        :id="id"
        :type="type"
        :value="modelValue"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        class="w-full rounded-lg border border-outline-variant bg-surface-container-lowest px-4 py-2.5 text-body-md focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all disabled:opacity-60 disabled:bg-surface-container-high disabled:cursor-not-allowed"
        :class="{ 'pl-10': icon }"
        @input="$emit('update:modelValue', $event.target.value)"
        @blur="$emit('blur')"
      />
    </div>
    <p v-if="error" class="text-label-md text-error">{{ error }}</p>
  </div>
</template>

<script setup>
defineProps({
  id: String,
  label: String,
  type: { type: String, default: 'text' },
  modelValue: [String, Number],
  placeholder: String,
  required: Boolean,
  disabled: Boolean,
  icon: String,
  error: String,
});
defineEmits(['update:modelValue', 'blur']);
</script>