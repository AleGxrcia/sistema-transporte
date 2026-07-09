import apiClient from './api';

export const driverApi = {
  list(params = {}) {
    return apiClient.get('/drivers', { params })
  },

  available(params = {}) {
    return apiClient.get('/drivers/available', { params })
  },

  getById(id) {
    return apiClient.get(`/drivers/${id}`)
  },

  create(data) {
    return apiClient.post('/drivers', data)
  },

  update(id, data) {
    return apiClient.put(`/drivers/${id}`, data)
  },

  delete(id) {
    return apiClient.delete(`/drivers/${id}`)
  },

  suspend(id, reason) {
    return apiClient.patch(`/drivers/${id}/suspend`, { reason })
  },

  reactivate(id) {
    return apiClient.patch(`/drivers/${id}/reactivate`)
  },

  restore(id) {
    return apiClient.patch(`/drivers/${id}/restore`)
  },

  renewLicense(id, data) {
    return apiClient.patch(`/drivers/${id}/license`, data)
  },
}