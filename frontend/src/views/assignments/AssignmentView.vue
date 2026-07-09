<script setup>
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { Truck, Check, Search, Calendar, AlertTriangle } from '@lucide/vue'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { RequestsService } from '@/services/requests.service'
import { VehiclesService } from '@/services/vehicles.service'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate, getInitials } from '@/utils/formatters'
import AppBadge from '@/components/ui/AppBadge.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'

const props = defineProps({ id: { type: String, required: true } })

const router = useRouter()
const store = useRequestsStore()
const { can } = useAuth()
const toast = useToast()

const request = computed(() => store.currentRequest)
const isLoading = computed(() => store.isLoadingDetail)
const isReassign = computed(() => request.value?.status === 'Assigned')

const isLoadingResources = ref(false)
const allVehicles = ref([])
const allDrivers = ref([])

const vehicleSearch = ref('')
const driverSearch = ref('')

const selectedVehicleId = ref(null)
const selectedDriverId = ref(null)

const LICENSE_WARN_DAYS = 30

onMounted(load)
watch(() => props.id, load)

async function load() {
  await store.fetchById(props.id)
  const r = request.value
  if (!r) return
  // Esta pantalla asigna recursos a una solicitud aprobada, o reasigna una ya asignada.
  if (!['Approved', 'Assigned'].includes(r.status) || !can('approve', 'requests')) {
    router.replace(`/requests/${props.id}`)
    return
  }
  await loadResources()
}

async function loadResources() {
  try {
    isLoadingResources.value = true
    // Ventana del viaje: el backend excluye recursos con asignación activa que se solape.
    const window = {
      from: request.value.departureDateTime,
      to: request.value.returnDateTime,
    }
    const [vehiclesRes, driversRes] = await Promise.all([
      VehiclesService.available(request.value.passengerCount, window),
      driverApi.available(window),
    ])
    allVehicles.value = vehiclesRes.data ?? []
    allDrivers.value = driversRes.data ?? []
  } catch {
    allVehicles.value = []
    allDrivers.value = []
  } finally {
    isLoadingResources.value = false
    refreshFades()
  }
}

const filteredVehicles = computed(() => {
  const q = vehicleSearch.value.trim().toLowerCase()
  if (!q) return allVehicles.value
  return allVehicles.value.filter((v) =>
    `${v.licensePlate} ${v.brand} ${v.model} ${v.type}`.toLowerCase().includes(q)
  )
})

const filteredDrivers = computed(() => {
  const q = driverSearch.value.trim().toLowerCase()
  if (!q) return allDrivers.value
  return allDrivers.value.filter((d) =>
    `${d.firstName} ${d.lastName} ${d.licenseType}`.toLowerCase().includes(q)
  )
})

const selectedVehicle = computed(() =>
  allVehicles.value.find((v) => v.id === selectedVehicleId.value)
)
const selectedDriver = computed(() =>
  allDrivers.value.find((d) => d.id === selectedDriverId.value)
)

function daysUntil(date) {
  return Math.ceil((new Date(date) - new Date()) / 86400000)
}
function driverWarning(d) {
  const days = daysUntil(d.licenseExpirationDate)
  return days <= LICENSE_WARN_DAYS ? `Licencia vence en ${days} días` : null
}

// --- Afford­ance de scroll: degradado inferior que se oculta al llegar al final ---
const vehicleListEl = ref(null)
const driverListEl = ref(null)
const vehicleHasMore = ref(false)
const driverHasMore = ref(false)

function updateFade(el, setter) {
  if (!el) return setter(false)
  setter(el.scrollTop + el.clientHeight < el.scrollHeight - 2)
}
function onVehicleScroll() {
  updateFade(vehicleListEl.value, (v) => (vehicleHasMore.value = v))
}
function onDriverScroll() {
  updateFade(driverListEl.value, (v) => (driverHasMore.value = v))
}
function refreshFades() {
  nextTick(() => {
    onVehicleScroll()
    onDriverScroll()
  })
}
watch([filteredVehicles, filteredDrivers], refreshFades)

// --- Confirmación ---
const confirmModal = useModal()
const isAssigning = ref(false)
const assignError = ref('')

function openConfirm() {
  assignError.value = ''
  confirmModal.open()
}

async function handleConfirm() {
  try {
    isAssigning.value = true
    if (isReassign.value) {
      await RequestsService.reassign(props.id, selectedVehicleId.value, selectedDriverId.value)
      confirmModal.close()
      toast.success('Asignación actualizada', 'El vehículo y conductor del viaje fueron reasignados.')
    } else {
      await RequestsService.assign(props.id, selectedVehicleId.value, selectedDriverId.value)
      confirmModal.close()
      toast.success('Asignación confirmada', 'El vehículo y conductor fueron asignados al viaje.')
    }
    router.push(`/requests/${props.id}`)
  } catch (err) {
    assignError.value = getErrorMessage(err, 'No se pudo confirmar la asignación')
  } finally {
    isAssigning.value = false
  }
}

