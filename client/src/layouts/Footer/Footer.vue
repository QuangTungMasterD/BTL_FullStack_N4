<template>
  <footer class="bg-gray-900 text-gray-300">
    <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
        <!-- Cột 1: Logo & mô tả -->
        <div>
          <div class="flex text-start items-start gap-2 text-xl font-bold text-white mb-4">
            <div class="w-8 h-8 bg-gradient-to-br from-primary to-purple-600 rounded-lg flex items-center justify-center text-white">
              <span class="material-symbols-outlined text-2xl">school</span>
            </div>
            <span>EduTrack</span>
          </div>
          <p class="text-sm leading-relaxed">
            Hệ thống quản lý trung tâm dạy học hàng đầu, giúp theo dõi điểm danh, đánh giá kết quả và quản lý khóa học hiệu quả.
          </p>
          <div class="flex gap-4 mt-4">
            <a href="#" class="text-gray-400 hover:text-white transition-colors">
              <span class="material-symbols-outlined">facebook</span>
            </a>
            <a href="#" class="text-gray-400 hover:text-white transition-colors">
              <span class="material-symbols-outlined">youtube</span>
            </a>
            <a href="#" class="text-gray-400 hover:text-white transition-colors">
              <span class="material-symbols-outlined">instagram</span>
            </a>
          </div>
        </div>

        <!-- Cột 2: Liên kết nhanh -->
        <div class="text-start">
          <h4 class="text-white font-semibold mb-4">Liên kết</h4>
          <ul class="space-y-2 text-sm">
            <li><router-link to="/about" class="hover:text-white transition-colors">Giới thiệu</router-link></li>
            <li><router-link to="/courses" class="hover:text-white transition-colors">Khóa học</router-link></li>
            <li><router-link to="/contact" class="hover:text-white transition-colors">Liên hệ</router-link></li>
            <li><router-link to="/login" class="hover:text-white transition-colors">Đăng nhập</router-link></li>
          </ul>
        </div>

        <!-- Cột 3: Thông tin liên hệ -->
        <div class="text-start">
          <h4 class="text-white font-semibold mb-4">Liên hệ</h4>
          <ul class="space-y-2 text-sm">
            <li class="flex items-start gap-2">
              <span class="material-symbols-outlined text-sm">location_on</span>
              <span>123 Đường ABC, Quận 1, TP. Hồ Chí Minh</span>
            </li>
            <li class="flex items-center gap-2">
              <span class="material-symbols-outlined text-sm">phone</span>
              <span>(+84) 325930536</span>
            </li>
            <li class="flex items-center gap-2">
              <span class="material-symbols-outlined text-sm">email</span>
              <span>tranquangtung26062005@gmail.com</span>
            </li>
          </ul>
        </div>

        <!-- Cột 4: Đăng ký nhận tin -->
        <!-- <div>
          <h4 class="text-white font-semibold mb-4">Đăng ký nhận tin</h4>
          <p class="text-sm mb-3">Nhận thông tin về các khóa học mới và sự kiện.</p>
          <form @submit.prevent="handleSubscribe" class="flex flex-col sm:flex-row gap-2">
            <input
              type="email"
              v-model="emailSubscribe"
              placeholder="Email của bạn"
              class="flex-1 px-4 py-2 rounded-lg bg-gray-800 border border-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-primary"
              required
            />
            <button type="submit" class="px-4 py-2 bg-primary text-white rounded-lg font-semibold hover:bg-primary/90 transition-all">
              Đăng ký
            </button>
          </form>
        </div> -->
      </div>

      <!-- Bản quyền -->
      <div class="border-t border-gray-800 mt-10 pt-6 text-center text-sm text-gray-500">
        &copy; {{ new Date().getFullYear() }} EduTrack. Tất cả quyền được bảo lưu.
      </div>
    </div>
  </footer>
</template>

<script setup>
import { ref } from 'vue';

const emailSubscribe = ref('');

const handleSubscribe = () => {
  if (emailSubscribe.value) {
    alert(`Cảm ơn bạn đã đăng ký! Chúng tôi sẽ gửi tin đến ${emailSubscribe.value}`);
    emailSubscribe.value = '';
  }
};
</script>