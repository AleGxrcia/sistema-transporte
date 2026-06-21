<script setup>
import { onMounted, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useAuthStore } from '@/stores/auth.store'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import BaseBadge from '@/components/ui/AppBadge.vue'
import VehicleForm from '@/components/forms/vehicles/VehicleForm.vue'
import MaintenanceForm from '@/components/forms/vehicles/MaintenanceForm.vue'
import FuelForm from '@/components/forms/vehicles/FuelForm.vue'
import { ArrowLeft, Pencil, Wrench, Fuel, PowerOff, Trash2, CheckCircle, RefreshCw } from '@lucide/vue'
import { formatDate, formatKilometers, formatCurrency } from '@/utils/formatters'
import { getVehicleTypeLabel, getMaintenanceTypeLabel } from '@/utils/enumLabels'

const props = defineProps({ id: { type: String, required: true } })

const route        = useRoute()
const router       = useRouter()
const vehicleStore = useVehicleStore()
const auth         = useAuthStore()
const toast        = useToast()

const vehicle   = computed(() => vehicleStore.currentVehicle)
const isLoading = computed(() => vehicleStore.isLoadingDetail)

// ── Modales ────────────────────────────────────────────────────────────────
const editModal         = useModal()  // BaseModal    — formulario edición
const maintenanceModal  = useModal()  // BaseModal    — formulario mantenimiento
const fuelModal         = useModal()  // BaseModal    — formulario combustible
const deactivateModal   = useModal()  // ConfirmModal warning
const deleteModal       = useModal()  // ConfirmModal danger
const closeMaintModal   = useModal()  // ConfirmModal primary — con inputs en slot

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

// ── Handlers ───────────────────────────────────────────────────────────────
onMounted(async () => {
  await vehicleStore.fetchById(props.id)
  if (route.query.edit === 'true' && vehicle.value) {
    editModal.open(vehicle.value)
  }
})

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