const scheduleLabel = computed(() => {
  const r = request.value
  if (!r) return ''
  return `${formatDate(r.departureDateTime, 'DD/MM/YY')} · ${formatDate(r.departureDateTime, 'HH:mm')} — ${formatDate(r.returnDateTime, 'HH:mm')}`
})
</script>

<template>
  <div>
    <div class="page-header">
      <h1>{{ isReassign ? 'Reasignar recursos' : 'Asignar recursos' }}</h1>
    </div>

    <div v-if="isLoading" class="loading-placeholder">Cargando solicitud…</div>
    <div v-else-if="store.detailError" class="alert red">{{ store.detailError }}</div>

    <template v-else-if="request">
      <!-- Contexto de la solicitud -->
      <div class="request-banner">
        <div class="rb-id">
          <span class="rb-number">{{ request.requestNumber }}</span>
          <AppBadge :status="request.status" />
        </div>
        <span class="rb-divider"></span>
        <div class="rb-item"><span>Área</span><strong>{{ request.requestingArea }}</strong></div>
        <div class="rb-item"><span>Destino</span><strong>{{ request.destination }}</strong></div>
        <div class="rb-item"><span>Fecha</span><strong>{{ formatDate(request.departureDateTime) }}</strong></div>
        <div class="rb-item">
          <span>Horario</span>
          <strong>{{ formatDate(request.departureDateTime, 'HH:mm') }} — {{ formatDate(request.returnDateTime, 'HH:mm') }}</strong>
        </div>
        <div class="rb-item"><span>Pasajeros</span><strong>{{ request.passengerCount }}</strong></div>
      </div>

      <div v-if="isLoadingResources" class="loading-placeholder">Cargando disponibilidad…</div>

      <div v-else class="selection-grid">
        <!-- Vehículos -->
        <section class="selection-col">
          <header class="col-header">
            <div>
              <h3 class="col-title">Seleccionar vehículo</h3>
              <p class="col-sub">Disponibles para {{ scheduleLabel }}</p>
            </div>
            <span class="available-badge">{{ allVehicles.length }} disponibles</span>
          </header>

          <div class="search-field">
            <Search :size="15" />
            <input v-model="vehicleSearch" type="text" placeholder="Buscar por placa, marca o modelo…" />
          </div>

          <div class="resource-scroll" :class="{ 'has-more': vehicleHasMore }">
            <div ref="vehicleListEl" class="resource-list" @scroll="onVehicleScroll">
              <p v-if="filteredVehicles.length === 0" class="resource-empty">
                {{ allVehicles.length === 0 ? 'No hay vehículos con la capacidad requerida.' : 'Sin coincidencias.' }}
              </p>
              <div
                v-for="v in filteredVehicles"
                :key="v.id"
                class="resource-card"
                :class="{ selected: selectedVehicleId === v.id }"
                @click="selectedVehicleId = v.id"
              >
                <div class="resource-icon"><Truck :size="20" /></div>
                <div class="resource-body">
                  <div class="resource-name mono">{{ v.licensePlate }}</div>
                  <div class="resource-meta">{{ v.brand }} {{ v.model }} · {{ v.type }} · {{ v.capacity }} pas.</div>
                </div>
                <span v-if="v.capacity === request.passengerCount" class="warn-tag">
                  <AlertTriangle :size="11" /> Sin margen
                </span>
                <span v-if="selectedVehicleId === v.id" class="check-badge"><Check :size="12" /></span>
              </div>
            </div>
          </div>
        </section>

        <!-- Conductores -->
        <section class="selection-col">
          <header class="col-header">
            <div>
              <h3 class="col-title">Seleccionar conductor</h3>
              <p class="col-sub">Libres con licencia vigente</p>
            </div>
            <span class="available-badge">{{ allDrivers.length }} disponibles</span>
          </header>

          <div class="search-field">
            <Search :size="15" />
            <input v-model="driverSearch" type="text" placeholder="Buscar por nombre o licencia…" />
          </div>

          <div class="resource-scroll" :class="{ 'has-more': driverHasMore }">
            <div ref="driverListEl" class="resource-list" @scroll="onDriverScroll">
              <p v-if="filteredDrivers.length === 0" class="resource-empty">
                {{ allDrivers.length === 0 ? 'No hay conductores disponibles.' : 'Sin coincidencias.' }}
              </p>
              <div
                v-for="d in filteredDrivers"
                :key="d.id"
                class="resource-card"
                :class="{ selected: selectedDriverId === d.id }"
                @click="selectedDriverId = d.id"
              >
                <div class="resource-avatar">{{ getInitials(d.firstName, d.lastName) }}</div>
                <div class="resource-body">
                  <div class="resource-name">{{ d.firstName }} {{ d.lastName }}</div>
                  <div class="resource-meta">Lic. {{ d.licenseType }} · vence {{ formatDate(d.licenseExpirationDate) }}</div>
                </div>
                <span v-if="driverWarning(d)" class="warn-tag"><AlertTriangle :size="11" /> {{ driverWarning(d) }}</span>
                <span v-if="selectedDriverId === d.id" class="check-badge"><Check :size="12" /></span>
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- Resumen -->
      <div v-if="selectedVehicle && selectedDriver" class="summary-bar">
        <div class="summary-tag">Resumen</div>
        <div class="summary-item">
          <div class="summary-icon"><Truck :size="18" /></div>
          <div>
            <div class="summary-label">Vehículo</div>
            <div class="summary-value mono">{{ selectedVehicle.licensePlate }} — {{ selectedVehicle.brand }} {{ selectedVehicle.model }}</div>
          </div>
        </div>
        <div class="summary-item">
          <div class="summary-avatar">{{ getInitials(selectedDriver.firstName, selectedDriver.lastName) }}</div>
          <div>
            <div class="summary-label">Conductor</div>
            <div class="summary-value">{{ selectedDriver.firstName }} {{ selectedDriver.lastName }}</div>
          </div>
        </div>
        <div class="summary-item">
          <div class="summary-icon"><Calendar :size="17" /></div>
          <div>
            <div class="summary-label">Horario</div>
            <div class="summary-value mono">{{ scheduleLabel }}</div>
          </div>
        </div>
        <div class="summary-actions">
          <button class="summary-cancel" @click="router.push(`/requests/${props.id}`)">Cancelar</button>
          <button class="summary-confirm" @click="openConfirm">
            <Check :size="15" /> {{ isReassign ? 'Confirmar reasignación' : 'Confirmar asignación' }}
          </button>
        </div>
      </div>
    </template>

    <!-- Modal de confirmación -->
    <ConfirmModal
      v-if="selectedVehicle && selectedDriver"
      :model-value="confirmModal.isOpen.value"
      :title="isReassign ? 'Confirmar reasignación' : 'Confirmar asignación'"
      :message="isReassign
        ? 'Se reemplazarán el vehículo y conductor actuales por los seleccionados para este horario.'
        : 'Una vez asignados, los recursos quedarán bloqueados para este horario.'"
      :confirm-text="isReassign ? 'Confirmar reasignación' : 'Confirmar asignación'"
      variant="primary"
      :is-loading="isAssigning"
      @update:model-value="confirmModal.close()"
      @confirm="handleConfirm"
    >
      <div class="confirm-grid">
        <div class="confirm-item">
          <dt>Vehículo</dt>
          <dd>{{ selectedVehicle.licensePlate }} — {{ selectedVehicle.brand }} {{ selectedVehicle.model }}</dd>
        </div>
        <div class="confirm-item">
          <dt>Conductor</dt>
          <dd>{{ selectedDriver.firstName }} {{ selectedDriver.lastName }}</dd>
        </div>
      </div>
      <span v-if="assignError" class="field-error">{{ assignError }}</span>
    </ConfirmModal>
  </div>
