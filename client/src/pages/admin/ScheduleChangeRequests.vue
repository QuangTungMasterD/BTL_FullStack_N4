<template>
  <div>
    <h1 class="font-headline-lg text-headline-lg mb-6">Quản lý yêu cầu đổi lịch</h1>
    
    <ErrorAlert
      v-if="scheduleRequestStore.error"
      :error="scheduleRequestStore.error"
      :status-code="scheduleRequestStore.errorStatusCode"
      :validation-errors="scheduleRequestStore.validationErrors"
      :timestamp="scheduleRequestStore.timestamp"
      @close="scheduleRequestStore.clearErrors"
    />

    <LoadingSpinner v-if="scheduleRequestStore.loading" />
    
    <DataTable v-else :columns="columns" :data="scheduleRequestStore.pendingRequests">
      <template #actions="{ row }">
        <Button v-if="row.status === 'Pending'" variant="primary" size="sm" @click="openApproveModal(row)">
          Xử lý
        </Button>
      </template>
      <template #status="{ row }">
        <Badge :variant="getStatusVariant(row.status)">
          {{ getStatusText(row.status) }}
        </Badge>
      </template>
      <template #originalStartTime="{ row }">
        {{ formatDateTime(row.originalStartTime) }}
      </template>
      <template #preferredSession="{ row }">
        {{ row.preferredSession === 'Morning' ? 'Sáng (7:20-10:20)' : (row.preferredSession === 'Afternoon' ? 'Chiều (13:15-16:15)' : '—') }}
      </template>
    </DataTable>

    <EmptyState
      v-if="!scheduleRequestStore.loading && scheduleRequestStore.pendingRequests.length === 0"
      message="Không có yêu cầu nào đang chờ xử lý"
    />

    <Modal v-model="showModal" title="Xử lý yêu cầu">
      <div class="space-y-4">
        <div class="grid grid-cols-2 gap-4">
          <div>
            <p class="text-label-md text-on-surface-variant">Giáo viên</p>
            <p class="font-title-md">{{ selectedRequest?.teacherName }}</p>
          </div>
          <div>
            <p class="text-label-md text-on-surface-variant">Lớp</p>
            <p class="font-title-md">{{ selectedRequest?.className }}</p>
          </div>
        </div>
        <div>
          <p class="text-label-md text-on-surface-variant">Thời gian cũ</p>
          <p class="font-body-md">{{ formatDateTime(selectedRequest?.originalStartTime) }} → {{ formatDateTime(selectedRequest?.originalEndTime) }}</p>
          <p class="text-label-sm text-on-surface-variant">Phòng: {{ selectedRequest?.originalRoomName }}</p>
        </div>
        <div>
          <p class="text-label-md text-on-surface-variant">Lý do</p>
          <p class="font-body-md">{{ selectedRequest?.reason }}</p>
        </div>
        <div v-if="selectedRequest?.requestType === 'Change'">
          <p class="text-label-md text-on-surface-variant">Ca đề xuất</p>
          <p class="font-body-md">{{ selectedRequest?.preferredSession === 'Morning' ? 'Sáng (7:20-10:20)' : 'Chiều (13:15-16:15)' }}</p>
        </div>
        <div v-else>
          <p class="text-label-md text-on-surface-variant">Yêu cầu</p>
          <p class="font-body-md text-error">Nghỉ buổi học</p>
        </div>
        
        <Input v-model="adminNote" label="Ghi chú của admin" type="textarea" rows="2" placeholder="Nhập lý do duyệt/từ chối..." />
        
        <div class="flex gap-3 justify-end pt-4">
          <Button variant="outline" @click="showModal = false">Đóng</Button>
          <Button variant="error" @click="process('Reject')" :loading="scheduleRequestStore.loading">
            Từ chối
          </Button>
          <Button variant="primary" @click="process('Approve')" :loading="scheduleRequestStore.loading">
            Duyệt
          </Button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useScheduleChangeRequestStore } from '@/stores';
import DataTable from '@/components/ui/DataTable.vue';
import Button from '@/components/ui/Button.vue';
import Badge from '@/components/ui/Badge.vue';
import Modal from '@/components/ui/Modal.vue';
import Input from '@/components/ui/Input.vue';
import LoadingSpinner from '@/components/ui/LoadingSpinner.vue';
import EmptyState from '@/components/ui/EmptyState.vue';
import ErrorAlert from '@/components/ui/ErrorAlert.vue';
import { formatDateTime } from '@/composables/useFormat';

const scheduleRequestStore = useScheduleChangeRequestStore();

const columns = [
  { key: 'id', label: 'ID', align: 'center' },
  { key: 'teacherName', label: 'Giáo viên', align: 'left' },
  { key: 'className', label: 'Lớp', align: 'left' },
  { key: 'originalStartTime', label: 'Thời gian cũ', align: 'left' },
  { key: 'suggestedDate', label: 'Ngày đề xuất', align: 'left', format: (v) => v ? formatDate(v) : '—' },
  { key: 'preferredSession', label: 'Ca đề xuất', align: 'left' },
  { key: 'status', label: 'Trạng thái', align: 'center' },
  { key: 'actions', label: 'Hành động', align: 'center' },
];

const showModal = ref(false);
const selectedRequest = ref(null);
const adminNote = ref('');

const getStatusVariant = (status) => {
  const map = { Pending: 'warning', Approved: 'success', Rejected: 'error' };
  return map[status] || 'default';
};

const getStatusText = (status) => {
  const map = { Pending: 'Chờ duyệt', Approved: 'Đã duyệt', Rejected: 'Từ chối' };
  return map[status] || status;
};

const openApproveModal = (req) => {
  selectedRequest.value = req;
  adminNote.value = '';
  showModal.value = true;
};

const process = async (action) => {
  try {
    await scheduleRequestStore.processRequest(selectedRequest.value.id, action, adminNote.value);
    showModal.value = false;
  } catch (err) {
    // Error already in store, ErrorAlert sẽ hiển thị
  }
};

onMounted(() => {
  scheduleRequestStore.fetchPendingRequests();
});
</script>