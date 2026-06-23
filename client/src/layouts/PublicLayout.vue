<template>
  <div class="public-layout min-h-screen flex flex-col bg-white">
    <!-- Header công khai -->
    <PublicHeader />

    <!-- Nội dung trang -->
    <main class="flex-1">
      <router-view />
    </main>

    <!-- Footer -->
    <Footer />
  </div>
</template>

<script setup>
import PublicHeader from '@/layouts/Header/PublicHeader.vue';
import Footer from '@/layouts/Footer/Footer.vue';
</script>

<style scoped>
.public-layout {
  font-family: 'Inter', system-ui, -apple-system, sans-serif;
}
</style>