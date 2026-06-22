import apiClient from './api'

export const ReportsService = {
    getSummary(year, month) {
        return apiClient.get('/reports/summary', { params: { year, month } })
    },

    exportExcel(year, month) {
        return apiClient.get('/reports/export/excel', { params: { year, month }, responseType: 'blob' })
    },

    exportPdf(year, month) {
        return apiClient.get('/reports/export/pdf', { params: { year, month }, responseType: 'blob' })
    },
}
