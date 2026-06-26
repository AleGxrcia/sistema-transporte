<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { User, Truck, Calendar, ChevronLeft, ChevronRight } from '@lucide/vue'
import { useScheduleStore } from '@/stores/schedule.store'
import { VehiclesService } from '@/services/vehicles.service'
import { driverApi } from '@/services/drivers.service'
import { buildMonthGrid, buildWeekDays, rangeForView } from '@/utils/dateRange'
import AppBadge from '@/components/ui/AppBadge.vue'

const router = useRouter()
const store = useScheduleStore()

const views = [
  { key: 'month', label: 'Mes' },
  { key: 'week', label: 'Semana' },
  { key: 'day', label: 'Día' },
]
const activeView = ref('month')

// Fecha ancla: controla el mes/semana visible.
const anchorDate = ref(dayjs().format('YYYY-MM-DD'))
// Fecha seleccionada: alimenta el panel lateral (mes) y la vista de día.
const selectedDate = ref(dayjs().format('YYYY-MM-DD'))

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

watch([activeView, anchorDate, selectedDate], loadRange)

function loadRange() {
  const refDate = activeView.value === 'day' ? selectedDate.value : anchorDate.value
  const { from, to } = rangeForView(activeView.value, refDate)
  store.fetchRange(from.toISOString(), to.toISOString())
}

function goPrev() {
  if (activeView.value === 'day') {
    selectedDate.value = dayjs(selectedDate.value).subtract(1, 'day').format('YYYY-MM-DD')
    anchorDate.value = selectedDate.value
    return
  }
  const unit = activeView.value
  anchorDate.value = dayjs(anchorDate.value).subtract(1, unit).format('YYYY-MM-DD')
  syncSelectedToAnchor()
}
function goNext() {
  if (activeView.value === 'day') {
    selectedDate.value = dayjs(selectedDate.value).add(1, 'day').format('YYYY-MM-DD')
    anchorDate.value = selectedDate.value
    return
  }
  const unit = activeView.value
  anchorDate.value = dayjs(anchorDate.value).add(1, unit).format('YYYY-MM-DD')
  syncSelectedToAnchor()
}
function goToday() {
  anchorDate.value = dayjs().format('YYYY-MM-DD')
  selectedDate.value = anchorDate.value
}

// Tras navegar de mes/semana, mueve la selección al rango visible si quedó fuera.
function syncSelectedToAnchor() {
  const unit = activeView.value
  if (!dayjs(selectedDate.value).isSame(dayjs(anchorDate.value), unit)) {
    selectedDate.value = dayjs(anchorDate.value).startOf(unit).format('YYYY-MM-DD')
  }
}

function selectDay(d) {
  selectedDate.value = d.format('YYYY-MM-DD')
}
function pickDateInput(e) {
  const v = e.target.value
  if (!v) return
  selectedDate.value = v
  anchorDate.value = v
}

const headerLabel = computed(() => {
  const d = dayjs(anchorDate.value)
  if (activeView.value === 'month') return capitalize(d.format('MMMM YYYY'))
  if (activeView.value === 'week') {
    const days = buildWeekDays(anchorDate.value)
    return `${days[0].format('D MMM')} — ${days[6].format('D MMM YYYY')}`
  }
  return capitalize(dayjs(selectedDate.value).format('MMMM YYYY'))
})

const selectedDayLabel = computed(() =>
  capitalize(dayjs(selectedDate.value).format('dddd D [de] MMMM, YYYY'))
)

function capitalize(s) {
  return s.charAt(0).toUpperCase() + s.slice(1)
}

const hasActiveFilters = computed(() => !!(driverFilter.value || vehicleFilter.value))

const filteredAssignments = computed(() => {
  let list = store.assignments
  if (driverFilter.value) list = list.filter((a) => a.driverId === driverFilter.value)
  if (vehicleFilter.value) list = list.filter((a) => a.vehicleId === vehicleFilter.value)
  return [...list].sort((a, b) => new Date(a.departureTime) - new Date(b.departureTime))
})

const noFilteredResults = computed(
  () =>
    hasActiveFilters.value &&
    store.assignments.length > 0 &&
    filteredAssignments.value.length === 0
)

// Colores de evento según estado real de la asignación.
const legend = [
  { label: 'Programada', cls: 'st-blue' },
  { label: 'Completada', cls: 'st-green' },
  { label: 'Cancelada', cls: 'st-gray' },
]
function eventStatusClass(status) {
  if (status === 'Completed') return 'st-green'
  if (status === 'Cancelled') return 'st-gray'
  return 'st-blue'
}

