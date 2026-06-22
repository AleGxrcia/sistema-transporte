<template>
  <div class="maintenance">

    <!-- Alerta -->
    <div v-if="alerts.length" class="alert-banner">
      <span>⚠️</span>
      <span>
        <strong>{{ alerts.length }} vehículo{{ alerts.length > 1 ? 's' : '' }} requiere{{ alerts.length > 1 ? 'n' : '' }} mantenimiento</strong>
        próximamente —
        <template v-for="(a, i) in alerts" :key="`${a.vehicleId}-${a.dueDate}`">
          {{ a.vehicleLabel }} ({{ a.daysRemaining }} día{{ a.daysRemaining === 1 ? '' : 's' }}){{ i < alerts.length - 1 ? ', ' : '.' }}
        </template>
      </span>
    </div>

    <div class="main-grid">

      <!-- Próximos mantenimientos -->
      <div class="card">
        <div class="card-section-title">PRÓXIMOS MANTENIMIENTOS</div>
        <div v-if="maintenanceStore.isLoadingScheduled" class="empty-state">Cargando…</div>
        <div v-else-if="!scheduled.length" class="empty-state">No hay mantenimientos programados.</div>
        <div v-else class="upcoming-list">
          <div v-for="item in scheduled" :key="item.id" class="upcoming-item">
            <div class="upcoming-icon" :class="daysColorClass(item.daysRemaining)">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
              </svg>
            </div>
            <div class="upcoming-info">
              <p class="upcoming-name">{{ item.vehicleLabel }} — {{ item.vehiclePlate }}</p>
              <p class="upcoming-detail">{{ getMaintenanceTypeLabel(item.type) }} · {{ item.workshop || 'Taller por definir' }}</p>
            </div>
            <span class="upcoming-days" :class="textColorClass(item.daysRemaining)">
              {{ item.daysRemaining >= 0 ? `en ${item.daysRemaining} días` : 'vencido' }}
            </span>
            <div class="upcoming-actions">
              <button
                class="btn-mini"
                :disabled="!isVehicleAvailable(item.vehicleId)"
                :title="isVehicleAvailable(item.vehicleId) ? '' : 'El vehículo no está disponible (en viaje o en mantenimiento)'"
                @click="openExecuteModal(item)"
              >
                Ejecutar
              </button>
              <button class="btn-mini btn-mini--danger" @click="openCancelModal(item)">Cancelar</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Formulario registro -->
      <div class="card">
        <div class="card-section-title-row">
          <div class="card-section-title">{{ mode === 'now' ? 'REGISTRAR MANTENIMIENTO' : 'PROGRAMAR MANTENIMIENTO' }}</div>
          <div class="mode-toggle">
            <button :class="{ active: mode === 'now' }" @click="mode = 'now'">Registrar ahora</button>
            <button :class="{ active: mode === 'schedule' }" @click="mode = 'schedule'">Programar para después</button>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Vehículo</label>
          <select v-model="form.vehicleId" class="form-input">
            <option value="">Seleccionar vehículo</option>
            <option v-for="v in vehicles" :key="v.id" :value="v.id">
              {{ v.brand }} {{ v.model }} — {{ v.licensePlate }}
            </option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Tipo</label>
            <select v-model="form.type" class="form-input">
              <option value="">Seleccionar</option>
              <option v-for="t in MAINTENANCE_TYPES" :key="t.value" :value="t.value">{{ t.label }}</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">{{ mode === 'now' ? 'Fecha de ingreso' : 'Fecha programada' }}</label>
            <input v-model="form.date" type="date" class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Taller {{ mode === 'schedule' ? '(tentativo, opcional)' : '' }}</label>
            <input v-model="form.workshop" type="text" class="form-input" placeholder="Nombre del taller" />
          </div>
          <div class="form-group">
            <label class="form-label">{{ mode === 'now' ? 'Kilometraje próximo mantenimiento' : 'Kilometraje programado' }} (opcional)</label>
            <input v-model="form.km" type="number" class="form-input" placeholder="0" />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Descripción</label>
          <textarea v-model="form.description" class="form-textarea"
            placeholder="Detalla el trabajo a realizar..." />
        </div>

        <div v-if="mode === 'now'" class="form-row">
          <div class="form-group">
            <label class="form-label">Fecha estimada de salida (opcional)</label>
            <input v-model="form.estimatedExitDate" type="date" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">Fecha próximo mantenimiento (opcional)</label>
            <input v-model="form.nextMaintenanceDate" type="date" class="form-input" />
          </div>
        </div>

        <div class="form-actions">
          <button class="btn-cancel" @click="resetForm">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="saving">
            {{ saving ? 'Guardando...' : (mode === 'now' ? 'Registrar' : 'Programar') }}
          </button>
        </div>
      </div>

    </div>

    <!-- Historial tabla -->
    <div class="card">
      <h3 class="section-title">Historial de mantenimiento</h3>
      <div v-if="maintenanceStore.isLoadingHistory" class="empty-state">Cargando…</div>
      <div v-else-if="!history.length" class="empty-state">Aún no hay mantenimientos registrados.</div>
      <div v-else class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>VEHÍCULO</th>
              <th>TIPO</th>
              <th>DESCRIPCIÓN</th>
              <th>FECHA</th>
              <th>COSTO</th>
              <th>TALLER</th>
              <th>PRÓX. MANT.</th>
              <th>ACCIONES</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in history" :key="item.id">
              <td class="td-bold">{{ item.vehicleLabel }} — {{ item.vehiclePlate }}</td>
              <td>
                <span class="badge" :class="item.type === 'Preventive' ? 'preventivo' : 'correctivo'">
                  • {{ getMaintenanceTypeLabel(item.type) }}
                </span>
              </td>
              <td>{{ item.description }}</td>
              <td class="td-gray">{{ formatDate(item.entryDate) }}</td>
              <td class="td-bold">{{ item.cost != null ? formatCurrency(item.cost) : '—' }}</td>
              <td class="td-gray">{{ item.workshop }}</td>
              <td :class="item.nextMaintenanceDateScheduled ? 'red' : 'td-gray'">
                {{ item.nextMaintenanceDateScheduled ? formatDate(item.nextMaintenanceDateScheduled) : '—' }}
              </td>
              <td>
                <span v-if="item.isClosed" class="td-gray">Cerrado</span>
                <button v-else class="btn-mini" @click="openCloseModal(item)">Cerrar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal: ejecutar mantenimiento programado -->
    <BaseModal
      :model-value="executeModal.isOpen.value"
      size="md"
      @update:model-value="executeModal.close()"
    >
      <template #header><h3>Ejecutar mantenimiento</h3></template>
      <div v-if="executeModal.payload.value" class="modal-form">
        <p class="modal-subtitle">{{ executeModal.payload.value.vehicleLabel }} — {{ executeModal.payload.value.vehiclePlate }}</p>

        <div class="form-group">
          <label class="form-label">Fecha de ingreso</label>
          <input v-model="executeForm.entryDate" type="date" class="form-input" />
        </div>
        <div class="form-group">
          <label class="form-label">Taller</label>
          <input v-model="executeForm.workshop" type="text" class="form-input" />
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Fecha estimada de salida (opcional)</label>
            <input v-model="executeForm.estimatedExitDate" type="date" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">Próximo mantenimiento (opcional)</label>
            <input v-model="executeForm.nextMaintenanceDate" type="date" class="form-input" />
          </div>
        </div>
      </div>
      <template #footer>
        <button class="btn-cancel" @click="executeModal.close()">Cancelar</button>
        <button class="btn-submit" :disabled="saving" @click="handleExecute">
          {{ saving ? 'Procesando...' : 'Confirmar' }}
        </button>
      </template>
    </BaseModal>

    <!-- Modal: cancelar mantenimiento programado -->
    <BaseModal
      :model-value="cancelModal.isOpen.value"
      size="sm"
      @update:model-value="cancelModal.close()"
    >
      <template #header><h3>Cancelar programación</h3></template>
      <div v-if="cancelModal.payload.value" class="modal-form">
        <p class="modal-subtitle">{{ cancelModal.payload.value.vehicleLabel }} — {{ cancelModal.payload.value.vehiclePlate }}</p>
        <div class="form-group">
          <label class="form-label">Motivo</label>
          <textarea v-model="cancelReason" class="form-textarea" placeholder="Indica el motivo de la cancelación..." />
        </div>
      </div>
      <template #footer>
        <button class="btn-cancel" @click="cancelModal.close()">Volver</button>
        <button class="btn-submit" :disabled="saving" @click="handleCancelScheduled">
          {{ saving ? 'Procesando...' : 'Cancelar programación' }}
        </button>
      </template>
    </BaseModal>

    <!-- Modal: cerrar mantenimiento -->
    <BaseModal
      :model-value="closeModal.isOpen.value"
      size="sm"
      @update:model-value="closeModal.close()"
    >
      <template #header><h3>Cerrar mantenimiento</h3></template>
      <div v-if="closeModal.payload.value" class="modal-form">
        <p class="modal-subtitle">{{ closeModal.payload.value.vehicleLabel }} — {{ closeModal.payload.value.vehiclePlate }}</p>
        <div class="form-group">
          <label class="form-label">Fecha de salida real</label>
          <input v-model="closeForm.actualExitDate" type="date" class="form-input" />
        </div>
        <div class="form-group">
          <label class="form-label">Costo (RD$)</label>
          <input v-model="closeForm.cost" type="number" class="form-input" placeholder="0.00" />
        </div>
      </div>
      <template #footer>
        <button class="btn-cancel" @click="closeModal.close()">Cancelar</button>
        <button class="btn-submit" :disabled="saving" @click="handleClose">
          {{ saving ? 'Procesando...' : 'Cerrar mantenimiento' }}
        </button>
      </template>
    </BaseModal>

  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useMaintenanceStore } from '@/stores/maintenance.store'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate, formatCurrency } from '@/utils/formatters'
