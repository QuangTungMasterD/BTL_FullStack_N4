<template>
  <div class="space-y-6 animate-pulse">
    <!-- Thông tin chung -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
      <div class="h-7 bg-surface-variant rounded w-40 mb-6"></div>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div v-for="i in 6" :key="i" class="space-y-2">
          <div class="h-4 bg-surface-variant rounded w-24"></div>
          <div class="h-5 bg-surface-variant rounded w-32"></div>
        </div>
      </div>
    </div>
    <!-- Danh sách lớp / buổi học / giáo viên (tuỳ page) -->
    <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
      <div class="h-7 bg-surface-variant rounded w-48 mb-4"></div>
      <div class="space-y-3">
        <div v-for="i in 3" :key="i" class="h-20 bg-surface-variant rounded"></div>
      </div>
    </div>
  </div>
</template>