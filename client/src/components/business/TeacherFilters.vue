<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-4 mb-6">
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <!-- Tìm kiếm -->
      <Input
        v-model="filters.search"
        label="Tìm kiếm"
        placeholder="Họ tên, email..."
        icon="search"
        @update:model-value="onFilterChange"
      />

      <!-- Giới tính -->
      <Select
        v-model="filters.gender"
        label="Giới tính"
        :options="genderOptions"
        @update:model-value="onFilterChange"
      />

      <!-- Trạng thái -->
      <Select
        v-model="filters.isActive"
        label="Trạng thái"
        :options="statusOptions"
        @update:model-value="onFilterChange"
      />

      <!-- Năm sinh -->
      <div class="flex gap-2">
        <Input
          v-model="filters.yoBFrom"
          type="number"
          label="Năm sinh từ"
          placeholder="VD: 1990"
          @update:model-value="onFilterChange"
        />
        <Input
          v-model="filters.yoBTo"
          type="number"
          label="Đến"
          placeholder="VD: 2000"
          @update:model-value="onFilterChange"
        />
      </div>
    </div>

    <div class="flex justify-between items-center mt-4 pt-4 border-t border-outline-variant">
      <Button variant="outline" size="sm" @click="resetFilters">
        <span class="material-symbols-outlined text-sm">refresh</span>
        Đặt lại
      </Button>
      <div v-if="hasActiveFilters" class="text-label-md text-on-surface-variant">
        Đang lọc theo {{ activeFiltersCount }} tiêu chí
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed } from 'vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Button from '@/components/ui/Button.vue';

const emit = defineEmits(['filter-change']);

const genderOptions = [
  { value: '', label: 'Tất cả' },
  { value: true, label: 'Nam' },
  { value: false, label: 'Nữ' },
];

const statusOptions = [
  { value: '', label: 'Tất cả trạng thái' },
  { value: true, label: 'Đang hoạt động' },
  { value: false, label: 'Ngừng hoạt động' },
];

const filters = reactive({
  search: '',
  gender: '',
  isActive: '',
  yoBFrom: null,
  yoBTo: null,
});

const hasActiveFilters = computed(() => {
  return filters.search || 
         filters.gender !== '' || 
         filters.isActive !== '' || 
         filters.yoBFrom !== null || 
         filters.yoBTo !== null;
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (filters.search) count++;
  if (filters.gender !== '') count++;
  if (filters.isActive !== '') count++;
  if (filters.yoBFrom !== null) count++;
  if (filters.yoBTo !== null) count++;
  return count;
});

let debounceTimer = null;

const applyFilters = () => {
  const queryParams = {};
  if (filters.search) queryParams.search = filters.search;
  if (filters.gender !== '') queryParams.gender = filters.gender;
  if (filters.isActive !== '') queryParams.isActive = filters.isActive;
  if (filters.yoBFrom) queryParams.yoBFrom = Number(filters.yoBFrom);
  if (filters.yoBTo) queryParams.yoBTo = Number(filters.yoBTo);
  
  emit('filter-change', queryParams);
};

const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(applyFilters, 500);
};

const resetFilters = () => {
  filters.search = '';
  filters.gender = '';
  filters.isActive = '';
  filters.yoBFrom = null;
  filters.yoBTo = null;
  applyFilters();
};
</script>