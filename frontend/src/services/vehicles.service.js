import apiClient from './api'

export const VehiclesService = {
    list(params = {}) {
        return apiClient.get('/vehicles', { params })
    },

    available(minPassengers = 1, params = {}) {
        return apiClient.get('/vehicles/available', { params: { minPassengers, ...params } })
    },

    getById(id) {
        return apiClient.get(`/vehicles/${id}`)
    },

    create(data) {
        return apiClient.post('/vehicles', data)
    },

    update(id, data) {
        return apiClient.put(`/vehicles/${id}`, data)
    },

    delete(id) {
        return apiClient.delete(`/vehicles/${id}`)
    },

    deactivate(id) {
        return apiClient.patch(`/vehicles/${id}/deactivate`)
    },

    reactivate(id) {
        return apiClient.patch(`/vehicles/${id}/reactivate`)
    },

    restore(id) {
        return apiClient.patch(`/vehicles/${id}/restore`)
    },

    registerMaintenance(id, data) {
        return apiClient.post(`/vehicles/${id}/maintenance`, data)
    },

    updateMaintenance(id, recordId, data) {
        return apiClient.put(`/vehicles/${id}/maintenance/${recordId}`, data)
    },

    deleteMaintenance(id, recordId) {
        return apiClient.delete(`/vehicles/${id}/maintenance/${recordId}`)
    },

    closeMaintenance(id, recordId, data) {
        return apiClient.patch(`/vehicles/${id}/maintenance/${recordId}/close`, data)
    },

    registerFuel(id, data) {
        return apiClient.post(`/vehicles/${id}/fuel`, data)
    },

    updateFuel(id, recordId, data) {
        return apiClient.put(`/vehicles/${id}/fuel/${recordId}`, data)
    },

    deleteFuel(id, recordId) {
        return apiClient.delete(`/vehicles/${id}/fuel/${recordId}`)
    },
}
