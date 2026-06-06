<template>
  <header class="sticky top-0 w-full z-40 bg-surface h-16 border-b border-outline-variant flex justify-between items-center px-margin-desktop">
    <!-- Search -->
    <div class="flex items-center flex-1 max-w-md">
      <div class="relative w-full">
        <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-outline">search</span>
        <input
          type="text"
          placeholder="Tìm kiếm dữ liệu..."
          class="w-full bg-surface-container-low border-none rounded-full pl-10 pr-4 py-2 text-body-md focus:ring-2 focus:ring-primary focus:bg-surface"
          @input="handleSearch"
        />
      </div>
    </div>

    <div class="flex items-center gap-6">
      <div class="flex items-center gap-4">
        <button
          class="w-10 h-10 rounded-full flex items-center justify-center text-on-surface-variant hover:bg-surface-container-high transition-all active:opacity-70"
          @mousedown="scaleDown"
          @mouseup="scaleUp"
          @mouseleave="scaleUp"
          @click="handleNotification"
        >
          <span class="material-symbols-outlined">notifications</span>
        </button>
        <button
          class="w-10 h-10 rounded-full flex items-center justify-center text-on-surface-variant hover:bg-surface-container-high transition-all active:opacity-70"
          @mousedown="scaleDown"
          @mouseup="scaleUp"
          @mouseleave="scaleUp"
          @click="handleChat"
        >
          <span class="material-symbols-outlined">chat_bubble</span>
        </button>
      </div>
      <div class="h-8 w-[1px] bg-outline-variant"></div>
      <div
        class="flex items-center gap-3 cursor-pointer group active:opacity-70"
        @click="handleProfile"
      >
        <div class="text-right hidden sm:block">
          <p class="font-title-md text-[14px] text-on-surface">Admin Manager</p>
          <p class="text-label-md text-outline">Quản trị viên</p>
        </div>
        <img
          src="https://lh3.googleusercontent.com/aida-public/AB6AXuBmgPhNgmTNZ6nXpIXANAuHbD9M_ppqwF16dyjjTMuz6hhsEZDZz3Q1BpbxhgFxLQTqW4Qo8uCrsYdZHLJE6KHDLaxL5eTb7GEGf77eOU0CGi9Qb5mJs1Egp6_toN3zGXFshXKZje65U0sGZ8CDRrrANXJCBz4D5yoJ2bVvc1cTnby2Zc_4G0X6isTqJoQftk3-Wj0zRLA1fnXL0MnHvyP0iPU5inUIkg-pXrjCCr7d2Qysbv_PTG1YGLgPKNBJcgCJLIqdeMFNKg0"
          alt="Avatar"
          class="w-10 h-10 rounded-full border border-outline-variant group-hover:border-primary transition-colors"
        />
      </div>
    </div>
  </header>
</template>

<script setup>
const scaleDown = (e) => e.currentTarget.classList.add('scale-95')
const scaleUp = (e) => e.currentTarget.classList.remove('scale-95')

const handleSearch = (e) => console.log('[Header] Tìm kiếm:', e.target.value)
const handleNotification = () => console.log('[Header] Mở thông báo')
const handleChat = () => console.log('[Header] Mở chat')
const handleProfile = () => console.log('[Header] Mở trang cá nhân')
</script>