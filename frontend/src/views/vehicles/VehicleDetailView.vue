<script setup>
import { onMounted, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useScheduleStore } from '@/stores/schedule.store'
import { useRequestsStore } from '@/stores/requests.store'
import { useMaintenanceStore } from '@/stores/maintenance.store'
import { useAuth } from '@/composables/useAuth'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { VehiclesService } from '@/services/vehicles.service'
import { ScheduleService } from '@/services/schedule.service'
import { FuelService } from '@/services/fuel.service'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import BaseBadge from '@/components/ui/AppBadge.vue'
import VehicleForm from '@/components/forms/vehicles/VehicleForm.vue'
import MaintenanceForm from '@/components/forms/vehicles/MaintenanceForm.vue'
import FuelForm from '@/components/forms/vehicles/FuelForm.vue'
import {
  ArrowLeft, Pencil, Wrench, Fuel, PowerOff, Trash2, CheckCircle, RefreshCw,
  Route, Calendar, ClipboardList, Truck, Eye,
} from '@lucide/vue'
import { formatDate, formatKilometers, formatCurrency, formatNumber } from '@/utils/formatters'
import { getVehicleTypeLabel, getMaintenanceTypeLabel } from '@/utils/enumLabels'

const props = defineProps({ id: { type: String, required: true } })

const route        = useRoute()
const router       = useRouter()
const vehicleStore = useVehicleStore()
const scheduleStore = useScheduleStore()
const requestsStore = useRequestsStore()
const maintenanceStore = useMaintenanceStore()
const { can }       = useAuth()
const toast        = useToast()

const vehicle   = computed(() => vehicleStore.currentVehicle)
const isLoading = computed(() => vehicleStore.isLoadingDetail)

const canEditVehicle      = computed(() => can('edit', 'vehicles'))
const canDeleteVehicle    = computed(() => can('delete', 'vehicles'))
const canManageMaintenance = computed(() => can('create', 'maintenance'))
const canManageFuel        = computed(() => can('create', 'fuel'))

// ── Tabs ───────────────────────────────────────────────────────────────────
const activeTab = ref(0)
const TABS = ['Información general', 'Historial de viajes', 'Mantenimiento', 'Consumo de combustible']

// ── Modales ────────────────────────────────────────────────────────────────
const editModal         = useModal()
const maintenanceModal  = useModal()
const fuelModal         = useModal()
const deactivateModal   = useModal()
const deleteModal       = useModal()
const closeMaintModal   = useModal()

const isDeactivating  = ref(false)
const isDeleting      = ref(false)
const isClosingMaint  = ref(false)
const closeMaintForm  = ref({ actualExitDate: new Date().toISOString().split('T')[0], cost: null })
const closeMaintError = ref('')

function openCloseMaintenance(record) {
  closeMaintForm.value = { actualExitDate: new Date().toISOString().split('T')[0], cost: null }
  closeMaintError.value = ''
  closeMaintModal.open(record)
}

function onCloseMaintClose() {
  closeMaintModal.close()
  closeMaintError.value = ''
}

// ── Historial de viajes ──────────────────────────────────────────────────
const TRIPS_PAGE_SIZE = 5
const tripsSearch   = ref('')
const tripsDateFrom = ref(dayjs().subtract(6, 'month').format('YYYY-MM-DD'))
const tripsDateTo   = ref(dayjs().format('YYYY-MM-DD'))
const tripsPage     = ref(1)
const isLoadingTrips = computed(() => scheduleStore.isLoading || requestsStore.isLoadingList)

function loadTrips() {
  tripsPage.value = 1
  scheduleStore.fetchRange(
    dayjs(tripsDateFrom.value).startOf('day').toISOString(),
    dayjs(tripsDateTo.value).endOf('day').toISOString()
  )
}

const vehicleTrips = computed(() => {
  if (!vehicle.value) return []
  const requestsByNumber = new Map(requestsStore.requests.map((r) => [r.requestNumber, r]))
  return scheduleStore.assignments
    .filter((a) => a.vehicleId === vehicle.value.id)
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
        passengerCount: req?.passengerCount ?? 0,
        status: a.status,
      }
    })
    .sort((a, b) => new Date(b.departureTime) - new Date(a.departureTime))
})

