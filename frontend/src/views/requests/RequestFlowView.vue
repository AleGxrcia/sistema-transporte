<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { RequestsService } from '@/services/requests.service'
import { VehiclesService } from '@/services/vehicles.service'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate, formatDateTime } from '@/utils/formatters'
import AppBadge from '@/components/ui/AppBadge.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import { ArrowLeft, Check, X, Play, Flag } from '@lucide/vue'

const props = defineProps({ id: { type: String, required: true } })

const router = useRouter()
const { can } = useAuth()
const toast = useToast()
const store = useRequestsStore()

const request = computed(() => store.currentRequest)
const isLoading = computed(() => store.isLoadingDetail)

const canManage = computed(() => can('approve', 'requests'))

const STEP_ORDER = ['Pending', 'Approved', 'Assigned', 'InProgress', 'Completed']

const currentStepIndex = computed(() => {
  if (!request.value) return 0
  const idx = STEP_ORDER.indexOf(request.value.status)
  return idx === -1 ? 0 : idx
})

const steps = [
  { key: 'created', label: 'Solicitud creada' },
  { key: 'review', label: 'Revisión supervisor' },
  { key: 'assignment', label: 'Asignación de recursos' },
  { key: 'confirmed', label: 'Confirmado' },
]

function stepClass(index) {
  if (request.value?.status === 'Rejected' || request.value?.status === 'Cancelled') {
    return index === 0 ? 'completed' : 'inactive'
  }
  if (index < currentStepIndex.value + 1) return 'completed'
  if (index === currentStepIndex.value + 1) return 'active'
  return 'inactive'
}

// Carga de la solicitud
async function load() {
  await store.fetchById(props.id)
  if (request.value?.status === 'Pending') {
    store.fetchPending()
  }
  if (['Assigned', 'InProgress', 'Completed'].includes(request.value?.status) && request.value) {
    loadAssignedResources()
  }
}

onMounted(load)
watch(() => props.id, load)

const otherPending = computed(() =>
  store.pendingRequests.filter((r) => r.id !== request.value?.id)
)

// Aprobar / Rechazar 
const isApproving = ref(false)
const rejectModal = useModal()
const rejectReason = ref('')
const rejectError = ref('')
const isRejecting = ref(false)

async function handleApprove() {
  try {
    isApproving.value = true
    await RequestsService.approve(request.value.id)
    toast.success('Solicitud aprobada', 'Ahora puedes asignar vehículo y conductor.')
    await store.fetchById(props.id)
  } catch (err) {
    toast.error('No se pudo aprobar', getErrorMessage(err))
    await store.fetchById(props.id)
  } finally {
    isApproving.value = false
  }
}

function openRejectModal() {
  rejectReason.value = ''
  rejectError.value = ''
  rejectModal.open()
}

async function handleReject() {
  if (!rejectReason.value.trim()) {
    rejectError.value = 'El motivo del rechazo es requerido.'
    return
  }
  try {
    isRejecting.value = true
    await RequestsService.reject(request.value.id, rejectReason.value.trim())
    rejectModal.close()
    toast.success('Solicitud rechazada', 'Se notificó el motivo del rechazo.')
    await store.fetchById(props.id)
  } catch (err) {
    rejectError.value = getErrorMessage(err, 'No se pudo rechazar la solicitud')
    await store.fetchById(props.id)
  } finally {
    isRejecting.value = false
  }
}

// Iniciar viaje (Assigned → InProgress)
const isStarting = ref(false)

async function handleStart() {
  try {
    isStarting.value = true
    await RequestsService.start(request.value.id)
    toast.success('Viaje iniciado', 'El viaje está ahora en curso.')
    await store.fetchById(props.id)
  } catch (err) {
    toast.error('No se pudo iniciar el viaje', getErrorMessage(err))
    await store.fetchById(props.id)
  } finally {
    isStarting.value = false
  }
}

// Completar viaje (InProgress → Completed)
const completeModal = useModal()
const actualDeparture = ref('')
const actualReturn = ref('')
const completeError = ref('')
const isCompleting = ref(false)