import { MAINTENANCE_TYPES, getMaintenanceTypeLabel } from '@/utils/enumLabels'
import BaseModal from '@/components/ui/AppBaseModal.vue'

const maintenanceStore = useMaintenanceStore()
const vehicleStore = useVehicleStore()
const toast = useToast()

onMounted(() => {
  vehicleStore.fetchAll()
  maintenanceStore.refreshAll()
})

const vehicles = computed(() => vehicleStore.vehicles)
const alerts = computed(() => maintenanceStore.alerts)
const scheduled = computed(() => maintenanceStore.scheduled)
const history = computed(() => maintenanceStore.history)

const saving = ref(false)
const mode = ref('schedule')

const form = reactive({
  vehicleId: '', type: '', date: '', workshop: '',
  km: '', description: '', estimatedExitDate: '', nextMaintenanceDate: '',
})

function resetForm() {
  Object.assign(form, {
    vehicleId: '', type: '', date: '', workshop: '',
    km: '', description: '', estimatedExitDate: '', nextMaintenanceDate: '',
  })
}

function daysColorClass(days) {
  if (days <= 5) return 'red'
  if (days <= 10) return 'orange'
  return 'gray'
}

function isVehicleAvailable(vehicleId) {
  const vehicle = vehicles.value.find((v) => v.id === vehicleId)
  return vehicle?.status === 'Available'
}

