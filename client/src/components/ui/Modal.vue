<template>
  <Teleport to="body">
    <div v-if="modelValue" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-inverse-surface/40 backdrop-blur-sm" @click="close" />
      <!-- Content -->
      <div class="relative w-full max-w-lg rounded-2xl bg-surface-container-lowest shadow-xl overflow-hidden animate-fade-in-up">
        <div class="flex items-center justify-between border-b border-outline-variant px-6 py-4 bg-surface-container-low">
          <h3 class="font-headline-md text-headline-md">{{ title }}</h3>
          <button class="p-1 w-[50px] h-[50px] flex align-center justify-center rounded-full hover:bg-surface-container-high" @click="close">
            <span class="material-symbols-outlined">close</span>
          </button>
        </div>
        <div class="px-6 py-5">
          <slot />
        </div>
        <div v-if="$slots.footer" class="border-t border-outline-variant px-6 py-4 bg-surface-container-low flex justify-end gap-3">
          <slot name="footer" />
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
const props = defineProps({ modelValue: Boolean, title: String });
const emit = defineEmits(['update:modelValue']);
const close = () => emit('update:modelValue', false);
</script>