function assignmentsForDay(d) {
  return filteredAssignments.value.filter((a) => dayjs(a.departureTime).isSame(d, 'day'))
}

const monthCells = computed(() => {
  const refDate = dayjs(anchorDate.value)
  return buildMonthGrid(anchorDate.value).map((d) => ({
    date: d,
    inMonth: d.month() === refDate.month(),
    isToday: d.isSame(dayjs(), 'day'),
    isSelected: d.isSame(dayjs(selectedDate.value), 'day'),
    items: assignmentsForDay(d),
  }))
})

const weekColumns = computed(() =>
  buildWeekDays(anchorDate.value).map((d) => ({
    date: d,
    isToday: d.isSame(dayjs(), 'day'),
    isSelected: d.isSame(dayjs(selectedDate.value), 'day'),
    items: assignmentsForDay(d),
  }))
)

const selectedDayItems = computed(() => assignmentsForDay(dayjs(selectedDate.value)))

function viewRequest(a) {
  router.push(`/requests/${a.requestId}`)
}
</script>

<template>
  <div>
    <div class="page-header">
      <h1>Agenda de viajes</h1>
    </div>

    <!-- Toolbar: navegación + segmented control -->
    <div class="ag-toolbar">
      <div class="ag-nav">
        <span class="ag-range-label">{{ headerLabel }}</span>
        <div class="ag-nav-group">
          <button class="ag-nav-btn" title="Anterior" @click="goPrev">
            <ChevronLeft :size="16" />
          </button>
          <button class="ag-nav-btn ag-nav-btn--today" @click="goToday">Hoy</button>
          <button class="ag-nav-btn" title="Siguiente" @click="goNext">
            <ChevronRight :size="16" />
          </button>
        </div>
      </div>

      <div class="page-tabs ag-segments">
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
    </div>

    <!-- Filtros + leyenda -->
    <div class="ag-filters">
      <label class="ag-filter">
        <User :size="14" class="ag-filter-icon" />
        <select v-model="driverFilter">
          <option value="">Todos los conductores</option>
          <option v-for="d in drivers" :key="d.id" :value="d.id">
            {{ d.firstName }} {{ d.lastName }}
          </option>
        </select>
      </label>

      <label class="ag-filter">
        <Truck :size="14" class="ag-filter-icon" />
        <select v-model="vehicleFilter">
          <option value="">Todos los vehículos</option>
          <option v-for="v in vehicles" :key="v.id" :value="v.id">
            {{ v.licensePlate }} — {{ v.brand }} {{ v.model }}
          </option>
        </select>
      </label>

      <label class="ag-filter">
        <Calendar :size="14" class="ag-filter-icon" />
        <input type="date" :value="selectedDate" @change="pickDateInput" />
      </label>

      <div class="ag-legend">
        <span v-for="l in legend" :key="l.label" class="ag-legend-item">
          <span class="ag-legend-swatch" :class="l.cls"></span>{{ l.label }}
        </span>
      </div>
    </div>

    <div v-if="noFilteredResults" class="alert amber">
      No hay viajes que coincidan con los filtros aplicados en este rango.
    </div>

    <div v-if="store.isLoading" class="loading-placeholder">Cargando agenda…</div>
    <div v-else-if="store.error" class="alert red">{{ store.error }}</div>

    <template v-else>
      <!-- ===== MES ===== -->
      <div v-if="activeView === 'month'" class="ag-month-layout">
        <div class="ag-month-card">
          <div class="ag-month-daynames">
            <div v-for="d in ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom']" :key="d">{{ d }}</div>
          </div>
          <div class="ag-month-grid">
            <div
              v-for="cell in monthCells"
              :key="cell.date.format('YYYY-MM-DD')"
              class="ag-cell"
              :class="{
                'is-muted': !cell.inMonth,
                'is-today': cell.isToday,
                'is-selected': cell.isSelected,
              }"
              @click="cell.inMonth && selectDay(cell.date)"
            >
              <div class="ag-cell-day">{{ cell.date.date() }}</div>
              <div class="ag-cell-events">
                <div
                  v-for="a in cell.items.slice(0, 3)"
                  :key="a.assignmentId"
                  class="ag-event"
                  :class="eventStatusClass(a.status)"
                  @click.stop="viewRequest(a)"
                >
                  {{ dayjs(a.departureTime).format('HH:mm') }} · {{ a.destination }}
                </div>
                <div v-if="cell.items.length > 3" class="ag-event-more">
                  +{{ cell.items.length - 3 }} más
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Panel del día seleccionado -->
        <aside class="ag-day-panel">
          <div class="ag-day-panel-title">{{ selectedDayLabel }}</div>
          <div class="ag-day-panel-sub">{{ selectedDayItems.length }} viaje(s) en agenda</div>

          <template v-if="selectedDayItems.length">
            <div
              v-for="a in selectedDayItems"
              :key="a.assignmentId"
              class="ag-panel-trip"
              @click="viewRequest(a)"
            >
              <div class="ag-panel-trip-time">{{ dayjs(a.departureTime).format('HH:mm') }}</div>
              <div class="ag-panel-trip-body">
                <div class="ag-panel-trip-title">{{ a.destination }}</div>
                <div class="ag-panel-trip-meta">{{ a.driverFullName }} · {{ a.vehiclePlate }}</div>
                <div class="ag-panel-trip-tag"><AppBadge :status="a.status" /></div>
              </div>
              <ChevronRight :size="15" class="ag-panel-trip-chevron" />
            </div>
          </template>
          <div v-else class="ag-day-panel-empty">Sin viajes este día</div>
        </aside>
      </div>

      <!-- ===== SEMANA ===== -->
      <div v-else-if="activeView === 'week'" class="ag-week-card">
        <div class="ag-week-cols">
          <div
            v-for="col in weekColumns"
            :key="col.date.format('YYYY-MM-DD')"
            class="ag-week-col"
            :class="{ 'is-selected': col.isSelected }"
          >
            <div
              class="ag-week-head"
              :class="{ 'is-today': col.isToday, 'is-selected': col.isSelected }"
              @click="selectDay(col.date)"
            >
              <div class="ag-week-dn">{{ col.date.format('ddd') }}</div>
              <div class="ag-week-day">{{ col.date.date() }}</div>
            </div>
            <div
              v-for="a in col.items"
              :key="a.assignmentId"
              class="ag-event ag-event--week"
              :class="eventStatusClass(a.status)"
              @click="viewRequest(a)"
            >
              {{ dayjs(a.departureTime).format('HH:mm') }} · {{ a.destination }}
            </div>
          </div>
        </div>
      </div>

      <!-- ===== DÍA ===== -->
      <div v-else class="ag-day-card">
        <div class="ag-day-card-head">
          <div class="ag-day-card-title">{{ selectedDayLabel }}</div>
          <span class="ag-day-card-count">{{ selectedDayItems.length }} viaje(s)</span>
        </div>
        <div class="ag-day-divider"></div>

        <div v-if="!selectedDayItems.length" class="empty-card">
          No hay viajes programados para este día.
        </div>
        <div
          v-for="a in selectedDayItems"
          :key="a.assignmentId"
          class="ag-timeline-row"
          @click="viewRequest(a)"
        >
          <div class="ag-timeline-time">
            <div class="ag-timeline-start">{{ dayjs(a.departureTime).format('HH:mm') }}</div>
            <div class="ag-timeline-end">{{ dayjs(a.returnTime).format('HH:mm') }}</div>
          </div>
          <div class="ag-timeline-rail">
            <span class="ag-timeline-dot" :class="eventStatusClass(a.status)"></span>
          </div>
          <div class="ag-timeline-body">
            <div class="ag-timeline-title-row">
              <span class="ag-timeline-title">{{ a.destination }}</span>
              <AppBadge :status="a.status" />
            </div>
            <div class="ag-timeline-sub">{{ a.requestNumber }}</div>
            <div class="ag-timeline-meta">
              <span><User :size="12" /> {{ a.driverFullName }}</span>
              <span><Truck :size="12" /> {{ a.vehiclePlate }} — {{ a.vehicleDescription }}</span>
            </div>
            <div v-if="a.cancellationReason" class="ag-timeline-cancel">
              Cancelado: {{ a.cancellationReason }}
            </div>
          </div>
          <ChevronRight :size="16" class="ag-timeline-chevron" />
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
/* ===== Toolbar ===== */
.ag-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 14px;
}
.ag-nav {
  display: flex;
  align-items: center;
  gap: 12px;
}
.ag-range-label {
  font-size: 17px;
  font-weight: 700;
  color: var(--text);
  text-transform: capitalize;
}
.ag-nav-group {
  display: flex;
  gap: 4px;
}
.ag-nav-btn {
  height: 32px;
  min-width: 32px;
  padding: 0 8px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--border-strong);
  background: var(--white);
  border-radius: 8px;
  color: var(--text-2);
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  font-family: inherit;
  transition: background 0.15s;
}
.ag-nav-btn:hover {
  background: var(--bg);
}
.ag-nav-btn--today {
  padding: 0 12px;
  color: var(--blue);
  font-weight: 600;
}
.ag-segments {
  margin-bottom: 0;
}

