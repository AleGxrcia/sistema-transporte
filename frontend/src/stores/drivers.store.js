import { defineStore } from 'pinia'
import { ref } from 'vue'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'

export const useDriverStore = defineStore('drivers', () => {
  const drivers = ref([])
  const isLoadingList = ref(false)
  const listError = ref(null)

  const currentDriver = ref(null)
  const isLoadingDetail = ref(false)
  const detailError = ref(null)

  async function fetchAll(archived = false) {
    try {
      isLoadingList.value = true
      listError.value = null
      const response = await driverApi.list({ archived })
      drivers.value = response.data
    } catch (err) {
      listError.value = getErrorMessage(err, 'Error al cargar conductores')
      drivers.value = []
    } finally {
      isLoadingList.value = false
    }
  }

  async function fetchById(id) {
    try {
      isLoadingDetail.value = true
      detailError.value = null
      const response = await driverApi.getById(id)
      currentDriver.value = response.data
    } catch (err) {
      detailError.value = getErrorMessage(err, 'Error al cargar conductor')
      currentDriver.value = null
    } finally {
      isLoadingDetail.value = false
    }
  }

  function clearCurrent() {
    currentDriver.value = null
    detailError.value = null
  }

  return {
    drivers,
    isLoadingList,
    listError,
    fetchAll,
    currentDriver,
    isLoadingDetail,
    detailError,
    fetchById,
    clearCurrent,
  }
})
