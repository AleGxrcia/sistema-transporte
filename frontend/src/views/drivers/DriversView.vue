<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { driverApi } from '@/services/drivers.service'
import { useDriverStore } from '@/stores/drivers.store'
import { useModal } from '@/composables/useModal'
import { useAuthStore } from '@/stores/auth.store'
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
const auth   = useAuthStore()
const toast  = useToast()
const driverStore = useDriverStore()

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
      <button v-if="auth.isAdmin" class="btn primary" @click="createModal.open()">
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
        v-if="auth.isAdmin && !hasActiveFilters"
        class="btn primary"
        @click="createModal.open()"
      >
        <Plus :size="14" /> Registrar primer conductor
      </button>
    </div>

    <!-- Tabla -->
    <template v-else>
      <div class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Conductor</th>
              <th>Cédula</th>
              <th>Teléfono</th>
              <th>Licencia</th>
              <th>Vencimiento</th>
              <th>Estado</th>
              <th style="text-align:center">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="driver in filteredDrivers" :key="driver.id">
              <td>
                <div style="display:flex;align-items:center;gap:8px">
                  <span class="avatar-sm">{{ getInitials(driver.firstName, driver.lastName) }}</span>
                  {{ driver.firstName }} {{ driver.lastName }}
                </div>
              </td>
              <td class="muted">{{ driver.nationalId }}</td>
              <td class="muted">{{ driver.phone }}</td>
              <td><span class="tag">{{ getLicenseCategoryLabel(driver.licenseType) }}</span></td>
              <td :class="{ 'text-red': driver.licenseExpired, 'text-amber': !driver.licenseExpired && driver.licenseExpiringSoon }">
                {{ formatDate(driver.licenseExpirationDate) }}
              </td>
              <td><AppBadge :status="driver.status" /></td>
              <td>
                <div class="action-buttons">
                  <button class="icon-btn" title="Ver detalle" @click="router.push(`/drivers/${driver.id}`)">
                    <Eye :size="13" />
                  </button>
                  <button
                    v-if="auth.isAdmin && driver.status !== 'OnTrip' && driver.status !== 'Suspended'"
                    class="icon-btn edit"
                    title="Editar"
                    @click="router.push(`/drivers/${driver.id}?edit=true`)"
                  >
                    <Pencil :size="13" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
                    class="icon-btn reject"
                    title="Eliminar"
                    @click="deleteModal.open(driver)"
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

.avatar-sm {
  width: 26px;
  height: 26px;
  border-radius: 50%;
  background: var(--blue-light, #dbeafe);
  color: var(--blue, #2563eb);
  font-size: 10px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.text-red   { color: var(--red); font-weight: 600; }
.text-amber { color: #b45309; font-weight: 600; }
</style>
