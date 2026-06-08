import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

import CoursesManager from '@/pages/academic/CoursesManager.vue';
import CoursesTrash from '@/pages/academic/CoursesTrash.vue';
import CourseDetail from '@/pages/academic/CourseDetail.vue';

import TeachersManager from '@/pages/teacher/TeachersManager.vue';
import TeachersTrash from '@/pages/teacher/TeachersTrash.vue';
import TeacherDetail from '@/pages/teacher/TeacherDetail.vue';

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
      {
        path: 'courses/:id',
        name: 'Courses information',
        component: CourseDetail
      },
      {
        path: 'teachers',
        name: 'Teachers',
        component: TeachersManager
      },
      {
        path: 'teachers/trash',
        name: 'Teachers Trash',
        component: TeachersTrash
      },
      {
        path: 'teachers/:id',
        name: 'Teachers Detail',
        component: TeacherDetail
      },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
