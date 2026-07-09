<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { VehiclesService } from '@/services/vehicles.service'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useModal } from '@/composables/useModal'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import VehicleForm from '@/components/forms/vehicles/VehicleForm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import AppEmptyState from '@/components/ui/AppEmptyState.vue'
import { Eye, Pencil, Trash2, Plus, Truck, CheckCircle2, Wrench, Ban, RotateCcw } from '@lucide/vue'
import { formatKilometers, formatDate } from '@/utils/formatters'
import { VEHICLE_STATUSES, VEHICLE_TYPES, getVehicleTypeLabel } from '@/utils/enumLabels'

const router = useRouter()
const { can, currentRole } = useAuth()
const toast  = useToast()
const vehicleStore = useVehicleStore()

const isAdmin = computed(() => currentRole.value === 'Administrador')

const search       = ref('')
const statusFilter = ref('')
const typeFilter   = ref('')
const viewMode     = ref('active') // 'active' | 'archived'
const isArchived   = computed(() => viewMode.value === 'archived')

onMounted(() => vehicleStore.fetchAll())

function setView(mode) {
  if (viewMode.value === mode) return
  viewMode.value = mode
  search.value = ''
  statusFilter.value = ''
  typeFilter.value = ''
  refresh()
}

const vehicles  = computed(() => vehicleStore.vehicles)
const isLoading = computed(() => vehicleStore.isLoadingList)

const filteredVehicles = computed(() => {
  let result = vehicles.value
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    result = result.filter((v) =>
      v.licensePlate?.toLowerCase().includes(q) ||
      v.brand?.toLowerCase().includes(q) ||
      v.model?.toLowerCase().includes(q)
    )
  }
  if (statusFilter.value) result = result.filter((v) => v.status === statusFilter.value)
  if (typeFilter.value) result = result.filter((v) => v.type === typeFilter.value)
  return result
})

const hasActiveFilters = computed(() => !!(search.value || statusFilter.value || typeFilter.value))

const fleetStats = computed(() => ({
  total: vehicles.value.length,
  available: vehicles.value.filter((v) => v.status === 'Available').length,
  maintenance: vehicles.value.filter((v) => v.status === 'InMaintenance').length,
  outOfService: vehicles.value.filter((v) => v.status === 'Inactive').length,
}))

const createModal = useModal()
const deleteModal  = useModal()
const isDeleting   = ref(false)

function refresh() {
  vehicleStore.fetchAll(isArchived.value)
}

async function confirmDelete() {
  try {
    isDeleting.value = true
    await VehiclesService.delete(deleteModal.payload.value.id)
    deleteModal.close()
    toast.success('Vehículo archivado', `${deleteModal.payload.value?.licensePlate} se archivó. Su historial se conserva.`)
    refresh()
  } catch (err) {
    deleteModal.close()
    const msg = err.response?.status === 409
      ? 'No se puede archivar: el vehículo tiene viajes activos.'
      : getErrorMessage(err, 'Error al archivar')
    toast.error('No se pudo archivar', msg)
  } finally {
    isDeleting.value = false
  }
}

