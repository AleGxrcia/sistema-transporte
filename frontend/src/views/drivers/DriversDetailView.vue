<template>
  <div class="driver-detail">

    <!-- Breadcrumb + acciones -->
    <div class="page-header">
      <div class="breadcrumb">
        <router-link to="/drivers" class="breadcrumb-link">← Conductores</router-link>
        <span class="breadcrumb-sep">/</span>
        <span class="breadcrumb-current">{{ driver.name }}</span>
      </div>
      <div class="header-actions">
        <button class="btn-edit">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
          </svg>
          Editar
        </button>
        <button class="btn-danger">Eliminar</button>
      </div>
    </div>

    <!-- Info card principal -->
    <div class="info-card">
      <div class="info-left">
        <div class="driver-avatar" :style="{ background: driver.avatarColor }">
          {{ driver.initials }}
        </div>
        <div>
          <div class="driver-name-row">
            <h2 class="driver-name">{{ driver.name }}</h2>
            <span class="badge disponible">• {{ driver.status }}</span>
          </div>
          <div class="driver-meta">
            <div class="meta-item">
              <span class="meta-label">CÉDULA</span>
              <span class="meta-value">{{ driver.cedula }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">LICENCIA</span>
              <span class="meta-value">{{ driver.license }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">TIPO DE LICENCIA</span>
              <span class="type-badge">Clase {{ driver.licenseType }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">VENCE LICENCIA</span>
              <span class="meta-value green">{{ driver.licenseExpiry }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">TELÉFONO</span>
              <span class="meta-value">{{ driver.phone }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">SUPERVISOR</span>
              <span class="meta-value">{{ driver.supervisor }}</span>
            </div>
          </div>
          <div class="driver-address">
            <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
            {{ driver.address }}
          </div>
        </div>
      </div>
      <button class="btn-status">Cambiar estado ▾</button>
    </div>

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL VIAJES</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">134</div>
        <div class="kpi-sub gray">Historial completo</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">VIAJES ESTE MES</span>
          <div class="kpi-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">18</div>
        <div class="kpi-sub green">↑ vs 14 abril</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">KM RECORRIDOS</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <polyline points="12 6 12 12 16 14"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">1,245</div>
        <div class="kpi-sub gray">Mayo 2026</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">DÍAS PARA VENCER LIC.</span>
          <div class="kpi-icon teal">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
              <line x1="16" y1="2" x2="16" y2="6"/>
              <line x1="8" y1="2" x2="8" y2="6"/>
              <line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">79</div>
        <div class="kpi-sub gray">15 agosto 2026</div>
      </div>
    </div>

    <!-- Tabs -->
    <div class="tabs-bar">
      <button
        v-for="tab in tabs"
        :key="tab.key"
        class="tab"
        :class="{ active: activeTab === tab.key }"
        @click="activeTab = tab.key"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- Tab: Información personal -->
    <div v-if="activeTab === 'info'" class="tab-content">
      <div class="two-col">

        <!-- Datos personales -->
        <div class="data-card">
          <div class="data-card-title">DATOS PERSONALES</div>
          <div class="data-grid">
            <div class="data-item full">
              <span class="data-label">Nombre completo</span>
              <span class="data-value">{{ driver.name }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Cédula</span>
              <span class="data-value">{{ driver.cedula }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Teléfono</span>
              <span class="data-value">{{ driver.phone }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Estado</span>
              <span class="badge disponible" style="width:fit-content">• {{ driver.status }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Supervisor asignado</span>
              <span class="data-value">{{ driver.supervisor }}</span>
            </div>
            <div class="data-item full">
              <span class="data-label">Dirección</span>
              <span class="data-value">{{ driver.address }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Fecha de registro</span>
              <span class="data-value">03/01/2024</span>
            </div>
          </div>
        </div>

        <!-- Datos de licencia -->
        <div class="data-card">
          <div class="data-card-title">DATOS DE LICENCIA</div>
          <div class="data-grid">
            <div class="data-item">
              <span class="data-label">Número de licencia</span>
              <span class="data-value">{{ driver.license }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Tipo de licencia</span>
              <span class="type-badge">Clase {{ driver.licenseType }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Fecha de vencimiento</span>
              <span class="data-value green">{{ driver.licenseExpiry }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Días restantes</span>
              <span class="data-value green">79 días</span>
            </div>
          </div>

          <!-- Barra vigencia -->
          <div class="license-bar-section">
            <div class="data-label" style="margin-bottom:0.5rem">Vigencia de la licencia</div>
            <div class="license-bar-track">
              <div class="license-bar-fill" style="width: 72%"></div>
            </div>
            <div class="license-bar-labels">
              <span>Emisión: 15/08/2023</span>
              <span class="green font-semibold">72% vigente</span>
              <span>Vence: 15/08/2026</span>
            </div>
          </div>

          <!-- Estadísticas del mes -->
          <div class="stats-section">
            <div class="data-card-title">ESTADÍSTICAS DEL MES</div>
            <div class="stats-grid">
              <div class="stat-item">
                <span class="stat-value">18</span>
                <span class="stat-label">Viajes</span>
              </div>
              <div class="stat-item">
                <span class="stat-value">1,245</span>
                <span class="stat-label">Km</span>
              </div>
              <div class="stat-item">
                <span class="stat-value">86h</span>
                <span class="stat-label">En ruta</span>
              </div>
              <div class="stat-item">
                <span class="stat-value green">★ 4.9</span>
                <span class="stat-label">Rating</span>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- Tab: Historial de viajes -->
    <div v-if="activeTab === 'trips'" class="tab-content">
      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>FECHA</th>
              <th>ÁREA</th>
              <th>DESTINO</th>
              <th>VEHÍCULO</th>
              <th>SALIDA</th>
              <th>REGRESO</th>
              <th>PASAJEROS</th>
              <th>KM</th>
              <th>ESTADO</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="trip in driverTrips" :key="trip.id">
              <td class="td-gray">{{ trip.date }}</td>
              <td class="td-bold">{{ trip.area }}</td>
              <td>{{ trip.destination }}</td>
              <td class="td-bold">{{ trip.vehicle }}</td>
              <td>{{ trip.departure }}</td>
              <td>{{ trip.return }}</td>
              <td>{{ trip.passengers }}</td>
              <td class="td-gray">{{ trip.km }} km</td>
              <td>
                <span class="badge" :class="trip.status.toLowerCase()">
                  • {{ trip.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="pagination">
        <span class="pagination-info">Mostrando 1-5 de 134 viajes</span>
        <div class="pagination-btns">
          <button class="page-btn" disabled>← Anterior</button>
          <button class="page-btn">Siguiente →</button>
        </div>
      </div>
    </div>

    <!-- Tab: Disponibilidad -->
    <div v-if="activeTab === 'availability'" class="tab-content">
      <div class="data-card">
        <div class="data-card-title">DISPONIBILIDAD — ESTA SEMANA</div>
        <div class="availability-list">
          <div v-for="day in availability" :key="day.day" class="avail-row">
            <span class="avail-day">{{ day.day }}</span>
            <div class="avail-bar" :class="day.type">{{ day.label }}</div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref } from 'vue'

const activeTab = ref('info')

const tabs = [
  { key: 'info',         label: 'Información personal' },
  { key: 'trips',        label: 'Historial de viajes'  },
  { key: 'availability', label: 'Disponibilidad'        },
]

const driver = ref({
  id: 1,
  name: 'Juan Pérez',
  initials: 'JP',
  avatarColor: '#3b82f6',
  cedula: '001-2345678-9',
  license: 'LIC-009-2023',
  licenseType: 'B',
  licenseExpiry: '15/08/2026',
  phone: '809-555-0101',
  supervisor: 'Carlos Méndez',
  address: 'Av. Luperón 45, Los Jardines, Santo Domingo',
  status: 'Disponible',
})

const availability = ref([
  { day: 'Lun', label: 'Disponible todo el día', type: 'available' },
  { day: 'Mar', label: 'Disponible todo el día', type: 'available' },
  { day: 'Mié', label: 'En viaje 07:00-10:30',  type: 'busy'      },
  { day: 'Jue', label: 'Disponible todo el día', type: 'available' },
  { day: 'Vie', label: 'En viaje 08:00-17:00',  type: 'busy'      },
  { day: 'Sáb', label: 'No operativo',           type: 'inactive'  },
])

const driverTrips = ref([
  { id: 1, date: '28/05/26', area: 'RRHH',     destination: 'Aeropuerto Las Américas',    vehicle: 'ABC-123', departure: '07:30', return: '09:30', passengers: 8,  km: 48,  status: 'Finalizado' },
  { id: 2, date: '25/05/26', area: 'Legal',    destination: 'Santiago de los Caballeros', vehicle: 'ABC-123', departure: '06:00', return: '20:00', passengers: 5,  km: 280, status: 'Finalizado' },
  { id: 3, date: '20/05/26', area: 'Ventas',   destination: 'Bávaro, La Altagracia',      vehicle: 'ABC-123', departure: '08:00', return: '19:00', passengers: 10, km: 320, status: 'Finalizado' },
  { id: 4, date: '15/05/26', area: 'Gerencia', destination: 'DGII Centro de los Héroes',  vehicle: 'ABC-123', departure: '09:00', return: '12:00', passengers: 3,  km: 18,  status: 'Finalizado' },
  { id: 5, date: '10/05/26', area: 'IT',       destination: 'Zona Franca Industrial',     vehicle: 'ABC-123', departure: '10:30', return: '14:00', passengers: 4,  km: 22,  status: 'Cancelado'  },
])
</script>

<style scoped>
.driver-detail {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Header */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.82rem;
}

.breadcrumb-link { color: #2563eb; text-decoration: none; }
.breadcrumb-link:hover { text-decoration: underline; }
.breadcrumb-sep { color: #d1d5db; }
.breadcrumb-current { color: #6b7280; }

.header-actions { display: flex; gap: 0.75rem; }

.btn-edit {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
}

.btn-danger {
  padding: 0.5rem 1rem;
  border: 1px solid #fecaca;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #dc2626;
  cursor: pointer;
}

/* Info card */
.info-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.info-left { display: flex; gap: 1.25rem; align-items: flex-start; }

.driver-avatar {
  width: 64px;
  height: 64px;
  min-width: 64px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  font-weight: 700;
  color: #fff;
}

.driver-name-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.driver-name {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.driver-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  margin-bottom: 0.75rem;
}

.meta-item { display: flex; flex-direction: column; gap: 2px; }

.meta-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.meta-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.meta-value.green { color: #16a34a; }

.type-badge {
  background: #f3f4f6;
  color: #374151;
  font-size: 0.75rem;
  padding: 2px 8px;
  border-radius: 4px;
  font-weight: 500;
  width: fit-content;
}

.driver-address {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.78rem;
  color: #6b7280;
}

.btn-status {
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
  white-space: nowrap;
}

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
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
}

.kpi-icon.blue   { background: #eff6ff; color: #2563eb; }
.kpi-icon.green  { background: #f0fdf4; color: #16a34a; }
.kpi-icon.orange { background: #fff7ed; color: #d97706; }
.kpi-icon.teal   { background: #f0fdfa; color: #0d9488; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  margin-bottom: 0.25rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.gray  { color: #6b7280; }
.kpi-sub.green { color: #16a34a; }

/* Tabs */
.tabs-bar {
  display: flex;
  border-bottom: 1px solid #e5e7eb;
}

.tab {
  padding: 0.75rem 1.25rem;
  border: none;
  background: none;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.2s;
  margin-bottom: -1px;
}

.tab:hover { color: #111827; }
.tab.active { color: #2563eb; border-bottom-color: #2563eb; font-weight: 600; }

/* Tab content */
.tab-content { display: flex; flex-direction: column; gap: 1rem; }

.two-col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

/* Data card */
.data-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.data-card-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
}

.data-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.data-item { display: flex; flex-direction: column; gap: 2px; }
.data-item.full { grid-column: 1 / -1; }

.data-label { font-size: 0.72rem; color: #9ca3af; }

.data-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.data-value.green { color: #16a34a; }

/* License bar */
.license-bar-section { display: flex; flex-direction: column; gap: 0.4rem; }

.license-bar-track {
  height: 8px;
  background: #e5e7eb;
  border-radius: 999px;
  overflow: hidden;
}

.license-bar-fill {
  height: 100%;
  background: #16a34a;
  border-radius: 999px;
}

.license-bar-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.72rem;
  color: #9ca3af;
}

/* Stats */
.stats-section { display: flex; flex-direction: column; gap: 0.75rem; }

.stats-grid {
  display: flex;
  gap: 1.5rem;
}

.stat-item { display: flex; flex-direction: column; gap: 2px; }

.stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.stat-label { font-size: 0.72rem; color: #9ca3af; }

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

.badge.disponible { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.en-viaje   { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.suspendido { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
.badge.finalizado { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.cancelado  { background: #f9fafb; color: #6b7280; border-color: #e5e7eb; }

/* Availability */
.availability-list { display: flex; flex-direction: column; gap: 0.5rem; }

.avail-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.avail-day {
  width: 32px;
  font-size: 0.8rem;
  color: #6b7280;
  font-weight: 500;
}

.avail-bar {
  flex: 1;
  padding: 0.35rem 0.75rem;
  border-radius: 4px;
  font-size: 0.78rem;
  font-weight: 500;
}

.avail-bar.available { background: #dcfce7; color: #16a34a; }
.avail-bar.busy      { background: #dbeafe; color: #2563eb; }
.avail-bar.inactive  { background: #f3f4f6; color: #9ca3af; }

/* Tabla */
.table-wrapper {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.table { width: 100%; border-collapse: collapse; }

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

.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #9ca3af; }
.green   { color: #16a34a; }
.font-semibold { font-weight: 600; }

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
}

.page-btn:hover:not(:disabled) { background: #f9fafb; }
.page-btn:disabled { color: #d1d5db; cursor: not-allowed; }
</style>