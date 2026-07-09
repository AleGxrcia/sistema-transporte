<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useDriverStore } from '@/stores/drivers.store'
import { useModal } from '@/composables/useModal'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import DriverForm from '@/components/forms/drivers/DriverForm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import AppEmptyState from '@/components/ui/AppEmptyState.vue'
import { Plus, AlertTriangle, Phone, ShieldCheck, RotateCcw } from '@lucide/vue'
import { formatDate, getInitials } from '@/utils/formatters'
import { getErrorMessage } from '@/utils/apiError'
import { driverApi } from '@/services/drivers.service'
import { DRIVER_STATUSES, getLicenseCategoryLabel } from '@/utils/enumLabels'

// Color de avatar determinista por conductor (paleta del sistema de diseño)
const AVATAR_COLORS = ['var(--blue)', 'var(--purple)', 'var(--mint-dark)', 'var(--amber)', 'var(--sky)', 'var(--navy)']
function avatarColor(driver) {
  const key = `${driver.firstName ?? ''}${driver.lastName ?? ''}${driver.id ?? ''}`
  let hash = 0
  for (let i = 0; i < key.length; i++) hash = (hash * 31 + key.charCodeAt(i)) | 0
  return AVATAR_COLORS[Math.abs(hash) % AVATAR_COLORS.length]
}

const router = useRouter()
const { can, currentRole } = useAuth()
const toast  = useToast()
const driverStore = useDriverStore()

const isAdmin = computed(() => currentRole.value === 'Administrador')

const search       = ref('')
const statusFilter = ref('')
const viewMode     = ref('active') // 'active' | 'archived'
const isArchived   = computed(() => viewMode.value === 'archived')

onMounted(() => driverStore.fetchAll())

function setView(mode) {
  if (viewMode.value === mode) return
  viewMode.value = mode
  search.value = ''
  statusFilter.value = ''
  refresh()
}

async function restoreDriver(driver) {
  try {
    await driverApi.restore(driver.id)
    toast.success('Conductor restaurado', `${driver.firstName} ${driver.lastName} volvió a los listados.`)
    refresh()
  } catch (err) {
    toast.error('No se pudo restaurar', getErrorMessage(err, 'Error al restaurar'))
  }
}

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

function refresh() {
  driverStore.fetchAll(isArchived.value)
}
</script>

