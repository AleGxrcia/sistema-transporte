import { defineStore } from 'pinia'
import { ref } from 'vue'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'

export const useVehicleStore = defineStore('vehicles', () => {
  const vehicles = ref([])
  const isLoadingList = ref(false)
  const listError = ref(null)

  const currentVehicle = ref(null)
  const isLoadingDetail = ref(false)
  const detailError = ref(null)

  async function fetchAll(archived = false) {
    try {
      isLoadingList.value = true
      listError.value = null
      const response = await VehiclesService.list({ archived })
      vehicles.value = response.data
    } catch (err) {
      listError.value = getErrorMessage(err, 'Error al cargar vehículos')
      vehicles.value = []
    } finally {
      isLoadingList.value = false
    }
  }

  async function fetchById(id) {
    try {
      isLoadingDetail.value = true
      detailError.value = null
      const response = await VehiclesService.getById(id)
      currentVehicle.value = response.data
    } catch (err) {
      detailError.value = getErrorMessage(err, 'Error al cargar vehículo')
      currentVehicle.value = null
    } finally {
      isLoadingDetail.value = false
    }
  }

  function clearCurrent() {
    currentVehicle.value = null
    detailError.value = null
  }

  return {
    vehicles,
    isLoadingList,
    listError,
    fetchAll,
    currentVehicle,
    isLoadingDetail,
    detailError,
    fetchById,
    clearCurrent,
  }
})
