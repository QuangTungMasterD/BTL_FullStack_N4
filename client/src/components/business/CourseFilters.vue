<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <!-- Tìm kiếm -->
      <Input
        v-model="filters.search"
        label="Tìm kiếm"
        placeholder="Tên khóa học..."
        icon="search"
        @update:model-value="onFilterChange"
      />

      <!-- Trình độ -->
      <Select
        v-model="filters.level"
        label="Trình độ"
        :options="courseStore.levelOptions"
        @update:model-value="onFilterChange"
      />

      <!-- Trạng thái -->
      <Select
        v-model="filters.isActive"
        label="Trạng thái"
        :options="courseStore.statusOptions"
        @update:model-value="onFilterChange"
      />

      <!-- Khoảng giá -->
      <div class="flex gap-2">
        <Input
          v-model="filters.minFee"
          type="number"
          label="Giá từ"
          placeholder="0"
          @update:model-value="onFilterChange"
        />
        <Input
          v-model="filters.maxFee"
          type="number"
          label="Đến"
          placeholder="∞"
          @update:model-value="onFilterChange"
        />
      </div>
    </div>

    <div class="flex justify-between items-center mt-4 pt-4 border-t border-outline-variant">
      <div class="flex gap-2">
        <Button variant="outline" size="sm" @click="resetFilters">
          <span class="material-symbols-outlined text-sm">refresh</span>
          Đặt lại
        </Button>
        <Button variant="primary" size="sm" @click="applyFilters">
          <span class="material-symbols-outlined text-sm">filter_alt</span>
          Áp dụng
        </Button>
      </div>
      <div v-if="hasActiveFilters" class="text-label-md text-on-surface-variant">
        Đang lọc theo {{ activeFiltersCount }} tiêu chí
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useCourseStore } from '@/stores';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Button from '@/components/ui/Button.vue';

const courseStore = useCourseStore();
const { pagedData } = storeToRefs(courseStore);

const emit = defineEmits(['filter-change']);

const filters = reactive({
  search: '',
  level: null,
  isActive: null,
  minFee: null,
  maxFee: null,
});

const hasActiveFilters = computed(() => {
  return filters.search || 
         filters.level !== null || 
         filters.isActive !== null || 
         filters.minFee !== null || 
         filters.maxFee !== null;
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (filters.search) count++;
  if (filters.level !== null) count++;
  if (filters.isActive !== null) count++;
  if (filters.minFee !== null) count++;
  if (filters.maxFee !== null) count++;
  return count;
});

let debounceTimer = null;

const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    applyFilters();
  }, 500);
};

const applyFilters = () => {
  const queryParams = {
    page: 1,
    pageSize: pagedData.value.pageSize,
    search: filters.search || undefined,
    level: filters.level !== null ? filters.level : undefined,
    isActive: filters.isActive !== null ? filters.isActive : undefined,
    minFee: filters.minFee !== null ? Number(filters.minFee) : undefined,
    maxFee: filters.maxFee !== null ? Number(filters.maxFee) : undefined,
  };
  
  // Xóa các key có value undefined/null
  Object.keys(queryParams).forEach(key => {
    if (queryParams[key] === undefined || queryParams[key] === null || queryParams[key] === '') {
      delete queryParams[key];
    }
  });
  
  emit('filter-change', queryParams);
};

const resetFilters = () => {
  filters.search = '';
  filters.level = null;
  filters.isActive = null;
  filters.minFee = null;
  filters.maxFee = null;
  applyFilters();
};
</script>