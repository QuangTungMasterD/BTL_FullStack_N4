<template>
  <div class="about-page">
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-bold text-gray-900 text-center">Về chúng tôi</h1>
        <div class="mt-6 max-w-3xl mx-auto text-center text-gray-600 leading-relaxed">
          <p>EduTrack là trung tâm đào tạo công nghệ và kỹ năng hàng đầu, được thành lập với sứ mệnh mang đến những chương trình đào tạo chất lượng cao, sát với thực tiễn. Với đội ngũ giảng viên giàu kinh nghiệm, chúng tôi cam kết đồng hành cùng học viên trên con đường phát triển sự nghiệp.</p>
        </div>
      </div>
    </section>

    <section class="py-16 container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="grid md:grid-cols-2 gap-12 items-center">
        <div>
          <h2 class="text-3xl font-bold text-gray-900">Sứ mệnh của chúng tôi</h2>
          <p class="mt-4 text-gray-600 leading-relaxed">
            Chúng tôi cam kết đào tạo thế hệ trẻ Việt Nam có kiến thức vững vàng, kỹ năng thực tiễn và tư duy sáng tạo để sẵn sàng hội nhập quốc tế.
          </p>
          <ul class="mt-6 space-y-3">
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">lightbulb</span>
              <span class="text-gray-700">Chất lượng đào tạo hàng đầu</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">people</span>
              <span class="text-gray-700">Phương pháp giảng dạy hiện đại</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">verified</span>
              <span class="text-gray-700">Đồng hành cùng sự phát triển</span>
            </li>
          </ul>
        </div>
        <div class="bg-white rounded-xl shadow-lg p-8 border border-gray-100">
          <div class="space-y-4">
            <div class="flex items-center gap-4">
              <div class="min-w-[72px] h-16 bg-primary/10 rounded-full flex items-center justify-center text-primary">
                <span class="material-symbols-outlined text-3xl">visibility</span>
              </div>
              <div class="text-center">
                <h4 class="font-bold text-gray-900">Tầm nhìn</h4>
                <p class="text-sm text-gray-600">Trở thành trung tâm đào tạo uy tín hàng đầu tại Việt Nam, được biết đến với chất lượng giảng dạy và sự hài lòng của học viên.</p>
              </div>
            </div>
            <div class="flex items-center gap-4">
              <div class="min-w-[72px] h-16 bg-purple-100 rounded-full flex items-center justify-center text-purple-600">
                <span class="material-symbols-outlined text-3xl">flag</span>
              </div>
              <div class="text-center w-[100%]">
                <h4 class="font-bold text-gray-900">Giá trị cốt lõi</h4>
                <p class="text-sm text-gray-600">Tâm huyết, Sáng tạo, Chất lượng, Trách nhiệm</p>
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