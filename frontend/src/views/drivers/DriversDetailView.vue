<script setup>
import { onMounted, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useDriverStore } from '@/stores/drivers.store'
import { useScheduleStore } from '@/stores/schedule.store'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { driverApi } from '@/services/drivers.service'
import { ScheduleService } from '@/services/schedule.service'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import DriverForm from '@/components/forms/drivers/DriverForm.vue'
import RenewLicenseForm from '@/components/forms/drivers/RenewLicenseForm.vue'
import {
  ArrowLeft, Pencil, RefreshCw, Ban, CheckCircle, Trash2, AlertTriangle,
  Route, Calendar, MapPin, Eye, CircleUser,
} from '@lucide/vue'
import { formatDate, getInitials, daysUntil } from '@/utils/formatters'
import { getLicenseCategoryLabel } from '@/utils/enumLabels'

const props = defineProps({ id: { type: String, required: true } })

const route        = useRoute()
const router       = useRouter()
const driverStore  = useDriverStore()
const scheduleStore = useScheduleStore()
const requestsStore = useRequestsStore()
const { can }      = useAuth()
const toast        = useToast()

const driver    = computed(() => driverStore.currentDriver)
const isLoading = computed(() => driverStore.isLoadingDetail)

const canEditDriver   = computed(() => can('edit', 'drivers'))
const canDeleteDriver = computed(() => can('delete', 'drivers'))

const initials = computed(() =>
  driver.value ? getInitials(driver.value.firstName, driver.value.lastName) : ''
)

const licenseAlert = computed(() => {
  if (!driver.value) return null
  const days = daysUntil(driver.value.licenseExpirationDate)
  if (days < 0) return { level: 'expired', label: `Licencia vencida hace ${Math.abs(days)} días` }
  if (days <= 30) return { level: 'warning', label: `Licencia vence en ${days} días` }
  return null
})

const supervisorName = computed(() => {
  if (!driver.value?.supervisorId) return null
  const sup = driverStore.drivers.find((d) => d.id === driver.value.supervisorId)
  return sup ? `${sup.firstName} ${sup.lastName}` : null
})

const seniority = computed(() => {
  if (!driver.value?.createdAt) return '—'
  const months = dayjs().diff(dayjs(driver.value.createdAt), 'month')
  if (months < 1) return 'Nuevo'
  if (months < 12) return `${months} ${months === 1 ? 'mes' : 'meses'}`
  const years = Math.floor(months / 12)
  return `${years} ${years === 1 ? 'año' : 'años'}`
})

// ── Tabs ───────────────────────────────────────────────────────────────────
const activeTab = ref(0)
const TABS = ['Información personal', 'Historial de viajes', 'Disponibilidad']

// ── Modales ────────────────────────────────────────────────────────────────
const editModal    = useModal()  // BaseModal    — formulario edición
const renewModal   = useModal()  // BaseModal    — formulario renovar licencia
const suspendModal = useModal()  // ConfirmModal warning — con textarea en slot
const deleteModal  = useModal()  // ConfirmModal danger

// Estado local del modal de suspensión
const suspendReason = ref('')
const suspendError  = ref('')
const isSuspending  = ref(false)
const isDeleting    = ref(false)