function openCompleteModal() {
  completeError.value = ''
  actualDeparture.value = formatDate(request.value.departureDateTime, 'YYYY-MM-DDTHH:mm')
  actualReturn.value = formatDate(request.value.returnDateTime, 'YYYY-MM-DDTHH:mm')
  completeModal.open()
}

async function handleComplete() {
  if (!actualDeparture.value || !actualReturn.value) {
    completeError.value = 'Debes indicar la hora real de salida y de regreso.'
    return
  }
  if (new Date(actualReturn.value) <= new Date(actualDeparture.value)) {
    completeError.value = 'La hora de regreso debe ser posterior a la de salida.'
    return
  }
  try {
    isCompleting.value = true
    await RequestsService.complete(
      request.value.id,
      new Date(actualDeparture.value).toISOString(),
      new Date(actualReturn.value).toISOString()
    )
    completeModal.close()
    toast.success('Viaje completado', 'El vehículo y el conductor quedaron disponibles nuevamente.')
    await store.fetchById(props.id)
  } catch (err) {
    completeError.value = getErrorMessage(err, 'No se pudo completar el viaje')
    await store.fetchById(props.id)
  } finally {
    isCompleting.value = false
  }
}

// Recursos ya asignados (para mostrar en estados posteriores)
const assignedVehicle = ref(null)
const assignedDriver = ref(null)

async function loadAssignedResources() {
  try {
    const tasks = []
    if (request.value.assignedVehicleId) {
      tasks.push(VehiclesService.getById(request.value.assignedVehicleId).then((r) => (assignedVehicle.value = r.data)))
    }
    if (request.value.assignedDriverId) {
      tasks.push(driverApi.getById(request.value.assignedDriverId).then((r) => (assignedDriver.value = r.data)))
    }
    await Promise.all(tasks)
  } catch {
    // si falla, simplemente no se muestra el detalle enriquecido
  }
}
</script>

