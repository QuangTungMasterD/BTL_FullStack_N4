<template>
  <div>
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-8">
      <div>
        <h1 class="font-headline-md text-headline-md text-on-surface">Lịch giảng dạy</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Quản lý lịch dạy của bạn</p>
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
        <Button variant="outline" @click="openRequestModal">
          <span class="material-symbols-outlined">event_busy</span>
          Yêu cầu đổi lịch
        </Button>
      </div>
    </div>

    <LoadingSpinner v-if="loading" />
    <div v-else>
      <ScheduleCalendar
        :view-mode="viewMode"
        :sessions="sessions"
        :week-days="weekDays"
        :month-days="monthDays"
        :hours="hours"
        @session-click="openSessionDetail"
      />
      <div class="mt-8 grid grid-cols-1 md:grid-cols-3 gap-6">
        <StatCard label="Tổng số tiết dạy" :value="totalSessions" />
        <StatCard label="Số lớp đang dạy" :value="uniqueClassesCount" />
        <StatCard label="Yêu cầu đổi lịch" :value="pendingRequests" />
      </div>
    </div>

    <!-- Modals (giữ nguyên) -->
    <Modal v-model="showRequestModal" title="Yêu cầu thay đổi lịch dạy">
      <form @submit.prevent="submitRequest" class="space-y-4">
        <Select v-model="requestData.sessionId" label="Chọn buổi học" :options="sessionOptions" required />
        <Select v-model="requestData.type" label="Loại yêu cầu" :options="requestTypeOptions" required />
        <Input v-model="requestData.reason" label="Lý do" type="textarea" rows="3" required />
        <div class="space-y-2">
          <Input v-model="requestData.suggestedDate" type="date" label="Chọn ngày" required />
          <label class="font-label-md text-label-md">Chọn ca học mới</label>
          <div class="flex gap-4">
            <label class="flex items-center gap-2 cursor-pointer">
              <input type="radio" value="Morning" v-model="requestData.preferredSession" />
              <span>Sáng (7:20 - 10:20)</span>
            </label>
            <label class="flex items-center gap-2 cursor-pointer">
              <input type="radio" value="Afternoon" v-model="requestData.preferredSession" />
              <span>Chiều (13:15 - 16:15)</span>
            </label>
          </div>
        </div>
        <div class="flex justify-end gap-3">
          <Button variant="outline" @click="showRequestModal = false">Hủy</Button>
          <Button variant="primary" type="submit" :loading="submitting">Gửi yêu cầu</Button>
        </div>
      </form>
    </Modal>

    <Modal v-model="showDetailModal" title="Chi tiết buổi học">
      <div v-if="selectedSession" class="space-y-4">
        <div class="flex items-start gap-3">
          <span class="material-symbols-outlined text-primary">menu_book</span>
          <div><p class="font-label-md">Lớp</p><p class="font-title-md">{{ selectedSession.className }}</p></div>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div><p class="font-label-md">Phòng</p><p class="font-body-md">{{ selectedSession.roomName || 'Chưa xác định' }}</p></div>
          <div><p class="font-label-md">Trạng thái</p><Badge :variant="getStatusVariant(selectedSession.status)">{{ getStatusText(selectedSession.status) }}</Badge></div>
        </div>
        <div><p class="font-label-md">Thời gian</p><p class="font-body-md">{{ formatDateTime(selectedSession.startTime) }} – {{ formatDateTime(selectedSession.endTime) }}</p></div>
        <div class="flex justify-end"><Button variant="outline" @click="openRequestForSession(selectedSession)">Yêu cầu đổi lịch</Button></div>
      </div>
    </Modal>

    <ConfirmDialog v-model="showConfirmDialog" title="Xác nhận" message="Yêu cầu đã được gửi." confirm-text="Đóng" @confirm="showConfirmDialog = false" />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useClassSessionStore, useClassStore, useRoomStore, useTeacherAssignmentStore, useScheduleChangeRequestStore } from '@/stores'
import { formatDateTime } from '@/composables/useFormat'
import api from '@/services/api'
import Button from '@/components/ui/Button.vue'
import Select from '@/components/ui/Select.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import Badge from '@/components/ui/Badge.vue'
import StatCard from '@/components/ui/StatCard.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import ScheduleCalendar from '@/components/business/ScheduleCalendar.vue'

const classSessionStore = useClassSessionStore()
const classStore = useClassStore()
const roomStore = useRoomStore()
const teacherAssignmentStore = useTeacherAssignmentStore()
const scheduleRequestStore = useScheduleChangeRequestStore();

const teacherId = ref(1) // hardcode, sau thay bằng auth
const loading = ref(false)
const submitting = ref(false)
const sessions = ref([])
const viewMode = ref('week')
const currentDate = ref(new Date())
const showRequestModal = ref(false)
const showDetailModal = ref(false)
const showConfirmDialog = ref(false)
const selectedSession = ref(null)

