import { defineStore } from 'pinia'
import { ref } from 'vue'
import { FuelService } from '@/services/fuel.service'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'

export const useFuelStore = defineStore('fuel', () => {
  const history = ref([])
  const isLoadingHistory = ref(false)
  const historyError = ref(null)

  const summary = ref(null)
  const isLoadingSummary = ref(false)
  const summaryError = ref(null)

  async function fetchHistory(params = {}) {
    try {
      isLoadingHistory.value = true
      historyError.value = null
      const response = await FuelService.getHistory(params)
      history.value = response.data
    } catch (err) {
      historyError.value = getErrorMessage(err, 'Error al cargar el historial de combustible')
      history.value = []
    } finally {
      isLoadingHistory.value = false
    }
  }

  async function fetchSummary(year, month) {
    try {
      isLoadingSummary.value = true
      summaryError.value = null
      const response = await FuelService.getSummary(year, month)
      summary.value = response.data
    } catch (err) {
      summaryError.value = getErrorMessage(err, 'Error al cargar el resumen de combustible')
      summary.value = null
    } finally {
      isLoadingSummary.value = false
    }
  }

  async function refreshAll(year, month) {
    await Promise.all([fetchHistory({ year, month }), fetchSummary(year, month)])
  }

  async function registerFuel(vehicleId, data) {
    await VehiclesService.registerFuel(vehicleId, data)
  }

  return {
    history,
    isLoadingHistory,
    historyError,
    fetchHistory,

    summary,
    isLoadingSummary,
    summaryError,
    fetchSummary,

    refreshAll,
    registerFuel,
  }
})
