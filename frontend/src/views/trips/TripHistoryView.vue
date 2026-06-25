<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useScheduleStore } from '@/stores/schedule.store'
import { useRequestsStore } from '@/stores/requests.store'
import { ASSIGNMENT_STATUSES } from '@/utils/enumLabels'
import { formatDate, formatNumber } from '@/utils/formatters'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Route, ListFilter, ChevronDown, Eye, Check, X, Users, Calendar } from '@lucide/vue'

const router = useRouter()
const scheduleStore = useScheduleStore()
const requestsStore = useRequestsStore()

const PAGE_SIZE = 8

const activeTab = ref('lista')
const search = ref('')
const showFilters = ref(false)
const page = ref(1)

const dateFrom = ref(dayjs().subtract(3, 'month').startOf('month').format('YYYY-MM-DD'))
const dateTo = ref(dayjs().format('YYYY-MM-DD'))
const areaFilter = ref('')
const statusFilter = ref('')
const driverFilter = ref('')
const vehicleFilter = ref('')
const destinationFilter = ref('')

const isLoading = computed(() => scheduleStore.isLoading || requestsStore.isLoadingList)

onMounted(() => {
  requestsStore.fetchAll()
  loadRange()
})

function loadRange() {
  scheduleStore.fetchRange(
    dayjs(dateFrom.value).startOf('day').toISOString(),
    dayjs(dateTo.value).endOf('day').toISOString()
  )
}

function applyFilters() {
  page.value = 1
  showFilters.value = false
  loadRange()
}

function clearFilters() {
  areaFilter.value = ''
  statusFilter.value = ''
  driverFilter.value = ''
  vehicleFilter.value = ''
  destinationFilter.value = ''
  dateFrom.value = dayjs().subtract(3, 'month').startOf('month').format('YYYY-MM-DD')
  dateTo.value = dayjs().format('YYYY-MM-DD')
  page.value = 1
  loadRange()
}

const trips = computed(() => {
  const requestsByNumber = new Map(requestsStore.requests.map((r) => [r.requestNumber, r]))
  return scheduleStore.assignments
    .map((a) => {
      const req = requestsByNumber.get(a.requestNumber)
      return {
        id: a.assignmentId,
        requestId: a.requestId,
        code: a.requestNumber,
        area: req?.requestingArea ?? '—',
        destination: a.destination,
        departureTime: a.departureTime,
        returnTime: a.returnTime,
        driver: a.driverFullName,
        vehicle: a.vehiclePlate,
        vehicleDescription: a.vehicleDescription,
        status: a.status,
        cancellationReason: a.cancellationReason,
        passengerCount: req?.passengerCount ?? 0,
      }
    })
    .sort((a, b) => new Date(b.departureTime) - new Date(a.departureTime))
})

const areas = computed(() => [...new Set(trips.value.map((t) => t.area).filter((a) => a !== '—'))].sort())
const driverNames = computed(() => [...new Set(trips.value.map((t) => t.driver).filter(Boolean))].sort())
const vehiclePlates = computed(() => [...new Set(trips.value.map((t) => t.vehicle).filter(Boolean))].sort())

const activeFilterCount = computed(() =>
  [areaFilter.value, statusFilter.value, driverFilter.value, vehicleFilter.value, destinationFilter.value].filter(Boolean).length
)

const filteredTrips = computed(() => {
  let list = trips.value
  if (areaFilter.value) list = list.filter((t) => t.area === areaFilter.value)
  if (statusFilter.value) list = list.filter((t) => t.status === statusFilter.value)
  if (driverFilter.value) list = list.filter((t) => t.driver === driverFilter.value)
  if (vehicleFilter.value) list = list.filter((t) => t.vehicle === vehicleFilter.value)
  if (destinationFilter.value.trim()) {
    const q = destinationFilter.value.trim().toLowerCase()
    list = list.filter((t) => t.destination.toLowerCase().includes(q))
  }
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    list = list.filter((t) =>
      t.destination.toLowerCase().includes(q) ||
      t.area.toLowerCase().includes(q) ||
      t.driver?.toLowerCase().includes(q) ||
      t.code.toLowerCase().includes(q)
    )
  }
  return list
})

const totalPages = computed(() => Math.max(1, Math.ceil(filteredTrips.value.length / PAGE_SIZE)))
const paginatedTrips = computed(() => {
  const start = (page.value - 1) * PAGE_SIZE
  return filteredTrips.value.slice(start, start + PAGE_SIZE)
})
const pageRangeLabel = computed(() => {
  if (filteredTrips.value.length === 0) return 'Mostrando 0 viajes'
  const start = (page.value - 1) * PAGE_SIZE + 1
  const end = Math.min(page.value * PAGE_SIZE, filteredTrips.value.length)
  return `Mostrando ${start}–${end} de ${filteredTrips.value.length} viajes`
})

