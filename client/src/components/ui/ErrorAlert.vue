<template>
  <div v-if="error" class="rounded-lg bg-error-container/10 border border-error-container p-4 mb-6">
    <div class="flex items-start gap-3">
      <span class="material-symbols-outlined text-error">error</span>
      <div class="flex-1">
        <p class="font-title-md text-error">Lỗi {{ statusCode }}</p>
        <p class="text-body-md">{{ error }}</p>
        <div v-if="validationErrors && Object.keys(validationErrors).length" class="mt-2">
          <p class="text-label-md font-medium">Chi tiết:</p>
          <ul class="list-disc list-inside text-label-md">
            <li v-for="(errors, field) in validationErrors" :key="field">
              {{ field }}: {{ Array.isArray(errors) ? errors.join(', ') : errors }}
            </li>
          </ul>
        </div>
        <p v-if="timestamp" class="text-label-sm text-on-surface-variant mt-2">
          {{ formatDateTime(timestamp) }}
        </p>
      </div>
      <button @click="$emit('close')" class="text-on-surface-variant hover:text-error transition">
        <span class="material-symbols-outlined">close</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { formatDateTime } from '@/composables/useFormat';

defineProps({
  error: String,
  statusCode: Number,
  validationErrors: Object,
  timestamp: String,
});

defineEmits(['close']);
</script>