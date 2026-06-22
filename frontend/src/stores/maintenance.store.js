import { defineStore } from 'pinia'
import { ref } from 'vue'
import { MaintenanceService } from '@/services/maintenance.service'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'

export const useMaintenanceStore = defineStore('maintenance', () => {
  const history = ref([])
  const isLoadingHistory = ref(false)
  const historyError = ref(null)

  const scheduled = ref([])
  const isLoadingScheduled = ref(false)
  const scheduledError = ref(null)

  const alerts = ref([])
  const isLoadingAlerts = ref(false)
  const alertsError = ref(null)

  async function fetchHistory(vehicleId = null) {
    try {
      isLoadingHistory.value = true
      historyError.value = null
      const response = await MaintenanceService.getHistory(vehicleId)
      history.value = response.data
    } catch (err) {
      historyError.value = getErrorMessage(err, 'Error al cargar el historial de mantenimiento')
      history.value = []
    } finally {
      isLoadingHistory.value = false
    }
  }

  async function fetchScheduled(onlyPending = true) {
    try {
      isLoadingScheduled.value = true
      scheduledError.value = null
      const response = await MaintenanceService.getScheduled(onlyPending)
      scheduled.value = response.data
    } catch (err) {
      scheduledError.value = getErrorMessage(err, 'Error al cargar los mantenimientos programados')
      scheduled.value = []
    } finally {
      isLoadingScheduled.value = false
    }
  }

  async function fetchAlerts(withinDays = 15) {
    try {
      isLoadingAlerts.value = true
      alertsError.value = null
      const response = await MaintenanceService.getAlerts(withinDays)
      alerts.value = response.data
    } catch (err) {
      alertsError.value = getErrorMessage(err, 'Error al cargar las alertas de mantenimiento')
      alerts.value = []
    } finally {
      isLoadingAlerts.value = false
    }
  }

  async function refreshAll() {
    await Promise.all([fetchHistory(), fetchScheduled(), fetchAlerts()])
  }

  async function scheduleMaintenance(data) {
    await MaintenanceService.schedule(data)
    await refreshAll()
  }

  async function cancelScheduled(vehicleId, id, reason) {
    await MaintenanceService.cancelScheduled(vehicleId, id, reason)
    await refreshAll()
  }

  async function executeScheduled(vehicleId, id, data) {
    await MaintenanceService.executeScheduled(vehicleId, id, data)
    await refreshAll()
  }

  async function registerNow(vehicleId, data) {
    await VehiclesService.registerMaintenance(vehicleId, data)
    await refreshAll()
  }

  async function closeMaintenance(vehicleId, recordId, data) {
    await VehiclesService.closeMaintenance(vehicleId, recordId, data)
    await refreshAll()
  }

  return {
    history,
    isLoadingHistory,
    historyError,
    fetchHistory,

    scheduled,
    isLoadingScheduled,
    scheduledError,
    fetchScheduled,

    alerts,
    isLoadingAlerts,
    alertsError,
    fetchAlerts,

    refreshAll,
    scheduleMaintenance,
    cancelScheduled,
    executeScheduled,
    registerNow,
    closeMaintenance,
  }
})
