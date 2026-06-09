import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

import CoursesManager from '@/pages/academic/CoursesManager.vue';
import CoursesTrash from '@/pages/academic/CoursesTrash.vue';
import CourseDetail from '@/pages/academic/CourseDetail.vue';

import TeachersManager from '@/pages/teacher/TeachersManager.vue';
import TeachersTrash from '@/pages/teacher/TeachersTrash.vue';
import TeacherDetail from '@/pages/teacher/TeacherDetail.vue';

import ClassManager from '@/pages/class/ClassManager.vue';
import ClassTrash from '@/pages/class/ClassTrash.vue';
import ClassDetail from '@/pages/class/ClassDetail.vue';

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
      {
        path: 'classes',
        name: 'Class Manager',
        component: ClassManager
      },
      {
        path: 'classes/trash',
        name: 'Class Trash',
        component: ClassTrash
      },
      {
        path: 'classes/:id',
        name: 'Class Detail',
        component: ClassDetail
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('authToken');
  const userRole = localStorage.getItem('userRole');

  if (to.meta.requiresAuth && !token) {
    return next('/login');
  }

  if (to.meta.role && userRole !== to.meta.role) {
    return next('/');
  }

  next();
});

export default router