function textColorClass(days) {
  if (days <= 5) return 'text-red'
  if (days <= 10) return 'text-orange'
  return 'text-gray'
}

async function handleSave() {
  if (!form.vehicleId || form.type === '' || !form.date || !form.description) {
    toast.error('Datos incompletos', 'Completa los campos requeridos.')
    return
  }

  saving.value = true
  try {
    if (mode.value === 'now') {
      if (!form.workshop) {
        toast.error('Datos incompletos', 'El taller es requerido para registrar un mantenimiento.')
        return
      }
      await maintenanceStore.registerNow(form.vehicleId, {
        type: Number(form.type),
        description: form.description,
        entryDate: form.date,
        workshop: form.workshop,
        estimatedExitDate: form.estimatedExitDate || null,
        nextMaintenanceDateScheduled: form.nextMaintenanceDate || null,
        nextMaintenanceKmScheduled: form.km ? Number(form.km) : null,
      })
      toast.success('Mantenimiento registrado', 'El vehículo entró a mantenimiento.')
    } else {
      await maintenanceStore.scheduleMaintenance({
        vehicleId: form.vehicleId,
        type: Number(form.type),
        description: form.description,
        scheduledDate: form.date,
        workshop: form.workshop || null,
        scheduledKm: form.km ? Number(form.km) : null,
      })
      toast.success('Mantenimiento programado', 'Se programó correctamente.')
    }
    resetForm()
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al guardar el mantenimiento'))
  } finally {
    saving.value = false
  }
}