const filteredTrips = computed(() => {
  let list = vehicleTrips.value
  if (tripsSearch.value.trim()) {
    const q = tripsSearch.value.trim().toLowerCase()
    list = list.filter((t) =>
      t.destination.toLowerCase().includes(q) ||
      t.area.toLowerCase().includes(q) ||
      t.driver?.toLowerCase().includes(q)
    )
  }
  return list
})

const tripsTotalPages = computed(() => Math.max(1, Math.ceil(filteredTrips.value.length / TRIPS_PAGE_SIZE)))
const paginatedTrips = computed(() => {
  const start = (tripsPage.value - 1) * TRIPS_PAGE_SIZE
  return filteredTrips.value.slice(start, start + TRIPS_PAGE_SIZE)
})
const tripsPageLabel = computed(() => {
  if (filteredTrips.value.length === 0) return 'Mostrando 0 viajes'
  const start = (tripsPage.value - 1) * TRIPS_PAGE_SIZE + 1
  const end = Math.min(tripsPage.value * TRIPS_PAGE_SIZE, filteredTrips.value.length)
  return `Mostrando ${start}–${end} de ${filteredTrips.value.length} viajes`
})

function goTripsPrev() { if (tripsPage.value > 1) tripsPage.value-- }
function goTripsNext() { if (tripsPage.value < tripsTotalPages.value) tripsPage.value++ }

function viewTrip(trip) {
  router.push(`/requests/${trip.requestId}`)
}

// ── Disponibilidad — esta semana ───────────────────────────────────────────
const weekAssignments = ref([])
const DAY_LABELS = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb']

function mondayOf(d) {
  const day = d.day()
  const diff = day === 0 ? -6 : 1 - day
  return d.add(diff, 'day').startOf('day')
}

async function loadWeekAvailability() {
  const start = mondayOf(dayjs())
  const end = start.add(5, 'day').endOf('day')
  try {
    const response = await ScheduleService.getRange(start.toISOString(), end.toISOString())
    weekAssignments.value = (response.data ?? []).filter((a) => a.vehicleId === props.id)
  } catch {
    weekAssignments.value = []
  }
}

const weekDays = computed(() => {
  const start = mondayOf(dayjs())
  return DAY_LABELS.map((label, i) => {
    const date = start.add(i, 'day')
    const assignments = weekAssignments.value
      .filter((a) => dayjs(a.departureTime).isSame(date, 'day') && a.status !== 'Cancelled')
      .sort((a, b) => new Date(a.departureTime) - new Date(b.departureTime))
    return { label, assignments }
  })
})

// ── Mantenimiento programado (para KPI "Próx. mantenimiento") ─────────────
const nextScheduledMaintenance = computed(() => {
  const list = maintenanceStore.scheduled.filter((s) => s.vehicleId === props.id)
  if (!list.length) return null
  return [...list].sort((a, b) => new Date(a.scheduledDate) - new Date(b.scheduledDate))[0]
})

// ── Combustible ─────────────────────────────────────────────────────────────
const FUEL_PAGE_SIZE = 5
const fuelHistory     = ref([])
const isLoadingFuel   = ref(false)
const fuelPage        = ref(1)

async function loadFuelHistory() {
  try {
    isLoadingFuel.value = true
    const response = await FuelService.getHistory({ vehicleId: props.id })
    fuelHistory.value = (response.data ?? []).sort((a, b) => new Date(b.recordDate) - new Date(a.recordDate))
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'No se pudo cargar el historial de combustible'))
    fuelHistory.value = []
  } finally {
    isLoadingFuel.value = false
  }
}

const fuelThisMonth = computed(() =>
  fuelHistory.value.filter((r) => dayjs(r.recordDate).isSame(dayjs(), 'month')))

const fuelKpis = computed(() => {
  const list = fuelThisMonth.value
  const totalGallons = list.reduce((sum, r) => sum + r.gallons, 0)
  const totalCost = list.reduce((sum, r) => sum + r.totalCost, 0)
  const effs = list.filter((r) => r.efficiencyKmPerGallon != null).map((r) => r.efficiencyKmPerGallon)
  const avgEfficiency = effs.length ? effs.reduce((s, n) => s + n, 0) / effs.length : null
  return { totalGallons, totalCost, avgEfficiency, count: list.length }
})

