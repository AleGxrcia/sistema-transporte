<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useScheduleStore } from '@/stores/schedule.store'
import { VehiclesService } from '@/services/vehicles.service'
import { driverApi } from '@/services/drivers.service'
import { buildMonthGrid, buildWeekDays, rangeForView } from '@/utils/dateRange'
import { ASSIGNMENT_STATUSES } from '@/utils/enumLabels'
import AppBadge from '@/components/ui/AppBadge.vue'

const router = useRouter()
const store = useScheduleStore()

const views = [
  { key: 'month', label: 'Mes' },
  { key: 'week', label: 'Semana' },
  { key: 'day', label: 'Día' },
]
const activeView = ref('month')

const currentDate = ref(dayjs().format('YYYY-MM-DD'))
const driverFilter = ref('')
const vehicleFilter = ref('')

const vehicles = ref([])
const drivers = ref([])

onMounted(async () => {
  try {
    const [vRes, dRes] = await Promise.all([VehiclesService.list(), driverApi.list()])
    vehicles.value = vRes.data
    drivers.value = dRes.data
  } catch {
    vehicles.value = []
    drivers.value = []
  }
  loadRange()
})

watch([activeView, currentDate], loadRange)

function loadRange() {
  const { from, to } = rangeForView(activeView.value, currentDate.value)
  store.fetchRange(from.toISOString(), to.toISOString())
}

function goPrev() {
  const unit = activeView.value
  currentDate.value = dayjs(currentDate.value).subtract(1, unit).format('YYYY-MM-DD')
}
function goNext() {
  const unit = activeView.value
  currentDate.value = dayjs(currentDate.value).add(1, unit).format('YYYY-MM-DD')
}
function goToday() {
  currentDate.value = dayjs().format('YYYY-MM-DD')
}
function goToDay(d) {
  currentDate.value = d.format('YYYY-MM-DD')
  activeView.value = 'day'
}

const headerLabel = computed(() => {
  const d = dayjs(currentDate.value)
  if (activeView.value === 'month') return capitalize(d.format('MMMM YYYY'))
  if (activeView.value === 'week') {
    const days = buildWeekDays(currentDate.value)
    return `${days[0].format('D MMM')} — ${days[6].format('D MMM YYYY')}`
  }
  return capitalize(d.format('dddd, D [de] MMMM YYYY'))
})

function capitalize(s) {
  return s.charAt(0).toUpperCase() + s.slice(1)
}

const filteredAssignments = computed(() => {
  let list = store.assignments
  if (driverFilter.value) list = list.filter((a) => a.driverId === driverFilter.value)
  if (vehicleFilter.value) list = list.filter((a) => a.vehicleId === vehicleFilter.value)
  return [...list].sort((a, b) => new Date(a.departureTime) - new Date(b.departureTime))
})

function assignmentsForDay(d) {
  return filteredAssignments.value.filter((a) => dayjs(a.departureTime).isSame(d, 'day'))
}

const monthCells = computed(() => {
  const refDate = dayjs(currentDate.value)
  return buildMonthGrid(currentDate.value).map((d) => ({
    date: d,
    inMonth: d.month() === refDate.month(),
    isToday: d.isSame(dayjs(), 'day'),
    items: assignmentsForDay(d),
  }))
})

const weekColumns = computed(() =>
  buildWeekDays(currentDate.value).map((d) => ({
    date: d,
    isToday: d.isSame(dayjs(), 'day'),
    items: assignmentsForDay(d),
  }))
)

const dayItems = computed(() => assignmentsForDay(dayjs(currentDate.value)))

function viewRequest(a) {
  router.push(`/requests/${a.requestId}`)
}
</script>

