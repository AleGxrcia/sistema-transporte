<script setup>
import { onMounted, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useDriverStore } from '@/stores/drivers.store'
import { useAuthStore } from '@/stores/auth.store'
import { useModal } from '@/composables/useModal'
import { useToast } from '@/composables/useToast'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'
import BaseModal from '@/components/ui/AppBaseModal.vue'
import ConfirmModal from '@/components/modals/ModalConfirm.vue'
import AppBadge from '@/components/ui/AppBadge.vue'
import DriverForm from '@/components/forms/drivers/DriverForm.vue'
import RenewLicenseForm from '@/components/forms/drivers/RenewLicenseForm.vue'
import { ArrowLeft, Pencil, RefreshCw, Ban, CheckCircle, Trash2, AlertTriangle } from '@lucide/vue'
import { formatDate, getInitials, daysUntil } from '@/utils/formatters'
import { getLicenseCategoryLabel } from '@/utils/enumLabels'

const props = defineProps({ id: { type: String, required: true } })

const route        = useRoute()
const router       = useRouter()
const driverStore  = useDriverStore()
const auth         = useAuthStore()
const toast        = useToast()

const driver    = computed(() => driverStore.currentDriver)
const isLoading = computed(() => driverStore.isLoadingDetail)

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

// ── Handlers ───────────────────────────────────────────────────────────────
onMounted(async () => {
  await driverStore.fetchById(props.id)
  if (route.query.edit === 'true' && driver.value) {
    editModal.open(driver.value)
  }
})

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

      <div v-if="driver && auth.isAdmin" style="display:flex;gap:8px;flex-wrap:wrap">
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
        <button class="btn danger" @click="deleteModal.open()">
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
        <button class="btn-sm" @click="renewModal.open(driver)">Renovar ahora</button>
      </div>

      <div class="detail-grid">
        <!-- Card: Perfil -->
        <div class="card profile-card">
          <div class="profile-avatar">{{ initials }}</div>
          <h2 class="profile-name">{{ driver.firstName }} {{ driver.lastName }}</h2>
          <AppBadge :status="driver.status" />
          <dl class="info-list">
            <div class="info-row"><dt>Cédula</dt><dd>{{ driver.nationalId }}</dd></div>
            <div class="info-row"><dt>Teléfono</dt><dd>{{ driver.phone }}</dd></div>
            <div class="info-row"><dt>Dirección</dt><dd>{{ driver.address || '—' }}</dd></div>
          </dl>
        </div>

        <!-- Card: Licencia -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Licencia de conducir</span>
          </div>
          <dl class="info-list">
            <div class="info-row"><dt>Número</dt><dd>{{ driver.licenseNumber }}</dd></div>
            <div class="info-row"><dt>Categoría</dt><dd>{{ getLicenseCategoryLabel(driver.licenseType) }}</dd></div>
            <div class="info-row">
              <dt>Vencimiento</dt>
              <dd :class="{
                'text-red':   licenseAlert?.level === 'expired',
                'text-amber': licenseAlert?.level === 'warning',
              }">
                {{ formatDate(driver.licenseExpirationDate) }}
              </dd>
            </div>
          </dl>
        </div>

        <!-- Card: Viajes recientes -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Viajes recientes</span>
          </div>
          <div v-if="!driver.recentTrips?.length" class="empty-card">
            Sin viajes recientes
          </div>
          <div v-else class="trip-list">
            <div v-for="trip in driver.recentTrips" :key="trip.id" class="trip-item">
              <span class="trip-number">{{ trip.requestNumber }}</span>
              <span class="muted">{{ formatDate(trip.date) }}</span>
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
.detail-grid {
  display: grid;
  grid-template-columns: 260px 1fr 1fr;
  gap: 16px;
  align-items: start;
}
@media (max-width: 900px) { .detail-grid { grid-template-columns: 1fr; } }

.profile-card { display: flex; flex-direction: column; align-items: center; gap: 10px; text-align: center; }
.profile-avatar {
  width: 64px; height: 64px; border-radius: 50%;
  background: var(--blue-light); color: var(--blue-hover);
  font-size: 22px; font-weight: 700;
  display: flex; align-items: center; justify-content: center;
}
.profile-name { font-size: 16px; font-weight: 700; margin: 0; color: var(--text); }

.info-list { display: flex; flex-direction: column; gap: 8px; width: 100%; margin-top: 8px; }
.info-row  { display: flex; justify-content: space-between; font-size: 13px; }
.info-row dt { color: var(--text-3); }
.info-row dd { font-weight: 500; }

.text-red   { color: var(--red); font-weight: 600; }
.text-amber { color: var(--amber-text); font-weight: 600; }

.license-banner {
  display: flex; align-items: center; gap: 8px;
  padding: 10px 16px; border-radius: 8px;
  font-size: 13px; font-weight: 600;
  margin-bottom: 16px;
}
.banner--warning { background: var(--amber-bg); color: var(--amber-text); border: 1px solid var(--amber-border); }
.banner--expired { background: var(--red-bg); color: var(--red); border: 1px solid var(--red-border); }

.trip-list { display: flex; flex-direction: column; gap: 6px; }
.trip-item { display: flex; justify-content: space-between; font-size: 13px; padding: 6px 0; border-bottom: 1px solid var(--border); }
.trip-number { font-weight: 600; }
.empty-card { font-size: 13px; color: var(--text-3); text-align: center; padding: 24px; }
.loading-placeholder { padding: 48px; text-align: center; color: var(--text-3); }

.btn-sm {
  margin-left: auto; font-size: 11px; padding: 4px 10px;
  border: 1px solid currentColor; border-radius: 6px;
  background: transparent; cursor: pointer;
}
</style>