/* ===== Filtros + leyenda ===== */
.ag-filters {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
  margin-bottom: 16px;
}
.ag-filter {
  display: flex;
  align-items: center;
  gap: 7px;
  height: 38px;
  padding: 0 11px;
  background: var(--white);
  border: 1px solid var(--border-strong);
  border-radius: 9px;
}
.ag-filter-icon {
  color: var(--text-3);
  flex-shrink: 0;
}
.ag-filter select,
.ag-filter input {
  border: none;
  background: transparent;
  font-size: 12.5px;
  color: var(--text-2);
  outline: none;
  cursor: pointer;
  font-family: inherit;
  height: auto;
  padding: 0;
  min-width: 0;
}
.ag-legend {
  margin-left: auto;
  display: flex;
  gap: 14px;
  flex-wrap: wrap;
}
.ag-legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 11.5px;
  color: var(--text-3);
}
.ag-legend-swatch {
  width: 11px;
  height: 11px;
  border-radius: 3px;
  border-left-width: 2px;
  border-left-style: solid;
}
.ag-legend-swatch.st-blue {
  background: var(--blue-light);
  border-left-color: var(--blue);
}
.ag-legend-swatch.st-green {
  background: var(--mint-bg);
  border-left-color: var(--mint-dark);
}
.ag-legend-swatch.st-gray {
  background: var(--bg);
  border-left-color: var(--border-strong);
}