function goPrevPage() { if (page.value > 1) page.value-- }
function goNextPage() { if (page.value < totalPages.value) page.value++ }

const kpis = computed(() => {
  const total = filteredTrips.value.length
  const completed = filteredTrips.value.filter((t) => t.status === 'Completed').length
  const cancelled = filteredTrips.value.filter((t) => t.status === 'Cancelled').length
  const passengers = filteredTrips.value.reduce((sum, t) => sum + (t.passengerCount || 0), 0)

  const thisMonthCount = trips.value.filter((t) => dayjs(t.departureTime).isSame(dayjs(), 'month')).length
  const lastMonthCount = trips.value.filter((t) => dayjs(t.departureTime).isSame(dayjs().subtract(1, 'month'), 'month')).length
  const trend = lastMonthCount === 0 ? null : Math.round(((thisMonthCount - lastMonthCount) / lastMonthCount) * 100)

  return {
    total,
    completed,
    completedRate: total ? Math.round((completed / total) * 100) : 0,
    cancelled,
    cancelledRate: total ? Math.round((cancelled / total) * 100) : 0,
    passengers,
    thisMonthCount,
    trend,
  }
})

const timelineGroups = computed(() => {
  const groups = new Map()
  for (const trip of filteredTrips.value) {
    const key = dayjs(trip.departureTime).format('YYYY-MM-DD')
    if (!groups.has(key)) groups.set(key, [])
    groups.get(key).push(trip)
  }
  return [...groups.entries()]
    .sort((a, b) => (a[0] < b[0] ? 1 : -1))
    .map(([date, items]) => ({ date, items }))
})

function isToday(date) {
  return dayjs(date).isSame(dayjs(), 'day')
}
function formatGroupDate(date) {
  const d = dayjs(date)
  const label = d.format('D [de] MMMM YYYY')
  return isToday(date) ? `Hoy — ${label}` : label
}
function statusAccent(status) {
  if (status === 'Completed') return 'var(--mint-dark)'
  if (status === 'Cancelled') return 'var(--red)'
  if (status === 'InProgress') return 'var(--blue)'
  return 'var(--amber)'
}

function viewTrip(trip) {
  router.push(`/requests/${trip.requestId}`)
}
</script>

