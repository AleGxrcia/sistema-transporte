import apiClient from '@/services/api.js'

const AuthService = {
  async login(email, password) {
    const { data } = await apiClient.post('/auth/login', { email, password })
    return data
  },

  async refresh(refreshToken) {
    const { data } = await apiClient.post('/auth/refresh', { refreshToken })
    return data
  },

  async logout(refreshToken) {
    await apiClient.post('/auth/logout', { refreshToken })
  },

  async confirmEmail(userId, token) {
    const { data } = await apiClient.post('/auth/confirm-email', { userId, token })
    return data
  },

  async forgotPassword(email) {
    await apiClient.post('/auth/forgot-password', { email })
  },

  async resetPassword(email, token, newPassword) {
    await apiClient.post('/auth/reset-password', { email, token, newPassword })
  },
}

export default AuthService
