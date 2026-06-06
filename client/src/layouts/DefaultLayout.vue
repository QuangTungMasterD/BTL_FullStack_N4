<template>
  <div class="flex min-h-screen bg-background text-on-surface">
    <Sidebar :menuItems="menuItems" :footerMenuItems="footerMenuItems" />

    <main class="ml-[280px] w-[calc(100%-280px)] flex flex-col">
      <Header />

      <div class="p-margin-desktop flex-1">
        <slot />
      </div>
    </main>
  </div>
</template>

<script setup>
import Sidebar from './SideBar/SideBar.vue'
import Header from './Header/Header.vue'

const menuItems = [
  { label: 'Dashboard', icon: 'dashboard', path: '/' },
  { label: 'Academic', icon: 'school', path: '/academic' },
  { label: 'Users', icon: 'group', path: '/users' },
  { label: 'Finance', icon: 'payments', path: '/finance' },
  { label: 'Library', icon: 'local_library', path: '/library' },
  { label: 'Reports', icon: 'analytics', path: '/reports' }
]

const footerMenuItems = [
  { label: 'Settings', icon: 'settings', path: '/settings' },
  { label: 'Support', icon: 'help', path: '/support' }
]
</script>