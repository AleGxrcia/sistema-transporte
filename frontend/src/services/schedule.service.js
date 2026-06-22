import apiClient from './api'

export const ScheduleService = {
  getRange(from, to) {
    return apiClient.get('/schedule', { params: { from, to } })
  },
}
