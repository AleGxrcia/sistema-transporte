export default [
  { path: '/',        redirect: '/dashboard' },
  { path: '/login',    name: 'login',    component: () => import('@/views/auth/LoginView.vue'),           meta: { requiresAuth: false } },
  { path: '/recover',  name: 'recover',  component: () => import('@/views/auth/RecoverPasswordView.vue'), meta: { requiresAuth: false } },
  { path: '/register', name: 'register', component: () => import('@/views/auth/RegisterView.vue'),        meta: { requiresAuth: false } },
]