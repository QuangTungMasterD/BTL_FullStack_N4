import { createRouter, createWebHistory } from 'vue-router'

import Index from '@/pages/index.vue';

import CoursesManager from '@/pages/academic/CoursesManager.vue';
import CoursesTrash from '@/pages/academic/CoursesTrash.vue';
import CourseDetail from '@/pages/academic/CourseDetail.vue';

import TeachersManager from '@/pages/teacher/TeachersManager.vue';
import TeachersTrash from '@/pages/teacher/TeachersTrash.vue';
import TeacherDetail from '@/pages/teacher/TeacherDetail.vue';
import TeacherSchedule from '@/pages/teacher/TeacherSchedule.vue';

import ClassManager from '@/pages/class/ClassManager.vue';
import ClassTrash from '@/pages/class/ClassTrash.vue';
import ClassDetail from '@/pages/class/ClassDetail.vue';

import StudentSchedule from '@/pages/student/StudentSchedule.vue';

import RoomsManager from '@/pages/rooms/RoomsManager.vue';
import RoomsTrash from '@/pages/rooms/RoomsTrash.vue';

import SpecializationsManager from '@/pages/specializations/SpecializationsManager.vue';

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
        path: 'teachers/schedule',
        name: 'Teachers Schedule',
        component: TeacherSchedule
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
      },
      {
        path: 'student/schedule',
        name: 'Student Schedule',
        component: StudentSchedule
      },
      {
        path: 'rooms',
        name: 'Rooms Manager',
        component: RoomsManager
      },
      {
        path: 'rooms/trash',
        name: 'Rooms Trash',
        component: RoomsTrash
      },
      {
        path: 'specializations',
        name: 'Specialization Trash',
        component: SpecializationsManager
      },
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