</template>

<style scoped>
.mono {
  font-variant-numeric: tabular-nums;
  letter-spacing: 0.01em;
}

/* Banner de contexto */
.request-banner {
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 15px 20px;
  margin-bottom: 18px;
  display: flex;
  gap: 18px;
  align-items: center;
  flex-wrap: wrap;
  box-shadow: var(--shadow-xs);
}
.rb-id {
  display: flex;
  align-items: center;
  gap: 9px;
}
.rb-number {
  font-size: 13px;
  font-weight: 700;
  color: var(--text);
}
.rb-divider {
  width: 1px;
  height: 22px;
  background: var(--border);
}
.rb-item {
  font-size: 12.5px;
  color: var(--text-2);
}
.rb-item span {
  color: var(--text-3);
}
.rb-item strong {
  font-weight: 600;
  color: var(--text);
  margin-left: 5px;
}

/* Grid de selección */
.selection-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}
.selection-col {
  display: flex;
  flex-direction: column;
  gap: 11px;
}
.col-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 10px;
}
.col-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text);
}
.col-sub {
  font-size: 11.5px;
  color: var(--text-3);
  margin-top: 2px;
}
.available-badge {
  font-size: 11px;
  font-weight: 600;
  background: var(--mint-bg);
  color: var(--mint-dark);
  border: 1px solid var(--mint-border);
  padding: 2px 9px;
  border-radius: 20px;
  white-space: nowrap;
}

