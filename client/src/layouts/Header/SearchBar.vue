<template>
  <div class="relative w-full">
    <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-outline">
      search
    </span>
    <input
      type="text"
      :placeholder="placeholder"
      class="w-full bg-surface-container-low border-none rounded-full pl-10 pr-4 py-2 text-body-md focus:ring-2 focus:ring-primary focus:bg-surface transition"
      v-model="keyword"
      @input="onInput"
    />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  placeholder: { type: String, default: 'Tìm kiếm dữ liệu...' },
  modelValue: { type: String, default: '' }
})

const emit = defineEmits(['update:modelValue', 'search'])

const keyword = ref(props.modelValue)

watch(keyword, (newVal) => {
  emit('update:modelValue', newVal)
})

const onInput = () => {
  emit('search', keyword.value)
}
</script>