// Ejecutar mantenimiento programado
const executeModal = useModal()
const executeForm = reactive({ entryDate: '', workshop: '', estimatedExitDate: '', nextMaintenanceDate: '' })

function openExecuteModal(item) {
  Object.assign(executeForm, {
    entryDate: new Date().toISOString().slice(0, 10),
    workshop: item.workshop || '',
    estimatedExitDate: '',
    nextMaintenanceDate: '',
  })
  executeModal.open(item)
}

async function handleExecute() {
  if (!executeForm.entryDate || !executeForm.workshop) {
    toast.error('Datos incompletos', 'La fecha de ingreso y el taller son requeridos.')
    return
  }

  saving.value = true
  try {
    const item = executeModal.payload.value
    await maintenanceStore.executeScheduled(item.vehicleId, item.id, {
      entryDate: executeForm.entryDate,
      workshop: executeForm.workshop,
      estimatedExitDate: executeForm.estimatedExitDate || null,
      nextMaintenanceDateScheduled: executeForm.nextMaintenanceDate || null,
      nextMaintenanceKmScheduled: null,
    })
    toast.success('Mantenimiento ejecutado', 'El vehículo entró a mantenimiento.')
    executeModal.close()
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al ejecutar el mantenimiento'))
  } finally {
    saving.value = false
  }
}

// Cancelar mantenimiento programado
const cancelModal = useModal()
const cancelReason = ref('')

function openCancelModal(item) {
  cancelReason.value = ''
  cancelModal.open(item)
}

async function handleCancelScheduled() {
  if (!cancelReason.value.trim()) {
    toast.error('Datos incompletos', 'Indica el motivo de la cancelación.')
    return
  }

  saving.value = true
  try {
    const item = cancelModal.payload.value
    await maintenanceStore.cancelScheduled(item.vehicleId, item.id, cancelReason.value)
    toast.success('Programación cancelada', '')
    cancelModal.close()
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al cancelar la programación'))
  } finally {
    saving.value = false
  }
}

// Cerrar mantenimiento abierto
const closeModal = useModal()
const closeForm = reactive({ actualExitDate: '', cost: '' })

function openCloseModal(item) {
  Object.assign(closeForm, { actualExitDate: new Date().toISOString().slice(0, 10), cost: '' })
  closeModal.open(item)
}

async function handleClose() {
  if (!closeForm.actualExitDate || closeForm.cost === '') {
    toast.error('Datos incompletos', 'La fecha de salida y el costo son requeridos.')
    return
  }

  saving.value = true
  try {
    const item = closeModal.payload.value
    await maintenanceStore.closeMaintenance(item.vehicleId, item.id, {
      actualExitDate: closeForm.actualExitDate,
      cost: Number(closeForm.cost),
    })
    toast.success('Mantenimiento cerrado', 'Se cerró correctamente.')
    closeModal.close()
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al cerrar el mantenimiento'))
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.maintenance {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Alerta */
.alert-banner {
  background: var(--amber-bg);
  border: 1px solid var(--amber-border);
  border-radius: 8px;
  padding: 0.875rem 1rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: var(--amber-text);
}

/* Grid principal */
.main-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  align-items: stretch;
}

/* Card */
.card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.25rem;
  box-shadow: var(--shadow-xs);
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.card-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
}

.card-section-title-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
}

