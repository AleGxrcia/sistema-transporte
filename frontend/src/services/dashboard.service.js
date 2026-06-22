import apiClient from './api'

export const DashboardService = {
  summary() {
    return apiClient.get('/dashboard/summary')
  },
}
