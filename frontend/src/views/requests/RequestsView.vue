<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Eye, Plus } from '@lucide/vue'
import { formatDate } from '@/utils/formatters'
import { REQUEST_STATUSES } from '@/utils/enumLabels'

const router = useRouter()
const { can } = useAuth()
const requestsStore = useRequestsStore()

const search = ref('')
const statusFilter = ref('')

onMounted(() => requestsStore.fetchAll())

const requests = computed(() => requestsStore.requests)
const isLoading = computed(() => requestsStore.isLoadingList)

const tabs = computed(() => {
  const counts = { '': requests.value.length }
  for (const s of REQUEST_STATUSES) {
    counts[s.name] = requests.value.filter((r) => r.status === s.name).length
  }
  return [
    { key: '', label: 'Todas', count: counts[''] },
    ...REQUEST_STATUSES.map((s) => ({ key: s.name, label: s.label, count: counts[s.name] })),
  ]
})

const filteredRequests = computed(() => {
  let result = requests.value
  if (statusFilter.value) result = result.filter((r) => r.status === statusFilter.value)
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    result = result.filter(
      (r) =>
        r.requestingArea?.toLowerCase().includes(q) ||
        r.destination?.toLowerCase().includes(q) ||
        r.requestNumber?.toLowerCase().includes(q)
    )
  }
  return result
})

const hasActiveFilters = computed(() => !!(search.value || statusFilter.value))
</script>

<template>
  <div>
    <div class="page-header">
      <h1>Solicitudes de transporte</h1>
      <button v-if="can('create', 'requests')" class="btn primary" @click="router.push('/requests/new')">
        <Plus :size="14" /> Nueva solicitud
      </button>
    </div>

    <div class="page-tabs">
      <span
        v-for="tab in tabs"
        :key="tab.key"
        class="pt"
        :class="{ active: statusFilter === tab.key }"
        @click="statusFilter = tab.key"
      >
        {{ tab.label }} ({{ tab.count }})
      </span>
    </div>

    <div class="search-row">
      <input v-model="search" class="search-input" placeholder="Buscar por área, destino o número…" />
    </div>

    <div v-if="isLoading" class="loading-placeholder">Cargando solicitudes…</div>

    <div v-else-if="requestsStore.listError" class="alert red">{{ requestsStore.listError }}</div>

    <div v-else-if="filteredRequests.length === 0" class="empty-state">
      <div class="empty-icon empty-icon--blue">
        <svg width="30" height="30" fill="none" stroke="var(--blue)" stroke-width="1.5" viewBox="0 0 24 24">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
          <polyline points="14 2 14 8 20 8"/>
        </svg>
      </div>
      <p class="empty-title">No hay solicitudes</p>
      <p class="empty-sub">
        {{
          hasActiveFilters
            ? 'No se encontraron solicitudes con los filtros aplicados.'
            : 'Aún no se ha creado ninguna solicitud de transporte.'
        }}
      </p>
      <button
        v-if="can('create', 'requests') && !hasActiveFilters"
        class="btn primary"
        @click="router.push('/requests/new')"
      >
        <Plus :size="14" /> Crear primera solicitud
      </button>
    </div>

    <div v-else class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>N°</th>
            <th>Área</th>
            <th>Destino</th>
            <th>Fecha</th>
            <th>Horario</th>
            <th>Pasajeros</th>
            <th>Estado</th>
            <th style="text-align:center">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="req in filteredRequests" :key="req.id">
            <td class="plate-cell">{{ req.requestNumber }}</td>
            <td>{{ req.requestingArea }}</td>
            <td>{{ req.destination }}</td>
            <td class="muted">{{ formatDate(req.departureDateTime) }}</td>
            <td class="muted">{{ formatDate(req.departureDateTime, 'HH:mm') }} — {{ formatDate(req.returnDateTime, 'HH:mm') }}</td>
            <td>{{ req.passengerCount }}</td>
            <td><AppBadge :status="req.status" /></td>
            <td>
              <div class="action-buttons">
                <button class="icon-btn" title="Ver detalle" @click="router.push(`/requests/${req.id}`)">
                  <Eye :size="13" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.loading-placeholder {
  padding: 48px;
  text-align: center;
  color: var(--text-3);
}
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 64px 24px;
  gap: 10px;
}
.empty-icon {
  width: 64px;
  height: 64px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 6px;
}
.empty-icon--blue {
  background: var(--blue-light);
  border: 1px solid var(--blue-mid, #93c5fd);
}
.empty-title {
  font-size: 16px;
  font-weight: 700;
  color: var(--text);
  margin: 0;
}
.empty-sub {
  font-size: 14px;
  color: var(--text-3);
  line-height: 1.6;
  max-width: 360px;
  margin: 0 0 8px;
}
</style>
