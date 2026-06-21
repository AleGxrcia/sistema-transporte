import apiClient from './api'

export const RequestsService = {
  list(params = {}) {
    return apiClient.get('/requests/all', { params })
  },

  listPending() {
    return apiClient.get('/requests')
  },

  getById(id) {
    return apiClient.get(`/requests/${id}`)
  },

  create(data) {
    return apiClient.post('/requests', data)
  },

  approve(id) {
    return apiClient.patch(`/requests/${id}/approve`)
  },

  reject(id, reason) {
    return apiClient.patch(`/requests/${id}/reject`, { reason })
  },

  assign(id, vehicleId, driverId) {
    return apiClient.patch(`/requests/${id}/assign`, { vehicleId, driverId })
  },

  cancel(id, reason) {
    return apiClient.patch(`/requests/${id}/cancel`, { reason })
  },
}
