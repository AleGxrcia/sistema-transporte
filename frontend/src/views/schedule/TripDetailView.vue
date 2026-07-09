<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { User, Truck } from '@lucide/vue'
import { useRequestsStore } from '@/stores/requests.store'
import { VehiclesService } from '@/services/vehicles.service'
import { driverApi } from '@/services/drivers.service'
import { formatDate, getInitials } from '@/utils/formatters'
import { getLicenseCategoryLabel } from '@/utils/license'
import AppBadge from '@/components/ui/AppBadge.vue'
import AppBreadcrumb from '@/components/ui/AppBreadcrumb.vue'

const props = defineProps({ id: { type: String, required: true } })

const store = useRequestsStore()

// La DGA opera siempre desde su sede; el backend no modela un origen por viaje.
const INSTITUTIONAL_ORIGIN = 'DGA – Sede central'

const request = computed(() => store.currentRequest)
const isLoading = computed(() => store.isLoadingDetail)

const vehicle = ref(null)
const driver = ref(null)

onMounted(load)
watch(() => props.id, load)

async function load() {
  vehicle.value = null
  driver.value = null
  await store.fetchById(props.id)
  const r = request.value
  if (!r) return
  const tasks = []
  if (r.assignedVehicleId) {
    tasks.push(VehiclesService.getById(r.assignedVehicleId).then((res) => (vehicle.value = res.data)))
  }
  if (r.assignedDriverId) {
    tasks.push(driverApi.getById(r.assignedDriverId).then((res) => (driver.value = res.data)))
  }
  try {
    await Promise.all(tasks)
  } catch {
    // Si falla la resolución de recursos, se muestra el detalle sin enriquecer.
  }
}

const breadcrumbItems = computed(() => [
  { label: 'Agenda', to: '/schedules' },
  { label: request.value?.destination ?? 'Viaje' },
])
</script>

<template>
  <div class="trip-detail">
    <div v-if="isLoading" class="loading-placeholder">Cargando viaje…</div>
    <div v-else-if="store.detailError" class="alert red">{{ store.detailError }}</div>

    <template v-else-if="request">
      <AppBreadcrumb :items="breadcrumbItems" />

      <!-- Cabecera -->
      <div class="card trip-header">
        <div class="trip-header-top">
          <div>
            <div class="trip-title-row">
              <h2 class="trip-title">{{ request.destination }}</h2>
              <AppBadge :status="request.status" />
            </div>
            <div class="trip-subtitle">
              {{ request.requestingArea }} · {{ INSTITUTIONAL_ORIGIN }} → {{ request.destination }}
            </div>
          </div>
        </div>

        <div class="trip-divider"></div>

        <div class="trip-meta-grid">
          <div class="trip-meta">
            <div class="trip-meta-label">Fecha</div>
            <div class="trip-meta-value mono">{{ formatDate(request.departureDateTime) }}</div>
          </div>
          <div class="trip-meta">
            <div class="trip-meta-label">Horario</div>
            <div class="trip-meta-value mono">
              {{ formatDate(request.departureDateTime, 'HH:mm') }} — {{ formatDate(request.returnDateTime, 'HH:mm') }}
            </div>
          </div>
          <div class="trip-meta">
            <div class="trip-meta-label">Pasajeros</div>
            <div class="trip-meta-value mono">{{ request.passengerCount }}</div>
          </div>
          <div class="trip-meta">
            <div class="trip-meta-label">Área</div>
            <div class="trip-meta-value">{{ request.requestingArea }}</div>
          </div>
        </div>
      </div>

      <!-- Recursos asignados -->
      <div class="resource-grid">
        <div class="card resource-block">
          <div class="resource-block-label">Conductor asignado</div>
          <div v-if="driver" class="resource-block-body">
            <div class="resource-block-avatar">{{ getInitials(driver.firstName, driver.lastName) }}</div>
            <div>
              <div class="resource-block-name">{{ driver.firstName }} {{ driver.lastName }}</div>
              <div class="resource-block-sub">Licencia {{ getLicenseCategoryLabel(driver.licenseType) }} · vigente</div>
            </div>
          </div>
          <div v-else class="resource-block-empty">
            <div class="resource-block-avatar muted"><User :size="20" /></div>
            <div class="resource-block-sub">Sin conductor asignado</div>
          </div>
        </div>

        <div class="card resource-block">
          <div class="resource-block-label">Vehículo asignado</div>
          <div v-if="vehicle" class="resource-block-body">
            <div class="resource-block-icon"><Truck :size="20" /></div>
            <div>
              <div class="resource-block-name mono">{{ vehicle.licensePlate }} — {{ vehicle.brand }} {{ vehicle.model }}</div>
              <div class="resource-block-sub">{{ vehicle.type }} · {{ vehicle.capacity }} pasajeros</div>
            </div>
          </div>
          <div v-else class="resource-block-empty">
            <div class="resource-block-icon muted"><Truck :size="20" /></div>
            <div class="resource-block-sub">Sin vehículo asignado</div>
          </div>
        </div>
      </div>

      <!-- Ruta y motivo -->
      <div class="card">
        <div class="resource-block-label">Ruta y motivo</div>
        <div class="route">
          <div class="route-rail">
            <span class="route-dot origin"></span>
            <span class="route-line"></span>
            <span class="route-dot dest"></span>
          </div>
          <div class="route-points">
            <div>
              <div class="route-label">Origen</div>
              <div class="route-value">{{ INSTITUTIONAL_ORIGIN }}</div>
            </div>
            <div>
              <div class="route-label">Destino</div>
              <div class="route-value">{{ request.destination }}</div>
            </div>
          </div>
        </div>
        <div class="route-purpose">{{ request.tripPurpose }}</div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.trip-detail {
  max-width: 820px;
}
.mono {
  font-variant-numeric: tabular-nums;
  letter-spacing: 0.01em;
}

