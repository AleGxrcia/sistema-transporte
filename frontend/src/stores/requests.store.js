import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { RequestsService } from '@/services/requests.service'
import { getErrorMessage } from '@/utils/apiError'

export const useRequestsStore = defineStore('requests', () => {
  const requests = ref([])
  const isLoadingList = ref(false)
  const listError = ref(null)

  const pendingRequests = ref([])
  const isLoadingPending = ref(false)

  const currentRequest = ref(null)
  const isLoadingDetail = ref(false)
  const detailError = ref(null)

  const pendingCount = computed(() => pendingRequests.value.length)

  async function fetchAll() {
    try {
      isLoadingList.value = true
      listError.value = null
      const response = await RequestsService.list()
      requests.value = response.data
    } catch (err) {
      listError.value = getErrorMessage(err, 'Error al cargar solicitudes')
      requests.value = []
    } finally {
      isLoadingList.value = false
    }
  }

  async function fetchPending() {
    try {
      isLoadingPending.value = true
      const response = await RequestsService.listPending()
      pendingRequests.value = response.data
    } catch {
      pendingRequests.value = []
    } finally {
      isLoadingPending.value = false
    }
  }

  async function fetchById(id) {
    try {
      isLoadingDetail.value = true
      detailError.value = null
      const response = await RequestsService.getById(id)
      currentRequest.value = response.data
    } catch (err) {
      detailError.value = getErrorMessage(err, 'Error al cargar la solicitud')
      currentRequest.value = null
    } finally {
      isLoadingDetail.value = false
    }
  }

  function clearCurrent() {
    currentRequest.value = null
    detailError.value = null
  }

  return {
    requests,
    isLoadingList,
    listError,
    fetchAll,

    pendingRequests,
    isLoadingPending,
    pendingCount,
    fetchPending,

    currentRequest,
    isLoadingDetail,
    detailError,
    fetchById,
    clearCurrent,
  }
})
