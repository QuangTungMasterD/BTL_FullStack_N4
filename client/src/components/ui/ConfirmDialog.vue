<template>
  <Modal v-model="internalValue" :title="title">
    <p class="text-body-md text-on-surface-variant">{{ message }}</p>
    <template #footer>
      <Button variant="outline" @click="cancel">Hủy</Button>
      <Button variant="error" @click="confirm">{{ confirmText }}</Button>
    </template>
  </Modal>
</template>

<script setup>
import { computed } from 'vue';
import Modal from './Modal.vue';
import Button from './Button.vue';

const props = defineProps({
  modelValue: Boolean,
  title: { type: String, default: 'Xác nhận' },
  message: String,
  confirmText: { type: String, default: 'Xóa' },
});
const emit = defineEmits(['update:modelValue', 'confirm']);

const internalValue = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
});

const confirm = () => {
  internalValue.value = false;
  emit('confirm');
};
const cancel = () => { internalValue.value = false; };
</script>