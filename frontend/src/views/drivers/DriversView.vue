<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { driverApi } from '@/services/drivers.service'
import { useDriverStore } from '@/stores/drivers.store'
import { useModal } from '@/composables/useModal'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import DriverForm from '@/components/forms/drivers/DriverForm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Eye, Pencil, Trash2, Plus } from '@lucide/vue'
import { formatDate, getInitials } from '@/utils/formatters'
import { DRIVER_STATUSES, getLicenseCategoryLabel } from '@/utils/enumLabels'

const router = useRouter()
const { can } = useAuth()
const toast  = useToast()
const driverStore = useDriverStore()

const AVATAR_PALETTE = [
  { background: 'var(--blue-light)', color: 'var(--blue)' },
  { background: 'var(--sky-bg)', color: 'var(--sky)' },
  { background: 'var(--purple-bg)', color: 'var(--purple)' },
  { background: 'var(--amber-bg)', color: 'var(--amber-text)' },
  { background: 'var(--mint-bg)', color: 'var(--mint-dark)' },
]
function avatarStyle(index) {
  return AVATAR_PALETTE[index % AVATAR_PALETTE.length]
}

const search       = ref('')
const statusFilter = ref('')

onMounted(() => driverStore.fetchAll())

const drivers   = computed(() => driverStore.drivers)
const isLoading = computed(() => driverStore.isLoadingList)

const filteredDrivers = computed(() => {
  let result = drivers.value
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    result = result.filter((d) =>
      d.firstName?.toLowerCase().includes(q) ||
      d.lastName?.toLowerCase().includes(q) ||
      d.nationalId?.includes(q)
    )
  }
  if (statusFilter.value) result = result.filter((d) => d.status === statusFilter.value)
  return result
})

const hasActiveFilters = computed(() => !!(search.value || statusFilter.value))

const createModal = useModal()
const deleteModal  = useModal()
const isDeleting   = ref(false)

function refresh() {
  driverStore.fetchAll()
}