const requestData = reactive({ 
    sessionId: '', 
    type: 'change', 
    reason: '', 
    preferredSession: 'Morning',
    suggestedDate: ''
});
const requestTypeOptions = [
  { value: 'change', label: 'Đổi lịch dạy' },
  { value: 'cancel', label: 'Nghỉ buổi học' }
]

// Hàm tạo mảng weekDays
const getWeekDays = (baseDate) => {
  const start = new Date(baseDate)
  const dayOfWeek = start.getDay()
  const diff = dayOfWeek === 0 ? 6 : dayOfWeek - 1
  start.setDate(start.getDate() - diff)
  const days = []
  const today = new Date()
  for (let i = 0; i < 7; i++) {
    const date = new Date(start)
    date.setDate(start.getDate() + i)
    days.push({
      name: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật'][i],
      day: date.getDate(),
      fullDate: date,
      isToday: date.toDateString() === today.toDateString()
    })
  }
  return days
}

const getMonthDays = (baseDate) => {
  const year = baseDate.getFullYear()
  const month = baseDate.getMonth()
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const days = []
  const today = new Date()
  for (let d = 1; d <= lastDay.getDate(); d++) {
    const date = new Date(year, month, d)
    days.push({ day: d, date, isToday: date.toDateString() === today.toDateString() })
  }
  return days
}

const weekDays = computed(() => getWeekDays(currentDate.value))
const monthDays = computed(() => getMonthDays(currentDate.value))
const hours = computed(() => Array.from({ length: 14 }, (_, i) => i + 7))

const dateRangeText = computed(() => {
  if (viewMode.value === 'week') {
    const days = weekDays.value
    if (days.length) {
      const start = days[0].fullDate
      const end = days[6].fullDate
      return `${start.getDate()}/${start.getMonth()+1} – ${end.getDate()}/${end.getMonth()+1}/${start.getFullYear()}`
    }
    return ''
  }
  return `Tháng ${currentDate.value.getMonth() + 1}/${currentDate.value.getFullYear()}`
})

const totalSessions = computed(() => sessions.value.length)
const uniqueClassesCount = computed(() => new Set(sessions.value.map(s => s.classId)).size)
const pendingRequests = ref(0)

const sessionOptions = computed(() =>
  sessions.value.map(s => ({ value: s.id, label: `${s.className} - ${formatDateTime(s.startTime)}` }))
)

const getStatusText = (status) => {
  const map = { 1: 'Đã lên lịch', 2: 'Đang diễn ra', 3: 'Đã kết thúc', 4: 'Đã hủy' }
  return map[status] || 'Không xác định'
}
const getStatusVariant = (status) => {
  const map = { 1: 'info', 2: 'success', 3: 'default', 4: 'error' }
  return map[status] || 'default'
}

const loadSchedule = async () => {
  if (!teacherId.value) return
  loading.value = true
  try {
    const params = { teacherId: teacherId.value, pageSize: 100 }
    if (viewMode.value === 'week') {
      const days = weekDays.value
      if (days.length) {
        const start = days[0].fullDate
        const end = days[6].fullDate
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

    // Tạo map từ teacherAssignmentId -> classId
    const assignmentMap = {}
    teacherAssignmentStore.assignments.forEach(ta => {
      assignmentMap[ta.id] = ta.classId
    })

    data = data.map(s => {
      const classId = assignmentMap[s.teacherAssignmentId]
      const cls = classStore.classes.find(c => c.id === classId)
      return {
        ...s,
        classId: classId,
        className: cls?.className || 'N/A',
        roomName: roomStore.rooms.find(r => r.id === s.roomId)?.roomName || 'N/A'
      }
    })
    sessions.value = data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

const navigate = (dir) => {
  const newDate = new Date(currentDate.value)
  if (viewMode.value === 'week') newDate.setDate(newDate.getDate() + dir * 7)
  else newDate.setMonth(newDate.getMonth() + dir)
  currentDate.value = newDate
  loadSchedule()
}

const openRequestModal = () => {
  requestData.sessionId = ''
  requestData.type = 'change'
  requestData.reason = ''
  requestData.suggestedDate = ''
  showRequestModal.value = true
}
const openRequestForSession = (session) => {
  requestData.sessionId = session.id
  showDetailModal.value = false
  showRequestModal.value = true
}
const openSessionDetail = (session) => {
  selectedSession.value = session
  showDetailModal.value = true
}

const submitRequest = async () => {
  submitting.value = true;
  try {
    await scheduleRequestStore.createRequest({
    classSessionId: requestData.sessionId,
    requestType: requestData.type,
    reason: requestData.reason,
    preferredSession: requestData.preferredSession,
    suggestedDate: requestData.suggestedDate,
});
    showRequestModal.value = false;
    showConfirmDialog.value = true;
  } catch (err) {
    console.error(err);
    alert(err.message);
  } finally {
    submitting.value = false;
  }
};

// watch
watch(viewMode, () => {
  loadSchedule()
})

onMounted(async () => {
  await Promise.all([
    classStore.fetchAll(),
    roomStore.fetchAll(),
    teacherAssignmentStore.fetchAll()
  ])
  await loadSchedule()
})
</script>