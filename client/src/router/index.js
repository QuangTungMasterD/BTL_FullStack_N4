import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

import CoursesManager from '@/pages/academic/CoursesManager.vue';
import CoursesTrash from '@/pages/academic/CoursesTrash.vue';

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
      {
        path: 'courses/trash',
        name: 'Courses trash',
        component: CoursesTrash
      },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
