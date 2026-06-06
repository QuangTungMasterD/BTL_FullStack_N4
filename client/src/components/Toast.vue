<template>
  <Teleport to="body">
    <div
      v-if="visible"
      class="fixed bottom-8 right-8 z-50 flex items-center gap-3 rounded-xl bg-inverse-surface px-6 py-4 text-inverse-on-surface shadow-xl animate-slide-in"
    >
      <span class="material-symbols-outlined">{{ icon }}</span>
      <p class="font-body-md">{{ message }}</p>
    </div>
  </Teleport>
</template>

<script setup>
import { watch } from 'vue';

const props = defineProps({
  visible: Boolean,
  message: String,
  type: { type: String, default: 'success' }, // success, error, info
  duration: { type: Number, default: 3000 },
});
const emit = defineEmits(['update:visible']);

const icon = props.type === 'success' ? 'check_circle' : props.type === 'error' ? 'error' : 'info';

let timer = null;

watch(() => props.visible, (val) => {
  if (timer) clearTimeout(timer);
  if (val) {
    timer = setTimeout(() => {
      emit('update:visible', false);
    }, props.duration);
  }
});
</script>