const fuelTotalPages = computed(() => Math.max(1, Math.ceil(fuelHistory.value.length / FUEL_PAGE_SIZE)))
const paginatedFuel = computed(() => {
  const start = (fuelPage.value - 1) * FUEL_PAGE_SIZE
  return fuelHistory.value.slice(start, start + FUEL_PAGE_SIZE)
})
const fuelPageLabel = computed(() => {
  if (fuelHistory.value.length === 0) return 'Mostrando 0 registros'
  const start = (fuelPage.value - 1) * FUEL_PAGE_SIZE + 1
  const end = Math.min(fuelPage.value * FUEL_PAGE_SIZE, fuelHistory.value.length)
  return `Mostrando ${start}–${end} de ${fuelHistory.value.length} registros`
})

function goFuelPrev() { if (fuelPage.value > 1) fuelPage.value-- }
function goFuelNext() { if (fuelPage.value < fuelTotalPages.value) fuelPage.value++ }

// ── KPIs generales ──────────────────────────────────────────────────────────
const currentMonthLabel = computed(() => {
  const m = dayjs().format('MMMM')
  return m.charAt(0).toUpperCase() + m.slice(1)
})
const currentYear = computed(() => dayjs().year())

const maintenanceCostThisYear = computed(() => {
  if (!vehicle.value) return 0
  return vehicle.value.maintenanceRecords
    .filter((m) => m.cost != null && dayjs(m.entryDate).year() === currentYear.value)
    .reduce((sum, m) => sum + m.cost, 0)
})

function maintenanceAccent(type) {
  if (type === 'Preventive') return 'var(--mint-dark)'
  if (type === 'Corrective') return 'var(--amber)'
  return 'var(--sky)'
}

// ── Carga inicial ───────────────────────────────────────────────────────────
onMounted(async () => {
  await vehicleStore.fetchById(props.id)
  if (route.query.edit === 'true' && vehicle.value) {
    editModal.open(vehicle.value)
  }
  if (!vehicle.value) return

  tripsDateFrom.value = dayjs(vehicle.value.createdAt).format('YYYY-MM-DD')

  await Promise.all([
    loadTrips(),
    requestsStore.fetchAll(),
    maintenanceStore.fetchScheduled(true),
    loadFuelHistory(),
    loadWeekAvailability(),
  ])
})

// ── Acciones sobre el vehículo ───────────────────────────────────────────────
async function handleDeactivate() {
  try {
    isDeactivating.value = true
    await VehiclesService.deactivate(props.id)
    deactivateModal.close()
    toast.success('Vehículo desactivado', 'El vehículo fue puesto fuera de servicio.')
    vehicleStore.fetchById(props.id)
  } catch (err) {
    deactivateModal.close()
    toast.error('Error', getErrorMessage(err, 'No se pudo desactivar'))
  } finally {
    isDeactivating.value = false
  }
}

async function handleReactivate() {
  try {
    await VehiclesService.reactivate(props.id)
    toast.success('Vehículo reactivado', 'El vehículo vuelve a estar disponible.')
    vehicleStore.fetchById(props.id)
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'No se pudo reactivar'))
  }
}

async function handleDelete() {
  try {
    isDeleting.value = true
    await VehiclesService.delete(props.id)
    deleteModal.close()
    toast.success('Vehículo eliminado', 'El registro fue eliminado del sistema.')
    router.push('/vehicles')
  } catch (err) {
    deleteModal.close()
    const msg = err.response?.status === 409
      ? 'No se puede eliminar: tiene viajes activos.'
      : getErrorMessage(err, 'Error al eliminar')
    toast.error('No se pudo eliminar', msg)
  } finally {
    isDeleting.value = false
  }
}

async function handleCloseMaintenance() {
  if (!closeMaintForm.value.actualExitDate || closeMaintForm.value.cost === null || closeMaintForm.value.cost < 0) {
    closeMaintError.value = 'Indica la fecha de salida y un costo válido (≥ 0).'
    return
  }
  try {
    isClosingMaint.value = true
    await VehiclesService.closeMaintenance(props.id, closeMaintModal.payload.value.id, {
      actualExitDate: closeMaintForm.value.actualExitDate,
      cost: closeMaintForm.value.cost,
    })
    onCloseMaintClose()
    toast.success('Mantenimiento cerrado', 'El registro fue marcado como finalizado.')
    vehicleStore.fetchById(props.id)
  } catch (err) {
    closeMaintError.value = getErrorMessage(err, 'No se pudo cerrar el mantenimiento')
  } finally {
    isClosingMaint.value = false
  }
}

function afterVehicleSaved() {
  editModal.close()
  toast.success('Guardado', 'Vehículo actualizado correctamente')
  vehicleStore.fetchById(props.id)
}