<template>
  <div>
    <div class="page-header">
      <div style="display:flex;align-items:center;gap:10px">
        <button class="icon-btn" @click="router.push('/requests')">
          <ArrowLeft :size="16" />
        </button>
        <h1 v-if="request">Solicitud {{ request.requestNumber }}</h1>
        <h1 v-else>Detalle de solicitud</h1>
      </div>
    </div>

    <div v-if="isLoading" class="loading-placeholder">Cargando solicitud…</div>
    <div v-else-if="store.detailError" class="alert red">{{ store.detailError }}</div>

    <template v-else-if="request">
      <!-- Stepper -->
      <div class="stepper card">
        <template v-for="(step, index) in steps" :key="step.key">
          <div class="step">
            <div class="step-circle" :class="stepClass(index)">
              <Check v-if="stepClass(index) === 'completed'" :size="13" />
              <span v-else>{{ index + 1 }}</span>
            </div>
            <span class="step-label" :class="stepClass(index)">{{ step.label }}</span>
          </div>
          <div v-if="index < steps.length - 1" class="step-line" :class="{ completed: stepClass(index) === 'completed' }" />
        </template>
      </div>

      <div class="grid2">
        <!-- Detalle -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">DETALLES DE LA SOLICITUD</span>
            <AppBadge :status="request.status" />
          </div>
          <dl class="info-grid">
            <div class="info-item"><dt>Área solicitante</dt><dd>{{ request.requestingArea }}</dd></div>
            <div class="info-item"><dt>Pasajeros</dt><dd>{{ request.passengerCount }} personas</dd></div>
            <div class="info-item"><dt>Destino</dt><dd>{{ request.destination }}</dd></div>
            <div class="info-item"><dt>Fecha</dt><dd>{{ formatDate(request.departureDateTime) }}</dd></div>
            <div class="info-item"><dt>Horario</dt><dd>{{ formatDate(request.departureDateTime, 'HH:mm') }} — {{ formatDate(request.returnDateTime, 'HH:mm') }}</dd></div>
            <div class="info-item"><dt>Creada</dt><dd>{{ formatDateTime(request.createdAt) }}</dd></div>
            <div class="info-item full"><dt>Motivo del viaje</dt><dd>{{ request.tripPurpose }}</dd></div>

            <div v-if="request.status === 'Rejected'" class="info-item full">
              <dt>Motivo del rechazo</dt><dd class="text-red">{{ request.rejectionReason }}</dd>
            </div>
            <div v-if="request.status === 'Cancelled'" class="info-item full">
              <dt>Motivo de cancelación</dt><dd class="text-red">{{ request.cancellationReason }}</dd>
            </div>

            <div v-if="assignedVehicle" class="info-item">
              <dt>Vehículo asignado</dt>
              <dd>{{ assignedVehicle.licensePlate }} — {{ assignedVehicle.brand }} {{ assignedVehicle.model }}</dd>
            </div>
            <div v-if="assignedDriver" class="info-item">
              <dt>Conductor asignado</dt>
              <dd>{{ assignedDriver.firstName }} {{ assignedDriver.lastName }}</dd>
            </div>
          </dl>

          <!-- Acciones: aprobar / rechazar -->
          <div v-if="request.status === 'Pending' && canManage" class="form-actions" style="justify-content:flex-start">
            <button class="btn success" :disabled="isApproving" @click="handleApprove">
              <Check :size="14" /> Aprobar solicitud
            </button>
            <button class="btn danger" @click="openRejectModal">
              <X :size="14" /> Rechazar
            </button>
          </div>
          <div v-else-if="request.status === 'Pending'" class="alert amber" style="margin-top:14px">
            Esta solicitud está pendiente de revisión por un supervisor.
          </div>

          <!-- Acciones: iniciar / completar viaje -->
          <div v-if="request.status === 'Assigned' && canManage" class="form-actions" style="justify-content:flex-start">
            <button class="btn primary" :disabled="isStarting" @click="handleStart">
              <Play :size="14" /> Iniciar viaje
            </button>
          </div>
          <div v-else-if="request.status === 'InProgress' && canManage" class="form-actions" style="justify-content:flex-start">
            <button class="btn success" @click="openCompleteModal">
              <Flag :size="14" /> Completar viaje
            </button>
          </div>
        </div>

        <!-- Otras pendientes (solo mientras esta está Pending) -->
        <div v-if="request.status === 'Pending'">
          <div class="sec-header" style="display:flex;justify-content:space-between;align-items:center;margin-bottom:10px">
            <span class="card-title" style="margin:0">OTRAS SOLICITUDES PENDIENTES</span>
            <router-link to="/requests" style="font-size:13px;color:var(--blue)">Ver todas →</router-link>
          </div>
          <div v-if="otherPending.length === 0" class="card" style="text-align:center;color:var(--text-3);font-size:13.5px">
            No hay otras solicitudes pendientes.
          </div>
          <div v-else style="display:flex;flex-direction:column;gap:8px">
            <div
              v-for="r in otherPending"
              :key="r.id"
              class="card"
              style="padding:12px 14px;cursor:pointer"
              @click="router.push(`/requests/${r.id}`)"
            >
              <div style="display:flex;align-items:center;gap:8px;margin-bottom:4px">
                <span style="font-size:12px;font-weight:700;color:var(--text-3)">{{ r.requestNumber }}</span>
                <AppBadge status="Pending" />
              </div>
              <div style="font-size:13.5px;font-weight:600;color:var(--text)">{{ r.requestingArea }} → {{ r.destination }}</div>
              <div style="font-size:12.5px;color:var(--text-3)">{{ formatDate(r.departureDateTime) }} · {{ r.passengerCount }} pers.</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Asignación de recursos (pantalla aparte) -->
      <div
        v-if="request.status === 'Approved' && canManage"
        class="card assign-cta"
      >
        <div class="assign-cta-text">
          <div class="assign-cta-title">✓ Solicitud aprobada</div>
          <div class="assign-cta-sub">Asigna el vehículo y el conductor para completar el viaje.</div>
        </div>
        <button class="btn primary" @click="router.push(`/requests/${request.id}/assign`)">
          Asignar recursos →
        </button>
      </div>
      <div v-else-if="request.status === 'Approved'" class="alert amber">
        Esta solicitud está aprobada y en espera de asignación de recursos por un supervisor.
      </div>
    </template>

    <!-- Modal: Rechazar -->
    <ConfirmModal
      :model-value="rejectModal.isOpen.value"
      title="Rechazar solicitud"
      message="Indica el motivo por el cual se rechaza esta solicitud."
      confirm-text="Rechazar solicitud"
      variant="danger"
      :is-loading="isRejecting"
      @update:model-value="rejectModal.close()"
      @confirm="handleReject"
    >
      <div class="form-group">
        <label>Motivo del rechazo <span class="required">*</span></label>
        <textarea v-model="rejectReason" maxlength="300" placeholder="Explica por qué se rechaza esta solicitud…" />
      </div>
      <span v-if="rejectError" class="field-error">{{ rejectError }}</span>
    </ConfirmModal>

    <!-- Modal: Completar viaje -->
    <ConfirmModal
      :model-value="completeModal.isOpen.value"
      title="Completar viaje"
      message="Registra las horas reales de salida y regreso. Al completar, el vehículo y el conductor quedarán disponibles."
      confirm-text="Completar viaje"
      variant="primary"
      :is-loading="isCompleting"
      @update:model-value="completeModal.close()"
      @confirm="handleComplete"
    >
      <div class="complete-grid">
        <div class="form-group">
          <label>Salida real <span class="required">*</span></label>
          <input v-model="actualDeparture" type="datetime-local" />
        </div>
        <div class="form-group">
          <label>Regreso real <span class="required">*</span></label>
          <input v-model="actualReturn" type="datetime-local" />
        </div>
      </div>
      <span v-if="completeError" class="field-error">{{ completeError }}</span>
    </ConfirmModal>
  </div>
