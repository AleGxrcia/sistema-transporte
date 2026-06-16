<template>
  <div class="calendar">

    <!-- Header -->
    <div class="cal-header">
      <div class="view-tabs">
        <button
          v-for="view in views"
          :key="view.key"
          class="view-tab"
          :class="{ active: activeView === view.key }"
          @click="activeView = view.key"
        >
          {{ view.label }}
        </button>
      </div>
      <div class="filters">
        <select v-model="driverFilter" class="filter-select">
          <option value="">Todos los conductores ▾</option>
          <option value="juan">Juan Pérez</option>
          <option value="ana">Ana Martínez</option>
          <option value="carlos">Carlos López</option>
        </select>
        <select v-model="vehicleFilter" class="filter-select">
          <option value="">Todos los vehículos ▾</option>
          <option value="abc">ABC-123</option>
          <option value="def">DEF-456</option>
          <option value="jkl">JKL-012</option>
        </select>
      </div>
    </div>

    <!-- Navegación mes -->
    <div class="month-nav">
      <h2 class="month-title">{{ currentMonthLabel }}</h2>
      <div class="nav-btns">
        <button class="nav-btn" @click="prevMonth">←</button>
        <button class="nav-btn today-btn" @click="goToday">Hoy</button>
        <button class="nav-btn" @click="nextMonth">→</button>
      </div>
    </div>

    <!-- Grid del calendario -->
    <div class="calendar-grid">

      <!-- Cabeceras días -->
      <div class="day-header" v-for="day in dayHeaders" :key="day">
        {{ day }}
      </div>

      <!-- Celdas -->
      <div
        v-for="cell in calendarCells"
        :key="cell.date"
        class="day-cell"
        :class="{
          'other-month': !cell.currentMonth,
          'today': cell.isToday,
        }"
      >
        <span class="day-number" :class="{ today: cell.isToday }">
          {{ cell.day }}
        </span>
        <div class="events">
          <div
            v-for="event in cell.events"
            :key="event.id"
            class="event"
            :class="event.type"
          >
            {{ event.label }}
          </div>
        </div>
      </div>

    </div>

    <!-- Leyenda -->
    <div class="legend">
      <div class="legend-item">
        <span class="legend-dot trip"></span>
        Viaje aprobado
      </div>
      <div class="legend-item">
        <span class="legend-dot maintenance"></span>
        Mantenimiento
      </div>
      <div class="legend-item">
        <span class="legend-dot pending"></span>
        Pendiente aprobación
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const activeView = ref('mes')
const driverFilter = ref('')
const vehicleFilter = ref('')

const views = [
  { key: 'mes',    label: 'Mes'    },
  { key: 'semana', label: 'Semana' },
  { key: 'dia',    label: 'Día'    },
]

const dayHeaders = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom']

// Fecha actual
const currentDate = ref(new Date(2026, 4, 1)) // Mayo 2026

const currentMonthLabel = computed(() => {
  return currentDate.value.toLocaleDateString('es-DO', { month: 'long', year: 'numeric' })
    .replace(/^\w/, c => c.toUpperCase())
})

function prevMonth() {
  const d = new Date(currentDate.value)
  d.setMonth(d.getMonth() - 1)
  currentDate.value = d
}

function nextMonth() {
  const d = new Date(currentDate.value)
  d.setMonth(d.getMonth() + 1)
  currentDate.value = d
}

function goToday() {
  currentDate.value = new Date(2026, 4, 1)
}

// Eventos mock
const events = [
  { date: '2026-05-02', id: 1,  label: 'IT → Z.Franca',   type: 'trip'        },
  { date: '2026-05-06', id: 2,  label: 'Ventas → SDE',    type: 'trip'        },
  { date: '2026-05-08', id: 3,  label: 'Legal → DGII',    type: 'trip'        },
  { date: '2026-05-08', id: 4,  label: 'RRHH → Apto.',    type: 'trip'        },
  { date: '2026-05-12', id: 5,  label: 'Finanzas → BHD',  type: 'trip'        },
  { date: '2026-05-16', id: 6,  label: 'Mant. ABC-123',   type: 'maintenance' },
  { date: '2026-05-18', id: 7,  label: 'Oper. → Romana',  type: 'trip'        },
  { date: '2026-05-22', id: 8,  label: 'Legal → Santiago', type: 'trip'       },
  { date: '2026-05-26', id: 9,  label: 'RRHH → Bávaro',   type: 'trip'       },
  { date: '2026-05-28', id: 10, label: 'RRHH → Apto.',    type: 'trip'        },
  { date: '2026-05-28', id: 11, label: 'Fin. → Banco',    type: 'trip'        },
  { date: '2026-05-29', id: 12, label: 'Pendiente',        type: 'pending'     },
  { date: '2026-05-30', id: 13, label: 'Pendiente',        type: 'pending'     },
  { date: '2026-05-31', id: 14, label: 'Pendiente',        type: 'pending'     },
]

