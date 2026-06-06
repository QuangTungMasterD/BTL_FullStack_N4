<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant overflow-hidden">
    <div class="flex border-b border-outline-variant">
      <button
        v-for="(tab, idx) in tabs"
        :key="idx"
        class="px-6 py-3 font-title-md transition-all"
        :class="[
          modelValue === tab.name
            ? 'text-primary border-b-2 border-primary'
            : 'text-on-surface-variant hover:text-primary'
        ]"
        @click="$emit('update:modelValue', tab.name)"
      >
        {{ tab.label }}
      </button>
    </div>
    <div class="p-6">
      <slot />
    </div>
  </div>
</template>

<script setup>
defineProps({
  tabs: { type: Array, required: true }, // [{ name, label }]
  modelValue: { type: String, required: true },
});
defineEmits(['update:modelValue']);
</script>