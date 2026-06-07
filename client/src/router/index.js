import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

import CoursesManager from '@/pages/academic/CoursesManager.vue';

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'Home',
        component: { template: '<div>HOME PAGE</div>' }
      },
      {
        path: 'courses',
        name: 'Courses',
        component: CoursesManager
      },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