<template>
  <div>
    <!-- Header -->
    <div class="page-header">
      <h1>Gestión de conductores</h1>
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
          placeholder="Buscar por nombre o cédula…"
          style="max-width:360px"
        />
        <select v-model="statusFilter">
          <option value="">Todos los estados</option>
          <option v-for="s in DRIVER_STATUSES" :key="s.name" :value="s.name">{{ s.label }}</option>
        </select>
      </div>
      <button v-if="can('create', 'drivers') && !isArchived" class="btn primary" @click="createModal.open()">
        <Plus :size="14" /> Nuevo conductor
      </button>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-placeholder">Cargando conductores…</div>

    <!-- Empty state -->
    <AppEmptyState
      v-else-if="filteredDrivers.length === 0"
      :title="isArchived ? 'No hay conductores archivados' : 'Aún no hay conductores'"
      :message="isArchived
        ? 'Los conductores que archives aparecerán aquí y podrás restaurarlos.'
        : hasActiveFilters
          ? 'No se encontraron conductores con los filtros aplicados.'
          : 'Registra el primer conductor para comenzar a asignar viajes.'"
    >
      <template #icon>
        <svg width="30" height="30" fill="none" stroke="var(--blue)" stroke-width="1.5" viewBox="0 0 24 24">
          <circle cx="12" cy="8" r="4"/>
          <path d="M4 21v-1a8 8 0 0 1 16 0v1"/>
        </svg>
      </template>
      <template #action v-if="can('create', 'drivers') && !hasActiveFilters && !isArchived">
        <button class="btn primary" @click="createModal.open()">
          <Plus :size="14" /> Registrar primer conductor
        </button>
      </template>
    </AppEmptyState>

    <!-- Grid de conductores -->
    <template v-else>
      <div class="grid3">
        <div
          v-for="driver in filteredDrivers"
          :key="driver.id"
          class="driver-card"
          @click="router.push(`/drivers/${driver.id}`)"
        >
          <!-- Identidad: avatar + nombre + cédula + estado -->
          <div class="dc-top">
            <div class="driver-avatar" :style="{ background: avatarColor(driver) }">
              {{ getInitials(driver.firstName, driver.lastName) }}
            </div>
            <div class="dc-identity">
              <div class="driver-name">{{ driver.firstName }} {{ driver.lastName }}</div>
              <div class="dc-cedula">{{ driver.nationalId }}</div>
            </div>
            <AppBadge :status="driver.status" />
          </div>

          <!-- Contacto + licencia -->
          <div class="dc-details">
            <div class="dc-line">
              <Phone :size="13" class="dc-ico" />
              <span>{{ driver.phone }}</span>
            </div>
            <div
              class="dc-line"
              :class="{ danger: driver.licenseExpired, warn: !driver.licenseExpired && driver.licenseExpiringSoon }"
            >
              <AlertTriangle v-if="driver.licenseExpired || driver.licenseExpiringSoon" :size="13" class="dc-ico" />
              <ShieldCheck v-else :size="13" class="dc-ico" />
              <span>
                Lic. {{ getLicenseCategoryLabel(driver.licenseType) }} —
                {{ driver.licenseExpired ? 'Vencida' : 'Vence' }} {{ formatDate(driver.licenseExpirationDate) }}
              </span>
            </div>
          </div>

          <!-- Acción restaurar (modo archivados) -->
          <div v-if="isArchived" class="dc-archived">
            <span class="dc-archived-date">Archivado {{ formatDate(driver.deletedAt) }}</span>
            <button class="btn ghost sm" @click.stop="restoreDriver(driver)">
              <RotateCcw :size="13" /> Restaurar
            </button>
          </div>
        </div>

        <div v-if="can('create', 'drivers') && !isArchived" class="driver-card driver-card--add" @click="createModal.open()">
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

/* Pie de card en modo archivado */
.dc-archived {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  margin-top: 14px;
  padding-top: 12px;
  border-top: 1px solid var(--border);
}
.dc-archived-date {
  font-size: 11.5px;
  color: var(--text-3);
}

/* Card de conductor en columna con zonas claras (igual que la referencia) */
.driver-card {
  flex-direction: column;
  align-items: stretch;
  gap: 0;
  padding: 18px;
  border-radius: 13px;
}
.driver-card:hover {
  border-color: var(--blue-mid);
}

/* Zona 1 — identidad */
.dc-top {
  display: flex;
  align-items: center;
  gap: 13px;
  width: 100%;
  margin-bottom: 14px;
}
.driver-avatar {
  width: 46px;
  height: 46px;
  border-radius: 12px;
  color: #fff;
  font-size: 15px;
  font-weight: 600;
}
.dc-identity {
  flex: 1;
  min-width: 0;
}
.driver-name {
  display: block;
  font-size: 14.5px;
  letter-spacing: -0.01em;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.dc-cedula {
  font-size: 11px;
  color: var(--text-3);
  margin-top: 2px;
}

/* Zona 2 — contacto + licencia (divisor superior, igual que la referencia) */
.dc-details {
  display: flex;
  flex-direction: column;
  gap: 7px;
  padding-top: 14px;
  border-top: 1px solid var(--border);
}
.dc-line {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-2);
}
.dc-ico {
  color: var(--text-3);
  flex-shrink: 0;
}
.dc-line.warn {
  color: var(--amber-text);
}
.dc-line.warn .dc-ico {
  color: var(--amber);
}
.dc-line.danger {
  color: var(--red);
}
.dc-line.danger .dc-ico {
  color: var(--red);
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
  font-size: 13.5px;
  font-weight: 500;
  min-height: 160px;
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
