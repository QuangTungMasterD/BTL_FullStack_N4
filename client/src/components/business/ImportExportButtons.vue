<template>
  <div class="flex gap-2">
    <Button 
      variant="outline" 
      :loading="store.isExporting"
      @click="handleExport"
    >
      <span class="material-symbols-outlined">download</span>
      Xuất Excel
    </Button>

    <Button 
      variant="outline" 
      :loading="store.isImporting"
      @click="triggerFileInput"
    >
      <span class="material-symbols-outlined">upload</span>
      Nhập Excel
    </Button>

    <input
      ref="fileInput"
      type="file"
      accept=".xlsx, .xls"
      class="hidden"
      @change="handleFileSelect"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue';
import Button from '@/components/ui/Button.vue';

const props = defineProps({
  // Nhận store từ bên ngoài (courseStore, teacherStore, etc.)
  store: {
    type: Object,
    required: true,
  },
  // Params filter hiện tại để export
  exportParams: {
    type: Object,
    default: () => ({}),
  },
});

const fileInput = ref(null);

const triggerFileInput = () => {
  fileInput.value.click();
};

const handleExport = async () => {
  try {
    await props.store.exportToExcel(props.exportParams);
    // Có thể thêm toast thông báo
  } catch (err) {
    console.error('Export failed:', err);
    alert('Xuất file thất bại: ' + err.message);
  }
};

const handleFileSelect = async (event) => {
  const file = event.target.files[0];
  if (!file) return;
  
  fileInput.value.value = '';
  
  try {
    const result = await props.store.importFromExcel(file);
    if (result.errors.length === 0) {
      alert(`Import thành công ${result.success.length} bản ghi`);
    } else {
      alert(`Import hoàn tất: ${result.success.length} thành công, ${result.errors.length} thất bại`);
      console.log('Import errors:', result.errors);
    }
  } catch (err) {
    console.error('Import failed:', err);
    alert('Import thất bại: ' + err.message);
  }
};
</script>