<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { DashboardService } from '@/services/dashboard.service'
import { RequestsService } from '@/services/requests.service'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate } from '@/utils/formatters'
import BarChart from '@/components/charts/BarChart.vue'
import HorizontalBarChart from '@/components/charts/HorizontalBarChart.vue'
import {
  Truck,
  CheckCircle2,
  ClipboardList,
  Route,
  Check,
  Eye,
  AlertTriangle,
} from '@lucide/vue'

const router = useRouter()
const { can } = useAuth()
const toast = useToast()
const requestsStore = useRequestsStore()

const MONTH_LABELS = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']
const TRIP_STATUSES = ['Assigned', 'InProgress', 'Completed']

const summary = ref(null)
const isLoadingSummary = ref(true)
const summaryError = ref('')

async function loadSummary() {
  try {
    isLoadingSummary.value = true
    summaryError.value = ''
    const { data } = await DashboardService.summary()
    summary.value = data
  } catch (err) {
    summaryError.value = getErrorMessage(err, 'No se pudo cargar el resumen del dashboard')
  } finally {
    isLoadingSummary.value = false
  }
}

onMounted(() => {
  loadSummary()
  requestsStore.fetchPending()
  requestsStore.fetchAll()
})

/* KPIs */
const kpis = computed(() => {
  const s = summary.value
  if (!s) return []
  const fleetPct = s.totalVehicles ? Math.round((s.availableVehicles / s.totalVehicles) * 100) : 0
  return [
    {
      label: 'Vehículos disponibles',
      value: s.availableVehicles,
      sub: `${fleetPct}% de ${s.totalVehicles} en flota`,
      icon: Truck,
      variant: 'green',
    },
    {
      label: 'Solicitudes pendientes',
      value: s.pendingRequests,
      sub: s.pendingRequests > 0 ? 'Requieren aprobación' : 'Todo al día',
      icon: ClipboardList,
      variant: s.pendingRequests > 0 ? 'amber' : 'blue',
    },
    {
      label: 'Viajes de hoy',
      value: s.todayTrips,
      sub: `${s.driversOnTrip} conductores en ruta`,
      icon: Route,
      variant: 'blue',
    },
    {
      label: 'Completados este mes',
      value: s.completedThisMonth,
      sub: `${s.approvedRequests} aprobadas activas`,
      icon: CheckCircle2,
      variant: 'sky',
    },
  ]
})

/* Alertas */
const alerts = computed(() => {
  const s = summary.value
  if (!s) return []
  const list = []
  if (s.maintenanceAlerts?.length) {
    list.push({
      key: 'maintenance',
      title: `${s.maintenanceAlerts.length} alerta(s) de mantenimiento`,
      detail: s.maintenanceAlerts.join(' · '),
    })
  }
  if (s.licenseAlerts?.length) {
    list.push({
      key: 'license',
      title: `${s.licenseAlerts.length} licencia(s) por vencer`,
      detail: s.licenseAlerts.join(' · '),
    })
  }
  return list
})

/* Viajes de hoy */
const todayTrips = computed(() => {
  const today = dayjs().startOf('day')
  return requestsStore.requests
    .filter(
      (r) => TRIP_STATUSES.includes(r.status) && dayjs(r.departureDateTime).isSame(today, 'day')
    )
    .sort((a, b) => dayjs(a.departureDateTime).valueOf() - dayjs(b.departureDateTime).valueOf())
    .slice(0, 5)
})

const statusMeta = {
  Assigned: { label: 'Asignado', color: 'var(--blue)' },
  InProgress: { label: 'En curso', color: 'var(--mint-dark)' },
  Completed: { label: 'Completado', color: 'var(--text-3)' },
}

/* Pendientes de aprobar */
const visiblePending = computed(() => requestsStore.pendingRequests.slice(0, 4))

const approvingId = ref(null)
async function quickApprove(req) {
  try {
    approvingId.value = req.id
    await RequestsService.approve(req.id)
    toast.success('Solicitud aprobada', `${req.requestNumber} fue aprobada correctamente.`)
    requestsStore.fetchPending()
    loadSummary()
  } catch (err) {
    toast.error('No se pudo aprobar', getErrorMessage(err))
  } finally {
    approvingId.value = null
  }
}

/* Viajes por mes (año actual) */
const currentYear = dayjs().year()
const monthlyTrips = computed(() => {
  const counts = new Array(12).fill(0)
  for (const r of requestsStore.requests) {
    if (!TRIP_STATUSES.includes(r.status)) continue
    const d = dayjs(r.departureDateTime)
    if (d.year() === currentYear) counts[d.month()] += 1
  }
  return MONTH_LABELS.map((label, i) => ({ label, value: counts[i] }))
})

/* Solicitudes por área (mes actual) */
const areaStats = computed(() => {
  const startOfMonth = dayjs().startOf('month')
  const tally = new Map()
  for (const r of requestsStore.requests) {
    if (!dayjs(r.departureDateTime).isSame(startOfMonth, 'month')) continue
    const area = r.requestingArea || 'Sin área'
    tally.set(area, (tally.get(area) || 0) + 1)
  }
  return [...tally.entries()]
    .map(([label, value]) => ({ label, value }))
    .sort((a, b) => b.value - a.value)
    .slice(0, 6)
})
</script>

