<template>
  <div class="bg-surface-container-lowest rounded-xl p-6 border border-outline-variant shadow-sm">
    <div class="flex items-center justify-between">
      <div class="w-10 h-10 rounded-lg" :class="iconBgClass" />
      <span v-if="trend" class="text-label-md flex items-center gap-1" :class="trendClass">
        <span class="material-symbols-outlined text-sm">{{ trendIcon }}</span>
        {{ trend }}
      </span>
    </div>
    <p class="font-label-md text-label-md text-on-surface-variant uppercase mt-4 mb-1">{{ label }}</p>
    <p class="font-headline-lg text-headline-lg">{{ value }}</p>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  label: String,
  value: [String, Number],
  icon: String,
  iconBg: { type: String, default: 'bg-primary/10' },
  trend: String,
  trendUp: Boolean,
});

const iconBgClass = computed(() => `flex items-center justify-center ${props.iconBg}`);
const trendIcon = computed(() => props.trendUp ? 'trending_up' : 'trending_down');
const trendClass = computed(() => props.trendUp ? 'text-secondary' : 'text-error');
</script>