function afterSaved(successMsg) {
  return () => {
    editModal.close()
    maintenanceModal.close()
    fuelModal.close()
    toast.success('Guardado', successMsg)
    vehicleStore.fetchById(props.id)
  }
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

      <div v-if="vehicle && auth.isAdmin" style="display:flex;gap:8px">
        <button class="btn" @click="editModal.open(vehicle)">
          <Pencil :size="14" /> Editar
        </button>
        <button class="btn" @click="maintenanceModal.open()">
          <Wrench :size="14" /> Mantenimiento
        </button>
        <button class="btn" @click="fuelModal.open()">
          <Fuel :size="14" /> Combustible
        </button>
        <button
          v-if="vehicle.status === 'Inactive'"
          class="btn"
          style="border-color:#16a34a;color:#16a34a"
          @click="handleReactivate"
        >
          <RefreshCw :size="14" /> Reactivar
        </button>
        <button
          v-else
          class="btn"
          style="border-color:var(--amber-border);color:#b45309"
          @click="deactivateModal.open()"
        >
          <PowerOff :size="14" /> Desactivar
        </button>
        <button class="btn danger" @click="deleteModal.open()">
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
      <div class="detail-grid">
        <!-- Card: Info general -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Información general</span>
            <BaseBadge :status="vehicle.status" />
          </div>
          <dl class="info-list">
            <div class="info-row"><dt>Matrícula</dt><dd>{{ vehicle.licensePlate }}</dd></div>
            <div class="info-row"><dt>Marca</dt><dd>{{ vehicle.brand }}</dd></div>
            <div class="info-row"><dt>Modelo</dt><dd>{{ vehicle.model }}</dd></div>
            <div class="info-row"><dt>Año</dt><dd>{{ vehicle.year }}</dd></div>
            <div class="info-row"><dt>Color</dt><dd>{{ vehicle.color || '—' }}</dd></div>
            <div class="info-row"><dt>Tipo</dt><dd>{{ getVehicleTypeLabel(vehicle.type) }}</dd></div>
            <div class="info-row"><dt>Capacidad</dt><dd>{{ vehicle.capacity }} pasajeros</dd></div>
            <div class="info-row"><dt>Kilometraje</dt><dd>{{ formatKilometers(vehicle.currentMileage) }}</dd></div>
          </dl>
        </div>

        <!-- Card: Mantenimiento -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Mantenimiento</span>
            <button v-if="auth.isAdmin" class="btn-sm" @click="maintenanceModal.open()">
              + Registrar
            </button>
          </div>
          <div v-if="!vehicle.maintenanceRecords?.length" class="empty-card">
            Sin registros de mantenimiento
          </div>
          <div v-else class="maintenance-list">
            <div
              v-for="record in vehicle.maintenanceRecords"
              :key="record.id"
              class="maintenance-item"
            >
              <div class="maint-top">
                <span class="tag">{{ getMaintenanceTypeLabel(record.type) }}</span>
                <span class="muted">{{ formatDate(record.entryDate) }}</span>
              </div>
              <p class="maint-desc">{{ record.description }}</p>
              <div v-if="record.workshop" class="muted" style="font-size:12px">
                Taller: {{ record.workshop }}
              </div>
              <div v-if="record.isClosed" class="muted" style="font-size:12px">
                Cerrado: {{ formatDate(record.actualExitDate) }} · Costo: {{ formatCurrency(record.cost) }}
              </div>
              <button
                v-else-if="auth.isAdmin"
                class="btn-sm"
                style="margin-top:4px;align-self:flex-start"
                @click="openCloseMaintenance(record)"
              >
                <CheckCircle :size="12" /> Cerrar mantenimiento
              </button>
            </div>
          </div>
        </div>

        <!-- Card: Combustible -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Combustible</span>
            <button v-if="auth.isAdmin" class="btn-sm" @click="fuelModal.open()">
              + Registrar
            </button>
          </div>
          <div v-if="!vehicle.fuelRecords?.length" class="empty-card">
            Sin registros de combustible
          </div>
          <div v-else class="fuel-list">
            <div v-for="record in vehicle.fuelRecords" :key="record.id" class="fuel-item">
              <div class="fuel-top">
                <span>{{ record.gallons }} gal</span>
                <span class="muted">{{ formatDate(record.recordDate) }}</span>
              </div>
              <div class="muted" style="font-size:12px">
                {{ formatCurrency(record.pricePerGallon) }}/gal
                · Total: {{ formatCurrency(record.totalCost) }}
              </div>
              <div v-if="record.mileageAtRefuel" class="muted" style="font-size:12px">
                {{ formatKilometers(record.mileageAtRefuel) }}
              </div>
            </div>
          </div>
        </div>
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
        @saved="afterSaved('Vehículo actualizado correctamente')()"
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
        @saved="afterSaved('Mantenimiento registrado')()"
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
        @saved="afterSaved('Carga de combustible registrada')()"
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
.detail-grid {
  display: grid;
  grid-template-columns: 300px 1fr 1fr;
  gap: 16px;
  align-items: start;
}
@media (max-width: 900px) {
  .detail-grid { grid-template-columns: 1fr; }
}
.info-list   { display: flex; flex-direction: column; gap: 6px; }
.info-row    { display: flex; justify-content: space-between; font-size: 13px; }
.info-row dt { color: var(--text-3); }
.info-row dd { font-weight: 500; color: var(--text); }

.maintenance-list, .fuel-list { display: flex; flex-direction: column; gap: 12px; }
.maintenance-item, .fuel-item {
  padding: 10px 12px;
  background: var(--bg);
  border: 1px solid var(--border);
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.maint-top, .fuel-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
}
.maint-desc { font-size: 13px; color: var(--text-2); margin: 0; }
.empty-card { font-size: 13px; color: var(--text-3); text-align: center; padding: 24px; }
.loading-placeholder { padding: 48px; text-align: center; color: var(--text-3); }
.btn-sm {
  font-size: 11px;
  padding: 4px 10px;
  border: 1px solid var(--border-strong);
  border-radius: 6px;
  background: transparent;
  cursor: pointer;
  color: var(--text-2);
}
.btn-sm:hover { background: var(--bg); }
</style>