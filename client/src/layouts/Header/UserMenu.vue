<!-- components/UserMenu.vue -->
<template>
  <div
    class="flex items-center gap-3 cursor-pointer group active:opacity-70"
    @click="$emit('click')"
  >
    <div class="text-right hidden sm:block">
      <p class="font-title-md text-[14px] text-on-surface">{{ userName }}</p>
      <p class="text-label-md text-outline">{{ userRole }}</p>
    </div>
    <img
      :src="avatarUrl"
      alt="Avatar"
      class="w-10 h-10 rounded-full border border-outline-variant group-hover:border-primary transition-colors"
    />
  </div>
</template>

<script setup>
defineProps({
  userName: { type: String, default: 'Admin Manager' },
  userRole: { type: String, default: 'Quản trị viên' },
  avatarUrl: { type: String, default: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBmgPhNgmTNZ6nXpIXANAuHbD9M_ppqwF16dyjjTMuz6hhsEZDZz3Q1BpbxhgFxLQTqW4Qo8uCrsYdZHLJE6KHDLaxL5eTb7GEGf77eOU0CGi9Qb5mJs1Egp6_toN3zGXFshXKZje65U0sGZ8CDRrrANXJCBz4D5yoJ2bVvc1cTnby2Zc_4G0X6isTqJoQftk3-Wj0zRLA1fnXL0MnHvyP0iPU5inUIkg-pXrjCCr7d2Qysbv_PTG1YGLgPKNBJcgCJLIqdeMFNKg0' }
})
defineEmits(['click'])
</script>