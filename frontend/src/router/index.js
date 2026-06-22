import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store.js'

const AppLayout = () => import('@/components/layout/AppLayout.vue')

const routes = [
  {
    path: '/',
    redirect: '/dashboard',
  },

  // Auth routes (no layout, no auth required)
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: { requiresAuth: false, title: 'Iniciar sesión' },
  },
  {
    path: '/forgot-password',
    name: 'forgot-password',
    component: () => import('@/views/auth/RecoverPasswordView.vue'),
    meta: { requiresAuth: false, title: 'Recuperar contraseña' },
  },
  {
    path: '/auth/confirm-email',
    name: 'confirm-email',
    component: () => import('@/views/auth/ConfirmEmailView.vue'),
    meta: { requiresAuth: false, title: 'Confirmar cuenta' },
  },
  {
    path: '/auth/reset-password',
    name: 'reset-password',
    component: () => import('@/views/auth/ResetPasswordView.vue'),
    meta: { requiresAuth: false, title: 'Restablecer contraseña' },
  },

  // App routes (with layout, auth required)
  {
    path: '/',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'dashboard',
        name: 'dashboard',
        component: () => import('@/views/dashboard/DashboardView.vue'),
        meta: { title: 'Dashboard', module: 'dashboard' },
      },

      {
        path: 'vehicles',
        name: 'vehicles',
        component: () => import('@/views/vehicles/VehiclesView.vue'),
        meta: { title: 'Vehículos', module: 'vehicles' },
      },
      {
        path: 'vehicles/:id',
        name: 'vehicle-detail',
        component: () => import('@/views/vehicles/VehicleDetailView.vue'),
        props: true,
        meta: { title: 'Detalle de vehículo', module: 'vehicles' },
      },

      {
        path: 'drivers',
        name: 'drivers',
        component: () => import('@/views/drivers/DriversView.vue'),
        meta: { title: 'Conductores', module: 'drivers' },
      },
      {
        path: 'drivers/:id',
        name: 'driver-detail',
        component: () => import('@/views/drivers/DriversDetailView.vue'),
        props: true,
        meta: { title: 'Detalle de conductor', module: 'drivers' },
      },

      {
        path: 'requests',
        name: 'requests',
        component: () => import('@/views/requests/RequestsView.vue'),
        meta: { title: 'Solicitudes', module: 'requests' },
      },
      {
        path: 'requests/new',
        name: 'request-new',
        component: () => import('@/views/requests/RequestNewView.vue'),
        meta: {
          title: 'Nueva solicitud',
          module: 'requests',
          roles: ['Administrador', 'Operador'],
        },
      },
      {
        path: 'requests/:id',
        name: 'request-detail',
        component: () => import('@/views/requests/RequestFlowView.vue'),
        props: true,
        meta: { title: 'Detalle de solicitud', module: 'requests' },
      },

      {
        path: 'schedules',
        name: 'schedules',
        component: () => import('@/views/schedule/ScheduleView.vue'),
        meta: { title: 'Agenda', module: 'schedules' },
      },

      {
        path: 'trips',
        name: 'trips',
        component: () => import('@/views/trips/TripHistoryView.vue'),
        meta: { title: 'Historial de viajes', module: 'trips' },
      },

      {
        path: 'fuel',
        name: 'fuel',
        component: () => import('@/views/fuel/FuelView.vue'),
        meta: { title: 'Combustible', module: 'fuel' },
      },

      {
        path: 'maintenance',
        name: 'maintenance',
        component: () => import('@/views/maintenance/MaintenanceView.vue'),
        meta: {
          title: 'Mantenimiento',
          module: 'maintenance',
          roles: ['Administrador', 'Supervisor'],
        },
      },

      {
        path: 'reports',
        name: 'reports',
        component: () => import('@/views/reports/ReportsView.vue'),
        meta: {
          title: 'Reportes',
          module: 'reports',
          roles: ['Administrador', 'Supervisor'],
        },
      },

      {
        path: 'users',
        name: 'users',
        component: () => import('@/views/admin/UsersView.vue'),
        meta: {
          title: 'Usuarios',
          module: 'users',
          roles: ['Administrador'],
        },
      },
      {
        path: 'roles',
        name: 'roles',
        component: () => import('@/views/admin/RolesView.vue'),
        meta: {
          title: 'Roles y permisos',
          module: 'roles',
          roles: ['Administrador'],
        },
      },

      {
        path: 'profile',
        name: 'profile',
        component: () => import('@/views/profile/ProfileView.vue'),
        meta: { title: 'Mi perfil' },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

router.beforeEach(async (to, _from) => {
  document.title = to.meta.title
    ? `${to.meta.title} — TransFleet`
    : 'TransFleet'

  const authStore = useAuthStore()

  authStore.restoreSession()

  const isAuthenticated = authStore.isAuthenticated
  const requiresAuth = to.meta.requiresAuth !== false

  if (!requiresAuth && isAuthenticated) {
    return { name: 'dashboard' }
  }

  if (requiresAuth && !isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (requiresAuth && isAuthenticated && to.meta.roles?.length) {
    const userRole = authStore.currentRole
    if (!to.meta.roles.includes(userRole)) {
      return { name: 'dashboard' }
    }
  }

  return true
})

export default router