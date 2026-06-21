<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { VehiclesService } from '@/services/vehicles.service'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useModal } from '@/composables/useModal'
import { useAuthStore } from '@/stores/auth.store'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import VehicleForm from '@/components/forms/vehicles/VehicleForm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Eye, Pencil, Trash2, Plus } from '@lucide/vue'
import { formatKilometers } from '@/utils/formatters'
import { VEHICLE_STATUSES, VEHICLE_TYPES, getVehicleTypeLabel } from '@/utils/enumLabels'

const router = useRouter()
const auth   = useAuthStore()
const toast  = useToast()
const vehicleStore = useVehicleStore()

const search       = ref('')
const statusFilter = ref('')
const typeFilter   = ref('')

onMounted(() => vehicleStore.fetchAll())

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

const createModal = useModal()
const deleteModal  = useModal()
const isDeleting   = ref(false)

function refresh() {
  vehicleStore.fetchAll()
}

async function confirmDelete() {
  try {
    isDeleting.value = true
    await VehiclesService.delete(deleteModal.payload.value.id)
    deleteModal.close()
    toast.success('Vehículo eliminado', `${deleteModal.payload.value?.licensePlate} fue eliminado correctamente.`)
    refresh()
  } catch (err) {
    deleteModal.close()
    const msg = err.response?.status === 409
      ? 'No se puede eliminar: el vehículo tiene viajes activos.'
      : getErrorMessage(err, 'Error al eliminar')
    toast.error('No se pudo eliminar', msg)
  } finally {
    isDeleting.value = false
  }
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <h1>Gestión de vehículos</h1>
      <button v-if="auth.isAdmin" class="btn primary" @click="createModal.open()">
        <Plus :size="14" /> Nuevo vehículo
      </button>
    </div>

    <!-- Filtros -->
    <div class="search-row">
      <input
        v-model="search"
        class="search-input"
        placeholder="Buscar por matrícula, marca, modelo…"
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

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando vehículos…</div>

    <!-- Empty state -->
    <div v-else-if="filteredVehicles.length === 0" class="empty-state">
      <div class="empty-icon empty-icon--blue">
        <svg width="30" height="30" fill="none" stroke="var(--blue)" stroke-width="1.5" viewBox="0 0 24 24">
          <path d="M7 17m-2 0a2 2 0 1 0 4 0"/>
          <path d="M17 17m-2 0a2 2 0 1 0 4 0"/>
          <path d="M5 17h-2v-11a1 1 0 0 1 1-1h9v12m-4 0h6m4 0h2v-6h-8m0-5h5l3 5"/>
        </svg>
      </div>
      <p class="empty-title">Aún no hay vehículos</p>
      <p class="empty-sub">
        {{
          hasActiveFilters
            ? 'No se encontraron vehículos con los filtros aplicados.'
            : 'Registra el primer vehículo de tu flota para comenzar a gestionar solicitudes y viajes.'
        }}
      </p>
      <button
        v-if="auth.isAdmin && !hasActiveFilters"
        class="btn primary"
        @click="createModal.open()"
      >
        <Plus :size="14" /> Registrar primer vehículo
      </button>
    </div>

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
                  <button class="icon-btn" title="Ver detalle" @click="router.push(`/vehicles/${vehicle.id}`)">
                    <Eye :size="13" />
                  </button>
                  <button
                    v-if="auth.isAdmin && vehicle.status !== 'OnTrip'"
                    class="icon-btn edit"
                    title="Editar"
                    @click="router.push(`/vehicles/${vehicle.id}?edit=true`)"
                  >
                    <Pencil :size="13" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
                    class="icon-btn reject"
                    title="Eliminar"
                    @click="deleteModal.open(vehicle)"
                  >
                    <Trash2 :size="13" />
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
      :message="`Esta acción eliminará permanentemente el vehículo ${deleteModal.payload.value?.licensePlate} del sistema.`"
      confirm-text="Eliminar"
      variant="danger"
      :is-loading="isDeleting"
      @update:model-value="deleteModal.close()"
      @confirm="confirmDelete"
    />
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
  font-size: 15px;
  font-weight: 700;
  color: var(--text);
  margin: 0;
}
.empty-sub {
  font-size: 13px;
  color: var(--text-3);
  line-height: 1.6;
  max-width: 360px;
  margin: 0 0 8px;
}
</style>
