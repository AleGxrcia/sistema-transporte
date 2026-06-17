import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/login' },
    { path: '/login',    name: 'login',    component: () => import('@/views/auth/LoginView.vue') },
    { path: '/recover',  name: 'recover',  component: () => import('@/views/auth/RecoverPasswordView.vue') },
    { path: '/register', name: 'register', component: () => import('@/views/auth/RegisterView.vue') },

    // Rutas con layout
    {
      path: '/dashboard',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'dashboard', component: () => import('@/views/dashboard/DashboardView.vue') },
      ]
    },
    {
      path: '/requests',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'requests', component: () => import('@/views/requests/RequestsView.vue') },
        { path: 'new', name: 'request-new', component: () => import('@/views/requests/RequestNewView.vue') },
      ]
    },
    {
      path: '/trips',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'trips', component: () => import('@/views/trips/TripHistoryView.vue') },
      ]
    },
    {
      path: '/vehicles',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'vehicles', component: () => import('@/views/vehicles/VehiclesView.vue') },
        { path: ':id', name: 'vehicle-detail', component: () => import('@/views/vehicles/VehicleDetailView.vue') },
      ]
    },
    {
      path: '/drivers',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'drivers', component: () => import('@/views/drivers/DriversView.vue') },
        { path: ':id', name: 'driver-detail', component: () => import('@/views/drivers/DriversDetailView.vue') },
      ]
    },
    {
      path: '/calendar',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'calendar', component: () => import('@/views/calendar/CalendarView.vue') },
      ]
    },
    {
      path: '/assignments',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'assignments', component: () => import('@/views/assignments/AssignmentView.vue') },
      ]
    },
    {
      path: '/maintenance',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'maintenance', component: () => import('@/views/maintenance/MaintenanceView.vue') },
      ]
    },
    {
      path: '/fuel',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'fuel', component: () => import('@/views/fuel/FuelView.vue') },
      ]
    },
    {
      path: '/reports',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'reports', component: () => import('@/views/reports/ReportsView.vue') },
      ]
    },
    {
      path: '/admin/users',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'admin-users', component: () => import('@/views/admin/UsersView.vue') },
      ]
    },
    {
      path: '/profile',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'profile', component: () => import('@/views/profile/ProfileView.vue') },
      ]
    },
    {
      path: '/notifications',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'notifications', component: () => import('@/views/notifications/NotificationsView.vue') },
      ]
    },
        {
      path: '/admin/roles',
      component: () => import('@/components/layout/AppLayout.vue'),
      children: [
        { path: '', name: 'admin-roles', component: () => import('@/views/admin/RolesView.vue') },
      ]
    },
  ]
})

export default router