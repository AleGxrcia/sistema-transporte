import { defineStore } from 'pinia'
import { ref } from 'vue'
import { ReportsService } from '@/services/reports.service'
import { getErrorMessage } from '@/utils/apiError'

function triggerDownload(blob, filename) {
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  document.body.appendChild(link)
  link.click()
  link.remove()
  window.URL.revokeObjectURL(url)
}

export const useReportsStore = defineStore('reports', () => {
  const summary = ref(null)
  const isLoading = ref(false)
  const error = ref(null)

  const isExporting = ref(false)

  async function fetchSummary(year, month) {
    try {
      isLoading.value = true
      error.value = null
      const response = await ReportsService.getSummary(year, month)
      summary.value = response.data
    } catch (err) {
      error.value = getErrorMessage(err, 'Error al cargar el reporte')
      summary.value = null
    } finally {
      isLoading.value = false
    }
  }

  async function downloadExcel(year, month) {
    try {
      isExporting.value = true
      const response = await ReportsService.exportExcel(year, month)
      triggerDownload(response.data, `reporte-${year}-${String(month).padStart(2, '0')}.xlsx`)
    } finally {
      isExporting.value = false
    }
  }

  async function downloadPdf(year, month) {
    try {
      isExporting.value = true
      const response = await ReportsService.exportPdf(year, month)
      triggerDownload(response.data, `reporte-${year}-${String(month).padStart(2, '0')}.pdf`)
    } finally {
      isExporting.value = false
    }
  }

  return {
    summary,
    isLoading,
    error,
    isExporting,
    fetchSummary,
    downloadExcel,
    downloadPdf,
  }
})