.mode-toggle {
  display: flex;
  border: 1px solid var(--border);
  border-radius: 8px;
  overflow: hidden;
}

.mode-toggle button {
  padding: 0.4rem 0.75rem;
  font-size: 0.75rem;
  font-family: 'Inter', sans-serif;
  background: var(--white);
  border: none;
  cursor: pointer;
  color: var(--text-3);
}

.mode-toggle button.active {
  background: var(--blue-hover);
  color: var(--white);
}

.section-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text);
}

.empty-state {
  font-size: 0.85rem;
  color: var(--text-3);
  padding: 1rem 0;
  text-align: center;
}

/* Upcoming */
.upcoming-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.upcoming-item {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0.75rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  flex-wrap: wrap;
}

.upcoming-icon {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.upcoming-icon.red    { background: var(--red-bg); color: var(--red); }
.upcoming-icon.orange { background: var(--amber-bg); color: var(--amber); }
.upcoming-icon.gray   { background: var(--border); color: var(--text-3); }

.upcoming-info { flex: 1; min-width: 160px; }

.upcoming-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text);
}

.upcoming-detail {
  font-size: 0.75rem;
  color: var(--text-3);
  margin-top: 2px;
}

.upcoming-days {
  font-size: 0.82rem;
  font-weight: 500;
  white-space: nowrap;
}

.upcoming-actions {
  display: flex;
  gap: 0.4rem;
}

.text-red    { color: var(--red); }
.text-orange { color: var(--amber); }
.text-gray   { color: var(--text-3); }

.btn-mini {
  padding: 0.35rem 0.65rem;
  border: 1px solid var(--border-strong);
  border-radius: 6px;
  background: var(--white);
  font-size: 0.75rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-mini:hover { background: var(--surface-hover); }
.btn-mini:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-mini:disabled:hover { background: var(--white); }
.btn-mini--danger { color: var(--red); border-color: var(--red-border); }
.btn-mini--danger:hover { background: var(--red-bg); }

/* Formulario */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: var(--text-2);
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  outline: none;
  transition: border-color 0.2s;
  background: var(--white);
}

.form-input:focus {
  border-color: var(--blue-hover);
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-textarea {
  width: 100%;
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  outline: none;
  resize: vertical;
  min-height: 90px;
  transition: border-color 0.2s;
}

.form-textarea:focus {
  border-color: var(--blue-hover);
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 0.25rem;
}

.btn-cancel {
  padding: 0.6rem 1rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-cancel:hover { background: var(--surface-hover); }

.btn-submit {
  padding: 0.6rem 1rem;
  background: var(--blue-hover);
  color: var(--white);
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: var(--blue-hover); }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }

/* Tabla */
.table-wrapper {
  overflow: hidden;
  border-radius: 8px;
  border: 1px solid var(--border);
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  text-align: left;
  font-size: 0.68rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border);
  background: var(--white);
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: var(--text-2);
  border-bottom: 1px solid var(--surface-hover);
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: var(--surface-hover); }

.td-bold { font-weight: 600; color: var(--text); }
.td-gray { color: var(--text-3); }
.red     { color: var(--red); font-weight: 600; }

/* Badges */
.badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
  border: 1px solid transparent;
}

.badge.preventivo { background: var(--mint-bg); color: var(--mint-dark); border-color: var(--mint-border); }
.badge.correctivo { background: var(--amber-bg); color: var(--amber); border-color: var(--amber-border); }

/* Modales */
.modal-form {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.modal-subtitle {
  font-size: 0.85rem;
  color: var(--text-2);
  margin: -0.5rem 0 0;
}
</style>
