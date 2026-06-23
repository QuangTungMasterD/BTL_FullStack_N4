<template>
  <div class="home-page">
    <!-- Hero Section -->
    <section class="relative bg-gradient-to-br from-blue-600 via-purple-700 to-indigo-800 text-white overflow-hidden">
      <div class="absolute inset-0 opacity-20">
        <div class="absolute top-10 left-10 w-72 h-72 bg-white rounded-full mix-blend-overlay animate-pulse"></div>
        <div class="absolute bottom-10 right-10 w-96 h-96 bg-purple-400 rounded-full mix-blend-overlay animate-pulse delay-1000"></div>
      </div>
      <div class="container mx-auto px-4 sm:px-6 lg:px-8 py-20 md:py-28 relative z-10">
        <div class="max-w-3xl">
          <h1 class="text-4xl sm:text-5xl md:text-6xl font-extrabold leading-tight">
            Nền tảng quản lý <br />
            <span class="text-yellow-300">giáo dục toàn diện</span>
          </h1>
          <p class="mt-6 text-lg md:text-xl text-white/90 leading-relaxed">
            EduTrack giúp bạn dễ dàng quản lý điểm danh, đánh giá kết quả học tập,
            và theo dõi tiến độ của hàng nghìn học viên.
          </p>
          <div class="mt-8 flex flex-wrap gap-4">
            <router-link to="/courses" class="px-8 py-4 bg-white text-primary font-semibold rounded-xl shadow-lg hover:shadow-xl hover:scale-105 transition-all">
              Khám phá khóa học
            </router-link>
            <router-link to="/contact" class="px-8 py-4 bg-transparent border-2 border-white text-white font-semibold rounded-xl hover:bg-white/10 transition-all">
              Liên hệ ngay
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- Thống kê nổi bật (mock vì chưa có API) -->
    <section class="bg-gray-50 py-12 border-y border-gray-200">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-2 md:grid-cols-3 gap-6 text-center">
          <div>
            <div class="text-4xl font-bold text-primary">{{ stats.students }}</div>
            <div class="text-sm text-gray-600 mt-1">Học viên</div>
          </div>
          <div>
            <div class="text-4xl font-bold text-primary">{{ stats.courses }}</div>
            <div class="text-sm text-gray-600 mt-1">Khóa học</div>
          </div>
          <div>
            <div class="text-4xl font-bold text-primary">{{ stats.teachers }}</div>
            <div class="text-sm text-gray-600 mt-1">Giảng viên</div>
          </div>
          <!-- <div>
            <div class="text-4xl font-bold text-primary">{{ stats.satisfaction }}%</div>
            <div class="text-sm text-gray-600 mt-1">Hài lòng</div>
          </div> -->
        </div>
      </div>
    </section>

    <!-- Giới thiệu ngắn -->
    <section class="py-16 container mx-auto px-4 sm:px-6 lg:px-8">
      <div class="grid md:grid-cols-2 gap-12 items-center">
        <div>
          <h2 class="text-3xl font-bold text-gray-900">Tại sao chọn <span class="text-primary">EduTrack</span>?</h2>
          <p class="mt-4 text-gray-600 leading-relaxed">
            Chúng tôi cung cấp giải pháp quản lý trung tâm dạy học thông minh, giúp tiết kiệm thời gian và nâng cao hiệu quả giảng dạy.
          </p>
          <ul class="mt-6 space-y-3">
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">check_circle</span>
              <span class="text-gray-700">Điểm danh thông minh với công nghệ AI</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">chart_bar</span>
              <span class="text-gray-700">Báo cáo chi tiết và trực quan</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">security</span>
              <span class="text-gray-700">Bảo mật dữ liệu cao cấp</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="material-symbols-outlined text-primary">devices</span>
              <span class="text-gray-700">Hỗ trợ đa nền tảng, dễ dàng sử dụng</span>
            </li>
          </ul>
        </div>
        <div class="bg-gray-100 rounded-2xl p-8 text-center">
          <span class="material-symbols-outlined text-8xl text-primary/30">school</span>
          <p class="mt-4 text-gray-500 italic">“Giải pháp quản lý giáo dục hàng đầu”</p>
        </div>
      </div>
    </section>

    <!-- Khóa học nổi bật -->
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center mb-10">
          <div>
            <h2 class="text-3xl font-bold text-gray-900">Khóa học nổi bật</h2>
            <p class="text-gray-600 mt-1">Những khóa học được yêu thích nhất</p>
          </div>
          <router-link to="/courses" class="text-primary font-semibold hover:underline flex items-center gap-1">
            Xem tất cả <span class="material-symbols-outlined text-sm">arrow_forward</span>
          </router-link>
        </div>

        <div v-if="loadingCourses" class="flex justify-center py-12">
          <div class="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
        </div>
        <div v-else-if="error" class="text-center py-12 text-red-500">
          {{ error }}
        </div>
        <div v-else-if="featuredCourses.length === 0" class="text-center py-12 text-gray-500">
          Chưa có khóa học nổi bật.
        </div>
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <div v-for="course in featuredCourses" :key="course.id" class="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-xl transition-all hover:-translate-y-1">
            <div class="h-40 bg-gradient-to-br from-primary/20 to-purple-500/20 flex items-center justify-center">
              <span class="material-symbols-outlined text-6xl text-primary/40">menu_book</span>
            </div>
            <div class="p-5">
              <h3 class="text-lg font-bold text-gray-900">{{ course.courseName }}</h3>
              <p class="mt-1 text-sm text-gray-500 line-clamp-2">{{ course.desct || 'Mô tả đang cập nhật' }}</p>
              <div class="mt-4 flex items-center justify-between">
                <span class="text-primary font-bold text-lg">{{ formatVND(course.tuitionFee) }}</span>
                <span class="text-sm bg-green-100 text-green-700 px-3 py-1 rounded-full">{{ courseLevelText(course.level) }}</span>
              </div>
              <router-link :to="`/courses/${course.id}`" class="mt-4 block w-full text-center px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary/90 transition">
                Xem chi tiết
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Đánh giá học viên (mock) -->
    <!-- <section class="py-16 container mx-auto px-4 sm:px-6 lg:px-8">
      <h2 class="text-3xl font-bold text-gray-900 text-center">Học viên nói gì về chúng tôi</h2>
      <div class="mt-10 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div v-for="(review, idx) in testimonials" :key="idx" class="bg-gray-50 rounded-xl p-6 shadow-sm hover:shadow-md transition">
          <div class="flex items-center gap-1 text-yellow-400 text-sm">
            <span v-for="i in 5" :key="i" class="material-symbols-outlined text-base">star</span>
          </div>
          <p class="mt-3 text-gray-700 italic">“{{ review.text }}”</p>
          <div class="mt-4 flex items-center gap-3">
            <div class="w-10 h-10 rounded-full bg-primary/20 flex items-center justify-center text-primary font-semibold">
              {{ review.name.charAt(0) }}
            </div>
            <div>
              <div class="font-semibold text-gray-900">{{ review.name }}</div>
              <div class="text-xs text-gray-500">{{ review.role }}</div>
            </div>
          </div>
        </div>
      </div>
    </section> -->

    <!-- Call to Action -->
    <section class="bg-primary py-16">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8 text-center text-white">
        <h2 class="text-3xl font-bold">Sẵn sàng để bắt đầu?</h2>
        <p class="mt-3 text-white/80 text-lg">Đăng ký ngay để trải nghiệm nền tảng quản lý giáo dục tốt nhất.</p>
        <router-link to="/login?tab=register" class="mt-6 inline-block px-8 py-4 bg-white text-primary font-bold rounded-xl shadow-lg hover:shadow-xl hover:scale-105 transition-all">
          Đăng ký miễn phí
        </router-link>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useCourseStore, useTeacherStore } from '@/stores';
