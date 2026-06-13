<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="text-left">
        <div class="flex items-center gap-3">
          <Link to="/classes" variant="muted" icon-left="arrow_back">
            Quay lại danh sách lớp
          </Link>
          <h1 class="font-headline-lg text-headline-lg">Chi tiết lớp học</h1>
        </div>
      </div>
      <div class="flex gap-3">
        <Button variant="outline" @click="openEditModal">
          <span class="material-symbols-outlined">edit</span>
          Sửa thông tin
        </Button>
        <Button variant="error" @click="confirmDelete" :disabled="classStore.loading">
          <span class="material-symbols-outlined">delete</span>
          Xóa lớp
        </Button>
      </div>
    </div>

    <!-- Loading -->
    <SkeletonDetail v-if="classStore.loading" />

    <div v-else class="space-y-6">
      <!-- Thông tin chung -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <h2 class="font-headline-md text-headline-md mb-4">Thông tin chung</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p class="text-label-md text-on-surface-variant">Tên lớp</p>
            <p class="font-body-md text-title-md">{{ classData?.className }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Mã lớp</p>
            <p class="font-body-md">{{ classData?.id }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Khóa học</p>
            <p class="font-body-md">{{ courseName }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Sĩ số tối đa</p>
            <p class="font-body-md">{{ classData?.maxStudent }} học viên</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Ngày bắt đầu</p>
            <p class="font-body-md">{{ formatDate(classData?.startDate) }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Ngày kết thúc</p>
            <p class="font-body-md">{{ formatDate(classData?.endDate) }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Trạng thái</p>
            <Badge :variant="classStatusBadgeVariant(classData?.status)">
              {{ classStatusText(classData?.status) }}
            </Badge>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">số tiết</p>
            <p class="font-body-md">{{ classData?.lesson }} buổi</p>
          </div>
        </div>
      </div>

      <!-- Giáo viên phụ trách -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <h2 class="font-headline-md text-headline-md mb-4">Giáo viên phụ trách</h2>
        <div v-if="teachers.length === 0" class="text-center py-4 text-on-surface-variant">
          Chưa có giáo viên nào được phân công
        </div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div
            v-for="teacher in teachers"
            :key="teacher.id"
            class="flex items-center gap-3 p-3 rounded-lg border border-outline-variant hover:shadow-sm transition"
          >
            <Avatar :name="teacher.fullName" size="md" />
            <div>
              <p class="font-title-md">{{ teacher.fullName }}</p>
              <p class="text-label-sm text-on-surface-variant">{{ teacher.email }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Danh sách buổi học -->
      <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="font-headline-md text-headline-md">Lịch học (buổi)</h2>
          <Button size="sm" variant="outline" @click="goToSchedule">
            <span class="material-symbols-outlined">calendar_month</span>
            Quản lý lịch
          </Button>
        </div>

        <LoadingSpinner v-if="loadingSessions" />
        <div v-else-if="sessions.length === 0" class="text-center py-8 text-on-surface-variant">
          Chưa có buổi học nào được lên lịch
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="session in sessions"
            :key="session.id"
            class="p-4 rounded-lg border border-outline-variant hover:shadow-sm transition cursor-pointer"
            @click="goToSessionDetail(session.id)"
          >
            <div class="flex justify-between items-start">
              <div>
                <h4 class="font-title-md">Buổi {{ session.lesson }}</h4>
                <p class="text-label-sm text-on-surface-variant mt-1">
                  {{ formatDateTime(session.startTime) }} – {{ formatDateTime(session.endTime) }}
                </p>
              </div>
              <Badge :variant="classSessionStatusBadgeVariant(session.status)">
                {{ classSessionStatusText(session.status) }}
              </Badge>
            </div>
            <div class="grid grid-cols-2 gap-3 mt-3 text-label-sm text-on-surface-variant">
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">meeting_room</span>
                Phòng: {{ session.roomId ? 'Đã phân công' : 'Chưa có phòng' }}
              </div>
              <div class="flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">fact_check</span>
                Điểm danh: {{ session.attendanceCount || 0 }}/{{ currentSlots }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Danh sách học viên (tạm thời placeholder nếu chưa có API) -->
      <!-- <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="font-headline-md text-headline-md">Danh sách học viên</h2>
        </div>
        <div class="text-center py-8 text-on-surface-variant">
          Tính năng đang phát triển
        </div>
      </div> -->
    </div>

    <!-- Modal Sửa lớp -->
    <Modal v-model="showEditModal" title="Sửa thông tin lớp">
      <form @submit.prevent="handleUpdate" class="space-y-4">
        <Input
          v-model="editForm.className"
          label="Tên lớp"
          required
          :error="validationErrors?.className?.join(', ')"
        />
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.maxStudent"
            type="number"
            label="Sĩ số tối đa"
            required
            :min="1"
            :error="validationErrors?.maxStudent?.join(', ')"
          />
          <Select
            v-model="editForm.status"
            label="Trạng thái"
            :options="classStatusOptions"
            required
            :error="validationErrors?.status?.join(', ')"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.startDate"
            type="date"
            label="Ngày bắt đầu"
            required
            :error="validationErrors?.startDate?.join(', ')"
          />
          <Input
            v-model="editForm.endDate"
            type="date"
            label="Ngày kết thúc"
            required
            :error="validationErrors?.endDate?.join(', ')"
          />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Input
            v-model="editForm.lesson"
            type="number"
            label="số tiết"
            required
            :min="1"
            :error="validationErrors?.lesson?.join(', ')"
          />
          <Select
            v-model="editForm.courseId"
            label="Khóa học"
            :options="courseOptions"
            required
            :error="validationErrors?.courseId?.join(', ')"
          />
        </div>
        <Select
          v-model="editForm.teacherIds"
          label="Giáo viên phụ trách"
          :options="teacherOptions"
          multiple
          :error="validationErrors?.teacherIds?.join(', ')"
        />
        <div class="flex justify-end gap-3 pt-4">
          <Button variant="outline" @click="closeEditModal">Hủy</Button>
          <Button variant="primary" type="submit" :loading="classStore.loading">
            Cập nhật
          </Button>
        </div>
      </form>
    </Modal>

    <!-- Confirm Delete Dialog -->
    <ConfirmDialog
      v-model="showDeleteConfirm"
      title="Xác nhận xóa"
      :message="`Bạn có chắc chắn muốn xóa lớp '${classData?.className}' không?`"
      confirm-text="Xóa"
      @confirm="handleDeleteClass"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useClassStore, useCourseStore, useTeacherStore, useTeacherAssignmentStore, useClassSessionStore } from '@/stores'
import { CLASS_STATUS_OPTIONS } from '@/constants'
import { formatDate, formatDateTime, classStatusText, classStatusBadgeVariant, classSessionStatusText, classSessionStatusBadgeVariant } from '@/composables/useFormat'
import Link from '@/components/ui/Link.vue'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Select from '@/components/ui/Select.vue'
import Modal from '@/components/ui/Modal.vue'
import Badge from '@/components/ui/Badge.vue'
import Avatar from '@/components/ui/Avatar.vue'
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'
import SkeletonDetail from '@/components/skeleton/SkeletonDetail.vue';

const router = useRouter()
const route = useRoute()
const classStore = useClassStore()
const courseStore = useCourseStore()
const teacherStore = useTeacherStore()
const teacherAssignmentStore = useTeacherAssignmentStore()
const classSessionStore = useClassSessionStore()

const { validationErrors } = storeToRefs(classStore)

// Data
const classData = ref(null)
const sessions = ref([])
const loadingSessions = ref(false)

// Computed helpers
const courseName = computed(() => {
  if (!classData.value?.courseId) return 'N/A'
  const course = courseStore.courses.find(c => c.id === classData.value.courseId)
  return course?.courseName || 'N/A'
})

const teachers = ref([])

const currentSlots = computed(() => {
  // Tạm thời lấy từ classData.maxStudent (có thể thay bằng API lấy số học viên thực tế)
  return classData.value?.maxStudent || 0
})

// Edit form
const showEditModal = ref(false)
const editForm = reactive({
  className: '',
  maxStudent: null,
  status: null,
  startDate: '',
  endDate: '',
  lesson: null,
  courseId: null,
  teacherIds: []
})

const classStatusOptions = CLASS_STATUS_OPTIONS
const courseOptions = computed(() =>
  courseStore.courses.map(c => ({ value: c.id, label: c.courseName }))
)
const teacherOptions = computed(() =>
  teacherStore.teachers.map(t => ({ value: t.id, label: t.fullName }))
)

// Load data
const loadClassDetail = async () => {
  const id = route.params.id
  if (!id) {
    router.push('/classes')
    return
  }
  try {
    classData.value = await classStore.fetchById(id)
    // Load teachers for this class
    await loadTeachersForClass(id)
    await loadSessionsForClass(id)
  } catch (err) {
    console.error('Failed to load class:', err)
    router.push('/classes')
  }
}

const loadTeachersForClass = async (classId) => {
  await teacherAssignmentStore.fetchAll()
  const assignments = teacherAssignmentStore.assignments.filter(a => a.classId === Number(classId))
  const teacherIds = assignments.map(a => a.teacherId)
  if (teacherIds.length) {
    await teacherStore.fetchAll()
    teachers.value = teacherStore.teachers.filter(t => teacherIds.includes(t.id))
  } else {
    teachers.value = []
  }
}

const loadSessionsForClass = async (classId) => {
  loadingSessions.value = true
  try {
    await classSessionStore.fetchPaged({ classId, pageSize: 100 })
    sessions.value = classSessionStore.pagedData.data
  } catch (err) {
    console.error('Failed to load sessions:', err)
  } finally {
    loadingSessions.value = false
  }
}

// Edit modal
const openEditModal = () => {
  editForm.className = classData.value.className
  editForm.maxStudent = classData.value.maxStudent
  editForm.status = classData.value.status
  editForm.startDate = classData.value.startDate?.split('T')[0] || ''
  editForm.endDate = classData.value.endDate?.split('T')[0] || ''
  editForm.lesson = classData.value.lesson
  editForm.courseId = classData.value.courseId
  editForm.teacherIds = teachers.value.map(t => t.id)
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
}

const handleUpdate = async () => {
  const submitData = {
    className: editForm.className,
    maxStudent: Number(editForm.maxStudent),
    status: Number(editForm.status),
    startDate: editForm.startDate,
    endDate: editForm.endDate,
    lesson: Number(editForm.lesson),
    courseId: Number(editForm.courseId)
  }
  try {
    await classStore.update(classData.value.id, submitData)
    // Update teacher assignments
    const currentTeacherIds = teachers.value.map(t => t.id)
    const newTeacherIds = editForm.teacherIds.map(id => Number(id))
    const toRemove = currentTeacherIds.filter(id => !newTeacherIds.includes(id))
    const toAdd = newTeacherIds.filter(id => !currentTeacherIds.includes(id))
    for (const teacherId of toRemove) {
      const assignment = teacherAssignmentStore.assignments.find(a => a.classId === classData.value.id && a.teacherId === teacherId)
      if (assignment) await teacherAssignmentStore.delete(assignment.id)
    }
    for (const teacherId of toAdd) {
      await teacherAssignmentStore.create({ teacherId, classId: classData.value.id })
    }
    closeEditModal()
    await loadClassDetail()
  } catch (err) {
    console.error('Update failed:', err)
  }
}

// Delete
const showDeleteConfirm = ref(false)
const confirmDelete = () => {
  showDeleteConfirm.value = true
}
const handleDeleteClass = async () => {
  await classStore.delete(classData.value.id)
  showDeleteConfirm.value = false
  router.push('/classes')
}

// Navigation helpers
const goToSchedule = () => {
  router.push(`/classes/${classData.value.id}/sessions`)
}
const goToSessionDetail = (sessionId) => {
  router.push(`/class-sessions/${sessionId}`)
}
const manageStudents = () => {
  router.push(`/classes/${classData.value.id}/students`)
}

onMounted(() => {
  Promise.all([courseStore.fetchAll(), teacherStore.fetchAll()])
  loadClassDetail()
})
</script>