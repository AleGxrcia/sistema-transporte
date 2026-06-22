<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { DashboardService } from '@/services/dashboard.service'
import { RequestsService } from '@/services/requests.service'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate } from '@/utils/formatters'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Truck, CheckCircle2, Wrench, OctagonX, Users, ClipboardList, Check, Eye } from '@lucide/vue'

const router = useRouter()
const { can } = useAuth()
const toast = useToast()
const requestsStore = useRequestsStore()

const summary = ref(null)
const isLoadingSummary = ref(true)
const summaryError = ref('')

async function loadSummary() {
  try {
    isLoadingSummary.value = true
    summaryError.value = ''
    const response = await DashboardService.summary()
    summary.value = response.data
  } catch (err) {
    summaryError.value = getErrorMessage(err, 'No se pudo cargar el resumen del dashboard')
  } finally {
    isLoadingSummary.value = false
  }
}

onMounted(() => {
  loadSummary()
  requestsStore.fetchPending()
})

const inactiveVehicles = computed(() => {
  if (!summary.value) return 0
  const { totalVehicles, availableVehicles, vehiclesOnTrip, vehiclesInMaintenance } = summary.value
  return Math.max(0, totalVehicles - availableVehicles - vehiclesOnTrip - vehiclesInMaintenance)
})

const visiblePendingRequests = computed(() => requestsStore.pendingRequests.slice(0, 5))

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
</script>

<template>
  <div>
    <div v-if="isLoadingSummary" class="loading-placeholder">Cargando dashboard…</div>
    <div v-else-if="summaryError" class="alert red">{{ summaryError }}</div>

    <template v-else-if="summary">
      <div v-if="summary.maintenanceAlerts.length" class="alert amber">
        <strong>{{ summary.maintenanceAlerts.length }} alerta(s) de mantenimiento</strong> —
        {{ summary.maintenanceAlerts.join(' · ') }}
      </div>
      <div v-if="summary.licenseAlerts.length" class="alert amber">
        <strong>{{ summary.licenseAlerts.length }} licencia(s) por vencer</strong> —
        {{ summary.licenseAlerts.join(' · ') }}
      </div>

      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Total vehículos</div>
          <div class="kpi-val">{{ summary.totalVehicles }}</div>
          <div class="kpi-sub">{{ summary.vehiclesOnTrip }} en viaje</div>
          <div class="kpi-icon blue"><Truck :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Disponibles</div>
          <div class="kpi-val">{{ summary.availableVehicles }}</div>
          <div class="kpi-sub">
            {{ summary.totalVehicles ? Math.round((summary.availableVehicles / summary.totalVehicles) * 100) : 0 }}% de la flota
          </div>
          <div class="kpi-icon green"><CheckCircle2 :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">En mantenimiento</div>
          <div class="kpi-val">{{ summary.vehiclesInMaintenance }}</div>
          <div class="kpi-icon amber"><Wrench :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Fuera de servicio</div>
          <div class="kpi-val">{{ inactiveVehicles }}</div>
          <div class="kpi-trend down" v-if="inactiveVehicles > 0">Requieren atención</div>
          <div class="kpi-icon red"><OctagonX :size="16" /></div>
        </div>
      </div>

      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Total conductores</div>
          <div class="kpi-val">{{ summary.totalDrivers }}</div>
          <div class="kpi-sub">{{ summary.availableDrivers }} disponibles ahora</div>
          <div class="kpi-icon purple"><Users :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Solicitudes pendientes</div>
          <div class="kpi-val">{{ summary.pendingRequests }}</div>
          <div class="kpi-trend down" v-if="summary.pendingRequests > 0">Requieren aprobación</div>
          <div class="kpi-icon red"><ClipboardList :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Viajes de hoy</div>
          <div class="kpi-val">{{ summary.todayTrips }}</div>
          <div class="kpi-sub"><router-link to="/schedules">Ver agenda →</router-link></div>
          <div class="kpi-icon sky"><Truck :size="16" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Completados este mes</div>
          <div class="kpi-val">{{ summary.completedThisMonth }}</div>
          <div class="kpi-icon green"><CheckCircle2 :size="16" /></div>
        </div>
      </div>

      <div class="card">
        <div class="card-header">
          <span class="card-title">SOLICITUDES PENDIENTES</span>
          <router-link to="/requests" style="font-size:12px;color:var(--blue)">Gestionar →</router-link>
        </div>

        <div v-if="requestsStore.isLoadingPending" class="loading-placeholder">Cargando…</div>
        <div v-else-if="visiblePendingRequests.length === 0" class="empty-card">
          No hay solicitudes pendientes de aprobación.
        </div>
        <div v-else style="display:flex;flex-direction:column;gap:8px">
          <div
            v-for="req in visiblePendingRequests"
            :key="req.id"
            style="display:flex;align-items:center;justify-content:space-between;padding:10px 12px;border:1px solid var(--border);border-radius:8px"
          >
            <div>
              <div style="font-size:12.5px;font-weight:600;color:var(--text)">{{ req.requestingArea }} → {{ req.destination }}</div>
              <div style="font-size:11.5px;color:var(--text-3)">
                {{ formatDate(req.departureDateTime) }} · {{ req.passengerCount }} pers. · {{ req.requestNumber }}
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
    </template>
  </div>
</template>

<style scoped>
.loading-placeholder {
  padding: 32px;
  text-align: center;
  color: var(--text-3);
}
.empty-card {
  font-size: 13px;
  color: var(--text-3);
  text-align: center;
  padding: 24px;
}
</style>
