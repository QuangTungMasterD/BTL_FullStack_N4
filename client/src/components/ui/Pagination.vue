<template>
  <div class="flex items-center justify-between flex-wrap gap-4">
    <p class="text-label-md text-on-surface-variant">
      Hiển thị {{ from }}-{{ to }} trong số {{ total }} bản ghi
    </p>
    <div class="flex gap-2">
      <button
        class="p-2 rounded-lg border border-outline-variant hover:bg-surface-container disabled:opacity-30"
        :disabled="currentPage === 1"
        @click="changePage(currentPage - 1)"
      >
        <span class="material-symbols-outlined">chevron_left</span>
      </button>
      <button
        v-for="p in visiblePages"
        :key="p"
        class="px-3 py-1 rounded-lg font-title-md"
        :class="p === currentPage ? 'bg-primary text-on-primary' : 'hover:bg-surface-container'"
        @click="changePage(p)"
      >
        {{ p }}
      </button>
      <button
        class="p-2 rounded-lg border border-outline-variant hover:bg-surface-container disabled:opacity-30"
        :disabled="currentPage === totalPages"
        @click="changePage(currentPage + 1)"
      >
        <span class="material-symbols-outlined">chevron_right</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  total: { type: Number, required: true },
  perPage: { type: Number, default: 10 },
  currentPage: { type: Number, required: true },
});

const emit = defineEmits(['update:currentPage']);

const totalPages = computed(() => Math.ceil(props.total / props.perPage));
const from = computed(() => (props.currentPage - 1) * props.perPage + 1);
const to = computed(() => Math.min(props.currentPage * props.perPage, props.total));
const visiblePages = computed(() => {
  let pages = [];
  for (let i = 1; i <= Math.min(totalPages.value, 5); i++) pages.push(i);
  return pages;
});

const changePage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    emit('update:currentPage', page);
  }
};
</script>