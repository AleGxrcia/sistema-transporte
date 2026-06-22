import { defineStore } from 'pinia'
import { ref } from 'vue'
import { ScheduleService } from '@/services/schedule.service'
import { getErrorMessage } from '@/utils/apiError'

export const useScheduleStore = defineStore('schedule', () => {
  const assignments = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  async function fetchRange(from, to) {
    try {
      isLoading.value = true
      error.value = null
      const response = await ScheduleService.getRange(from, to)
      assignments.value = response.data ?? []
    } catch (err) {
      error.value = getErrorMessage(err, 'Error al cargar la agenda')
      assignments.value = []
    } finally {
      isLoading.value = false
    }
  }

  return {
    assignments,
    isLoading,
    error,
    fetchRange,
  }
})
