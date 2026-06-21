import { ref, reactive, watch, onMounted } from 'vue'

/**
 * Composable genérico para listados CRUD con filtros y paginación.
 * 
 * @param {Function} fetchFn - Función API que recibe params y retorna { data, pagination }
 * @param {Object} defaultFilters - Filtros iniciales (ej: { statusId: null, search: '' })
 */
export function useCrudList(fetchFn, defaultFilters = {}) {
  const items = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  const filters = reactive({
    search: '',
    page: 1,
    size: 20,
    ...defaultFilters,
  })

  const pagination = reactive({
    page: 1,
    size: 20,
    total: 0,
    totalPages: 0,
  })

  async function fetchItems() {
    try {
      isLoading.value = true
      error.value = null
      
      // Limpiar filtros vacíos antes de enviar
      const params = {}
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== null && value !== '' && value !== undefined) {
          params[key] = value
        }
      })

      const response = await fetchFn(params)
      items.value = response.data.data
      Object.assign(pagination, response.data.pagination)
    } catch (err) {
      error.value = err.response?.data?.message || 'Error al cargar datos'
      items.value = []
    } finally {
      isLoading.value = false
    }
  }

  // Recargar cuando cambien los filtros (excepto page)
  watch(
    () => ({ ...filters, page: undefined }),  // Excluir page del trigger
    () => {
      filters.page = 1  // Resetear a página 1 al filtrar
      fetchItems()
    },
    { deep: true }
  )

  // Recargar cuando cambie la página
  watch(() => filters.page, fetchItems)

  // Cargar datos iniciales
  onMounted(fetchItems)

  function nextPage() {
    if (filters.page < pagination.totalPages) filters.page++
  }

  function prevPage() {
    if (filters.page > 1) filters.page--
  }

  function refresh() {
    fetchItems()
  }

  return {
    items,
    isLoading,
    error,
    filters,
    pagination,
    nextPage,
    prevPage,
    refresh,
  }
}
