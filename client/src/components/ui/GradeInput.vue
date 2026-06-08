<template>
  <div class="space-y-1">
    <input
      type="number"
      :value="modelValue"
      :min="min"
      :max="max"
      :step="step"
      class="w-24 px-3 py-2 rounded-lg border text-center focus:ring-2 focus:ring-primary transition-all"
      :class="isValid ? 'border-outline-variant' : 'border-error bg-error-container/10'"
      @input="handleInput"
    />
    <p v-if="!isValid" class="text-label-md text-error">Từ {{ min }} - {{ max }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const props = defineProps({
  modelValue: [Number, String],
  min: { type: Number, default: 0 },
  max: { type: Number, default: 10 },
  step: { type: Number, default: 0.1 },
});
const emit = defineEmits(['update:modelValue']);

const isValid = ref(true);

const handleInput = (e) => {
  let val = parseFloat(e.target.value);
  if (isNaN(val)) {
    isValid.value = false;
    return;
  }
  if (val < props.min || val > props.max) {
    isValid.value = false;
  } else {
    isValid.value = true;
    emit('update:modelValue', val);
  }
};
</script>