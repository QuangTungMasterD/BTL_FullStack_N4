<template>
  <header class="sticky top-0 z-50 bg-white/95 backdrop-blur-md border-b border-gray-100 shadow-sm">
    <div class="container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Logo -->
        <router-link to="/" class="flex items-center gap-2 text-xl font-bold text-primary">
          <div class="w-8 h-8 bg-gradient-to-br from-primary to-purple-600 rounded-lg flex items-center justify-center text-white">
            <span class="material-symbols-outlined text-2xl">school</span>
          </div>
          <span>EduTrack</span>
        </router-link>

        <!-- Menu chính (Desktop) -->
        <nav class="hidden md:flex items-center gap-1 text-sm font-medium text-gray-700">
          <router-link to="/" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Trang chủ</router-link>
          <router-link to="/about" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Giới thiệu</router-link>
          <router-link to="/courses" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Khóa học</router-link>
          <router-link to="/contact" exact-active-class="text-primary bg-gray-100" class="hover:text-primary px-5 py-3 rounded-md transition-colors">Liên hệ</router-link>
        </nav>

        <!-- Hành động: Đăng nhập / Đăng ký -->
        <div class="flex items-center gap-3">
          <router-link to="/login" class="hidden sm:inline-block text-sm font-medium text-gray-700 hover:text-primary transition-colors">
            Đăng nhập
          </router-link>
          <router-link to="/login?tab=register" class="px-4 py-2 bg-primary text-white text-sm font-semibold rounded-lg hover:bg-primary/90 transition-all shadow-md hover:shadow-lg">
            Đăng ký
          </router-link>
          <!-- Nút menu mobile -->
          <button @click="toggleMobileMenu" class="md:hidden p-2 rounded-lg hover:bg-gray-100">
            <span class="material-symbols-outlined">menu</span>
          </button>
        </div>
      </div>

      <!-- Menu mobile (dropdown) -->
      <div v-if="mobileMenuOpen" class="md:hidden py-4 border-t border-gray-100">
        <nav class="flex flex-col gap-3 text-base font-medium text-gray-700">
          <router-link to="/" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Trang chủ</router-link>
          <router-link to="/about" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Giới thiệu</router-link>
          <router-link to="/courses" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Khóa học</router-link>
          <router-link to="/contact" class="px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Liên hệ</router-link>
          <div class="border-t border-gray-100 my-2 pt-2">
            <router-link to="/login" class="block px-2 py-2 hover:bg-gray-50 rounded-lg" @click="mobileMenuOpen = false">Đăng nhập</router-link>
          </div>
        </nav>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue';

const mobileMenuOpen = ref(false);

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value;
};
</script>