/* ===== Vista MES ===== */
.ag-month-layout {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 16px;
  align-items: start;
}
.ag-month-card,
.ag-week-card,
.ag-day-panel,
.ag-day-card {
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 13px;
}
.ag-month-card {
  padding: 16px;
}
.ag-month-daynames,
.ag-month-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}
.ag-month-daynames {
  margin-bottom: 8px;
}
.ag-month-daynames > div {
  text-align: center;
  font-size: 10.5px;
  font-weight: 600;
  color: var(--text-3);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.ag-cell {
  min-height: 86px;
  border: 1px solid var(--border);
  border-radius: 9px;
  padding: 6px 7px;
  background: var(--white);
  cursor: pointer;
  transition: border-color 0.12s, background 0.12s;
}
.ag-cell.is-muted {
  opacity: 0.4;
  cursor: default;
}
.ag-cell.is-today {
  border: 1.5px solid var(--mint);
  background: var(--mint-bg);
}
.ag-cell.is-selected {
  border: 1.5px solid var(--blue);
  background: var(--blue-light);
}
.ag-cell-day {
  font-size: 11px;
  font-weight: 500;
  color: var(--text-2);
}
.ag-cell.is-today .ag-cell-day {
  font-weight: 700;
  color: var(--mint-dark);
}
.ag-cell.is-selected .ag-cell-day {
  font-weight: 700;
  color: var(--blue);
}
.ag-cell-events {
  display: flex;
  flex-direction: column;
  gap: 3px;
  margin-top: 3px;
}

/* Eventos (mes + semana) */
.ag-event {
  font-size: 9.5px;
  font-weight: 600;
  padding: 2px 5px;
  border-radius: 3px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.3;
  cursor: pointer;
  border-left: 2px solid transparent;
}
.ag-event--week {
  white-space: normal;
}
.ag-event.st-blue {
  background: var(--blue-light);
  color: var(--blue-hover);
  border-left-color: var(--blue);
}
.ag-event.st-green {
  background: var(--mint-bg);
  color: var(--mint-dark);
  border-left-color: var(--mint-border);
}
.ag-event.st-gray {
  background: var(--bg);
  color: var(--text-3);
  border-left-color: var(--border-strong);
}
.ag-event-more {
  font-size: 9.5px;
  color: var(--text-3);
  padding: 0 5px;
  cursor: default;
}

/* Panel del día (mes) */
.ag-day-panel {
  padding: 18px;
}
.ag-day-panel-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text);
  text-transform: capitalize;
}
.ag-day-panel-sub {
  font-size: 11.5px;
  color: var(--text-3);
  margin-top: 2px;
  margin-bottom: 14px;
}
.ag-panel-trip {
  display: flex;
  gap: 11px;
  padding: 11px 0;
  border-top: 1px solid var(--border);
  cursor: pointer;
  transition: opacity 0.12s;
}
.ag-panel-trip:hover {
  opacity: 0.7;
}
.ag-panel-trip-time {
  font-size: 11.5px;
  font-weight: 700;
  color: var(--text-2);
  padding-top: 1px;
  white-space: nowrap;
}
.ag-panel-trip-body {
  flex: 1;
  min-width: 0;
}
.ag-panel-trip-title {
  font-size: 12.5px;
  font-weight: 600;
  color: var(--text);
  line-height: 1.3;
}
.ag-panel-trip-meta {
  font-size: 11px;
  color: var(--text-3);
  margin-top: 3px;
}
.ag-panel-trip-tag {
  margin-top: 6px;
}
.ag-panel-trip-chevron {
  color: var(--border-strong);
  flex-shrink: 0;
  margin-top: 2px;
}
.ag-day-panel-empty {
  padding: 26px 0 30px;
  text-align: center;
  border-top: 1px solid var(--border);
  font-size: 12px;
  color: var(--text-3);
}

