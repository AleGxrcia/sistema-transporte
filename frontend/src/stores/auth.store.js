import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import AuthService from '@/services/auth.service.js'

const KEYS = {
  ACCESS_TOKEN:  'tf_access_token',
  REFRESH_TOKEN: 'tf_refresh_token',
  USER:          'tf_user',
}

export const useAuthStore = defineStore('auth', () => {
  const accessToken  = ref(localStorage.getItem(KEYS.ACCESS_TOKEN)  || null)
  const refreshToken = ref(localStorage.getItem(KEYS.REFRESH_TOKEN) || null)
  const user         = ref(_loadUser())

  const isAuthenticated = computed(() => !!accessToken.value)
  const currentRole = computed(() => user.value?.role ?? null)
  const currentUser     = computed(() => user.value)

  async function login(email, password) {
    const result = await AuthService.login(email, password)
    _applyAuthResult(result)
  }

  async function refreshAccessToken() {
    if (!refreshToken.value) {
      throw new Error('No hay refresh token disponible')
    }

    const result = await AuthService.refresh(refreshToken.value)
    _applyAuthResult(result)
    return result.jwtToken
  }

  async function logout() {
    if (refreshToken.value) {
      try {
        await AuthService.logout(refreshToken.value)
      } catch {
      }
    }
    _clearAuth()
  }

  function restoreSession() {
    const storedToken = localStorage.getItem(KEYS.ACCESS_TOKEN)
    const storedUser  = _loadUser()

    if (storedToken && storedUser) {
      accessToken.value  = storedToken
      refreshToken.value = localStorage.getItem(KEYS.REFRESH_TOKEN)
      user.value         = storedUser
    } else {
      _clearAuth()
    }
  }

  function _applyAuthResult(result) {
    const ROLE_MAP = { 0: 'Administrador', 1: 'Supervisor', 2: 'Operador' }
    accessToken.value  = result.jwtToken
    refreshToken.value = result.refreshToken
    user.value = {
      id:       result.userId,
      email:    result.email,
      role:     ROLE_MAP[result.role] ?? 'Operador',
      fullName: result.fullName ?? result.email,
    }

    localStorage.setItem(KEYS.ACCESS_TOKEN,  result.jwtToken)
    localStorage.setItem(KEYS.REFRESH_TOKEN, result.refreshToken)
    localStorage.setItem(KEYS.USER, JSON.stringify(user.value))
  }

  function _clearAuth() {
    accessToken.value  = null
    refreshToken.value = null
    user.value         = null

    localStorage.removeItem(KEYS.ACCESS_TOKEN)
    localStorage.removeItem(KEYS.REFRESH_TOKEN)
    localStorage.removeItem(KEYS.USER)
  }

  function _loadUser() {
    try {
      const raw = localStorage.getItem(KEYS.USER)
      return raw ? JSON.parse(raw) : null
    } catch {
      return null
    }
  }

  return {
    accessToken,
    refreshToken,
    user,
    isAuthenticated,
    currentRole,
    currentUser,
    login,
    logout,
    refreshAccessToken,
    restoreSession,
  }
})