</template>

<style scoped>
.loading-placeholder {
  padding: 48px;
  text-align: center;
  color: var(--text-3);
}

.stepper {
  display: flex;
  align-items: center;
  gap: 0;
  margin-bottom: 16px;
}
.step {
  display: flex;
  align-items: center;
  gap: 7px;
  white-space: nowrap;
}
.step-circle {
  width: 26px;
  height: 26px;
  min-width: 26px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  color: var(--white);
}
.step-circle.completed { background: var(--mint-dark); }
.step-circle.active { background: var(--blue); }
.step-circle.inactive { background: var(--border-strong); color: var(--text-3); }
.step-label { font-size: 13px; font-weight: 600; }
.step-label.completed { color: var(--mint-dark); }
.step-label.active { color: var(--blue); }
.step-label.inactive { color: var(--text-3); font-weight: 500; }
.step-line {
  flex: 1;
  min-width: 20px;
  max-width: 60px;
  height: 2px;
  background: var(--border);
  margin: 0 8px;
  border-radius: 2px;
}
.step-line.completed { background: var(--mint-dark); }

.info-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 14px;
}
.info-item { display: flex; flex-direction: column; gap: 2px; }
.info-item.full { grid-column: 1 / -1; }
.info-item dt { font-size: 11.5px; font-weight: 600; color: var(--text-3); text-transform: uppercase; letter-spacing: 0.04em; }
.info-item dd { font-size: 14px; font-weight: 600; color: var(--text); }
.text-red { color: var(--red); font-weight: 500; }

.assign-cta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
  background: var(--mint-bg);
  border: 1px solid var(--mint-border);
}
.assign-cta-title {
  font-size: 14px;
  font-weight: 700;
  color: var(--mint-dark);
}
.assign-cta-sub {
  font-size: 13px;
  color: var(--text-2);
  margin-top: 2px;
}

.complete-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
.complete-grid .form-group { min-width: 0; }
.complete-grid input {
  width: 100%;
  box-sizing: border-box;
}
</style>
