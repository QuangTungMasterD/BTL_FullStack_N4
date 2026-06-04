<template>
  <router-link
    :to="to"
    class="flex items-center gap-3 px-3 py-3 rounded-lg transition-all duration-200 group"
    :class="[
      isActive
        ? 'bg-primary-container text-on-primary-container font-bold'
        : 'text-on-surface-variant hover:bg-surface-container-high'
    ]"
    @mousedown="scaleDown"
    @mouseup="scaleUp"
    @mouseleave="scaleUp"
    @click="logClick"
  >
    <span
      class="material-symbols-outlined"
      :class="{ 'group-hover:text-primary': !isActive, 'text-primary': isActive }"
    >
      {{ icon }}
    </span>
    <span class="font-body-md text-body-md">{{ label }}</span>
  </router-link>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const props = defineProps(['to', 'icon', 'label'])
const route = useRoute()
const isActive = computed(() => route.path === props.to)

const scaleDown = (e) => e.currentTarget.classList.add('scale-95')
const scaleUp = (e) => e.currentTarget.classList.remove('scale-95')
const logClick = () => console.log(`[Sidebar] Chọn: ${props.label}`)
</script>