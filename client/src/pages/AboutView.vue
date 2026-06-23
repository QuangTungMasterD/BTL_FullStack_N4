<template>
  <div class="about-page">
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-bold text-gray-900 text-center">Về chúng tôi</h1>
        <div class="mt-6 max-w-3xl mx-auto text-center text-gray-600 leading-relaxed">
          <p>EduTrack là nền tảng quản lý trung tâm dạy học hàng đầu, được phát triển bởi đội ngũ chuyên gia công nghệ giáo dục với hơn 10 năm kinh nghiệm.</p>
        </div>
      </div>
    </section>

    <section class="py-16 container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="grid md:grid-cols-2 gap-12 items-center">
        <div>
          <h2 class="text-3xl font-bold text-gray-900">Sứ mệnh của chúng tôi</h2>
          <p class="mt-4 text-gray-600 leading-relaxed">
            Chúng tôi cam kết mang đến giải pháp quản lý giáo dục hiệu quả, giúp các trung tâm đào tạo tối ưu hóa quy trình vận hành và nâng cao trải nghiệm học tập của học viên.
          </p>
          <ul class="mt-6 space-y-3">
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">lightbulb</span>
              <span class="text-gray-700">Đổi mới sáng tạo trong giáo dục</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">people</span>
              <span class="text-gray-700">Đặt người dùng làm trung tâm</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">verified</span>
              <span class="text-gray-700">Chất lượng và độ tin cậy hàng đầu</span>
            </li>
          </ul>
        </div>
        <div class="bg-white rounded-xl shadow-lg p-8 border border-gray-100">
          <div class="space-y-4">
            <div class="flex items-center gap-4">
              <div class="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center text-primary">
                <span class="material-symbols-outlined text-3xl">visibility</span>
              </div>
              <div>
                <h4 class="font-bold text-gray-900">Tầm nhìn</h4>
                <p class="text-sm text-gray-600">Trở thành nền tảng quản lý giáo dục số 1 tại Việt Nam và khu vực.</p>
              </div>
            </div>
            <div class="flex items-center gap-4">
              <div class="w-16 h-16 bg-purple-100 rounded-full flex items-center justify-center text-purple-600">
                <span class="material-symbols-outlined text-3xl">flag</span>
              </div>
              <div>
                <h4 class="font-bold text-gray-900">Giá trị cốt lõi</h4>
                <p class="text-sm text-gray-600">Trung thực, sáng tạo, hợp tác, và hướng đến khách hàng.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-3xl font-bold text-gray-900 text-center">Đội ngũ lãnh đạo</h2>
        <div class="mt-10 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
          <div v-for="member in team" :key="member.name" class="bg-white rounded-xl shadow-md p-6 text-center hover:shadow-lg transition">
            <div class="w-24 h-24 mx-auto rounded-full bg-primary/10 flex items-center justify-center text-3xl text-primary font-bold">
              {{ member.name.charAt(0) }}
            </div>
            <h4 class="mt-4 font-bold text-gray-900">{{ member.name }}</h4>
            <p class="text-sm text-gray-500">{{ member.role }}</p>
          </div>
        </div>
      </div>
    </section> -->
  </div>
</template>

<script setup>
const team = [
  { name: 'Nguyễn Văn X', role: 'Giám đốc điều hành' },
  { name: 'Trần Thị Y', role: 'Trưởng phòng đào tạo' },
  { name: 'Lê Văn Z', role: 'Trưởng phòng công nghệ' },
];
</script>