async function restoreVehicle(vehicle) {
  try {
    await VehiclesService.restore(vehicle.id)
    toast.success('Vehículo restaurado', `${vehicle.licensePlate} volvió a los listados.`)
    refresh()
  } catch (err) {
    toast.error('No se pudo restaurar', getErrorMessage(err, 'Error al restaurar'))
  }
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <h1>Gestión de vehículos</h1>
    </div>

    <!-- KPIs -->
    <div v-if="!isArchived" class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
      <div class="kpi">
        <div class="kpi-label">Total</div>
        <div class="kpi-val">{{ fleetStats.total }}</div>
        <div class="kpi-sub">Vehículos en la flota</div>
        <div class="kpi-icon blue"><Truck :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Disponibles</div>
        <div class="kpi-val" style="color:var(--mint-dark)">{{ fleetStats.available }}</div>
        <div class="kpi-sub">Listos para asignar</div>
        <div class="kpi-icon green"><CheckCircle2 :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Mantenimiento</div>
        <div class="kpi-val" style="color:var(--amber)">{{ fleetStats.maintenance }}</div>
        <div class="kpi-sub">En taller actualmente</div>
        <div class="kpi-icon amber"><Wrench :size="16" /></div>
      </div>
      <div class="kpi">
        <div class="kpi-label">Fuera de servicio</div>
        <div class="kpi-val" style="color:var(--red)">{{ fleetStats.outOfService }}</div>
        <div class="kpi-sub">Inactivos</div>
        <div class="kpi-icon red"><Ban :size="16" /></div>
      </div>
    </div>

    <!-- Toggle Activos / Archivados (solo Admin) -->
    <div v-if="isAdmin" class="view-toggle">
      <button :class="{ active: !isArchived }" @click="setView('active')">Activos</button>
      <button :class="{ active: isArchived }" @click="setView('archived')">Archivados</button>
    </div>

    <!-- Filtros -->
    <div class="filter-bar">
      <div class="filter-group">
        <input
          v-model="search"
          class="search-input"
          placeholder="Buscar por matrícula, marca, modelo…"
          style="max-width:340px"
        />
        <select v-model="statusFilter">
          <option value="">Todos los estados</option>
          <option v-for="s in VEHICLE_STATUSES" :key="s.name" :value="s.name">{{ s.label }}</option>
        </select>
        <select v-model="typeFilter">
          <option value="">Todos los tipos</option>
          <option v-for="t in VEHICLE_TYPES" :key="t.name" :value="t.name">{{ t.label }}</option>
        </select>
      </div>
      <button v-if="can('create', 'vehicles') && !isArchived" class="btn primary" @click="createModal.open()">
        <Plus :size="14" /> Nuevo vehículo
      </button>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando vehículos…</div>

    <!-- Empty state -->
    <AppEmptyState
      v-else-if="filteredVehicles.length === 0"
      :title="isArchived ? 'No hay vehículos archivados' : 'Aún no hay vehículos'"
      :message="isArchived
        ? 'Los vehículos que archives aparecerán aquí y podrás restaurarlos.'
        : hasActiveFilters
          ? 'No se encontraron vehículos con los filtros aplicados.'
          : 'Registra el primer vehículo de tu flota para comenzar a gestionar solicitudes y viajes.'"
    >
      <template #icon>
        <svg width="30" height="30" fill="none" stroke="var(--blue)" stroke-width="1.5" viewBox="0 0 24 24">
          <path d="M7 17m-2 0a2 2 0 1 0 4 0"/>
          <path d="M17 17m-2 0a2 2 0 1 0 4 0"/>
          <path d="M5 17h-2v-11a1 1 0 0 1 1-1h9v12m-4 0h6m4 0h2v-6h-8m0-5h5l3 5"/>
        </svg>
      </template>
      <template #action v-if="can('create', 'vehicles') && !hasActiveFilters && !isArchived">
        <button class="btn primary" @click="createModal.open()">
          <Plus :size="14" /> Registrar primer vehículo
        </button>
      </template>
    </AppEmptyState>

    <!-- Tabla -->
    <template v-else>
      <div class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Matrícula</th>
              <th>Marca / Modelo</th>
              <th>Tipo</th>
              <th>Año</th>
              <th>Capacidad</th>
              <th>Kilometraje</th>
              <th>Estado</th>
              <th style="text-align:center">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="vehicle in filteredVehicles" :key="vehicle.id">
              <td class="plate-cell">{{ vehicle.licensePlate }}</td>
              <td>{{ vehicle.brand }} {{ vehicle.model }}</td>
              <td><span class="tag">{{ getVehicleTypeLabel(vehicle.type) }}</span></td>
              <td class="muted">{{ vehicle.year }}</td>
              <td>{{ vehicle.capacity }} pas.</td>
              <td class="muted">{{ formatKilometers(vehicle.currentMileage) }}</td>
              <td><AppBadge :status="vehicle.status" /></td>
              <td>
                <div class="action-buttons">
                  <button class="icon-btn view" title="Ver detalle" @click="router.push(`/vehicles/${vehicle.id}`)">
                    <Eye :size="14" />
                  </button>
                  <template v-if="!isArchived">
                    <button
                      v-if="can('edit', 'vehicles') && vehicle.status !== 'OnTrip'"
                      class="icon-btn edit"
                      title="Editar"
                      @click="router.push(`/vehicles/${vehicle.id}?edit=true`)"
                    >
                      <Pencil :size="14" />
                    </button>
                    <button
                      v-if="can('delete', 'vehicles')"
                      class="icon-btn reject"
                      title="Eliminar"
                      @click="deleteModal.open(vehicle)"
                    >
                      <Trash2 :size="14" />
                    </button>
                  </template>
                  <button
                    v-else
                    class="icon-btn restore"
                    :title="vehicle.deletedAt ? `Archivado el ${formatDate(vehicle.deletedAt)} · Restaurar` : 'Restaurar'"
                    @click="restoreVehicle(vehicle)"
                  >
                    <RotateCcw :size="14" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

    </template>

    <!-- ── BaseModal: Crear vehículo ─────────────────────────────────────── -->
    <BaseModal
      :model-value="createModal.isOpen.value"
      @update:model-value="createModal.close()"
    >
      <template #header><h3>Nuevo vehículo</h3></template>
      <VehicleForm
        @saved="createModal.close(); toast.success('Vehículo registrado', 'El vehículo fue agregado a la flota.'); refresh()"
        @cancel="createModal.close()"
      />
    </BaseModal>

    <!-- ── ConfirmModal: Eliminar ─────────────────────────────────────────── -->
    <ConfirmModal
      :model-value="deleteModal.isOpen.value"
      title="¿Eliminar vehículo?"
      :message="`El vehículo ${deleteModal.payload.value?.licensePlate} se archivará y dejará de aparecer en los listados. Su historial de viajes, consumo y mantenimiento se conserva.`"
      confirm-text="Eliminar"
      variant="danger"
      :is-loading="isDeleting"
      @update:model-value="deleteModal.close()"
      @confirm="confirmDelete"
    />
  </div>
</template>

<style scoped>
/* Fila de filtros transparente (sin tarjeta): los inputs y el botón
   quedan directamente sobre el lienzo, igual que la referencia. */
.filter-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
  margin-bottom: 16px;
}
/* Grupo búsqueda + filtros: ocupa el espacio disponible y empuja
   el botón de acción al final de la fila, dejando el hueco intermedio. */
.filter-group {
  display: flex;
  align-items: center;
  gap: 10px;
  flex: 1;
  min-width: 280px;
  flex-wrap: wrap;
}
.filter-bar select {
  width: auto;
  flex: 0 0 auto;
  min-width: 160px;
  height: 34px;
  padding: 0 10px;
  font-size: 13.5px;
  color: var(--text-2);
}

/* Toggle Activos / Archivados */
.view-toggle {
  display: inline-flex;
  gap: 2px;
  padding: 3px;
  margin-bottom: 14px;
  background: var(--surface-2, #eef1f5);
  border-radius: 8px;
}
.view-toggle button {
  border: none;
  background: transparent;
  padding: 6px 16px;
  font-size: 13px;
  font-weight: 600;
  color: var(--text-2);
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.15s, color 0.15s;
}
.view-toggle button.active {
  background: var(--surface, #fff);
  color: var(--blue);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.08);
}
.icon-btn.restore {
  color: var(--blue);
}
</style>