/* Cabecera */
.trip-header {
  margin-bottom: 16px;
}
.trip-header-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
}
.trip-title-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}
.trip-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--text);
}
.trip-subtitle {
  font-size: 13px;
  color: var(--text-2);
}
.trip-divider {
  height: 1px;
  background: var(--border);
  margin: 18px 0;
}
.trip-meta-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
}
.trip-meta-label {
  font-size: 10.5px;
  color: var(--text-3);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 5px;
}
.trip-meta-value {
  font-size: 13.5px;
  font-weight: 600;
  color: var(--text);
}

/* Recursos */
.resource-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}
.resource-block-label {
  font-size: 11px;
  color: var(--text-3);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 13px;
}
.resource-block-body,
.resource-block-empty {
  display: flex;
  align-items: center;
  gap: 12px;
}
.resource-block-avatar {
  width: 42px;
  height: 42px;
  min-width: 42px;
  border-radius: 50%;
  background: var(--blue);
  color: var(--white);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
}
.resource-block-icon {
  width: 42px;
  height: 42px;
  min-width: 42px;
  border-radius: 10px;
  background: var(--blue-light);
  border: 1px solid var(--blue-mid);
  color: var(--blue);
  display: flex;
  align-items: center;
  justify-content: center;
}
.resource-block-avatar.muted,
.resource-block-icon.muted {
  background: var(--bg);
  border-color: var(--border);
  color: var(--text-3);
}
.resource-block-name {
  font-size: 13.5px;
  font-weight: 600;
  color: var(--text);
}
.resource-block-sub {
  font-size: 11.5px;
  color: var(--text-3);
  margin-top: 2px;
}

/* Ruta */
.route {
  display: flex;
  gap: 13px;
  margin-bottom: 16px;
}
.route-rail {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 4px;
}
.route-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}
.route-dot.origin {
  background: var(--mint-dark);
}
.route-dot.dest {
  border: 2px solid var(--red);
}
.route-line {
  width: 2px;
  flex: 1;
  min-height: 26px;
  background: var(--border-strong);
  margin: 3px 0;
}
.route-points {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.route-label {
  font-size: 10.5px;
  color: var(--text-3);
}
.route-value {
  font-size: 13px;
  font-weight: 600;
  color: var(--text);
  margin-top: 2px;
}
.route-purpose {
  background: var(--bg);
  border-radius: 10px;
  padding: 13px 15px;
  font-size: 12.5px;
  color: var(--text-2);
  line-height: 1.5;
}

@media (max-width: 700px) {
  .trip-meta-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .resource-grid {
    grid-template-columns: 1fr;
  }
}
</style>
