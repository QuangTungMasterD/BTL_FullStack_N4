import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

const routes = [
  {
    path: '/',
    name: 'Home',
    // component: () => import('@/pages/index.vue'), // Nếu dùng cái này không cần import trên
    component: Index, //Nếu dùng cái này thì import ở trên
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
