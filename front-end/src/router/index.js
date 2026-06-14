import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

// Layout components
import AuthLayout from '@/layouts/AuthLayout.vue'
import MainLayout from '@/layouts/MainLayout.vue'

// Auth pages
import LoginView from '@/pages/LoginView.vue'

// Common pages
import UnauthorizedView from '@/pages/UnauthorizedView.vue'

// Shared pages
import ProfileView from '@/pages/shared/ProfileView.vue'

// Student pages
import StudentDashboard from '@/pages/student/DashboardView.vue'
import StudentAttendance from '@/pages/student/AttendanceView.vue'
import StudentGrades from '@/pages/student/GradesView.vue'
import StudentEnrollments from '@/pages/student/EnrollmentsView.vue'

// Lecturer pages
import LecturerDashboard from '@/pages/lecturer/DashboardView.vue'
import MyStudents from '@/pages/lecturer/MyStudents.vue'
import LecturerAttendance from '@/pages/lecturer/AttendanceManagement.vue'
import LecturerGrades from '@/pages/lecturer/GradeManagement.vue'

// Admin pages
import AdminDashboard from '@/pages/admin/DashboardView.vue'
import StudentManagement from '@/pages/admin/StudentManagement.vue'
import LecturerManagement from '@/pages/admin/LecturerManagement.vue'
import AdminCourseManagement from '@/pages/admin/CourseManagement.vue'
import AdminAttendanceManagement from '@/pages/admin/AttendanceManagement.vue'
import AdminGradeManagement from '@/pages/admin/GradeManagement.vue'
import ReportsView from '@/pages/admin/ReportsView.vue'

const routes = [
  {
    path: '/login',
    component: AuthLayout,
    children: [
      {
        path: '',
        name: 'Login',
        component: LoginView,
        meta: { requiresAuth: false },
      },
    ],
  },
  {
    path: '/unauthorized',
    component: MainLayout,
    children: [
      {
        path: '',
        name: 'Unauthorized',
        component: UnauthorizedView,
        meta: { requiresAuth: true },
      },
    ],
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      // ============ ADMIN ROUTES ============
      {
        path: '',
        name: 'AdminDashboard',
        component: AdminDashboard,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'students',
        name: 'StudentManagement',
        component: StudentManagement,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'lecturers',
        name: 'LecturerManagement',
        component: LecturerManagement,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'admin-courses',
        name: 'AdminCourseManagement',
        component: AdminCourseManagement,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'admin-attendance',
        name: 'AdminAttendanceManagement',
        component: AdminAttendanceManagement,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'admin-grades',
        name: 'AdminGradeManagement',
        component: AdminGradeManagement,
        meta: { roles: ['ADMIN'] },
      },
      {
        path: 'reports',
        name: 'Reports',
        component: ReportsView,
        meta: { roles: ['ADMIN'] },
      },
      
      // ============ LECTURER ROUTES ============
      {
        path: 'lecturer-dashboard',
        name: 'LecturerDashboard',
        component: LecturerDashboard,
        meta: { roles: ['LECTURER'] },
      },
      {
        path: 'my-students',
        name: 'MyStudents',
        component: MyStudents,
        meta: { roles: ['LECTURER'] },
      },
      {
        path: 'lecturer-attendance',
        name: 'LecturerAttendance',
        component: LecturerAttendance,
        meta: { roles: ['LECTURER'] },
      },
      {
        path: 'lecturer-grades',
        name: 'LecturerGrades',
        component: LecturerGrades,
        meta: { roles: ['LECTURER'] },
      },
      
      // ============ STUDENT ROUTES ============
      {
        path: 'student-dashboard',
        name: 'StudentDashboard',
        component: StudentDashboard,
        meta: { roles: ['STUDENT'] },
      },
      {
        path: 'student-attendance',
        name: 'StudentAttendance',
        component: StudentAttendance,
        meta: { roles: ['STUDENT'] },
      },
      {
        path: 'student-grades',
        name: 'StudentGrades',
        component: StudentGrades,
        meta: { roles: ['STUDENT'] },
      },
      {
        path: 'enrollments',
        name: 'StudentEnrollments',
        component: StudentEnrollments,
        meta: { roles: ['STUDENT'] },
      },
      
      // ============ SHARED ROUTES ============
      {
        path: 'profile',
        name: 'Profile',
        component: ProfileView,
        meta: { roles: ['ADMIN', 'LECTURER', 'STUDENT'] },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // Kiểm tra xem route có yêu cầu đăng nhập không
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  
  // Nếu chưa đăng nhập và cần đăng nhập -> chuyển đến login
  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  // Nếu đã đăng nhập và đang ở trang login -> chuyển đến dashboard theo role
  if (to.path === '/login' && authStore.isAuthenticated) {
    const userRole = authStore.user?.role
    if (userRole === 'ADMIN') {
      next('/')
    } else if (userRole === 'LECTURER') {
      next('/lecturer-dashboard')
    } else if (userRole === 'STUDENT') {
      next('/student-dashboard')
    } else {
      next('/')
    }
    return
  }
  
  // Kiểm tra role-based access control
  const requiredRoles = to.matched.flatMap(record => record.meta.roles || [])
  
  if (requiredRoles.length > 0) {
    const userRole = authStore.user?.role
    if (!userRole || !requiredRoles.includes(userRole)) {
      // Nếu không có quyền, chuyển đến unauthorized
      if (to.path !== '/unauthorized') {
        next('/unauthorized')
        return
      }
    }
  }
  
  // Xử lý redirect khi vào root path với role khác nhau
  // QUAN TRỌNG: Chỉ redirect nếu đang ở root path và chưa phải là dashboard đúng role
  if (to.path === '/') {
    const userRole = authStore.user?.role
    if (userRole === 'LECTURER' && from.path !== '/lecturer-dashboard') {
      next('/lecturer-dashboard')
      return
    } else if (userRole === 'STUDENT' && from.path !== '/student-dashboard') {
      next('/student-dashboard')
      return
    }
  }
  
  // Mặc định cho phép đi tiếp
  next()
})

export default router