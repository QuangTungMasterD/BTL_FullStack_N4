import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    children: [
      { path: '', component: Index },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