async function confirmDelete() {
  try {
    isDeleting.value = true
    await driverApi.delete(deleteModal.payload.value.id)
    deleteModal.close()
    toast.success('Conductor eliminado', `${deleteModal.payload.value?.firstName} ${deleteModal.payload.value?.lastName} fue eliminado correctamente.`)
    refresh()
  } catch (err) {
    deleteModal.close()
    toast.error('No se pudo eliminar', getErrorMessage(err, 'Error al eliminar'))
  } finally {
    isDeleting.value = false
  }
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <h1>Gestión de conductores</h1>
      <button v-if="can('create', 'drivers')" class="btn primary" @click="createModal.open()">
        <Plus :size="14" /> Nuevo conductor
      </button>
    </div>

    <!-- Filtros -->
    <div class="search-row">
      <input
        v-model="search"
        class="search-input"
        placeholder="Buscar por nombre o cédula…"
      />
      <select v-model="statusFilter">
        <option value="">Todos los estados</option>
        <option v-for="s in DRIVER_STATUSES" :key="s.name" :value="s.name">{{ s.label }}</option>
      </select>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando conductores…</div>

    <!-- Empty state -->
    <div v-else-if="filteredDrivers.length === 0" class="empty-state">
      <div class="empty-icon empty-icon--blue">
        <svg width="30" height="30" fill="none" stroke="var(--blue)" stroke-width="1.5" viewBox="0 0 24 24">
          <circle cx="12" cy="8" r="4"/>
          <path d="M4 21v-1a8 8 0 0 1 16 0v1"/>
        </svg>
      </div>
      <p class="empty-title">Aún no hay conductores</p>
      <p class="empty-sub">
        {{
          hasActiveFilters
            ? 'No se encontraron conductores con los filtros aplicados.'
            : 'Registra el primer conductor para comenzar a asignar viajes.'
        }}
      </p>
      <button
        v-if="can('create', 'drivers') && !hasActiveFilters"
        class="btn primary"
        @click="createModal.open()"
      >
        <Plus :size="14" /> Registrar primer conductor
      </button>
    </div>

    <!-- Grid de conductores -->
    <template v-else>
      <div class="grid3">
        <div v-for="(driver, index) in filteredDrivers" :key="driver.id" class="driver-card">
          <div class="driver-avatar" :style="avatarStyle(index)">
            {{ getInitials(driver.firstName, driver.lastName) }}
          </div>
          <div class="driver-info">
            <div class="driver-name">{{ driver.firstName }} {{ driver.lastName }}</div>
            <div class="driver-meta">Cédula: {{ driver.nationalId }}</div>
            <div class="driver-meta">Tel: {{ driver.phone }}</div>

            <div class="driver-card-footer">
              <AppBadge :status="driver.status" />
              <div class="action-buttons">
                <button class="icon-btn" title="Ver detalle" @click="router.push(`/drivers/${driver.id}`)">
                  <Eye :size="12" />
                </button>
                <button
                  v-if="can('edit', 'drivers') && driver.status !== 'OnTrip' && driver.status !== 'Suspended'"
                  class="icon-btn edit"
                  title="Editar"
                  @click="router.push(`/drivers/${driver.id}?edit=true`)"
                >
                  <Pencil :size="12" />
                </button>
                <button
                  v-if="can('delete', 'drivers')"
                  class="icon-btn reject"
                  title="Eliminar"
                  @click="deleteModal.open(driver)"
                >
                  <Trash2 :size="12" />
                </button>
              </div>
            </div>

            <div
              class="driver-lic"
              :class="{ danger: driver.licenseExpired, warn: !driver.licenseExpired && driver.licenseExpiringSoon }"
            >
              <span v-if="driver.licenseExpired || driver.licenseExpiringSoon">⚠ </span>
              Lic. {{ getLicenseCategoryLabel(driver.licenseType) }} —
              {{ driver.licenseExpired ? 'Vencida' : 'Vence' }} {{ formatDate(driver.licenseExpirationDate) }}
            </div>
          </div>
        </div>

        <div v-if="can('create', 'drivers')" class="driver-card driver-card--add" @click="createModal.open()">
          <div class="add-icon"><Plus :size="16" /></div>
          <span>Agregar conductor</span>
        </div>
      </div>
    </template>

    <!-- ── BaseModal: Crear conductor ────────────────────────────────────── -->
    <BaseModal
      :model-value="createModal.isOpen.value"
      @update:model-value="createModal.close()"
    >
      <template #header><h3>Nuevo conductor</h3></template>
      <DriverForm
        @saved="createModal.close(); toast.success('Conductor registrado', 'El conductor fue agregado al sistema.'); refresh()"
        @cancel="createModal.close()"
      />
    </BaseModal>

    <!-- ── ConfirmModal: Eliminar ─────────────────────────────────────────── -->
    <ConfirmModal
      :model-value="deleteModal.isOpen.value"
      title="¿Eliminar conductor?"
      :message="`Esta acción eliminará permanentemente a ${deleteModal.payload.value?.firstName} ${deleteModal.payload.value?.lastName} del sistema.`"
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

.driver-card-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: 9px;
}

.driver-card--add {
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: 8px;
  border-style: dashed;
  border-color: var(--border-strong);
  background: var(--surface-hover);
  box-shadow: none;
  cursor: pointer;
  color: var(--text-3);
  font-size: 12.5px;
  font-weight: 500;
  min-height: 132px;
}
.driver-card--add:hover {
  transform: none;
}
.driver-card--add:hover {
  border-color: var(--blue-mid);
  background: var(--blue-light);
  color: var(--blue);
}
.driver-card--add .add-icon {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: var(--white);
  border: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-3);
  transition: color 0.15s, border-color 0.15s;
}
.driver-card--add:hover .add-icon {
  color: var(--blue);
  border-color: var(--blue-mid);
}
</style>