<template>
  <div>
    <div v-if="isLoadingSummary" class="loading-placeholder">Cargando dashboard…</div>
    <div v-else-if="summaryError" class="alert red">{{ summaryError }}</div>

    <template v-else-if="summary">
      <!-- Alertas -->
      <div v-for="alert in alerts" :key="alert.key" class="alert amber dash-alert">
        <AlertTriangle :size="18" class="dash-alert__icon" />
        <div>
          <strong>{{ alert.title }}</strong> — {{ alert.detail }}
        </div>
        <router-link to="/maintenance" class="dash-alert__link">Revisar →</router-link>
      </div>

      <!-- KPIs -->
      <div class="kpi-grid kpi-grid--4">
        <div v-for="kpi in kpis" :key="kpi.label" class="kpi">
          <div class="kpi-label">{{ kpi.label }}</div>
          <div class="kpi-val">{{ kpi.value }}</div>
          <div class="kpi-sub">{{ kpi.sub }}</div>
          <div class="kpi-icon" :class="kpi.variant"><component :is="kpi.icon" :size="16" /></div>
        </div>
      </div>

      <!-- Viajes de hoy + Pendientes de aprobar -->
      <div class="dash-row">
        <div class="card">
          <div class="card-header">
            <span class="card-title">Viajes de hoy</span>
            <router-link to="/schedules" class="dash-link">Ver agenda →</router-link>
          </div>
          <div v-if="todayTrips.length === 0" class="empty-card">No hay viajes programados para hoy.</div>
          <div v-else class="dash-list">
            <div v-for="trip in todayTrips" :key="trip.id" class="dash-item">
              <span class="dash-dot" :style="{ background: statusMeta[trip.status]?.color }" />
              <div class="dash-item__body">
                <div class="dash-item__title">{{ trip.requestingArea }} → {{ trip.destination }}</div>
                <div class="dash-item__meta" :style="{ color: statusMeta[trip.status]?.color }">
                  {{ statusMeta[trip.status]?.label }} · {{ trip.passengerCount }} pers.
                </div>
              </div>
              <div class="dash-item__time">{{ formatDate(trip.departureDateTime, 'HH:mm') }}</div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-header">
            <span class="card-title">Pendientes de aprobar</span>
            <span class="dash-pill">{{ requestsStore.pendingCount }}</span>
          </div>
          <div v-if="requestsStore.isLoadingPending" class="loading-placeholder">Cargando…</div>
          <div v-else-if="visiblePending.length === 0" class="empty-card">
            No hay solicitudes pendientes de aprobación.
          </div>
          <div v-else class="dash-list">
            <div v-for="req in visiblePending" :key="req.id" class="dash-item">
              <div class="dash-item__body">
                <div class="dash-item__title">
                  {{ req.requestingArea }}
                  <span class="dash-item__id">{{ req.requestNumber }}</span>
                </div>
                <div class="dash-item__meta">
                  {{ formatDate(req.departureDateTime) }} · {{ req.destination }} · {{ req.passengerCount }} pers.
                </div>
              </div>
              <div class="action-buttons">
                <button
                  v-if="can('approve', 'requests')"
                  class="icon-btn approve"
                  title="Aprobar"
                  :disabled="approvingId === req.id"
                  @click="quickApprove(req)"
                >
                  <Check :size="13" />
                </button>
                <button class="icon-btn" title="Ver detalle" @click="router.push(`/requests/${req.id}`)">
                  <Eye :size="13" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos -->
      <div class="dash-row">
        <div class="card">
          <div class="card-header">
            <span class="card-title">Viajes por mes · {{ currentYear }}</span>
          </div>
          <BarChart :data="monthlyTrips" :height="140" />
        </div>

        <div class="card">
          <div class="card-header">
            <span class="card-title">Solicitudes por área · este mes</span>
          </div>
          <div v-if="areaStats.length === 0" class="empty-card">Sin solicitudes este mes.</div>
          <HorizontalBarChart v-else :data="areaStats" />
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.kpi-grid--4 {
  grid-template-columns: repeat(4, 1fr);
}
@media (max-width: 1100px) {
  .kpi-grid--4 {
    grid-template-columns: repeat(2, 1fr);
  }
}
@media (max-width: 560px) {
  .kpi-grid--4 {
    grid-template-columns: 1fr;
  }
}

.dash-row {
  display: grid;
  grid-template-columns: 1.35fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}
@media (max-width: 960px) {
  .dash-row {
    grid-template-columns: 1fr;
  }
}

.dash-alert {
  display: flex;
  align-items: center;
  gap: 12px;
}
.dash-alert__icon {
  flex-shrink: 0;
}
.dash-alert__link {
  margin-left: auto;
  font-size: 12.5px;
  font-weight: 600;
  color: inherit;
  white-space: nowrap;
}

.dash-link {
  font-size: 13px;
  color: var(--blue);
}

.dash-list {
  display: flex;
  flex-direction: column;
}
.dash-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 11px 0;
  border-bottom: 1px solid var(--border);
}
.dash-item:last-child {
  border-bottom: none;
}
.dash-dot {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  flex-shrink: 0;
}
.dash-item__body {
  flex: 1;
  min-width: 0;
}
.dash-item__title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.dash-item__id {
  font-size: 11.5px;
  font-weight: 500;
  color: var(--text-3);
  margin-left: 4px;
}
.dash-item__meta {
  font-size: 12px;
  color: var(--text-3);
  margin-top: 2px;
}
.dash-item__time {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-2);
}

.dash-pill {
  font-size: 12px;
  font-weight: 600;
  color: var(--purple);
  background: var(--purple-bg);
  border: 1px solid var(--purple-border);
  padding: 2px 10px;
  border-radius: 20px;
}
</style>