function afterMaintenanceSaved() {
  maintenanceModal.close()
  toast.success('Guardado', 'Mantenimiento registrado')
  vehicleStore.fetchById(props.id)
  maintenanceStore.fetchScheduled(true)
}

function afterFuelSaved() {
  fuelModal.close()
  toast.success('Guardado', 'Carga de combustible registrada')
  vehicleStore.fetchById(props.id)
  loadFuelHistory()
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <div style="display:flex;align-items:center;gap:10px">
        <button class="icon-btn" @click="router.push('/vehicles')">
          <ArrowLeft :size="16" />
        </button>
        <h1 v-if="vehicle">{{ vehicle.licensePlate }} — {{ vehicle.brand }} {{ vehicle.model }}</h1>
        <h1 v-else>Detalle de vehículo</h1>
      </div>

      <div v-if="vehicle" style="display:flex;gap:8px;flex-wrap:wrap">
        <button v-if="canEditVehicle" class="btn" @click="editModal.open(vehicle)">
          <Pencil :size="14" /> Editar
        </button>
        <button v-if="canManageMaintenance" class="btn" @click="maintenanceModal.open()">
          <Wrench :size="14" /> Mantenimiento
        </button>
        <button v-if="canManageFuel" class="btn" @click="fuelModal.open()">
          <Fuel :size="14" /> Combustible
        </button>
        <button
          v-if="canEditVehicle && vehicle.status === 'Inactive'"
          class="btn"
          style="border-color:var(--mint-dark);color:var(--mint-dark)"
          @click="handleReactivate"
        >
          <RefreshCw :size="14" /> Reactivar
        </button>
        <button
          v-else-if="canEditVehicle"
          class="btn"
          style="border-color:var(--amber-border);color:var(--amber-text)"
          @click="deactivateModal.open()"
        >
          <PowerOff :size="14" /> Desactivar
        </button>
        <button v-if="canDeleteVehicle" class="btn danger" @click="deleteModal.open()">
          <Trash2 :size="14" /> Eliminar
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando vehículo…</div>

    <!-- Error -->
    <div v-else-if="vehicleStore.detailError" class="alert red">
      {{ vehicleStore.detailError }}
    </div>

    <!-- Contenido -->
    <div v-else-if="vehicle">

      <!-- Header card -->
      <div class="card vehicle-header-card">
        <div class="vehicle-icon"><Truck :size="32" /></div>
        <div style="flex:1;min-width:0">
          <div style="display:flex;align-items:center;gap:10px;margin-bottom:10px;flex-wrap:wrap">
            <h2 style="font-size:18px;font-weight:700;color:var(--text)">{{ vehicle.brand }} {{ vehicle.model }}</h2>
            <BaseBadge :status="vehicle.status" />
          </div>
          <div style="display:flex;gap:24px;flex-wrap:wrap">
            <div class="info-block"><span class="info-label">Matrícula</span><div class="info-value strong">{{ vehicle.licensePlate }}</div></div>
            <div class="info-block"><span class="info-label">Año</span><div class="info-value">{{ vehicle.year }}</div></div>
            <div class="info-block"><span class="info-label">Tipo</span><div><span class="tag">{{ getVehicleTypeLabel(vehicle.type) }}</span></div></div>
            <div class="info-block"><span class="info-label">Color</span><div class="info-value">{{ vehicle.color || '—' }}</div></div>
            <div class="info-block"><span class="info-label">Capacidad</span><div class="info-value">{{ vehicle.capacity }} pasajeros</div></div>
            <div class="info-block"><span class="info-label">Kilometraje</span><div class="info-value">{{ formatKilometers(vehicle.currentMileage) }}</div></div>
            <div class="info-block">
              <span class="info-label">Últ. mantenimiento</span>
              <div class="info-value" :class="{ mint: vehicle.lastMaintenanceDate }">
                {{ vehicle.lastMaintenanceDate ? formatDate(vehicle.lastMaintenanceDate) : 'Sin registros' }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- KPIs -->
      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Viajes realizados</div>
          <div class="kpi-val">{{ vehicleTrips.length }}</div>
          <div class="kpi-sub">Historial total</div>
          <div class="kpi-icon blue"><Route :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Galones — {{ currentMonthLabel }}</div>
          <div class="kpi-val">{{ formatNumber(fuelKpis.totalGallons) }}</div>
          <div class="kpi-sub">{{ fuelKpis.count }} carga{{ fuelKpis.count === 1 ? '' : 's' }} este mes</div>
          <div class="kpi-icon amber"><Fuel :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Costo mantenimiento</div>
          <div class="kpi-val">{{ formatCurrency(maintenanceCostThisYear) }}</div>
          <div class="kpi-sub">Acumulado {{ currentYear }}</div>
          <div class="kpi-icon purple"><ClipboardList :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Próx. mantenimiento</div>
          <div class="kpi-val" style="font-size:18px;color:var(--mint-dark)">
            {{ nextScheduledMaintenance ? formatDate(nextScheduledMaintenance.scheduledDate) : '—' }}
          </div>
          <div class="kpi-sub">
            {{ nextScheduledMaintenance ? `En ${nextScheduledMaintenance.daysRemaining} días` : 'Sin programar' }}
          </div>
          <div class="kpi-icon green"><Calendar :size="15" /></div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="page-tabs">
        <div
          v-for="(tab, i) in TABS"
          :key="tab"
          class="pt"
          :class="{ active: activeTab === i }"
          @click="activeTab = i"
        >
          {{ tab }}
        </div>
      </div>

      <!-- TAB 0: Información general -->
      <div v-if="activeTab === 0" class="grid2">
        <div class="card">
          <div class="card-title">Datos del vehículo</div>
          <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px">
            <div><div class="field-label">Marca</div><div class="field-value">{{ vehicle.brand }}</div></div>
            <div><div class="field-label">Modelo</div><div class="field-value">{{ vehicle.model }}</div></div>
            <div><div class="field-label">Año</div><div class="field-value">{{ vehicle.year }}</div></div>
            <div><div class="field-label">Matrícula</div><div class="field-value strong">{{ vehicle.licensePlate }}</div></div>
            <div><div class="field-label">Color</div><div class="field-value">{{ vehicle.color || '—' }}</div></div>
            <div><div class="field-label">Tipo</div><div class="field-value">{{ getVehicleTypeLabel(vehicle.type) }}</div></div>
            <div><div class="field-label">Capacidad</div><div class="field-value">{{ vehicle.capacity }} pasajeros</div></div>
            <div><div class="field-label">Kilometraje actual</div><div class="field-value">{{ formatKilometers(vehicle.currentMileage) }}</div></div>
            <div style="grid-column:1/-1">
              <div class="field-label">Fecha último mantenimiento</div>
              <div class="field-value" :class="{ mint: vehicle.lastMaintenanceDate }">
                {{ vehicle.lastMaintenanceDate ? formatDate(vehicle.lastMaintenanceDate) : 'Sin registros' }}
              </div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-title">Disponibilidad — Esta semana</div>
          <div class="availability-list">
            <div v-for="day in weekDays" :key="day.label" class="avail-row">
              <span class="avail-day">{{ day.label }}</span>
              <div v-if="day.assignments.length" class="avail-bar avail-bar--busy">
                <span>En viaje {{ formatDate(day.assignments[0].departureTime, 'HH:mm') }}–{{ formatDate(day.assignments[0].returnTime, 'HH:mm') }}</span>
              </div>
              <div v-else-if="vehicle.status === 'Inactive'" class="avail-bar avail-bar--off">
                <span>No operativo</span>
              </div>
              <div v-else class="avail-bar avail-bar--free">
                <span>Disponible todo el día</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- TAB 1: Historial de viajes -->
      <div v-else-if="activeTab === 1">
        <div class="search-row">
          <input v-model="tripsSearch" class="search-input" placeholder="Buscar por destino, área, conductor…" />
          <input v-model="tripsDateFrom" type="date" />
          <input v-model="tripsDateTo" type="date" />
          <button class="btn" @click="loadTrips">Filtrar</button>
        </div>

        <div v-if="isLoadingTrips" class="empty-card">Cargando historial de viajes…</div>
        <div v-else-if="filteredTrips.length === 0" class="empty-card">No hay viajes registrados para este vehículo en el rango seleccionado.</div>
        <template v-else>
          <div class="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Fecha</th><th>Área</th><th>Destino</th><th>Conductor</th>
                  <th>Salida</th><th>Regreso</th><th>Pasajeros</th><th>Estado</th><th style="text-align:center">Detalle</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="trip in paginatedTrips" :key="trip.id">
                  <td class="muted">{{ formatDate(trip.departureTime) }}</td>
                  <td style="font-weight:600">{{ trip.area }}</td>
                  <td>{{ trip.destination }}</td>
                  <td style="color:var(--text-2)">{{ trip.driver || '—' }}</td>
                  <td class="muted">{{ formatDate(trip.departureTime, 'HH:mm') }}</td>
                  <td class="muted">{{ formatDate(trip.returnTime, 'HH:mm') }}</td>
                  <td class="muted">{{ trip.passengerCount }}</td>
                  <td><BaseBadge :status="trip.status" /></td>
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
            <span class="pager-info">{{ tripsPageLabel }}</span>
            <div class="pager-btns">
              <button class="pager-btn" :disabled="tripsPage === 1" @click="goTripsPrev">← Anterior</button>
              <button class="pager-btn" :disabled="tripsPage === tripsTotalPages" @click="goTripsNext">Siguiente →</button>
            </div>
          </div>
        </template>
      </div>

      <!-- TAB 2: Mantenimiento -->
      <div v-else-if="activeTab === 2">
        <div v-if="canManageMaintenance" style="display:flex;justify-content:flex-end;margin-bottom:12px">
          <button class="btn primary" @click="maintenanceModal.open()">
            <Wrench :size="14" /> Registrar mantenimiento
          </button>
        </div>

        <div v-if="!vehicle.maintenanceRecords?.length" class="empty-card">Sin registros de mantenimiento</div>
        <div v-else class="timeline">
          <div class="timeline-line"></div>
          <div
            v-for="record in [...vehicle.maintenanceRecords].sort((a, b) => new Date(b.entryDate) - new Date(a.entryDate))"
            :key="record.id"
            class="timeline-item"
          >
            <div class="timeline-dot" :style="{ background: maintenanceAccent(record.type) }"></div>
            <div class="card" :style="{ borderLeft: `3px solid ${maintenanceAccent(record.type)}` }">
              <div style="display:flex;align-items:flex-start;justify-content:space-between;margin-bottom:10px;gap:8px">
                <div>
                  <div style="font-size:13px;font-weight:700;color:var(--text)">{{ record.description }}</div>
                  <div style="font-size:11.5px;color:var(--text-3);margin-top:2px">
                    {{ formatDate(record.entryDate) }}<template v-if="record.workshop"> · {{ record.workshop }}</template>
                  </div>
                </div>
                <span class="tag">{{ getMaintenanceTypeLabel(record.type) }}</span>
              </div>
              <div style="display:flex;gap:16px;padding-top:10px;border-top:1px solid var(--border);flex-wrap:wrap">
                <div>
                  <span class="field-label">Costo</span>
                  <div class="field-value strong">{{ record.cost != null ? formatCurrency(record.cost) : '—' }}</div>
                </div>
                <div v-if="record.isClosed && record.actualExitDate">
                  <span class="field-label">Salida real</span>
                  <div class="field-value">{{ formatDate(record.actualExitDate) }}</div>
                </div>
                <div v-if="record.isClosed" style="margin-left:auto;align-self:center">
                  <span class="tag" style="color:var(--mint-dark);border-color:#6ee7b7;background:var(--mint-bg)">Cerrado</span>
                </div>
                <button
                  v-else-if="canManageMaintenance"
                  class="btn-sm"
                  style="margin-left:auto;align-self:center"
                  @click="openCloseMaintenance(record)"
                >
                  <CheckCircle :size="12" /> Cerrar mantenimiento
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- TAB 3: Consumo de combustible -->
      <div v-else-if="activeTab === 3">
        <div v-if="canManageFuel" style="display:flex;justify-content:flex-end;margin-bottom:12px">
          <button class="btn primary" @click="fuelModal.open()">
            <Fuel :size="14" /> Registrar consumo
          </button>
        </div>

        <div class="kpi-grid" style="grid-template-columns:repeat(3,1fr)">
          <div class="kpi">
            <div class="kpi-label">Total galones — {{ currentMonthLabel }}</div>
            <div class="kpi-val">{{ formatNumber(fuelKpis.totalGallons) }}</div>
            <div class="kpi-sub">{{ fuelKpis.count }} carga{{ fuelKpis.count === 1 ? '' : 's' }} registradas</div>
            <div class="kpi-icon amber"><Fuel :size="15" /></div>
          </div>
          <div class="kpi">
            <div class="kpi-label">Costo total — {{ currentMonthLabel }}</div>
            <div class="kpi-val">{{ formatCurrency(fuelKpis.totalCost) }}</div>
            <div class="kpi-sub">Mes en curso</div>
            <div class="kpi-icon purple"><ClipboardList :size="15" /></div>
          </div>
          <div class="kpi">
            <div class="kpi-label">Km por galón (promedio)</div>
            <div class="kpi-val">{{ fuelKpis.avgEfficiency != null ? fuelKpis.avgEfficiency.toFixed(1) : '—' }}</div>
            <div class="kpi-sub">Rendimiento del mes</div>
            <div class="kpi-icon green"><Route :size="15" /></div>
          </div>
        </div>

        <div v-if="isLoadingFuel" class="empty-card">Cargando registros de combustible…</div>
        <div v-else-if="!fuelHistory.length" class="empty-card">Sin registros de combustible para este vehículo</div>
        <template v-else>
          <div class="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Fecha</th><th>Galones</th><th>Precio/Galón</th><th>Costo total</th>
                  <th>Km al cargar</th><th>Km recorridos</th><th>Rendimiento</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="record in paginatedFuel" :key="record.id">
                  <td class="muted">{{ formatDate(record.recordDate) }}</td>
                  <td style="font-weight:600">{{ formatNumber(record.gallons) }}</td>
                  <td style="color:var(--text-2)">{{ formatCurrency(record.pricePerGallon) }}</td>
                  <td style="font-weight:700">{{ formatCurrency(record.totalCost) }}</td>
                  <td class="muted">{{ formatKilometers(record.mileageAtRefuel) }}</td>
                  <td style="color:var(--mint-dark);font-weight:500">
                    {{ record.kmDriven != null ? `+${formatKilometers(record.kmDriven)}` : '—' }}
                  </td>
                  <td style="color:var(--mint-dark);font-weight:600">
                    {{ record.efficiencyKmPerGallon != null ? `${record.efficiencyKmPerGallon} km/gl` : '—' }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="pager">
            <span class="pager-info">{{ fuelPageLabel }}</span>
            <div class="pager-btns">
              <button class="pager-btn" :disabled="fuelPage === 1" @click="goFuelPrev">← Anterior</button>
              <button class="pager-btn" :disabled="fuelPage === fuelTotalPages" @click="goFuelNext">Siguiente →</button>
            </div>
          </div>
        </template>
      </div>

    </div>

    <!-- ── BaseModal: Editar vehículo ────────────────────────────────────── -->
    <BaseModal
      :model-value="editModal.isOpen.value"
      @update:model-value="editModal.close()"
    >
      <template #header><h3>Editar vehículo</h3></template>
      <VehicleForm
        :vehicle="editModal.payload.value"
        @saved="afterVehicleSaved"
        @cancel="editModal.close()"
      />
    </BaseModal>

    <!-- ── BaseModal: Registrar mantenimiento ────────────────────────────── -->
    <BaseModal
      :model-value="maintenanceModal.isOpen.value"
      @update:model-value="maintenanceModal.close()"
    >
      <template #header><h3>Registrar mantenimiento</h3></template>
      <MaintenanceForm
        :vehicle-id="id"
        @saved="afterMaintenanceSaved"
        @cancel="maintenanceModal.close()"
      />
    </BaseModal>

    <!-- ── BaseModal: Registrar combustible ──────────────────────────────── -->
    <BaseModal
      :model-value="fuelModal.isOpen.value"
      @update:model-value="fuelModal.close()"
    >
      <template #header><h3>Registrar carga de combustible</h3></template>
      <FuelForm
        :vehicle-id="id"
        @saved="afterFuelSaved"
        @cancel="fuelModal.close()"
      />
    </BaseModal>

    <!-- ── ConfirmModal: Cerrar mantenimiento (variant primary + slot) ────── -->
    <ConfirmModal
      :model-value="closeMaintModal.isOpen.value"
      title="Cerrar mantenimiento"
      message="Indica la fecha real de salida y el costo total del servicio."
      confirm-text="Cerrar mantenimiento"
      variant="primary"
      :is-loading="isClosingMaint"
      @update:model-value="onCloseMaintClose"
      @confirm="handleCloseMaintenance"
    >
      <div class="form-group" style="margin-top:4px">
        <label>Fecha de salida real <span class="required">*</span></label>
        <input v-model="closeMaintForm.actualExitDate" type="date" />
      </div>
      <div class="form-group" style="margin-top:8px">
        <label>Costo <span class="required">*</span></label>
        <input v-model.number="closeMaintForm.cost" type="number" min="0" step="0.01" placeholder="0.00" />
      </div>
      <span v-if="closeMaintError" class="field-error">{{ closeMaintError }}</span>
    </ConfirmModal>

    <!-- ── ConfirmModal: Desactivar (variant warning) ─────────────────────── -->
    <ConfirmModal
      :model-value="deactivateModal.isOpen.value"
      title="¿Desactivar vehículo?"
      :message="`El vehículo ${vehicle?.licensePlate} pasará a estado Fuera de servicio y no podrá ser asignado a nuevos viajes.`"
      confirm-text="Desactivar"
      variant="warning"
      :is-loading="isDeactivating"
      @update:model-value="deactivateModal.close()"
      @confirm="handleDeactivate"
    />

    <!-- ── ConfirmModal: Eliminar (variant danger) ────────────────────────── -->
    <ConfirmModal
      :model-value="deleteModal.isOpen.value"
      title="¿Eliminar vehículo?"
      :message="`Esta acción eliminará permanentemente el vehículo ${vehicle?.licensePlate} del sistema.`"
      confirm-text="Eliminar"
      variant="danger"
      :is-loading="isDeleting"
      @update:model-value="deleteModal.close()"
      @confirm="handleDelete"
    />
  </div>
</template>

<style scoped>
.loading-placeholder { padding: 48px; text-align: center; color: var(--text-3); }
.empty-card { font-size: 13px; color: var(--text-3); text-align: center; padding: 32px; background: var(--white); border: 1px solid var(--border); border-radius: 10px; }

/* Header card */
.vehicle-header-card {
  display: flex;
  gap: 20px;
  align-items: flex-start;
  margin-bottom: 20px;
}
.vehicle-icon {
  width: 64px;
  height: 64px;
  border-radius: 12px;
  background: var(--blue-light);
  border: 1px solid var(--blue-mid, #93c5fd);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--blue);
  flex-shrink: 0;
}
.info-block { display: flex; flex-direction: column; gap: 3px; }
.info-label {
  font-size: 10.5px;
  color: var(--text-3);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}
.info-value { font-size: 13.5px; font-weight: 600; color: var(--text); }
.info-value.strong { color: var(--navy); font-weight: 700; }
.info-value.mint { color: var(--mint-dark); }

/* Info general — tab 0 */
.field-label { font-size: 11px; color: var(--text-3); font-weight: 500; margin-bottom: 3px; }
.field-value { font-size: 13px; font-weight: 600; color: var(--text); }
.field-value.strong { color: var(--navy); font-weight: 700; }
.field-value.mint { color: var(--mint-dark); }

/* Disponibilidad */
.availability-list { display: flex; flex-direction: column; gap: 6px; }
.avail-row { display: flex; align-items: center; gap: 10px; }
.avail-day { font-size: 11.5px; color: var(--text-2); min-width: 28px; font-weight: 500; }
.avail-bar {
  flex: 1;
  height: 26px;
  border-radius: 5px;
  display: flex;
  align-items: center;
  padding: 0 8px;
  font-size: 10px;
  font-weight: 600;
  border: 1px solid var(--border);
}
.avail-bar--free { background: var(--mint-bg); border-color: #6ee7b7; color: var(--mint-dark); }
.avail-bar--busy { background: var(--sky-bg); border-color: var(--sky-border); color: var(--sky); }
.avail-bar--off  { background: var(--bg); color: var(--text-3); font-weight: 500; }

/* Mantenimiento — timeline */
.timeline { position: relative; padding-left: 20px; }
.timeline-line { position: absolute; left: 6px; top: 0; bottom: 0; width: 2px; background: var(--border); }
.timeline-item { position: relative; margin-bottom: 16px; }
.timeline-item:last-child { margin-bottom: 0; }
.timeline-dot {
  position: absolute;
  left: -17px;
  top: 14px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: 2px solid var(--white);
  box-shadow: 0 0 0 1px var(--border);
}
.btn-sm {
  font-size: 11px;
  padding: 4px 10px;
  border: 1px solid var(--border-strong);
  border-radius: 6px;
  background: transparent;
  cursor: pointer;
  color: var(--text-2);
  display: inline-flex;
  align-items: center;
  gap: 4px;
}
.btn-sm:hover { background: var(--bg); }
</style>
