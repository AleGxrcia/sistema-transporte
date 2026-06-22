import apiClient from './api'

export const FuelService = {
    getHistory(params = {}) {
        return apiClient.get('/fuel/history', { params })
    },

    getSummary(year, month) {
        return apiClient.get('/fuel/summary', { params: { year, month } })
    },
}
