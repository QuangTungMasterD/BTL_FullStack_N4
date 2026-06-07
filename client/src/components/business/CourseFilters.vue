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
        :options="levelOptions"
        @update:model-value="onFilterChange"
      />

      <!-- Trạng thái -->
      <Select
        v-model="filters.isActive"
        label="Trạng thái"
        :options="statusOptions"
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
import { reactive, computed, watch } from 'vue';
import Input from '@/components/ui/Input.vue';
import Select from '@/components/ui/Select.vue';
import Button from '@/components/ui/Button.vue';

const emit = defineEmits(['filter-change']);

// Options định nghĩa tại đây
const levelOptions = [
  { value: '', label: 'Tất cả trình độ' },
  { value: 1, label: 'Sơ cấp' },
  { value: 2, label: 'Căn bản' },
  { value: 3, label: 'Trung cấp' },
  { value: 4, label: 'Cao cấp' },
  { value: 5, label: 'Chuyên gia' },
];

const statusOptions = [
  { value: '', label: 'Tất cả trạng thái' },
  { value: true, label: 'Đang mở' },
  { value: false, label: 'Ngừng mở' },
];

const filters = reactive({
  search: '',
  level: '',
  isActive: '',
  minFee: null,
  maxFee: null,
});

const hasActiveFilters = computed(() => {
  return filters.search || 
         filters.level !== '' || 
         filters.isActive !== '' || 
         filters.minFee !== null || 
         filters.maxFee !== null;
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (filters.search) count++;
  if (filters.level !== '') count++;
  if (filters.isActive !== '') count++;
  if (filters.minFee !== null) count++;
  if (filters.maxFee !== null) count++;
  return count;
});

let debounceTimer = null;

// Hàm áp dụng filter - gửi dữ liệu lên parent
const applyFilters = () => {
  const queryParams = {
    search: filters.search || undefined,
    level: filters.level !== null ? filters.level : undefined,
    isActive: filters.isActive !== null ? filters.isActive : undefined,
    minFee: filters.minFee !== null ? Number(filters.minFee) : undefined,
    maxFee: filters.maxFee !== null ? Number(filters.maxFee) : undefined,
  };
  
  // Xóa các key có value undefined/null
  Object.keys(queryParams).forEach(key => {
    if (queryParams[key] === undefined || queryParams[key] === null) {
      delete queryParams[key];
    }
  });
  emit('filter-change', queryParams);
};

const onFilterChange = () => {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    applyFilters();
  }, 500);
};

const resetFilters = () => {
  filters.search = '';
  filters.level = '';
  filters.isActive = '';
  filters.minFee = null;
  filters.maxFee = null;
  applyFilters();  // Gọi ngay sau khi reset
};

// Nếu có initial filters từ parent, có thể watch và cập nhật
// (tuỳ chọn, nếu cần đồng bộ filter từ URL)
</script>