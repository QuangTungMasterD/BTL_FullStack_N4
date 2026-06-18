<template>
  <Teleport to="body">
    <div class="fixed bottom-6 right-6 z-[100] flex flex-col gap-3 max-w-md">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="rounded-xl shadow-lg overflow-hidden animate-slide-in-right"
        :class="{
          'bg-success-container text-on-success-container': toast.type === 'success',
          'bg-error-container text-on-error-container': toast.type === 'error',
          'bg-info-container text-on-info-container': toast.type === 'info',
          'bg-warning-container text-on-warning-container': toast.type === 'warning',
        }"
      >
        <div class="flex items-start gap-3 p-4">
          <span class="material-symbols-outlined flex-shrink-0">{{ iconMap[toast.type] }}</span>
          <div class="flex-1">
            <p class="font-body-md whitespace-pre-wrap">{{ toast.message }}</p>
            <div v-if="toast.details" class="mt-2 text-label-md">
              <div v-for="(errors, field) in toast.details" :key="field">
                <span class="font-medium">{{ field }}:</span> {{ Array.isArray(errors) ? errors.join(', ') : errors }}
              </div>
            </div>
          </div>
          <button @click="toastStore.removeToast(toast.id)" class="opacity-70 hover:opacity-100">
            <span class="material-symbols-outlined text-sm">close</span>
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { storeToRefs } from 'pinia';
import { useToastStore } from '@/stores/toastStore';

const toastStore = useToastStore();
const { toasts } = storeToRefs(toastStore);

const iconMap = {
  success: 'check_circle',
  error: 'error',
  info: 'info',
  warning: 'warning',
};
</script>

<style scoped>
@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}
.animate-slide-in-right {
  animation: slideInRight 0.3s ease-out;
}
</style>