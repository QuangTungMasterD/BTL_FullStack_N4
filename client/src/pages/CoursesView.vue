<template>
  <div class="courses-page">
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-4xl font-bold text-gray-900 text-center">Tất cả khóa học</h1>
        <p class="mt-4 text-center text-gray-600">Khám phá các khóa học đa dạng của chúng tôi</p>
      </div>
    </section>

    <section class="py-12 container mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Bộ lọc -->
      <div class="flex flex-wrap gap-4 items-center justify-between mb-8">
        <div class="flex flex-wrap gap-3">
          <input
            v-model="filters.search"
            type="text"
            placeholder="Tìm khóa học..."
            class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent w-64"
            @input="loadCourses"
          />
          <select v-model="filters.level" class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary" @change="loadCourses">
            <option value="">Tất cả trình độ</option>
            <option value="1">Sơ cấp</option>
            <option value="2">Căn bản</option>
            <option value="3">Trung cấp</option>
            <option value="4">Cao cấp</option>
            <option value="5">Chuyên gia</option>
          </select>
        </div>
        <div class="text-sm text-gray-500">{{ totalCourses }} khóa học</div>
      </div>

      <!-- Danh sách khóa học -->
      <div v-if="loading" class="flex justify-center py-12">
        <div class="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
      </div>
      <div v-else-if="error" class="text-center py-12 text-red-500">
        {{ error }}
      </div>
      <div v-else-if="courses.length === 0" class="text-center py-12 text-gray-500">
        Không tìm thấy khóa học nào.
      </div>
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <div v-for="course in courses" :key="course.id" class="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-xl transition-all hover:-translate-y-1">
          <!-- Phần ảnh -->
          <div class="h-40 bg-gradient-to-br from-primary/20 to-purple-500/20 flex items-center justify-center relative overflow-hidden">
            <img 
              v-if="course.imageUrl" 
              :src="course.imageUrl" 
              alt="Course thumbnail" 
              loading="lazy"
              class="w-full h-full object-cover transition-opacity" 
              @error="(e) => { e.target.style.display = 'none'; e.target.parentElement.querySelector('.fallback-icon').style.display = 'flex'; }"
            />
            <span 
              class="material-symbols-outlined text-6xl text-primary/40 fallback-icon"
              :class="{ 'flex': !course.imageUrl, 'hidden': course.imageUrl }"
            >
              menu_book
            </span>
          </div>
          <!-- Nội dung -->
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

      <!-- Phân trang -->
      <div v-if="totalPages > 1" class="mt-10 flex justify-center">
        <button
          v-for="p in totalPages"
          :key="p"
          @click="currentPage = p; loadCourses()"
          class="mx-1 px-4 py-2 rounded-lg border border-gray-300 hover:bg-primary hover:text-white transition"
          :class="{ 'bg-primary text-white': currentPage === p }"
        >
          {{ p }}
        </button>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useCourseStore } from '@/stores';
import { formatVND, courseLevelText } from '@/composables/useFormat';
import { useToast } from '@/composables/useToast';

const toast = useToast();
const courseStore = useCourseStore();
const loading = ref(true);
const error = ref(null);
const courses = ref([]);
const totalCourses = ref(0);
const currentPage = ref(1);
const pageSize = 6;

const filters = ref({
  search: '',
  level: '',
});

const totalPages = computed(() => Math.ceil(totalCourses.value / pageSize));

const loadCourses = async () => {
  loading.value = true;
  error.value = null;
  try {
    const params = {
      page: currentPage.value,
      pageSize: pageSize,
      search: filters.value.search || undefined,
      level: filters.value.level || undefined,
      isActive: true,
    };
    await courseStore.fetchPaged(params);
    courses.value = courseStore.pagedData.data || [];
    totalCourses.value = courseStore.pagedData.totalRecords || 0;
  } catch (err) {
    error.value = 'Không thể tải danh sách khóa học. Vui lòng thử lại sau.';
    toast.error(error.value);
    courses.value = [];
    totalCourses.value = 0;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadCourses();
});
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>