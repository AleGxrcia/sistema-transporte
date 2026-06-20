<template>
  <div class="trips">

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card" v-for="kpi in kpis" :key="kpi.label">
        <div class="kpi-top">
          <span class="kpi-label">{{ kpi.label }}</span>
          <div class="kpi-icon" :class="kpi.color">
            <span>{{ kpi.icon }}</span>
          </div>
        </div>
        <div class="kpi-value">{{ kpi.value }}</div>
        <div class="kpi-sub" :class="kpi.subColor">{{ kpi.sub }}</div>
      </div>
    </div>

    <!-- Filtros -->
    <div class="filters-bar">
      <div class="filters-left">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
        </svg>
        <span class="filters-title">Filtros</span>
      </div>
      <button class="btn-expand">Expandir ▾</button>
    </div>

    <!-- Tabs vista -->
    <div class="view-tabs">
      <div class="tabs-left">
        <button
          class="view-tab"
          :class="{ active: activeView === 'lista' }"
          @click="activeView = 'lista'"
        >Vista lista</button>
        <button
          class="view-tab"
          :class="{ active: activeView === 'timeline' }"
          @click="activeView = 'timeline'"
        >Vista timeline</button>
      </div>
      <div class="search-box">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="search" type="text"
          placeholder="Buscar por destino, área, con..." class="search-input" />
        <span class="results-count">{{ trips.length }} resultados</span>
      </div>
    </div>

    <!-- Tabla -->
    <div class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>ID ↑</th>
            <th>ÁREA</th>
            <th>DESTINO</th>
            <th>FECHA ↑</th>
            <th>HORARIO</th>
            <th>CONDUCTOR</th>
            <th>VEHÍCULO</th>
            <th>KM</th>
            <th>ESTADO</th>
            <th>DETALLE</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="trip in filteredTrips" :key="trip.id">
            <td class="td-id">{{ trip.code }}</td>
            <td class="td-bold">{{ trip.area }}</td>
            <td>{{ trip.destination }}</td>
            <td>{{ trip.date }}</td>
            <td>{{ trip.schedule }}</td>
            <td>
              <div class="driver-cell">
                <div class="driver-avatar" :style="{ background: trip.avatarColor }">
                  {{ trip.driverInitials }}
                </div>
                <span>{{ trip.driver }}</span>
              </div>
            </td>
            <td class="td-bold">{{ trip.vehicle }}</td>
            <td class="td-gray">{{ trip.km }} km</td>
            <td>
              <span class="badge" :class="trip.status.toLowerCase()">
                • {{ trip.status }}
              </span>
            </td>
            <td>
              <button class="action-btn view">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                  viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                  <circle cx="12" cy="12" r="3"/>
                </svg>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Paginación -->
    <div class="pagination">
      <span class="pagination-info">Mostrando 1-{{ filteredTrips.length }} de {{ trips.length }} viajes</span>
      <div class="pagination-btns">
        <button class="page-btn" disabled>← Anterior</button>
        <button class="page-btn">Siguiente →</button>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const activeView = ref('lista')
const search = ref('')

const kpis = ref([
  { label: 'TOTAL VIAJES',  value: '342', sub: 'Historial completo', icon: '', color: 'blue',   subColor: 'gray'  },
  { label: 'FINALIZADOS',   value: '318', sub: '93% de tasa',        icon: '', color: 'green',  subColor: 'gray'  },
  { label: 'CANCELADOS',    value: '14',  sub: '4% del total',        icon: '',  color: 'red',    subColor: 'gray'  },
  { label: 'KM TOTALES',    value: '28,410', sub: 'Toda la flota',   icon: '', color: 'orange', subColor: 'gray'  },
  { label: 'ESTE MES',      value: '67',  sub: '↑ 12% vs anterior',  icon: '', color: 'teal',   subColor: 'green' },
])