/* Buscador */
.search-field {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 38px;
  padding: 0 12px;
  background: var(--white);
  border: 1px solid var(--border-strong);
  border-radius: 9px;
  color: var(--text-3);
}
.search-field input {
  flex: 1;
  min-width: 0;
  border: none;
  background: transparent;
  outline: none;
  font-size: 13px;
  color: var(--text);
  font-family: inherit;
  height: auto;
  padding: 0;
}

/* Lista con scroll + degradado inferior */
.resource-scroll {
  position: relative;
}
.resource-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  max-height: 244px; /* ~3 tarjetas */
  overflow-y: auto;
  padding: 2px;
}
.resource-scroll::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  height: 42px;
  background: linear-gradient(to bottom, transparent, var(--bg));
  border-radius: 0 0 10px 10px;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.15s;
}
.resource-scroll.has-more::after {
  opacity: 1;
}
.resource-empty {
  font-size: 13px;
  color: var(--text-3);
  padding: 14px 4px;
}

/* Tarjeta de recurso */
.resource-card {
  position: relative;
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 12px 14px;
  cursor: pointer;
  transition: border-color 0.15s, box-shadow 0.15s, background 0.15s;
}
.resource-card:hover {
  border-color: var(--blue-mid);
}
.resource-card.selected {
  border-color: var(--blue);
  background: var(--blue-light);
  box-shadow: 0 0 0 1px var(--blue);
}
.resource-icon {
  width: 40px;
  height: 40px;
  min-width: 40px;
  border-radius: 9px;
  background: var(--blue-light);
  color: var(--blue);
  display: flex;
  align-items: center;
  justify-content: center;
}
.resource-avatar {
  width: 40px;
  height: 40px;
  min-width: 40px;
  border-radius: 50%;
  background: var(--blue);
  color: var(--white);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 700;
}
.resource-body {
  flex: 1;
  min-width: 0;
}
.resource-name {
  font-size: 13px;
  font-weight: 600;
  color: var(--text);
}
.resource-meta {
  font-size: 11.5px;
  color: var(--text-2);
  margin-top: 2px;
}
.warn-tag {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 10px;
  font-weight: 600;
  background: var(--amber-bg);
  color: var(--amber-text);
  border: 1px solid var(--amber-border);
  padding: 2px 8px;
  border-radius: 10px;
  white-space: nowrap;
}
.check-badge {
  width: 20px;
  height: 20px;
  min-width: 20px;
  border-radius: 50%;
  background: var(--blue);
  color: var(--white);
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Barra resumen (navy) */
.summary-bar {
  background: var(--navy);
  border-radius: 13px;
  padding: 18px 22px;
  color: var(--white);
  display: flex;
  align-items: center;
  gap: 26px;
  flex-wrap: wrap;
}
.summary-tag {
  font-size: 11px;
  font-weight: 600;
  color: var(--mint-accent);
  text-transform: uppercase;
  letter-spacing: 0.06em;
}
.summary-item {
  display: flex;
  align-items: center;
  gap: 11px;
}
.summary-icon {
  width: 38px;
  height: 38px;
  border-radius: 9px;
  background: rgba(255, 255, 255, 0.08);
  color: var(--mint);
  display: flex;
  align-items: center;
  justify-content: center;
}
.summary-avatar {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: var(--blue);
  color: var(--white);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
}
.summary-label {
  font-size: 10px;
  color: rgba(255, 255, 255, 0.45);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.summary-value {
  font-size: 13px;
  font-weight: 600;
  margin-top: 1px;
}
.summary-actions {
  margin-left: auto;
  display: flex;
  gap: 10px;
}
.summary-cancel {
  height: 42px;
  padding: 0 18px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  background: transparent;
  border-radius: 9px;
  font-size: 13px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.8);
  cursor: pointer;
  font-family: inherit;
  transition: background 0.15s;
}
.summary-cancel:hover {
  background: rgba(255, 255, 255, 0.08);
}
.summary-confirm {
  height: 42px;
  padding: 0 24px;
  background: var(--mint-dark);
  color: var(--white);
  border: none;
  border-radius: 9px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: inherit;
  transition: background 0.15s;
}
.summary-confirm:hover {
  background: #047857;
}

/* Modal */
.confirm-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
.confirm-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.confirm-item dt {
  font-size: 11.5px;
  font-weight: 600;
  color: var(--text-3);
  text-transform: uppercase;
  letter-spacing: 0.04em;
}
.confirm-item dd {
  font-size: 14px;
  font-weight: 600;
  color: var(--text);
}

@media (max-width: 760px) {
  .selection-grid {
    grid-template-columns: 1fr;
  }
  .summary-actions {
    margin-left: 0;
    width: 100%;
  }
}
</style>
