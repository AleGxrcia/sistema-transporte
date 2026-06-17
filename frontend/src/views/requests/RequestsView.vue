<template>
  <div class="requests">

    <!-- Header -->
    <div class="page-header">
      <div class="tabs">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          class="tab"
          :class="{ active: activeTab === tab.key }"
          @click="activeTab = tab.key"
        >
          {{ tab.label }}
          <span class="tab-badge" :class="{ active: activeTab === tab.key }">
            {{ tab.count }}
          </span>
        </button>
      </div>
      <router-link to="/requests/new" class="btn-primary">
        + Nueva solicitud
      </router-link>
    </div>

    <!-- Filtros -->
    <div class="filters">
      <div class="search-box">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="search" type="text" placeholder="Buscar..." class="search-input" />
      </div>
      <input v-model="dateFilter" type="date" class="date-input" />
      <button class="btn-filter">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
        </svg>
        Más filtros
      </button>
    </div>

    <!-- Tabla -->
    <div class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>ÁREA</th>
            <th>DESTINO</th>
            <th>FECHA</th>
            <th>HORARIO</th>
            <th>PERS.</th>
            <th>CONDUCTOR</th>
            <th>VEHÍCULO</th>
            <th>ESTADO</th>
            <th>ACCIONES</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="req in filteredRequests" :key="req.id">
            <td class="td-id">{{ req.code }}</td>
            <td class="td-bold">{{ req.area }}</td>
            <td>{{ req.destination }}</td>
            <td>{{ req.date }}</td>
            <td>{{ req.schedule }}</td>
            <td>{{ req.passengers }}</td>
            <td class="td-gray">{{ req.driver || '—' }}</td>
            <td class="td-gray">{{ req.vehicle || '—' }}</td>
            <td>
              <span class="badge" :class="req.status.toLowerCase()">
                • {{ req.status }}
              </span>
            </td>
            <td>
              <div class="actions">
                <button
                  v-if="req.status === 'Pendiente'"
                  class="action-btn approve" title="Aprobar">✓</button>
                <button
                  v-if="req.status === 'Pendiente'"
                  class="action-btn reject" title="Rechazar">✕</button>
                <button
                  v-if="req.status !== 'Pendiente'"
                  class="action-btn view" title="Ver detalle">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                    <circle cx="12" cy="12" r="3"/>
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Paginación -->
    <div class="pagination">
      <span class="pagination-info">Mostrando 1-{{ filteredRequests.length }} de {{ requests.length }} solicitudes</span>
      <div class="pagination-btns">
        <button class="page-btn" disabled>← Anterior</button>
        <button class="page-btn">Siguiente →</button>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const activeTab = ref('pendientes')
const search = ref('')
const dateFilter = ref('')

const tabs = [
  { key: 'todas',      label: 'Todas',      count: 28 },
  { key: 'pendientes', label: 'Pendientes', count: 5  },
  { key: 'aprobadas',  label: 'Aprobadas',  count: 12 },
  { key: 'finalizadas',label: 'Finalizadas',count: 8  },
  { key: 'rechazadas', label: 'Rechazadas', count: 3  },
]

const requests = ref([
  { id: 1, code: '#001', area: 'Ventas',     destination: 'Sto. Domingo Este', date: '29/05/26', schedule: '09:00-13:00', passengers: 6, driver: null,          vehicle: null,      status: 'Pendiente'  },
  { id: 2, code: '#002', area: 'Operaciones',destination: 'La Romana',         date: '30/05/26', schedule: '07:00-18:00', passengers: 3, driver: null,          vehicle: null,      status: 'Pendiente'  },
  { id: 3, code: '#003', area: 'RRHH',       destination: 'Aeropuerto',        date: '28/05/26', schedule: '07:30-09:30', passengers: 8, driver: 'Juan Pérez',  vehicle: 'ABC-123', status: 'Aprobada'   },
  { id: 4, code: '#004', area: 'Finanzas',   destination: 'Banco Central',     date: '28/05/26', schedule: '09:00-12:00', passengers: 4, driver: 'Carlos López',vehicle: 'DEF-456', status: 'Aprobada'   },
  { id: 5, code: '#005', area: 'Legal',      destination: 'Santiago',          date: '25/05/26', schedule: '06:00-20:00', passengers: 5, driver: 'Ana Martínez',vehicle: 'JKL-012', status: 'Finalizada' },
  { id: 6, code: '#006', area: 'Marketing',  destination: 'Punta Cana',        date: '22/05/26', schedule: '08:00-19:00', passengers: 7, driver: null,          vehicle: null,      status: 'Rechazada'  },
])

const filteredRequests = computed(() => {
  let result = requests.value
  if (activeTab.value !== 'todas') {
    const map = { pendientes: 'Pendiente', aprobadas: 'Aprobada', finalizadas: 'Finalizada', rechazadas: 'Rechazada' }
    result = result.filter(r => r.status === map[activeTab.value])
  }
  if (search.value) {
    const q = search.value.toLowerCase()
    result = result.filter(r =>
      r.area.toLowerCase().includes(q) ||
      r.destination.toLowerCase().includes(q)
    )
  }
  return result
})
</script>

<style scoped>
.requests {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  font-family: 'Inter', sans-serif;
}

/* Header */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tabs {
  display: flex;
  gap: 0.5rem;
}

.tab {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 999px;
  background: #fff;
  font-size: 0.82rem;
  color: #6b7280;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
  transition: all 0.2s;
}

.tab.active {
  background: #2563eb;
  color: #fff;
  border-color: #2563eb;
  font-weight: 600;
}

.tab-badge {
  background: #f3f4f6;
  color: #6b7280;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 1px 6px;
  border-radius: 999px;
}

.tab-badge.active {
  background: rgba(255,255,255,0.25);
  color: #fff;
}

.btn-primary {
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.6rem 1.1rem;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  transition: background 0.2s;
}

.btn-primary:hover { background: #1d4ed8; }

/* Filtros */
.filters {
  display: flex;
  gap: 0.75rem;
  align-items: center;
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
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  width: 180px;
}

.date-input {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  outline: none;
}

.btn-filter {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
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
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid #f3f4f6;
  background: #fff;
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: #374151;
  border-bottom: 1px solid #f9fafb;
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: #f9fafb; }

.td-id { color: #9ca3af; font-size: 0.8rem; }
.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #9ca3af; }

/* Badges de estado */
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

.badge.pendiente  { background: #f5f3ff; color: #7c3aed; border-color: #ddd6fe; }
.badge.aprobada   { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.finalizada { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.rechazada  { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
.badge.cancelada  { background: #f9fafb; color: #6b7280; border-color: #e5e7eb; }

/* Acciones */
.actions { display: flex; gap: 0.4rem; }

.action-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  background: #fff;
  cursor: pointer;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
  color: #6b7280;
}

.action-btn.approve:hover { background: #f0fdf4; border-color: #16a34a; color: #16a34a; }
.action-btn.reject:hover  { background: #fef2f2; border-color: #dc2626; color: #dc2626; }
.action-btn.view:hover    { background: #eff6ff; border-color: #2563eb; color: #2563eb; }

/* Paginación */
.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination-info {
  font-size: 0.8rem;
  color: #9ca3af;
}

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