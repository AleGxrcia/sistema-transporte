import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store.js'

const PERMISSIONS = {
  Administrador: {
    dashboard:    ['view'],
    vehicles:     ['view', 'create', 'edit', 'delete'],
    drivers:      ['view', 'create', 'edit', 'delete'],
    requests:     ['view', 'create', 'edit', 'delete', 'approve'],
    schedules:    ['view', 'create', 'edit', 'delete'],
    reports:      ['view', 'export'],
    maintenance:  ['view', 'create', 'edit', 'delete'],
    users:        ['view', 'create', 'edit', 'delete'],
    roles:        ['view', 'edit'],
    fuel:         ['view', 'create', 'edit'],
  },
  Supervisor: {
    dashboard:    ['view'],
    vehicles:     ['view'],
    drivers:      ['view'],
    requests:     ['view', 'create', 'edit', 'approve'],
    schedules:    ['view', 'create', 'edit'],
    reports:      ['view', 'export'],
    maintenance:  ['view', 'create'],
    fuel:         ['view', 'create'],
  },
  Operador: {
    dashboard:    ['view'],
    vehicles:     ['view'],
    drivers:      ['view'],
    requests:     ['view', 'create'],
    schedules:    ['view'],
    fuel:         ['view'],
  },
}

export function useAuth() {
  const authStore = useAuthStore()
  const router    = useRouter()

  const { user, isAuthenticated, currentRole, accessToken } = storeToRefs(authStore)

  function can(action, module) {
    const role = currentRole.value
    if (!role) return false
    return PERMISSIONS[role]?.[module]?.includes(action) ?? false
  }

  async function login(email, password) {
    await authStore.login(email, password)
    await router.push({ name: 'dashboard' })
  }

  async function logout() {
    await authStore.logout()
    await router.push({ name: 'login' })
  }

  return {
    user,
    isAuthenticated,
    currentRole,
    accessToken,
    can,
    login,
    logout,
  }
}