// Generar celdas del calendario
const calendarCells = computed(() => {
  const year  = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()

  const firstDay = new Date(year, month, 1)
  const lastDay  = new Date(year, month + 1, 0)

  // Lunes = 0, ajustar primer día
  let startDow = firstDay.getDay() - 1
  if (startDow < 0) startDow = 6

  const cells = []

  // Días del mes anterior
  for (let i = startDow - 1; i >= 0; i--) {
    const d = new Date(year, month, -i)
    cells.push({
      date: formatDate(d),
      day: d.getDate(),
      currentMonth: false,
      isToday: false,
      events: [],
    })
  }

  // Días del mes actual
  const today = new Date(2026, 4, 28) // mock "hoy"
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const d = new Date(year, month, i)
    const dateStr = formatDate(d)
    cells.push({
      date: dateStr,
      day: i,
      currentMonth: true,
      isToday: d.toDateString() === today.toDateString(),
      events: events.filter(e => e.date === dateStr),
    })
  }

  // Completar hasta 42 celdas (6 semanas)
  const remaining = 42 - cells.length
  for (let i = 1; i <= remaining; i++) {
    const d = new Date(year, month + 1, i)
    cells.push({
      date: formatDate(d),
      day: i,
      currentMonth: false,
      isToday: false,
      events: [],
    })
  }

  return cells
})

function formatDate(d) {
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
}
</script>

<style scoped>
.calendar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  font-family: 'Inter', sans-serif;
}

/* Header */
.cal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.view-tabs {
  display: flex;
  background: #f3f4f6;
  border-radius: 8px;
  padding: 3px;
  gap: 2px;
}

.view-tab {
  padding: 0.4rem 1rem;
  border: none;
  background: transparent;
  border-radius: 6px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.view-tab.active {
  background: #fff;
  color: #111827;
  font-weight: 600;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.filters { display: flex; gap: 0.5rem; }

.filter-select {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.4rem 0.75rem;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  background: #fff;
  outline: none;
  cursor: pointer;
}

/* Navegación mes */
.month-nav {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.month-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
  text-transform: capitalize;
}

.nav-btns { display: flex; gap: 0.4rem; align-items: center; }

.nav-btn {
  width: 32px;
  height: 32px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #fff;
  font-size: 0.875rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s;
}

.nav-btn:hover { background: #f9fafb; }

.today-btn {
  width: auto;
  padding: 0 0.75rem;
  font-family: 'Inter', sans-serif;
  font-size: 0.82rem;
}

/* Grid */
.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  overflow: hidden;
  background: #fff;
}

.day-header {
  padding: 0.6rem;
  text-align: center;
  font-size: 0.75rem;
  font-weight: 600;
  color: #9ca3af;
  background: #fff;
  border-bottom: 1px solid #e5e7eb;
}

.day-cell {
  min-height: 90px;
  padding: 0.5rem;
  border-right: 1px solid #f3f4f6;
  border-bottom: 1px solid #f3f4f6;
  display: flex;
  flex-direction: column;
  gap: 3px;
  background: #fff;
  transition: background 0.15s;
}

.day-cell:hover { background: #f9fafb; }

.day-cell.other-month { background: #fafafa; }
.day-cell.other-month .day-number { color: #d1d5db; }

.day-cell.today { background: #eff6ff; border: 1px solid #bfdbfe; }

.day-number {
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
}

.day-number.today {
  background: #2563eb;
  color: #fff;
  font-weight: 700;
}

/* Eventos */
.events { display: flex; flex-direction: column; gap: 2px; }

.event {
  font-size: 0.7rem;
  padding: 2px 6px;
  border-radius: 3px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  border-left: 3px solid transparent;
}

.event.trip {
  background: #dbeafe;
  color: #1e40af;
  border-left-color: #3b82f6;
}

.event.maintenance {
  background: #dcfce7;
  color: #15803d;
  border-left-color: #22c55e;
}

.event.pending {
  background: #f5f3ff;
  color: #6d28d9;
  border-left-color: #8b5cf6;
}

/* Leyenda */
.legend {
  display: flex;
  gap: 1.5rem;
  align-items: center;
  font-size: 0.82rem;
  color: #6b7280;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.legend-dot {
  width: 10px;
  height: 10px;
  border-radius: 2px;
  border-left: 3px solid transparent;
}

.legend-dot.trip        { background: #dbeafe; border-left-color: #3b82f6; }
.legend-dot.maintenance { background: #dcfce7; border-left-color: #22c55e; }
.legend-dot.pending     { background: #f5f3ff; border-left-color: #8b5cf6; }
</style>