const trips = ref([
  { id: 1, code: '#087', area: 'RRHH',     destination: 'Aeropuerto Las Américas',      date: '28/05/26', schedule: '07:30-09:30', driver: 'Juan Pérez',   driverInitials: 'JP', avatarColor: '#3b82f6', vehicle: 'ABC-123', km: 48,  status: 'Finalizado' },
  { id: 2, code: '#086', area: 'Ventas',   destination: 'Bávaro, La Altagracia',        date: '20/05/26', schedule: '08:00-19:00', driver: 'Juan Pérez',   driverInitials: 'JP', avatarColor: '#3b82f6', vehicle: 'ABC-123', km: 320, status: 'Finalizado' },
  { id: 3, code: '#085', area: 'Legal',    destination: 'Santiago de los Caballeros',   date: '18/05/26', schedule: '06:00-20:00', driver: 'Ana Martínez', driverInitials: 'AM', avatarColor: '#8b5cf6', vehicle: 'JKL-012', km: 280, status: 'Finalizado' },
  { id: 4, code: '#084', area: 'IT',       destination: 'Zona Franca Industrial',       date: '15/05/26', schedule: '10:30-14:00', driver: 'Carlos López', driverInitials: 'CL', avatarColor: '#22c55e', vehicle: 'DEF-456', km: 22,  status: 'Cancelado'  },
  { id: 5, code: '#083', area: 'Gerencia', destination: 'DGII — Centro de los Héroes', date: '10/05/26', schedule: '09:00-12:00', driver: 'Juan Pérez',   driverInitials: 'JP', avatarColor: '#3b82f6', vehicle: 'ABC-123', km: 18,  status: 'Finalizado' },
  { id: 6, code: '#082', area: 'Finanzas', destination: 'Banco Central de la Rep. Dom.',date: '08/05/26', schedule: '09:00-12:00', driver: 'Carlos López', driverInitials: 'CL', avatarColor: '#22c55e', vehicle: 'DEF-456', km: 14,  status: 'Finalizado' },
])

const filteredTrips = computed(() => {
  if (!search.value) return trips.value
  const q = search.value.toLowerCase()
  return trips.value.filter(t =>
    t.destination.toLowerCase().includes(q) ||
    t.area.toLowerCase().includes(q) ||
    t.driver.toLowerCase().includes(q)
  )
})
</script>

<style scoped>
.trips {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  font-family: 'Inter', sans-serif;
}

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 1rem;
}

.kpi-card {
  background: #fff;
  border-radius: 10px;
  padding: 1.1rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.kpi-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.kpi-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.kpi-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
}

.kpi-icon.blue   { background: #eff6ff; }
.kpi-icon.green  { background: #f0fdf4; }
.kpi-icon.red    { background: #fef2f2; }
.kpi-icon.orange { background: #fff7ed; }
.kpi-icon.teal   { background: #f0fdfa; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  line-height: 1;
  margin-bottom: 0.3rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.gray  { color: #6b7280; }
.kpi-sub.green { color: #16a34a; }

/* Filtros */
.filters-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.75rem 1rem;
}

.filters-left {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #374151;
  font-size: 0.875rem;
  font-weight: 500;
}

.filters-title { font-weight: 600; }

.btn-expand {
  background: none;
  border: none;
  font-size: 0.82rem;
  color: #2563eb;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
}

/* View tabs */
.view-tabs {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tabs-left { display: flex; gap: 0; }

.view-tab {
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.view-tab:first-child { border-radius: 8px 0 0 8px; }
.view-tab:last-child  { border-radius: 0 8px 8px 0; border-left: none; }

.view-tab.active {
  background: #f9fafb;
  color: #111827;
  font-weight: 600;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  background: #fff;
  color: #9ca3af;
}

.search-input {
  border: none;
  outline: none;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  width: 220px;
}

.results-count {
  font-size: 0.75rem;
  color: #9ca3af;
  white-space: nowrap;
}

/* Tabla */
.table-wrapper {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  text-align: left;
  font-size: 0.68rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: #374151;
  border-bottom: 1px solid #f9fafb;
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: #f9fafb; }

.td-id   { color: #9ca3af; font-size: 0.8rem; }
.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #9ca3af; }

/* Driver cell */
.driver-cell {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.driver-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.65rem;
  font-weight: 700;
  color: #fff;
}

/* Badges */
.badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
  border: 1px solid transparent;
}

.badge.finalizado { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.cancelado  { background: #f9fafb; color: #6b7280; border-color: #e5e7eb; }

/* Acciones */
.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  background: #fff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
  transition: all 0.15s;
}

.action-btn.view:hover { background: #eff6ff; border-color: #2563eb; color: #2563eb; }

/* Paginación */
.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination-info { font-size: 0.8rem; color: #9ca3af; }
.pagination-btns { display: flex; gap: 0.5rem; }

.page-btn {
  padding: 0.4rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #fff;
  font-size: 0.8rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
  transition: all 0.15s;
}

.page-btn:hover:not(:disabled) { background: #f9fafb; }
.page-btn:disabled { color: #d1d5db; cursor: not-allowed; }
</style>