/* ===== Vista SEMANA ===== */
.ag-week-card {
  padding: 16px;
}
.ag-week-cols {
  display: flex;
  gap: 8px;
}
.ag-week-col {
  flex: 1;
  min-width: 0;
  border: 1px solid var(--border);
  border-radius: 10px;
  background: var(--white);
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  min-height: 280px;
}
.ag-week-col.is-selected {
  border-color: var(--blue-mid);
  background: var(--surface-hover);
}
.ag-week-head {
  text-align: center;
  padding: 8px 4px;
  border-radius: 8px;
  cursor: pointer;
}
.ag-week-head.is-today {
  background: var(--mint-bg);
}
.ag-week-head.is-selected {
  background: var(--blue-light);
}
.ag-week-dn {
  font-size: 10px;
  font-weight: 600;
  color: var(--text-3);
  text-transform: uppercase;
  letter-spacing: 0.04em;
}
.ag-week-day {
  font-size: 16px;
  font-weight: 500;
  color: var(--text-2);
  margin-top: 2px;
}
.ag-week-head.is-today .ag-week-day,
.ag-week-head.is-selected .ag-week-day {
  font-weight: 700;
  color: var(--blue);
}

/* ===== Vista DÍA (timeline) ===== */
.ag-day-card {
  padding: 20px 22px;
  max-width: 680px;
}
.ag-day-card-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}
.ag-day-card-title {
  font-size: 15px;
  font-weight: 600;
  color: var(--text);
  text-transform: capitalize;
}
.ag-day-card-count {
  font-size: 11.5px;
  color: var(--text-3);
}
.ag-day-divider {
  height: 1px;
  background: var(--border);
  margin: 12px 0 4px;
}
.ag-timeline-row {
  display: flex;
  gap: 16px;
  padding: 14px 0;
  border-bottom: 1px solid var(--border);
  cursor: pointer;
  transition: background 0.12s;
}
.ag-timeline-row:hover {
  background: var(--surface-hover);
}
.ag-timeline-time {
  width: 62px;
  flex-shrink: 0;
  text-align: right;
}
.ag-timeline-start {
  font-size: 13px;
  font-weight: 700;
  color: var(--text);
}
.ag-timeline-end {
  font-size: 10.5px;
  color: var(--text-3);
  margin-top: 2px;
}
.ag-timeline-rail {
  width: 2px;
  background: var(--border);
  border-radius: 2px;
  position: relative;
}
.ag-timeline-dot {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  position: absolute;
  left: -3.5px;
  top: 3px;
}
.ag-timeline-dot.st-blue {
  background: var(--blue);
}
.ag-timeline-dot.st-green {
  background: var(--mint-dark);
}
.ag-timeline-dot.st-gray {
  background: var(--border-strong);
}
.ag-timeline-body {
  flex: 1;
  min-width: 0;
}
.ag-timeline-title-row {
  display: flex;
  align-items: center;
  gap: 9px;
  flex-wrap: wrap;
}
.ag-timeline-title {
  font-size: 13.5px;
  font-weight: 600;
  color: var(--text);
}
.ag-timeline-sub {
  font-size: 12px;
  color: var(--text-2);
  margin-top: 5px;
}
.ag-timeline-meta {
  display: flex;
  gap: 14px;
  margin-top: 7px;
  font-size: 11.5px;
  color: var(--text-3);
  flex-wrap: wrap;
}
.ag-timeline-meta span {
  display: flex;
  align-items: center;
  gap: 5px;
}
.ag-timeline-cancel {
  font-size: 11.5px;
  color: var(--red);
  margin-top: 6px;
}
.ag-timeline-chevron {
  color: var(--border-strong);
  flex-shrink: 0;
  align-self: center;
}

/* ===== Responsive ===== */
@media (max-width: 900px) {
  .ag-month-layout {
    grid-template-columns: 1fr;
  }
  .ag-week-cols {
    overflow-x: auto;
  }
  .ag-week-col {
    min-width: 130px;
  }
}
</style>