import { formatVND, courseLevelText } from '@/composables/useFormat';
import { useToast } from '@/composables/useToast';

const toast = useToast();
const courseStore = useCourseStore();
const teacherStore = useTeacherStore();

const loadingCourses = ref(true);
const error = ref(null);
const featuredCourses = ref([]);

// Dữ liệu thống kê từ API
const totalCourses = computed(() => courseStore.pagedData.totalRecords || 0);
const totalTeachers = computed(() => teacherStore.pagedData.totalRecords || 0);
// Tạm thời mock students và satisfaction vì chưa có API
const totalStudents = ref(1248);
const satisfaction = ref(98);

const stats = computed(() => ({
  students: totalStudents.value,
  courses: totalCourses.value,
  teachers: totalTeachers.value,
  satisfaction: satisfaction.value,
}));

// Đánh giá (mock)
const testimonials = [
  {
    name: 'Nguyễn Văn A',
    role: 'Học viên Python',
    text: 'EduTrack giúp tôi theo dõi tiến độ học tập và điểm danh rất dễ dàng. Giao diện thân thiện, tính năng đầy đủ.',
  },
  {
    name: 'Trần Thị B',
    role: 'Giảng viên Java',
    text: 'Tôi đã sử dụng nhiều hệ thống, nhưng EduTrack là tốt nhất. Quản lý lớp học và điểm số chưa bao giờ đơn giản đến thế.',
  },
  {
    name: 'Lê Văn C',
    role: 'Học viên Tiếng Anh',
    text: 'Các báo cáo chi tiết giúp tôi hiểu rõ điểm mạnh, điểm yếu của mình. Rất hài lòng!',
  },
];

const loadFeaturedCourses = async () => {
  loadingCourses.value = true;
  error.value = null;
  try {
    await courseStore.fetchPaged({ page: 1, pageSize: 6, isActive: true });
    featuredCourses.value = courseStore.pagedData.data.slice(0, 3);
  } catch (err) {
    error.value = 'Không thể tải danh sách khóa học nổi bật.';
    toast.error(error.value);
    featuredCourses.value = [];
  } finally {
    loadingCourses.value = false;
  }
};

const loadStats = async () => {
  try {
    // Lấy tổng số giáo viên
    await teacherStore.fetchPaged({ page: 1, pageSize: 1 });
    // Lấy tổng số khóa học (nếu chưa có, có thể gọi fetchPaged với pageSize=1)
    await courseStore.fetchPaged({ page: 1, pageSize: 1, isActive: true });
  } catch (err) {
    toast.error('Không thể tải thống kê');
  }
};

onMounted(async () => {
  await loadStats();
  await loadFeaturedCourses();
});
</script>