<template>
  <div class="space-y-2">
    <label v-if="label" class="font-label-md text-label-md text-on-surface-variant ml-1">
      {{ label }}
    </label>
    
    <!-- Danh sách các thẻ đã chọn -->
    <div class="flex flex-wrap gap-2 p-2 rounded-lg border border-outline-variant bg-surface-container-lowest min-h-[50px]">
      <Badge
        v-for="id in selectedIds"
        :key="id"
        variant="info"
        class="flex items-center gap-1"
      >
        {{ getLabelById(id) }}
        <button
          type="button"
          class="ml-1 hover:text-error transition-colors"
          @click="removeTag(id)"
        >
          <span class="material-symbols-outlined text-sm">close</span>
        </button>
      </Badge>
      
      <!-- Input tìm kiếm -->
      <div class="relative flex-1 min-w-[120px]">
        <input
          ref="inputRef"
          v-model="searchTerm"
          type="text"
          :placeholder="placeholder"
          class="w-full bg-transparent outline-none text-body-md"
          @focus="showDropdown = true"
          @blur="handleBlur"
          @keydown.enter.prevent="addFirstOption"
        />
        
        <!-- Dropdown gợi ý -->
        <div
          v-if="showDropdown && filteredOptions.length"
          class="absolute z-10 left-0 right-0 mt-1 max-h-48 overflow-y-auto bg-surface-container-lowest border border-outline-variant rounded-lg shadow-lg"
        >
          <div
            v-for="opt in filteredOptions"
            :key="opt.value"
            class="px-4 py-2 cursor-pointer hover:bg-surface-container-high transition-colors"
            @mousedown.prevent="addTag(opt.value)"
          >
            {{ opt.label }}
          </div>
        </div>
      </div>
    </div>
    
    <!-- Thông báo lỗi (nếu có) -->
    <p v-if="error" class="text-label-md text-error">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import Badge from '@/components/ui/Badge.vue';

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => [],
  },
  options: {
    type: Array,
    required: true, // [{ value, label }]
  },
  label: {
    type: String,
    default: 'Chuyên ngành',
  },
  placeholder: {
    type: String,
    default: 'Nhập để tìm kiếm...',
  },
  error: {
    type: String,
    default: '',
  },
});

const emit = defineEmits(['update:modelValue']);

const selectedIds = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
});

const searchTerm = ref('');
const showDropdown = ref(false);
const inputRef = ref(null);

// Lọc các option chưa được chọn và khớp với từ khóa
const filteredOptions = computed(() => {
  const selectedSet = new Set(selectedIds.value);
  return props.options
    .filter(opt => !selectedSet.has(opt.value))
    .filter(opt => opt.label.toLowerCase().includes(searchTerm.value.toLowerCase()))
    .slice(0, 10);
});

const getLabelById = (id) => {
  const found = props.options.find(opt => opt.value === id);
  return found ? found.label : id;
};

const addTag = (id) => {
  if (!selectedIds.value.includes(id)) {
    selectedIds.value = [...selectedIds.value, id];
  }
  searchTerm.value = '';
  showDropdown.value = false;
  inputRef.value?.focus();
};

const removeTag = (id) => {
  selectedIds.value = selectedIds.value.filter(i => i !== id);
};

const addFirstOption = () => {
  if (filteredOptions.value.length) {
    addTag(filteredOptions.value[0].value);
  }
};

const handleBlur = () => {
  // Delay để cho phép click vào dropdown trước khi đóng
  setTimeout(() => {
    showDropdown.value = false;
  }, 150);
};

// Xóa searchTerm khi dropdown đóng
watch(showDropdown, (val) => {
  if (!val) searchTerm.value = '';
});
</script>