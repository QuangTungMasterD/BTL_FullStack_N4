<template>
  <div class="grid grid-cols-3 gap-4">
    <div class="text-center p-3 rounded-lg bg-surface-container-low">
      <span class="material-symbols-outlined text-secondary">check_circle</span>
      <p class="font-headline-md text-headline-md">{{ present }}</p>
      <p class="text-label-md text-on-surface-variant">Có mặt</p>
    </div>
    <div class="text-center p-3 rounded-lg bg-surface-container-low">
      <span class="material-symbols-outlined text-error">cancel</span>
      <p class="font-headline-md text-headline-md">{{ absent }}</p>
      <p class="text-label-md text-on-surface-variant">Vắng</p>
    </div>
    <div class="text-center p-3 rounded-lg bg-surface-container-low">
      <span class="material-symbols-outlined text-primary">schedule</span>
      <p class="font-headline-md text-headline-md">{{ late }}</p>
      <p class="text-label-md text-on-surface-variant">Đi muộn</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  present: Number,
  absent: Number,
  late: Number,
});
</script>