<template>
  <div>
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-8">
      <div>
        <h1 class="font-headline-md text-headline-md text-on-surface">Lịch học của tôi</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Theo dõi lịch học các lớp bạn đang tham gia</p>
      </div>
      <div class="flex items-center gap-4">
        <div class="bg-surface-container-high p-1 rounded-lg flex items-center">
          <button
            @click="viewMode = 'week'"
            :class="['px-4 py-2 rounded-md font-title-md transition-all', viewMode === 'week' ? 'bg-surface-container-lowest shadow-sm text-primary' : 'text-on-surface-variant hover:text-on-surface']"
          >Tuần</button>
          <button
            @click="viewMode = 'month'"
            :class="['px-4 py-2 rounded-md font-title-md transition-all', viewMode === 'month' ? 'bg-surface-container-lowest shadow-sm text-primary' : 'text-on-surface-variant hover:text-on-surface']"
          >Tháng</button>
        </div>
        <div class="flex items-center bg-surface-container-lowest border border-outline-variant rounded-lg overflow-hidden">
          <button class="p-2 hover:bg-surface-container-high border-r border-outline-variant" @click="navigate(-1)">
            <span class="material-symbols-outlined">chevron_left</span>
          </button>
          <div class="px-4 font-title-md text-on-surface min-w-[200px] text-center">{{ dateRangeText }}</div>
          <button class="p-2 hover:bg-surface-container-high" @click="navigate(1)">
            <span class="material-symbols-outlined">chevron_right</span>
          </button>
        </div>
      </div>
    </div>

    <LoadingSpinner v-if="loading" />
    <div v-else>
      <ScheduleCalendar
        v-model:view-mode="viewMode"
        :sessions="sessions"
        :week-days="weekDays"
        :month-days="monthDays"
        :hours="hours"
        :current-date="currentDate"
        @session-click="openSessionDetail"
      />
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 gap-6">
        <StatCard label="Tổng số tiết học" :value="totalSessions" trend="Trong tháng" />
        <StatCard label="Số lớp đang học" :value="uniqueClassesCount" />
      </div>
    </div>

    <Modal v-model="showDetailModal" title="Chi tiết buổi học">
      <div v-if="selectedSession" class="space-y-4">
        <div class="flex items-start gap-3">
          <span class="material-symbols-outlined text-primary">menu_book</span>
          <div>
            <p class="font-label-md text-on-surface-variant">Lớp</p>
            <p class="font-title-md text-on-surface">{{ selectedSession.className }}</p>
          </div>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <p class="font-label-md text-on-surface-variant">Phòng</p>
            <p class="font-body-md">{{ selectedSession.roomName || 'Chưa xác định' }}</p>
          </div>
          <div>
            <p class="font-label-md text-on-surface-variant">Giáo viên</p>
            <p class="font-body-md">{{ selectedSession.teacherName || 'N/A' }}</p>
          </div>
        </div>
        <div>
          <p class="font-label-md text-on-surface-variant">Thời gian</p>
          <p class="font-body-md">{{ formatDateTime(selectedSession.startTime) }} – {{ formatDateTime(selectedSession.endTime) }}</p>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
// import { useAuthStore } from '@/stores/authStore'
import { useClassSessionStore, useClassStore, useRoomStore, useTeacherStore } from '@/stores'
import { formatDateTime } from '@/composables/useFormat'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import Modal from '@/components/ui/Modal.vue'
import StatCard from '@/components/ui/StatCard.vue'
import ScheduleCalendar from '@/components/business/ScheduleCalendar.vue'

// const authStore = useAuthStore()
const classSessionStore = useClassSessionStore()
const classStore = useClassStore()
const roomStore = useRoomStore()
const teacherStore = useTeacherStore()

const loading = ref(false)
const sessions = ref([])
const viewMode = ref('week')
const currentDate = ref(new Date())
const showDetailModal = ref(false)
const selectedSession = ref(null)

// Lấy danh sách classId mà học sinh tham gia (cần API riêng, tạm thời mock)
const studentClassIds = ref([]) // giả sử lấy từ API student/enrollments

const loadSchedule = async () => {
  loading.value = true
  try {
    // Nếu chưa có API lấy theo studentId, phải lấy tất cả classSession của các class của student
    if (studentClassIds.value.length === 0) return
    const params = {
      classIds: studentClassIds.value.join(','),
      pageSize: 100
    }
    // Thêm filter thời gian
    if (viewMode.value === 'week') {
      const start = weekDays.value[0]?.fullDate
      const end = weekDays.value[6]?.fullDate
      if (start && end) {
        params.startTimeFrom = start.toISOString()
        params.startTimeTo = end.toISOString()
      }
    } else {
      const firstDay = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth(), 1)
      const lastDay = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 0)
      params.startTimeFrom = firstDay.toISOString()
      params.startTimeTo = lastDay.toISOString()
    }
    await classSessionStore.fetchPaged(params)
    let data = classSessionStore.pagedData.data
    data = data.map(s => ({
      ...s,
      className: classStore.classes.find(c => c.id === s.classId)?.className || 'N/A',
      roomName: roomStore.rooms.find(r => r.id === s.roomId)?.roomName || 'N/A',
      teacherName: teacherStore.teachers.find(t => t.id === s.teacherId)?.fullName || 'N/A'
    }))
    sessions.value = data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const totalSessions = computed(() => sessions.value.length)
const uniqueClassesCount = computed(() => new Set(sessions.value.map(s => s.classId)).size)

// Các helper cho lịch (giống teacher)
const weekDays = computed(() => [])
const monthDays = computed(() => [])
const hours = computed(() => [])
const dateRangeText = computed(() => '')

const navigate = (dir) => {}
const openSessionDetail = (session) => {
  selectedSession.value = session
  showDetailModal.value = true
}

onMounted(async () => {
  await Promise.all([classStore.fetchAll(), roomStore.fetchAll(), teacherStore.fetchAll()])
  // Giả sử lấy danh sách lớp của học sinh từ API
  // studentClassIds.value = await getStudentClasses(authStore.user.id)
  // Sau đó loadSchedule
})
</script>