<template>
  <div class="trips-view">
    <div class="page-header">
      <h1>Historial de viajes</h1>
    </div>

    <!-- KPIs -->
    <div class="kpi-grid" style="grid-template-columns:repeat(5,1fr)">
      <div class="kpi">
        <div class="kpi-label">Total viajes</div>
        <div class="kpi-val">{{ kpis.total }}</div>
        <div class="kpi-sub">En el rango seleccionado</div>
        <div class="kpi-icon blue"><Route :size="15" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Completados</div>
        <div class="kpi-val" style="color:var(--mint-dark)">{{ kpis.completed }}</div>
        <div class="kpi-sub">{{ kpis.completedRate }}% de tasa</div>
        <div class="kpi-icon green"><Check :size="15" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Cancelados</div>
        <div class="kpi-val" style="color:var(--red)">{{ kpis.cancelled }}</div>
        <div class="kpi-sub">{{ kpis.cancelledRate }}% del total</div>
        <div class="kpi-icon red"><X :size="15" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Pasajeros movilizados</div>
        <div class="kpi-val">{{ formatNumber(kpis.passengers) }}</div>
        <div class="kpi-sub">Toda la selección</div>
        <div class="kpi-icon amber"><Users :size="15" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Este mes</div>
        <div class="kpi-val">{{ kpis.thisMonthCount }}</div>
        <div v-if="kpis.trend !== null" class="kpi-trend" :class="kpis.trend >= 0 ? 'up' : 'down'">
          {{ kpis.trend >= 0 ? '↑' : '↓' }} {{ Math.abs(kpis.trend) }}% vs anterior
        </div>
        <div v-else class="kpi-sub">Sin datos del mes anterior</div>
        <div class="kpi-icon sky"><Calendar :size="15" /></div>
      </div>
    </div>

    <!-- Filtros avanzados colapsables -->
    <div class="filters-panel">
      <div class="filters-toggle" @click="showFilters = !showFilters">
        <div class="filters-toggle-left">
          <ListFilter :size="14" />
          <span>Filtros</span>
          <span v-if="activeFilterCount > 0" class="filter-badge">{{ activeFilterCount }} activo{{ activeFilterCount > 1 ? 's' : '' }}</span>
        </div>
        <div class="filters-toggle-right">
          <span>{{ showFilters ? 'Contraer' : 'Expandir' }}</span>
          <ChevronDown :size="14" class="chevron" :class="{ rotated: showFilters }" />
        </div>
      </div>
      <div v-if="showFilters" class="filters-body">
        <div class="form-grid" style="grid-template-columns:repeat(4,1fr)">
          <div class="form-group"><label>Fecha desde</label><input v-model="dateFrom" type="date" /></div>
          <div class="form-group"><label>Fecha hasta</label><input v-model="dateTo" type="date" /></div>
          <div class="form-group">
            <label>Área</label>
            <select v-model="areaFilter">
              <option value="">Todas las áreas</option>
              <option v-for="a in areas" :key="a" :value="a">{{ a }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Estado</label>
            <select v-model="statusFilter">
              <option value="">Todos</option>
              <option v-for="s in ASSIGNMENT_STATUSES" :key="s.name" :value="s.name">{{ s.label }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Conductor</label>
            <select v-model="driverFilter">
              <option value="">Todos los conductores</option>
              <option v-for="d in driverNames" :key="d" :value="d">{{ d }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Vehículo</label>
            <select v-model="vehicleFilter">
              <option value="">Todos los vehículos</option>
              <option v-for="v in vehiclePlates" :key="v" :value="v">{{ v }}</option>
            </select>
          </div>
          <div class="form-group"><label>Destino</label><input v-model="destinationFilter" type="text" placeholder="Ciudad o dirección…" /></div>
          <div class="form-group">
            <label style="opacity:0">-</label>
            <div class="filters-actions">
              <button class="btn" @click="clearFilters">Limpiar</button>
              <button class="btn primary" @click="applyFilters">Aplicar</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabs + búsqueda -->
    <div class="tabs-row">
      <div class="page-tabs" style="margin-bottom:0">
        <div class="pt" :class="{ active: activeTab === 'lista' }" @click="activeTab = 'lista'">Vista lista</div>
        <div class="pt" :class="{ active: activeTab === 'timeline' }" @click="activeTab = 'timeline'">Vista timeline</div>
      </div>
      <div class="tabs-row-right">
        <input v-model="search" class="search-input" style="width:240px" placeholder="Buscar por destino, área, conductor…" />
        <span class="results-count">{{ filteredTrips.length }} resultados</span>
      </div>
    </div>

    <div v-if="isLoading" class="loading-placeholder">Cargando historial de viajes…</div>

    <div v-else-if="filteredTrips.length === 0" class="empty-state">
      <div class="empty-icon empty-icon--blue">
        <Route :size="28" stroke="var(--blue)" stroke-width="1.5" />
      </div>
      <p class="empty-title">No hay viajes para mostrar</p>
      <p class="empty-sub">Ajusta los filtros o el rango de fechas para ver resultados.</p>
    </div>

    <template v-else>
      <!-- Vista lista -->
      <div v-if="activeTab === 'lista'">
        <div class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Área</th>
                <th>Destino</th>
                <th>Fecha</th>
                <th>Horario</th>
                <th>Conductor</th>
                <th>Vehículo</th>
                <th>Estado</th>
                <th style="text-align:center">Detalle</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="trip in paginatedTrips" :key="trip.id">
                <td class="muted" style="font-weight:700">{{ trip.code }}</td>
                <td style="font-weight:600">{{ trip.area }}</td>
                <td>{{ trip.destination }}</td>
                <td class="muted">{{ formatDate(trip.departureTime) }}</td>
                <td class="muted">{{ formatDate(trip.departureTime, 'HH:mm') }}–{{ formatDate(trip.returnTime, 'HH:mm') }}</td>
                <td>{{ trip.driver || '—' }}</td>
                <td><span class="plate-cell">{{ trip.vehicle || '—' }}</span></td>
                <td><AppBadge :status="trip.status" /></td>
                <td style="text-align:center">
                  <button class="icon-btn" style="margin:auto" title="Ver solicitud" @click="viewTrip(trip)">
                    <Eye :size="13" />
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="pager">
          <span class="pager-info">{{ pageRangeLabel }}</span>
          <div class="pager-btns">
            <button class="pager-btn" :disabled="page === 1" @click="goPrevPage">← Anterior</button>
            <button class="pager-btn" :disabled="page === totalPages" @click="goNextPage">Siguiente →</button>
          </div>
        </div>
      </div>

      <!-- Vista timeline -->
      <div v-else class="timeline">
        <div class="timeline-line"></div>
        <div v-for="group in timelineGroups" :key="group.date" class="timeline-group">
          <div class="timeline-date" :class="{ today: isToday(group.date) }">{{ formatGroupDate(group.date) }}</div>
          <div v-for="trip in group.items" :key="trip.id" class="timeline-item">
            <div class="timeline-dot" :style="{ background: statusAccent(trip.status) }"></div>
            <div class="card timeline-card" :style="{ borderLeftColor: statusAccent(trip.status) }" @click="viewTrip(trip)">
              <div class="timeline-card-row">
                <div class="timeline-card-main">
                  <div class="timeline-card-title">{{ trip.area }} → {{ trip.destination }}</div>
                  <div class="timeline-card-meta">
                    {{ formatDate(trip.departureTime, 'HH:mm') }} — {{ formatDate(trip.returnTime, 'HH:mm') }}
                    · {{ trip.driver || '—' }} · {{ trip.vehicle || '—' }}
                  </div>
                  <div v-if="trip.cancellationReason" class="timeline-card-cancel">Cancelado: {{ trip.cancellationReason }}</div>
                </div>
                <AppBadge :status="trip.status" />
                <span class="timeline-card-code">{{ trip.code }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
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

/* Filtros */
.filters-panel {
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 10px;
  margin-bottom: 16px;
  overflow: hidden;
}
.filters-toggle {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  cursor: pointer;
  user-select: none;
  color: var(--text-2);
}
.filters-toggle-left {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
  color: var(--text);
}
.filter-badge {
  background: var(--blue);
  color: var(--white);
  font-size: 11px;
  font-weight: 700;
  padding: 1px 7px;
  border-radius: 10px;
}
.filters-toggle-right {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--blue);
}
.chevron {
  transition: transform 0.2s;
  color: var(--text-3);
}
.chevron.rotated {
  transform: rotate(180deg);
}
.filters-body {
  padding: 14px 16px 16px;
  border-top: 1px solid var(--border);
}
.filters-actions {
  display: flex;
  gap: 6px;
  margin-top: 1px;
}
.filters-actions .btn {
  flex: 1;
  font-size: 13px;
}

/* Tabs + búsqueda */
.tabs-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
  flex-wrap: wrap;
  gap: 8px;
}
.tabs-row-right {
  display: flex;
  gap: 8px;
  align-items: center;
}
.results-count {
  font-size: 13px;
  color: var(--text-3);
  white-space: nowrap;
}

/* Timeline */
.timeline {
  position: relative;
  padding-left: 22px;
}
.timeline-line {
  position: absolute;
  left: 8px;
  top: 4px;
  bottom: 4px;
  width: 2px;
  background: linear-gradient(to bottom, var(--blue), var(--border));
}
.timeline-group {
  margin-bottom: 4px;
}
.timeline-date {
  display: inline-flex;
  align-items: center;
  font-size: 11px;
  font-weight: 700;
  color: var(--text-2);
  text-transform: uppercase;
  letter-spacing: 0.06em;
  background: var(--bg);
  border: 1px solid var(--border);
  padding: 2px 9px;
  border-radius: 10px;
  margin: 14px 0 10px -6px;
}
.timeline-date.today {
  background: var(--blue);
  border-color: var(--blue);
  color: var(--white);
}
.timeline-item {
  position: relative;
  margin-bottom: 12px;
}
.timeline-dot {
  position: absolute;
  left: -17px;
  top: 16px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: 2px solid var(--white);
  box-shadow: 0 0 0 1px var(--border);
}
.timeline-card {
  border-left: 3px solid var(--border);
  cursor: pointer;
  transition: box-shadow 0.15s;
}
.timeline-card:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.07);
}
.timeline-card-row {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}
.timeline-card-main {
  flex: 1;
  min-width: 0;
}
.timeline-card-title {
  font-size: 14px;
  font-weight: 700;
  color: var(--text);
}
.timeline-card-meta {
  font-size: 12.5px;
  color: var(--text-3);
  margin-top: 2px;
}
.timeline-card-cancel {
  font-size: 12.5px;
  color: var(--red);
  margin-top: 4px;
  font-weight: 500;
}
.timeline-card-code {
  font-size: 12px;
  font-weight: 700;
  color: var(--navy);
}
</style>