function onSuspendClose() {
  suspendModal.close()
  suspendReason.value = ''
  suspendError.value  = ''
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

const driverTrips = computed(() => {
  if (!driver.value) return []
  const requestsByNumber = new Map(requestsStore.requests.map((r) => [r.requestNumber, r]))
  return scheduleStore.assignments
    .filter((a) => a.driverId === driver.value.id)
    .map((a) => {
      const req = requestsByNumber.get(a.requestNumber)
      return {
        id: a.assignmentId,
        requestId: a.requestId,
        area: req?.requestingArea ?? '—',
        destination: a.destination,
        departureTime: a.departureTime,
        returnTime: a.returnTime,
        vehicle: a.vehiclePlate,
        passengerCount: req?.passengerCount ?? 0,
        status: a.status,
      }
    })
    .sort((a, b) => new Date(b.departureTime) - new Date(a.departureTime))
})

const tripsThisMonth = computed(() =>
  driverTrips.value.filter((t) => dayjs(t.departureTime).isSame(dayjs(), 'month')))

const tripsKpis = computed(() => ({
  total: driverTrips.value.length,
  completed: driverTrips.value.filter((t) => t.status === 'Completed').length,
  cancelled: driverTrips.value.filter((t) => t.status === 'Cancelled').length,
  thisMonth: tripsThisMonth.value.length,
  completedThisMonth: tripsThisMonth.value.filter((t) => t.status === 'Completed').length,
  cancelledThisMonth: tripsThisMonth.value.filter((t) => t.status === 'Cancelled').length,
}))

const filteredTrips = computed(() => {
  let list = driverTrips.value
  if (tripsSearch.value.trim()) {
    const q = tripsSearch.value.trim().toLowerCase()
    list = list.filter((t) =>
      t.destination.toLowerCase().includes(q) ||
      t.area.toLowerCase().includes(q) ||
      t.vehicle?.toLowerCase().includes(q)
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
    weekAssignments.value = (response.data ?? []).filter((a) => a.driverId === props.id)
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

// ── Consultar disponibilidad ────────────────────────────────────────────────
const checkDate    = ref(dayjs().format('YYYY-MM-DD'))
const checkStart   = ref('08:00')
const checkEnd     = ref('17:00')
const isChecking   = ref(false)
const checkResult  = ref(null) // { ok: bool, message: string }

async function handleCheckAvailability() {
  if (!driver.value) return
  isChecking.value = true
  checkResult.value = null
  try {
    if (driver.value.licenseExpired) {
      checkResult.value = { ok: false, message: 'La licencia del conductor está vencida.' }
      return
    }

    const dayStart = dayjs(checkDate.value).startOf('day')
    const dayEnd = dayjs(checkDate.value).endOf('day')
    const response = await ScheduleService.getRange(dayStart.toISOString(), dayEnd.toISOString())
    const slotStart = dayjs(`${checkDate.value}T${checkStart.value}`)
    const slotEnd = dayjs(`${checkDate.value}T${checkEnd.value}`)

    const conflict = (response.data ?? [])
      .filter((a) => a.driverId === props.id && a.status !== 'Cancelled')
      .find((a) => dayjs(a.departureTime).isBefore(slotEnd) && dayjs(a.returnTime).isAfter(slotStart))

    if (conflict) {
      checkResult.value = {
        ok: false,
        message: `En viaje de ${formatDate(conflict.departureTime, 'HH:mm')} a ${formatDate(conflict.returnTime, 'HH:mm')} (${conflict.destination}).`,
      }
    } else {
      checkResult.value = { ok: true, message: `${driver.value.firstName} está libre para ese horario. Licencia vigente.` }
    }
  } catch (err) {
    checkResult.value = { ok: false, message: getErrorMessage(err, 'No se pudo verificar la disponibilidad') }
  } finally {
    isChecking.value = false
  }
}

// ── Carga inicial ───────────────────────────────────────────────────────────
onMounted(async () => {
  await Promise.all([driverStore.fetchById(props.id), driverStore.fetchAll()])
  if (route.query.edit === 'true' && driver.value) {
    editModal.open(driver.value)
  }
  if (!driver.value) return

  tripsDateFrom.value = dayjs(driver.value.createdAt).format('YYYY-MM-DD')

  await Promise.all([
    loadTrips(),
    requestsStore.fetchAll(),
    loadWeekAvailability(),
  ])
})

// ── Acciones sobre el conductor ─────────────────────────────────────────────
async function handleSuspend() {
  if (suspendReason.value.trim().length < 10) {
    suspendError.value = 'El motivo debe tener al menos 10 caracteres'
    return
  }
  try {
    isSuspending.value = true
    await driverApi.suspend(props.id, suspendReason.value)
    onSuspendClose()
    toast.warning('Conductor suspendido', 'El conductor fue suspendido del sistema.')
    driverStore.fetchById(props.id)
  } catch (err) {
    suspendError.value = getErrorMessage(err, 'Error al suspender')
  } finally {
    isSuspending.value = false
  }
}

async function handleReactivate() {
  try {
    await driverApi.reactivate(props.id)
    toast.success('Conductor reactivado', 'El conductor fue reactivado correctamente.')
    driverStore.fetchById(props.id)
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'No se pudo reactivar'))
  }
}

async function handleDelete() {
  try {
    isDeleting.value = true
    await driverApi.delete(props.id)
    deleteModal.close()
    toast.success('Conductor eliminado', 'El registro fue eliminado del sistema.')
    router.push('/drivers')
  } catch (err) {
    deleteModal.close()
    toast.error('Error', getErrorMessage(err, 'No se pudo eliminar'))
  } finally {
    isDeleting.value = false
  }
}

function afterSaved(msg) {
  editModal.close()
  renewModal.close()
  toast.success('Guardado', msg)
  driverStore.fetchById(props.id)
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <div style="display:flex;align-items:center;gap:10px">
        <button class="icon-btn" @click="router.push('/drivers')">
          <ArrowLeft :size="16" />
        </button>
        <h1 v-if="driver">{{ driver.firstName }} {{ driver.lastName }}</h1>
        <h1 v-else>Detalle de conductor</h1>
      </div>

      <div v-if="driver && (canEditDriver || canDeleteDriver)" style="display:flex;gap:8px;flex-wrap:wrap">
        <template v-if="canEditDriver">
          <button class="btn" @click="editModal.open(driver)">
            <Pencil :size="14" /> Editar
          </button>
          <button class="btn" @click="renewModal.open(driver)">
            <RefreshCw :size="14" /> Renovar licencia
          </button>
          <button
            v-if="driver.status === 'Suspended' || driver.status === 'Inactive'"
            class="btn"
            style="border-color:var(--mint-dark);color:var(--mint-dark)"
            @click="handleReactivate"
          >
            <CheckCircle :size="14" /> Reactivar
          </button>
          <button
            v-else-if="driver.status === 'Available'"
            class="btn"
            style="border-color:var(--amber-border);color:var(--amber-text)"
            @click="suspendModal.open()"
          >
            <Ban :size="14" /> Suspender
          </button>
        </template>
        <button v-if="canDeleteDriver" class="btn danger" @click="deleteModal.open()">
          <Trash2 :size="14" /> Eliminar
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando conductor…</div>

    <!-- Error -->
    <div v-else-if="driverStore.detailError" class="alert red">
      {{ driverStore.detailError }}
    </div>

    <!-- Contenido -->
    <div v-else-if="driver">
      <!-- Alerta de licencia -->
      <div v-if="licenseAlert" class="license-banner" :class="`banner--${licenseAlert.level}`">
        <AlertTriangle :size="15" />
        {{ licenseAlert.label }}
        <button v-if="canEditDriver" class="btn-sm" @click="renewModal.open(driver)">Renovar ahora</button>
      </div>

      <!-- Header card -->
      <div class="card driver-header-card">
        <div class="driver-avatar-lg">{{ initials }}</div>
        <div style="flex:1;min-width:0">
          <div style="display:flex;align-items:center;gap:10px;margin-bottom:10px;flex-wrap:wrap">
            <h2 style="font-size:18px;font-weight:700;color:var(--text)">{{ driver.firstName }} {{ driver.lastName }}</h2>
            <AppBadge :status="driver.status" />
          </div>
          <div style="display:flex;gap:24px;flex-wrap:wrap">
            <div class="info-block"><span class="info-label">Cédula</span><div class="info-value strong">{{ driver.nationalId }}</div></div>
            <div class="info-block"><span class="info-label">Licencia</span><div class="info-value">{{ driver.licenseNumber }}</div></div>
            <div class="info-block"><span class="info-label">Tipo de licencia</span><div><span class="tag">{{ getLicenseCategoryLabel(driver.licenseType) }}</span></div></div>
            <div class="info-block">
              <span class="info-label">Vence licencia</span>
              <div class="info-value" :class="{ 'text-red': licenseAlert?.level === 'expired', 'text-amber': licenseAlert?.level === 'warning', mint: !licenseAlert }">
                {{ formatDate(driver.licenseExpirationDate) }}
              </div>
            </div>
            <div class="info-block"><span class="info-label">Teléfono</span><div class="info-value">{{ driver.phone }}</div></div>
            <div class="info-block"><span class="info-label">Supervisor</span><div class="info-value">{{ supervisorName || 'Sin asignar' }}</div></div>
          </div>
          <div v-if="driver.address" style="margin-top:10px;font-size:12.5px;color:var(--text-3);display:flex;align-items:center;gap:4px">
            <MapPin :size="12" />{{ driver.address }}
          </div>
        </div>
      </div>

      <!-- KPIs -->
      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Total viajes</div>
          <div class="kpi-val">{{ tripsKpis.total }}</div>
          <div class="kpi-sub">Historial completo</div>
          <div class="kpi-icon blue"><Route :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Viajes este mes</div>
          <div class="kpi-val">{{ tripsKpis.thisMonth }}</div>
          <div class="kpi-sub">{{ tripsKpis.completedThisMonth }} finalizados</div>
          <div class="kpi-icon green"><CheckCircle :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Conductor desde</div>
          <div class="kpi-val" style="font-size:18px">{{ seniority }}</div>
          <div class="kpi-sub">{{ driver.createdAt ? formatDate(driver.createdAt) : '—' }}</div>
          <div class="kpi-icon purple"><CircleUser :size="15" /></div>
        </div>
        <div class="kpi">
          <div class="kpi-label">Días para vencer lic.</div>
          <div
            class="kpi-val"
            :style="{ color: licenseAlert?.level === 'expired' ? 'var(--red)' : licenseAlert?.level === 'warning' ? 'var(--amber-text)' : 'var(--mint-dark)' }"
          >
            {{ daysUntil(driver.licenseExpirationDate) }}
          </div>
          <div class="kpi-sub">{{ formatDate(driver.licenseExpirationDate) }}</div>
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

      <!-- TAB 0: Información personal -->
      <div v-if="activeTab === 0" class="grid2">
        <div class="card">
          <div class="card-title">Datos personales</div>
          <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px">
            <div><div class="field-label">Nombre completo</div><div class="field-value">{{ driver.firstName }} {{ driver.lastName }}</div></div>
            <div><div class="field-label">Cédula</div><div class="field-value strong">{{ driver.nationalId }}</div></div>
            <div><div class="field-label">Teléfono</div><div class="field-value">{{ driver.phone }}</div></div>
            <div><div class="field-label">Estado</div><div style="margin-top:2px"><AppBadge :status="driver.status" /></div></div>
            <div style="grid-column:1/-1"><div class="field-label">Dirección</div><div class="field-value">{{ driver.address || '—' }}</div></div>
            <div><div class="field-label">Supervisor asignado</div><div class="field-value">{{ supervisorName || 'Sin asignar' }}</div></div>
            <div><div class="field-label">Fecha de registro</div><div class="field-value">{{ driver.createdAt ? formatDate(driver.createdAt) : '—' }}</div></div>
          </div>
        </div>

        <div class="card">
          <div class="card-title">Datos de licencia</div>
          <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px">
            <div><div class="field-label">Número de licencia</div><div class="field-value strong">{{ driver.licenseNumber }}</div></div>
            <div><div class="field-label">Tipo de licencia</div><div style="margin-top:2px"><span class="tag">{{ getLicenseCategoryLabel(driver.licenseType) }}</span></div></div>
            <div>
              <div class="field-label">Fecha de vencimiento</div>
              <div class="field-value" :class="{ 'text-red': licenseAlert?.level === 'expired', 'text-amber': licenseAlert?.level === 'warning' }">
                {{ formatDate(driver.licenseExpirationDate) }}
              </div>
            </div>
            <div>
              <div class="field-label">Días restantes</div>
              <div class="field-value" :class="{ 'text-red': licenseAlert?.level === 'expired', 'text-amber': licenseAlert?.level === 'warning' }">
                {{ daysUntil(driver.licenseExpirationDate) }} días
              </div>
            </div>
          </div>
          <div style="margin-top:14px;padding-top:14px;border-top:1px solid var(--border)">
            <div class="card-title" style="margin-bottom:10px">Estadísticas del mes</div>
            <div style="display:flex;gap:20px">
              <div class="stat-item"><div class="val">{{ tripsKpis.thisMonth }}</div><div class="lbl">Viajes</div></div>
              <div class="stat-item"><div class="val">{{ tripsKpis.completedThisMonth }}</div><div class="lbl">Finalizados</div></div>
              <div class="stat-item"><div class="val">{{ tripsKpis.cancelledThisMonth }}</div><div class="lbl">Cancelados</div></div>
            </div>
          </div>
        </div>
      </div>

      <!-- TAB 1: Historial de viajes -->
      <div v-else-if="activeTab === 1">
        <div class="search-row">
          <input v-model="tripsSearch" class="search-input" placeholder="Buscar por destino, área, vehículo…" />
          <input v-model="tripsDateFrom" type="date" />
          <input v-model="tripsDateTo" type="date" />
          <button class="btn" @click="loadTrips">Filtrar</button>
        </div>

        <div style="display:flex;gap:10px;margin-bottom:14px;flex-wrap:wrap">
          <div class="mini-stat"><span class="mini-stat-label">TOTAL VIAJES</span><span class="mini-stat-val">{{ tripsKpis.total }}</span></div>
          <div class="mini-stat mini-stat--mint"><span class="mini-stat-label">FINALIZADOS</span><span class="mini-stat-val">{{ tripsKpis.completed }}</span></div>
          <div class="mini-stat mini-stat--red"><span class="mini-stat-label">CANCELADOS</span><span class="mini-stat-val">{{ tripsKpis.cancelled }}</span></div>
        </div>

        <div v-if="isLoadingTrips" class="empty-card">Cargando historial de viajes…</div>
        <div v-else-if="filteredTrips.length === 0" class="empty-card">No hay viajes registrados para este conductor en el rango seleccionado.</div>
        <template v-else>
          <div class="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Fecha</th><th>Área</th><th>Destino</th><th>Vehículo</th>
                  <th>Salida</th><th>Regreso</th><th>Pasajeros</th><th>Estado</th><th style="text-align:center">Detalle</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="trip in paginatedTrips" :key="trip.id">
                  <td class="muted">{{ formatDate(trip.departureTime) }}</td>
                  <td style="font-weight:600">{{ trip.area }}</td>
                  <td>{{ trip.destination }}</td>
                  <td style="color:var(--text-2)">{{ trip.vehicle || '—' }}</td>
                  <td class="muted">{{ formatDate(trip.departureTime, 'HH:mm') }}</td>
                  <td class="muted">{{ formatDate(trip.returnTime, 'HH:mm') }}</td>
                  <td class="muted">{{ trip.passengerCount }}</td>
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
            <span class="pager-info">{{ tripsPageLabel }}</span>
            <div class="pager-btns">
              <button class="pager-btn" :disabled="tripsPage === 1" @click="goTripsPrev">← Anterior</button>
              <button class="pager-btn" :disabled="tripsPage === tripsTotalPages" @click="goTripsNext">Siguiente →</button>
            </div>
          </div>
        </template>
      </div>

      <!-- TAB 2: Disponibilidad -->
      <div v-else-if="activeTab === 2" class="grid2">
        <div class="card">
          <div class="card-title">Disponibilidad — Esta semana</div>
          <div class="availability-list">
            <div v-for="day in weekDays" :key="day.label" class="avail-row">
              <span class="avail-day">{{ day.label }}</span>
              <div v-if="day.assignments.length" class="avail-bar avail-bar--busy">
                <span>En viaje {{ formatDate(day.assignments[0].departureTime, 'HH:mm') }}–{{ formatDate(day.assignments[0].returnTime, 'HH:mm') }} ({{ day.assignments[0].destination }})</span>
              </div>
              <div v-else-if="driver.status === 'Inactive' || driver.status === 'Suspended'" class="avail-bar avail-bar--off">
                <span>No operativo</span>
              </div>
              <div v-else class="avail-bar avail-bar--free">
                <span>Disponible todo el día</span>
              </div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-title">Consultar disponibilidad</div>
          <div class="form-grid" style="margin-bottom:0">
            <div class="form-group"><label>Fecha</label><input v-model="checkDate" type="date" /></div>
            <div class="form-group">
              <label>&nbsp;</label>
              <button class="btn primary" style="width:100%;margin-top:0" :disabled="isChecking" @click="handleCheckAvailability">
                {{ isChecking ? 'Verificando…' : 'Verificar' }}
              </button>
            </div>
            <div class="form-group"><label>Hora de salida</label><input v-model="checkStart" type="time" /></div>
            <div class="form-group"><label>Hora de regreso</label><input v-model="checkEnd" type="time" /></div>
          </div>

          <div
            v-if="checkResult"
            style="margin-top:12px;border-radius:8px;padding:12px;display:flex;gap:10px;align-items:center"
            :style="checkResult.ok
              ? 'background:var(--mint-bg);border:1px solid #6ee7b7'
              : 'background:var(--red-bg);border:1px solid var(--red-border)'"
          >
            <CheckCircle v-if="checkResult.ok" :size="18" style="color:var(--mint-dark)" />
            <AlertTriangle v-else :size="18" style="color:var(--red)" />
            <div>
              <div :style="checkResult.ok ? 'color:var(--mint-dark)' : 'color:var(--red)'" style="font-size:13px;font-weight:600">
                {{ checkResult.ok ? 'Conductor disponible' : 'Conductor no disponible' }}
              </div>
              <div style="font-size:11.5px;color:var(--text-2);margin-top:2px">{{ checkResult.message }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ── BaseModal: Editar conductor ──────────────────────────────────── -->
    <BaseModal
      :model-value="editModal.isOpen.value"
      @update:model-value="editModal.close()"
    >
      <template #header><h3>Editar conductor</h3></template>
      <DriverForm
        :driver="editModal.payload.value"
        @saved="afterSaved('Conductor actualizado correctamente')"
        @cancel="editModal.close()"
      />
    </BaseModal>

    <!-- ── BaseModal: Renovar licencia ──────────────────────────────────── -->
    <BaseModal
      :model-value="renewModal.isOpen.value"
      @update:model-value="renewModal.close()"
    >
      <template #header><h3>Renovar licencia</h3></template>
      <RenewLicenseForm
        :driver-id="id"
        :current-license="renewModal.payload.value"
        @saved="afterSaved('Licencia renovada correctamente')"
        @cancel="renewModal.close()"
      />
    </BaseModal>

    <!-- ── ConfirmModal: Suspender (variant warning + slot textarea) ─────── -->
    <ConfirmModal
      :model-value="suspendModal.isOpen.value"
      title="Suspender conductor"
      :message="`Indica el motivo de la suspensión de ${driver?.firstName} ${driver?.lastName}.`"
      confirm-text="Suspender"
      variant="warning"
      :is-loading="isSuspending"
      @update:model-value="onSuspendClose"
      @confirm="handleSuspend"
    >
      <div class="form-group" style="margin-top:4px">
        <label>Motivo <span class="required">*</span></label>
        <textarea
          v-model="suspendReason"
          rows="3"
          placeholder="Describe el motivo (mín. 10 caracteres)…"
          :class="{ 'input-error': suspendError }"
        />
        <span v-if="suspendError" class="field-error">{{ suspendError }}</span>
      </div>
    </ConfirmModal>

    <!-- ── ConfirmModal: Eliminar (variant danger) ───────────────────────── -->
    <ConfirmModal
      :model-value="deleteModal.isOpen.value"
      title="¿Eliminar conductor?"
      :message="`Esta acción eliminará permanentemente a ${driver?.firstName} ${driver?.lastName} del sistema.`"
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
.driver-header-card {
  display: flex;
  gap: 20px;
  align-items: flex-start;
  margin-bottom: 20px;
}
.driver-avatar-lg {
  width: 68px; height: 68px; border-radius: 50%;
  background: var(--blue-light); border: 2px solid var(--blue-mid, #93c5fd);
  display: flex; align-items: center; justify-content: center;
  font-size: 22px; font-weight: 700; color: var(--blue);
  flex-shrink: 0;
}
.info-block { display: flex; flex-direction: column; gap: 3px; }
.info-label {
  font-size: 10.5px; color: var(--text-3); font-weight: 500;
  text-transform: uppercase; letter-spacing: 0.04em;
}
.info-value { font-size: 13.5px; font-weight: 600; color: var(--text); }
.info-value.strong { color: var(--navy); font-weight: 700; }
.info-value.mint { color: var(--mint-dark); }

.text-red   { color: var(--red) !important; font-weight: 700; }
.text-amber { color: var(--amber-text) !important; font-weight: 700; }

/* Info general — tab 0 */
.field-label { font-size: 11px; color: var(--text-3); font-weight: 500; margin-bottom: 3px; }
.field-value { font-size: 13px; font-weight: 600; color: var(--text); }
.field-value.strong { color: var(--navy); font-weight: 700; }

/* Disponibilidad */
.availability-list { display: flex; flex-direction: column; gap: 6px; }
.avail-row { display: flex; align-items: center; gap: 10px; }
.avail-day { font-size: 11.5px; color: var(--text-2); min-width: 28px; font-weight: 500; }
.avail-bar {
  flex: 1; height: 26px; border-radius: 5px;
  display: flex; align-items: center; padding: 0 8px;
  font-size: 10px; font-weight: 600; border: 1px solid var(--border);
}
.avail-bar--free { background: var(--mint-bg); border-color: #6ee7b7; color: var(--mint-dark); }
.avail-bar--busy { background: var(--sky-bg); border-color: var(--sky-border); color: var(--sky); }
.avail-bar--off  { background: var(--bg); color: var(--text-3); font-weight: 500; }

.license-banner {
  display: flex; align-items: center; gap: 8px;
  padding: 10px 16px; border-radius: 8px;
  font-size: 13px; font-weight: 600; margin-bottom: 16px;
}
.banner--warning { background: var(--amber-bg); color: var(--amber-text); border: 1px solid var(--amber-border); }
.banner--expired { background: var(--red-bg); color: var(--red); border: 1px solid var(--red-border); }

.mini-stat {
  background: var(--white); border: 1px solid var(--border); border-radius: 8px;
  padding: 10px 16px; display: flex; align-items: center; gap: 10px;
}
.mini-stat--mint { background: var(--mint-bg); border-color: #6ee7b7; }
.mini-stat--red  { background: var(--red-bg); border-color: var(--red-border); }
.mini-stat-label { font-size: 11px; color: var(--text-3); font-weight: 500; }
.mini-stat--mint .mini-stat-label { color: var(--mint-dark); }
.mini-stat--red .mini-stat-label  { color: var(--red); }
.mini-stat-val { font-size: 18px; font-weight: 700; color: var(--text); }
.mini-stat--mint .mini-stat-val { color: var(--mint-dark); }
.mini-stat--red .mini-stat-val  { color: var(--red); }

.btn-sm {
  margin-left: auto; font-size: 11px; padding: 4px 10px;
  border: 1px solid currentColor; border-radius: 6px;
  background: transparent; cursor: pointer;
}
</style>