<template>
  <div>
    <div class="page-header">
      <div class="page-tabs">
        <span
          v-for="v in views"
          :key="v.key"
          class="pt"
          :class="{ active: activeView === v.key }"
          @click="activeView = v.key"
        >
          {{ v.label }}
        </span>
      </div>
      <div style="display:flex;gap:8px;flex-wrap:wrap">
        <select v-model="driverFilter">
          <option value="">Todos los conductores</option>
          <option v-for="d in drivers" :key="d.id" :value="d.id">{{ d.firstName }} {{ d.lastName }}</option>
        </select>
        <select v-model="vehicleFilter">
          <option value="">Todos los vehículos</option>
          <option v-for="v in vehicles" :key="v.id" :value="v.id">{{ v.licensePlate }} — {{ v.brand }} {{ v.model }}</option>
        </select>
      </div>
    </div>

    <div class="search-row" style="justify-content:space-between">
      <h2 style="font-size:16px;font-weight:700;color:var(--text);text-transform:capitalize">{{ headerLabel }}</h2>
      <div style="display:flex;gap:6px;align-items:center">
        <input v-model="currentDate" type="date" style="width:auto" />
        <button class="btn" @click="goPrev">←</button>
        <button class="btn" @click="goToday">Hoy</button>
        <button class="btn" @click="goNext">→</button>
      </div>
    </div>

    <div v-if="store.isLoading" class="loading-placeholder">Cargando agenda…</div>
    <div v-else-if="store.error" class="alert red">{{ store.error }}</div>

    <template v-else>
      <!-- Vista mensual -->
      <div v-if="activeView === 'month'" class="month-grid">
        <div class="month-day-header" v-for="d in ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom']" :key="d">{{ d }}</div>
        <div
          v-for="cell in monthCells"
          :key="cell.date.format('YYYY-MM-DD')"
          class="month-cell"
          :class="{ 'other-month': !cell.inMonth, today: cell.isToday }"
          @click="goToDay(cell.date)"
        >
          <span class="month-day-number" :class="{ today: cell.isToday }">{{ cell.date.date() }}</span>
          <div class="month-events">
            <div v-for="a in cell.items.slice(0, 3)" :key="a.assignmentId" class="month-event" @click.stop="viewRequest(a)">
              {{ dayjs(a.departureTime).format('HH:mm') }} {{ a.vehiclePlate }}
            </div>
            <div v-if="cell.items.length > 3" class="month-event-more">+{{ cell.items.length - 3 }} más</div>
          </div>
        </div>
      </div>

      <!-- Vista semanal -->
      <div v-else-if="activeView === 'week'" class="week-grid">
        <div v-for="col in weekColumns" :key="col.date.format('YYYY-MM-DD')" class="week-col" :class="{ today: col.isToday }">
          <div class="week-col-header">{{ capitalize(col.date.format('ddd D')) }}</div>
          <div v-if="col.items.length === 0" class="week-empty">Sin viajes</div>
          <div v-for="a in col.items" :key="a.assignmentId" class="card trip-card" @click="viewRequest(a)">
            <div style="display:flex;justify-content:space-between;align-items:center">
              <span style="font-weight:700;font-size:12px">{{ dayjs(a.departureTime).format('HH:mm') }}</span>
              <AppBadge :status="a.status" />
            </div>
            <div style="font-size:12px;font-weight:600;color:var(--text);margin-top:4px">{{ a.destination }}</div>
            <div style="font-size:11px;color:var(--text-3);margin-top:2px">{{ a.vehiclePlate }} · {{ a.driverFullName }}</div>
          </div>
        </div>
      </div>

      <!-- Vista diaria -->
      <div v-else>
        <div v-if="dayItems.length === 0" class="empty-card">No hay viajes programados para este día.</div>
        <div v-else style="display:flex;flex-direction:column;gap:8px">
          <div v-for="a in dayItems" :key="a.assignmentId" class="card trip-card" @click="viewRequest(a)">
            <div style="display:flex;justify-content:space-between;align-items:center">
              <span style="font-weight:700;font-size:14px">
                {{ dayjs(a.departureTime).format('HH:mm') }} — {{ dayjs(a.returnTime).format('HH:mm') }}
              </span>
              <AppBadge :status="a.status" />
            </div>
            <div style="font-size:13px;font-weight:600;color:var(--text);margin-top:6px">{{ a.requestNumber }} · {{ a.destination }}</div>
            <div style="font-size:12px;color:var(--text-3);margin-top:2px">
              {{ a.vehiclePlate }} — {{ a.vehicleDescription }} · {{ a.driverFullName }}
            </div>
            <div v-if="a.cancellationReason" class="text-red" style="font-size:11.5px;margin-top:4px">
              Cancelado: {{ a.cancellationReason }}
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Leyenda -->
    <div style="display:flex;gap:16px;align-items:center;font-size:12px;color:var(--text-3);margin-top:14px;flex-wrap:wrap">
      <span v-for="s in ASSIGNMENT_STATUSES" :key="s.name" style="display:flex;align-items:center;gap:6px">
        <AppBadge :status="s.name" />
      </span>
    </div>
  </div>
</template>

<style scoped>
.loading-placeholder {
  padding: 48px;
  text-align: center;
  color: var(--text-3);
}
.empty-card {
  font-size: 13px;
  color: var(--text-3);
  text-align: center;
  padding: 24px;
}
.text-red { color: var(--red); }

/* Mes */
.month-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  border: 1px solid var(--border);
  border-radius: 10px;
  overflow: hidden;
  background: var(--white);
  margin-bottom: 14px;
}
.month-day-header {
  padding: 8px;
  text-align: center;
  font-size: 11px;
  font-weight: 600;
  color: var(--text-3);
  background: var(--white);
  border-bottom: 1px solid var(--border);
}
.month-cell {
  min-height: 88px;
  padding: 6px;
  border-right: 1px solid var(--border);
  border-bottom: 1px solid var(--border);
  display: flex;
  flex-direction: column;
  gap: 3px;
  background: var(--white);
  cursor: pointer;
  transition: background 0.15s;
}
.month-cell:hover { background: var(--surface-hover); }
.month-cell.other-month { background: var(--bg); }
.month-cell.other-month .month-day-number { color: var(--text-3); }
.month-cell.today { background: var(--blue-light); }
.month-day-number {
  font-size: 12px;
  font-weight: 500;
  color: var(--text-2);
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
}
.month-day-number.today { background: var(--blue); color: #fff; font-weight: 700; }
.month-events { display: flex; flex-direction: column; gap: 2px; }
.month-event {
  font-size: 10.5px;
  padding: 2px 5px;
  border-radius: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  background: var(--blue-light);
  color: var(--blue);
  border-left: 2px solid var(--blue);
}
.month-event-more { font-size: 10.5px; color: var(--text-3); padding: 0 5px; }

/* Semana */
.week-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}
.week-col {
  display: flex;
  flex-direction: column;
  gap: 6px;
  min-height: 120px;
}
.week-col.today .week-col-header { color: var(--blue); }
.week-col-header {
  font-size: 11.5px;
  font-weight: 600;
  color: var(--text-2);
  text-align: center;
  padding-bottom: 4px;
  border-bottom: 1px solid var(--border);
}
.week-empty {
  font-size: 11px;
  color: var(--text-3);
  text-align: center;
  padding: 10px 0;
}

/* Tarjetas de viaje (semana/día) */
.trip-card {
  padding: 10px 12px;
  cursor: pointer;
  transition: box-shadow 0.15s;
}
.trip-card:hover { box-shadow: 0 2px 8px rgba(0, 0, 0, 0.07); }
</style>
