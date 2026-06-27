<template>
  <aside class="fixed left-0 top-0 h-full w-[280px] bg-surface-container-lowest shadow-sm border-r border-outline-variant flex flex-col py-6 px-4 z-50">
    <div class="mb-10 flex items-center gap-3 px-2">
      <div class="w-10 h-10 bg-primary rounded-lg flex items-center justify-center text-on-primary">
        <span class="material-symbols-outlined">school</span>
      </div>
      <div>
        <h1 class="font-title-lg text-title-lg font-bold text-primary">EduPortal</h1>
        <p class="text-[10px] uppercase tracking-wider text-outline font-bold">Management System</p>
      </div>
    </div>

    <nav class="flex-1 space-y-1">
      <SidebarItem v-for="item in menuItems" :key="item.path" :to="item.path" :icon="item.icon" :label="item.label" />
    </nav>

    <div class="pt-6 mt-6 border-t border-outline-variant space-y-1">
      <SidebarItem v-for="item in footerMenuItems" :key="item.path" :to="item.path" :icon="item.icon" :label="item.label" />
    </div>
  </aside>
</template>

<script setup>
import SidebarItem from './SideBarItem.vue'

const menuItems = [
  { label: 'Dashboard', icon: 'dashboard', path: '/' },
  { label: 'Course', icon: 'school', path: '/courses' },
  { label: 'Teacher', icon: 'payments', path: '/teachers' },
  { label: 'Classes', icon: 'group', path: '/classes' },
  { label: 'Rooms', icon: 'local_library', path: '/rooms' },
  { label: 'teacherschedule', icon: 'analytics', path: '/teachers/schedule' },
  { label: 'schedule req', icon: 'analytics', path: '/admin/schedule-requests' },
]
const footerMenuItems = [
  { label: 'Settings', icon: 'settings', path: '/settings' },
  { label: 'Support', icon: 'help', path: '/support' }
]
</script>