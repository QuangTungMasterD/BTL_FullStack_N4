<template>
  <div class="contact-page">
    <!-- Hero section -->
    <section class="relative py-20 bg-gradient-to-br from-primary/5 via-purple-50 to-white overflow-hidden">
      <div class="absolute inset-0 opacity-10">
        <div class="absolute top-0 right-0 w-96 h-96 bg-primary rounded-full mix-blend-multiply filter blur-3xl"></div>
        <div class="absolute bottom-0 left-0 w-96 h-96 bg-purple-400 rounded-full mix-blend-multiply filter blur-3xl"></div>
      </div>
      <div class="container mx-auto px-4 sm:px-6 lg:px-8 relative z-10">
        <div class="max-w-3xl mx-auto text-center">
          <div class="inline-flex items-center gap-2 bg-primary/10 text-primary px-4 py-2 rounded-full text-sm font-semibold mb-4">
            <span class="material-symbols-outlined text-sm">support_agent</span>
            Liên hệ
          </div>
          <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-4">Liên hệ với chúng tôi</h1>
          <p class="text-lg text-gray-600">Chúng tôi sẵn sàng giải đáp mọi thắc mắc của bạn. Hãy để lại tin nhắn, chúng tôi sẽ phản hồi trong vòng 24 giờ.</p>
        </div>
      </div>
    </section>

    <!-- Contact info & form -->
    <section class="py-16 container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="grid md:grid-cols-2 gap-12 items-start">
        <!-- Thông tin liên hệ -->
        <div class="space-y-8">
          <div>
            <h2 class="text-2xl font-bold text-gray-900 mb-6 items-center gap-2 flex justify-center">
              <span class="material-symbols-outlined text-primary">contact_support</span>
              Thông tin liên hệ
            </h2>
            <div class="space-y-6">
              <div class="flex items-center gap-4 p-4 rounded-xl bg-white shadow-sm hover:shadow-md transition-shadow">
                <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                  <span class="material-symbols-outlined">location_on</span>
                </div>
                <div class="flex flex-1 items-center justify-between">
                  <p class="font-semibold text-gray-900">Địa chỉ</p>
                  <p class="text-gray-600">123 Đường ABC, Quận 1, TP. Hồ Chí Minh</p>
                </div>
              </div>
              <div class="flex items-center gap-4 p-4 rounded-xl bg-white shadow-sm hover:shadow-md transition-shadow">
                <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                  <span class="material-symbols-outlined">phone</span>
                </div>
                <div class="flex flex-1 items-center justify-between">
                  <p class="font-semibold text-gray-900">Điện thoại</p>
                  <p class="text-gray-600">(+84) 325930536</p>
                </div>
              </div>
              <div class="flex items-center gap-4 p-4 rounded-xl bg-white shadow-sm hover:shadow-md transition-shadow">
                <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                  <span class="material-symbols-outlined">email</span>
                </div>
                <div class="flex flex-1 items-center justify-between">
                  <p class="font-semibold text-gray-900">Email</p>
                  <p class="text-gray-600">tranquangtung26062005@gmail.com</p>
                </div>
              </div>
              <div class="flex items-center gap-4 p-4 rounded-xl bg-white shadow-sm hover:shadow-md transition-shadow">
                <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center text-primary flex-shrink-0">
                  <span class="material-symbols-outlined">schedule</span>
                </div>
                <div class="flex flex-1 items-center justify-between">
                  <p class="font-semibold text-gray-900">Giờ làm việc</p>
                  <p class="text-gray-600">Thứ 2 - Thứ 6: 8:00 - 17:30</p>
                  <p class="text-gray-600">Thứ 7: 8:00 - 12:00</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Bản đồ (placeholder) -->
          <div class="rounded-xl overflow-hidden shadow-md bg-gray-200 h-64 flex items-center justify-center text-gray-500 border border-gray-200">
            <div class="text-center">
              <span class="material-symbols-outlined text-4xl text-gray-400">map</span>
              <p class="mt-2">Bản đồ Google Maps</p>
              <p class="text-sm">(Đang cập nhật)</p>
            </div>
          </div>
        </div>

        <!-- Form liên hệ -->
        <div class="bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
          <h2 class="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-2 justify-center">
            <span class="material-symbols-outlined text-primary">chat</span>
            Gửi tin nhắn
          </h2>
          <form @submit.prevent="handleSubmit" class="space-y-5">
            <div>
              <label class="flex justify-start ml-4 mb-2 text-sm font-medium text-gray-700 mb-1">Họ tên <span class="text-red-500">*</span></label>
              <div class="relative">
                <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">person</span>
                <input
                  type="text"
                  v-model="form.name"
                  required
                  placeholder="Nguyễn Văn A"
                  class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent transition-all"
                />
              </div>
            </div>
            <div>
              <label class="flex justify-start ml-4 mb-2 text-sm font-medium text-gray-700 mb-1">Email <span class="text-red-500">*</span></label>
              <div class="relative">
                <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">mail</span>
                <input
                  type="email"
                  v-model="form.email"
                  required
                  placeholder="example@email.com"
                  class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent transition-all"
                />
              </div>
            </div>
            <div>
              <label class="flex justify-start ml-4 mb-2 text-sm font-medium text-gray-700 mb-1">Số điện thoại</label>
              <div class="relative">
                <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">phone</span>
                <input
                  type="tel"
                  v-model="form.phone"
                  placeholder="0987654321"
                  class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent transition-all"
                />
              </div>
            </div>
            <div>
              <label class="flex justify-start ml-4 mb-2 text-sm font-medium text-gray-700 mb-1">Nội dung <span class="text-red-500">*</span></label>
              <div class="relative">
                <span class="material-symbols-outlined absolute left-3 top-3 text-gray-400">message</span>
                <textarea
                  v-model="form.message"
                  rows="4"
                  required
                  placeholder="Nhập nội dung tin nhắn..."
                  class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent transition-all resize-none"
                ></textarea>
              </div>
            </div>
            <button
              type="submit"
              class="w-full py-3.5 bg-primary text-white font-semibold rounded-xl hover:bg-primary/90 transition-all shadow-md hover:shadow-lg flex items-center justify-center gap-2"
            >
              <span class="material-symbols-outlined">send</span>
              Gửi tin nhắn
            </button>
          </form>
          <p v-if="submitSuccess" class="mt-4 text-green-600 text-center flex items-center justify-center gap-2">
            <span class="material-symbols-outlined">check_circle</span>
            Tin nhắn đã được gửi thành công!
          </p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue';
// import { useToast } from '@/composables/useToast'; // nếu có toast

// const toast = useToast();

const form = ref({
  name: '',
  email: '',
  phone: '',
  message: '',
});

const submitSuccess = ref(false);

const handleSubmit = () => {
  // Thực tế sẽ gọi API gửi mail
  console.log('Form data:', form.value);
  submitSuccess.value = true;
  // Reset form sau 3s
  setTimeout(() => {
    form.value = { name: '', email: '', phone: '', message: '' };
    submitSuccess.value = false;
  }, 3000);
  // toast.success('Gửi tin nhắn thành công!');
};
</script>