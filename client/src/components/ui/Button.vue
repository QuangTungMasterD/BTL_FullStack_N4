<template>
  <button
    :class="[
      'inline-flex items-center justify-center gap-2 rounded-lg font-title-md transition-all active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed',
      variant === 'primary' && 'bg-primary text-on-primary shadow-md hover:opacity-90',
      variant === 'secondary' && 'bg-secondary text-on-secondary shadow-md hover:opacity-90',
      variant === 'error' && 'bg-error text-on-error shadow-md hover:opacity-90',
      variant === 'outline' && 'border border-primary text-primary hover:bg-primary/10',
      variant === 'ghost' && 'text-on-surface-variant hover:bg-surface-container-high',
      size === 'sm' && 'px-3 py-1.5 text-label-md',
      size === 'md' && 'px-5 py-2.5 text-body-md',
      size === 'lg' && 'px-8 py-3 text-title-md',
    ]"
    :disabled="disabled || loading"
    @click="$emit('click')"
  >
    <span v-if="icon" class="material-symbols-outlined text-[1.25rem]">{{ icon }}</span>
    <slot />
  </button>
</template>

<script setup>
defineProps({
  loading: Boolean,
  variant: { type: String, default: 'primary' }, // primary, secondary, error, outline, ghost
  size: { type: String, default: 'md' }, // sm, md, lg
  disabled: Boolean,
  icon: String,
});
defineEmits(['click']);
</script>