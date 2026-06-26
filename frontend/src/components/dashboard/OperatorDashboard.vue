<script setup>
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useRequestsStore } from '@/stores/requests.store'
import { useAuth } from '@/composables/useAuth'
import { formatDate } from '@/utils/formatters'
import AppBadge from '@/components/ui/AppBadge.vue'
import { Plus, Clock, Truck, CheckCircle2 } from '@lucide/vue'

const router = useRouter()
const { user, can } = useAuth()
const requestsStore = useRequestsStore()

// El backend ya filtra /requests/all por el usuario actual cuando es Operador.
onMounted(() => requestsStore.fetchAll())

const firstName = computed(() => (user.value?.fullName || 'Operador').split(' ')[0])

const myRequests = computed(() => requestsStore.requests)

const kpis = computed(() => {
  const reqs = myRequests.value
  const count = (statuses) => reqs.filter((r) => statuses.includes(r.status)).length
  return [
    {
      label: 'Pendientes',
      value: count(['Pending']),
      sub: 'En espera de aprobación',
      icon: Clock,
      variant: 'amber',
    },
    {
      label: 'En proceso',
      value: count(['Approved', 'Assigned', 'InProgress']),
      sub: 'Aprobadas o en ruta',
      icon: Truck,
      variant: 'blue',
    },
    {
      label: 'Completadas',
      value: count(['Completed']),
      sub: 'Viajes finalizados',
      icon: CheckCircle2,
      variant: 'green',
    },
  ]
})

const recentRequests = computed(() => myRequests.value.slice(0, 6))

function goNew() {
  router.push('/requests/new')
}
</script>

<template>
  <div>
    <!-- Hero -->
    <section class="op-hero">
      <div class="op-hero__text">
        <div class="op-hero__greeting">Hola, {{ firstName }}</div>
        <h2 class="op-hero__title">¿Necesitas un vehículo?</h2>
        <p class="op-hero__sub">
          Crea una solicitud de transporte para tu área. Un supervisor la revisará y te notificará
          la asignación.
        </p>
      </div>
      <button v-if="can('create', 'requests')" class="btn primary op-hero__btn" @click="goNew">
        <Plus :size="15" /> Nueva solicitud
      </button>
    </section>

    <!-- KPIs -->
    <div class="kpi-grid kpi-grid--3">
      <div v-for="kpi in kpis" :key="kpi.label" class="kpi">
        <div class="kpi-label">{{ kpi.label }}</div>
        <div class="kpi-val">{{ kpi.value }}</div>
        <div class="kpi-sub">{{ kpi.sub }}</div>
        <div class="kpi-icon" :class="kpi.variant"><component :is="kpi.icon" :size="16" /></div>
      </div>
    </div>

    <!-- Mis solicitudes recientes -->
    <div class="card">
      <div class="card-header">
        <span class="card-title">Mis solicitudes recientes</span>
        <router-link to="/requests" class="dash-link">Ver todas →</router-link>
      </div>

      <div v-if="requestsStore.isLoadingList" class="loading-placeholder">Cargando solicitudes…</div>
      <div v-else-if="requestsStore.listError" class="alert red">{{ requestsStore.listError }}</div>
      <div v-else-if="recentRequests.length === 0" class="empty-card">
        Aún no has creado solicitudes de transporte.
      </div>
      <div v-else class="table-wrap op-table">
        <table>
          <thead>
            <tr>
              <th>N°</th>
              <th>Destino</th>
              <th>Fecha</th>
              <th>Pers.</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="req in recentRequests"
              :key="req.id"
              class="op-table__row"
              @click="router.push(`/requests/${req.id}`)"
            >
              <td class="plate-cell">{{ req.requestNumber }}</td>
              <td>{{ req.destination }}</td>
              <td class="muted">{{ formatDate(req.departureDateTime) }}</td>
              <td class="muted">{{ req.passengerCount }}</td>
              <td><AppBadge :status="req.status" /></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.op-hero {
  display: flex;
  align-items: center;
  gap: 16px;
  background: linear-gradient(100deg, var(--navy), #1d2a27);
  border-radius: 12px;
  padding: 22px 24px;
  margin-bottom: 20px;
  color: #fff;
}
.op-hero__text {
  flex: 1;
  min-width: 0;
}
.op-hero__greeting {
  font-size: 11px;
  letter-spacing: 0.14em;
  text-transform: uppercase;
  color: var(--mint-accent);
  font-weight: 600;
  margin-bottom: 8px;
}
.op-hero__title {
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 4px;
}
.op-hero__sub {
  font-size: 13px;
  color: rgba(255, 255, 255, 0.6);
  max-width: 400px;
  line-height: 1.5;
}
.op-hero__btn {
  flex-shrink: 0;
}

.kpi-grid--3 {
  grid-template-columns: repeat(3, 1fr);
}
@media (max-width: 760px) {
  .kpi-grid--3 {
    grid-template-columns: 1fr;
  }
}

.dash-link {
  font-size: 13px;
  color: var(--blue);
}

.op-table {
  margin-bottom: 0;
}
.op-table__row {
  cursor: pointer;
}
</style>
