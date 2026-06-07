import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/',         redirect: '/login' },
    { path: '/login',    name: 'login',    component: () => import('@/views/auth/LoginView.vue') },
    { path: '/recover',  name: 'recover',  component: () => import('@/views/auth/RecoverPasswordView.vue') },
    { path: '/register', name: 'register', component: () => import('@/views/auth/RegisterView.vue') },
  ]
})

export default router