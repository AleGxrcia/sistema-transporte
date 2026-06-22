import apiClient from './api'

export const MaintenanceService = {
    getHistory(vehicleId = null) {
        return apiClient.get('/maintenance/history', { params: vehicleId ? { vehicleId } : {} })
    },

    getScheduled(onlyPending = true) {
        return apiClient.get('/maintenance/scheduled', { params: { onlyPending } })
    },

    getAlerts(withinDays = 15) {
        return apiClient.get('/maintenance/alerts', { params: { withinDays } })
    },

    schedule(data) {
        return apiClient.post('/maintenance/scheduled', data)
    },

    cancelScheduled(vehicleId, id, reason) {
        return apiClient.patch(`/maintenance/scheduled/${vehicleId}/${id}/cancel`, { reason })
    },

    executeScheduled(vehicleId, id, data) {
        return apiClient.post(`/maintenance/scheduled/${vehicleId}/${id}/execute`, data)
    },
}
