import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    children: [
      {
        path: 'courses',
        name: 'Courses',
        component: () => import('@/pages/academic/CoursesManager.vue'),
      },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
