<template>
  <div class="bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant overflow-hidden">
    <!-- Week View -->
    <div v-if="viewMode === 'week'">
      <div class="grid grid-cols-8 border-b border-outline-variant bg-surface-container-low">
        <div class="h-14 flex items-center justify-center font-label-md text-on-surface-variant uppercase tracking-wider border-r border-outline-variant">
          Giờ
        </div>
        <div
          v-for="day in weekDays"
          :key="day.date"
          class="h-14 flex flex-col items-center justify-center border-r border-outline-variant"
          :class="{ 'bg-primary/5': day.isToday }"
        >
          <span class="font-label-md text-on-surface-variant uppercase">{{ day.name }}</span>
          <span class="font-title-md" :class="{ 'text-primary font-bold': day.isToday }">{{ day.day }}</span>
        </div>
      </div>
      <div class="relative overflow-y-auto max-h-[700px]">
        <div class="grid grid-cols-8">
          <div class="flex flex-col">
            <div
              v-for="hour in hours"
              :key="hour"
              class="time-row flex items-start justify-center pt-2 font-label-md text-on-surface-variant border-r border-outline-variant"
            >
              {{ hour }}:00
            </div>
          </div>
          <div v-for="(day, idx) in weekDays" :key="idx" class="flex flex-col">
            <div
              v-for="hour in hours"
              :key="hour"
              class="calendar-cell relative"
              :class="{ 'bg-primary/5': day.isToday }"
            >
              <div
                v-for="session in getSessionsAt(day.fullDate, hour)"
                :key="session.id"
                class="absolute inset-x-1 rounded-lg p-2 text-xs shadow-sm cursor-pointer hover:shadow-md transition-all z-10 bg-primary-container/20 border-l-4 border-primary"
                :style="getSessionStyle(session, hour)"
                @click.stop="$emit('sessionClick', session)"
              >
                <div class="font-bold truncate">{{ session.className }}</div>
                <div class="text-[11px] truncate">{{ session.roomName }}</div>
                <div class="text-[10px] mt-1">{{ formatTime(session.startTime) }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Month View -->
    <div v-else>
      <div class="grid grid-cols-7 gap-px bg-outline-variant">
        <div
          v-for="day in monthDays"
          :key="day.date"
          class="bg-surface-container-lowest p-2 min-h-[100px]"
          :class="{ 'bg-primary/5': day.isToday }"
        >
          <div class="font-title-md text-right mb-1" :class="{ 'text-primary': day.isToday }">
            {{ day.day }}
          </div>
          <div class="space-y-1">
            <div
              v-for="session in getSessionsOnDate(day.date)"
              :key="session.id"
              class="text-xs p-1 rounded cursor-pointer truncate bg-primary-container/20 border-l-4 border-primary"
              @click="$emit('sessionClick', session)"
            >
              {{ formatTime(session.startTime) }} {{ session.className }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  viewMode: { type: String, default: 'week' },
  sessions: { type: Array, default: () => [] },
  weekDays: { type: Array, default: () => [] },
  monthDays: { type: Array, default: () => [] },
  hours: { type: Array, default: () => [] }
})

const emit = defineEmits(['sessionClick'])

const getSessionsAt = (date, hour) => {
  const targetStart = new Date(date)
  targetStart.setHours(hour, 0, 0, 0)
  const targetEnd = new Date(targetStart)
  targetEnd.setHours(hour + 1, 0, 0, 0)
  return props.sessions.filter(s => {
    const start = new Date(s.startTime)
    return start >= targetStart && start < targetEnd
  })
}

const getSessionsOnDate = (date) => {
  const targetStart = new Date(date)
  targetStart.setHours(0, 0, 0, 0)
  const targetEnd = new Date(targetStart)
  targetEnd.setDate(targetStart.getDate() + 1)
  return props.sessions.filter(s => {
    const start = new Date(s.startTime)
    return start >= targetStart && start < targetEnd
  })
}

const getSessionStyle = (session, hour) => {
  const start = new Date(session.startTime)
  const end = new Date(session.endTime)
  const startMin = start.getMinutes()
  const durationHours = (end - start) / (1000 * 60 * 60)
  const topPercent = (startMin / 60) * 100
  const heightPercent = durationHours * 100
  return {
    top: `${topPercent}%`,
    height: `${heightPercent}%`,
    left: '4px',
    right: '4px'
  }
}

const formatTime = (dateStr) => new Date(dateStr).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
</script>

<style scoped>
.time-row {
  height: 100px;
  border-bottom: 1px solid #e2e8f0;
}
.calendar-cell {
  border-right: 1px solid #e2e8f0;
  border-bottom: 1px solid #e2e8f0;
  height: 100px;
  position: relative;
}
</style>