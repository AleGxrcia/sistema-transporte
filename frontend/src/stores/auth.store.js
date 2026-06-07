import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const user  = ref(null)
  const token = ref(localStorage.getItem('tf_token') || null)
  const role  = ref(localStorage.getItem('tf_role')  || null)

  const isAuthenticated = computed(() => !!token.value)

  async function login(email, password) {
    const mockUsers = {
      'admin@fleetpro.com':      { name: 'Admin',      role: 'Administrador', token: 'mock-admin-token' },
      'supervisor@fleetpro.com': { name: 'Supervisor', role: 'Supervisor',    token: 'mock-super-token' },
      'operador@fleetpro.com':   { name: 'Operador',   role: 'Operador',      token: 'mock-oper-token'  },
    }
    const found = mockUsers[email]
    if (!found || password !== '12345678') throw new Error('Credenciales incorrectas')

    token.value = found.token
    role.value  = found.role
    user.value  = { name: found.name, email, role: found.role }

    localStorage.setItem('tf_token', found.token)
    localStorage.setItem('tf_role',  found.role)
  }

  function logout() {
    user.value  = null
    token.value = null
    role.value  = null
    localStorage.removeItem('tf_token')
    localStorage.removeItem('tf_role')
  }

  return { user, token, role, isAuthenticated, login, logout }
})