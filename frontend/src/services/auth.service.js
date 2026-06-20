import apiClient from './api'

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
}

export default AuthService