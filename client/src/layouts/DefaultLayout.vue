<template>
  <div class="flex min-h-screen bg-background text-on-surface">
    <Sidebar :menuItems="menuItems" :footerMenuItems="footerMenuItems" />

    <main class="ml-[280px] w-[calc(100%-280px)] flex flex-col">
      <Header />

      <div class="p-margin-desktop flex-1">
        <router-view />
      </div>
    </main>
    <ToastContainer />
    <GlobalErrorWatcher />
  </div>
</template>

<script setup>
import ToastContainer from '@/components/ui/ToastContainer.vue';
import GlobalErrorWatcher from '@/components/ui/GlobalErrorWatcher.vue'
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