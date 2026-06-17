<template>
  <div class="reports">

    <!-- Header con filtros y exportación -->
    <div class="page-header">
      <input type="month" v-model="selectedMonth" class="month-input" />
      <div class="export-btns">
        <button class="btn-export">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
            <polyline points="7 10 12 15 17 10"/>
            <line x1="12" y1="15" x2="12" y2="3"/>
          </svg>
          Excel
        </button>
        <button class="btn-export">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
            <polyline points="7 10 12 15 17 10"/>
            <line x1="12" y1="15" x2="12" y2="3"/>
          </svg>
          PDF
        </button>
      </div>
    </div>

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">VIAJES REALIZADOS</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="1" y="3" width="15" height="13" rx="2"/>
              <path d="M16 8h4l3 3v5h-7V8z"/>
              <circle cx="5.5" cy="18.5" r="2.5"/>
              <circle cx="18.5" cy="18.5" r="2.5"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">67</div>
        <div class="kpi-sub green">↑ 12% vs abril</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TASA DE CUMPLIMIENTO</span>
          <div class="kpi-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">91%</div>
        <div class="kpi-sub gray">6 cancelados</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">COSTO COMBUSTIBLE</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="13 17 18 12 13 7"/>
              <polyline points="6 17 11 12 6 7"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">RD$84k</div>
        <div class="kpi-sub gray">842 galones</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">COSTO MANTENIMIENTO</span>
          <div class="kpi-icon purple">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">RD$32k</div>
        <div class="kpi-sub gray">8 servicios</div>
      </div>
    </div>

    <!-- Gráficos -->
    <div class="charts-grid">

      <!-- Viajes por mes -->
      <div class="card">
        <div class="card-title">VIAJES POR MES — 2026</div>
        <div class="bar-chart">
          <div v-for="mes in monthlyData" :key="mes.label" class="bar-col">
            <div class="bar-track">
              <div
                class="bar-fill"
                :style="{
                  height: (mes.value / maxMonthly * 100) + '%',
                  background: mes.active ? '#2563eb' : '#bfdbfe'
                }"
              ></div>
            </div>
            <span class="bar-label" :class="{ active: mes.active }">{{ mes.label }}</span>
          </div>
        </div>
        <div class="bar-values">
          <div v-for="mes in monthlyData" :key="mes.label" class="bar-value-col">
            <span class="bar-value" :class="{ active: mes.active }">{{ mes.value }}</span>
            <span class="bar-month-label">{{ mes.label }}</span>
          </div>
        </div>
      </div>

      <!-- Solicitudes por área -->
      <div class="card">
        <div class="card-title">SOLICITUDES POR ÁREA</div>
        <div class="hbar-list">
          <div v-for="area in areaData" :key="area.name" class="hbar-item">
            <span class="hbar-label">{{ area.name }}</span>
            <div class="hbar-track">
              <div
                class="hbar-fill"
                :style="{
                  width: (area.value / maxArea * 100) + '%',
                  background: area.color
                }"
              ></div>
            </div>
            <span class="hbar-value">{{ area.value }}</span>
          </div>
        </div>
      </div>

    </div>

    <!-- Tabla conductores -->
    <div class="card">
      <h3 class="section-title">Conductores con más viajes — Mayo 2026</h3>
      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>CONDUCTOR</th>
              <th>TOTAL VIAJES</th>
              <th>KM RECORRIDOS</th>
              <th>TIEMPO EN RUTA</th>
              <th>RATING</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="driver in driversReport" :key="driver.id">
              <td class="td-bold">{{ driver.name }}</td>
              <td>
                <div class="trips-cell">
                  <span>{{ driver.trips }}</span>
                  <div class="mini-bar-track">
                    <div class="mini-bar-fill"
                      :style="{ width: (driver.trips / maxTrips * 100) + '%' }">
                    </div>
                  </div>
                </div>
              </td>
              <td class="td-gray">{{ driver.km }}</td>
              <td class="td-gray">{{ driver.time }}</td>
              <td class="rating">★ {{ driver.rating }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const selectedMonth = ref('2026-05')

const monthlyData = ref([
  { label: 'Ene', value: 38 },
  { label: 'Feb', value: 47 },
  { label: 'Mar', value: 42 },
  { label: 'Abr', value: 60 },
  { label: 'May', value: 67, active: true },
])

const maxMonthly = computed(() => Math.max(...monthlyData.value.map(m => m.value)))

const areaData = ref([
  { name: 'RRHH',        value: 16, color: '#3b82f6' },
  { name: 'Operaciones', value: 13, color: '#22c55e' },
  { name: 'Finanzas',    value: 11, color: '#8b5cf6' },
  { name: 'Legal',       value: 8,  color: '#f59e0b' },
  { name: 'Ventas',      value: 6,  color: '#ef4444' },
  { name: 'Marketing',   value: 4,  color: '#d1d5db' },
])

const maxArea = computed(() => Math.max(...areaData.value.map(a => a.value)))

const driversReport = ref([
  { id: 1, name: 'Juan Pérez',    trips: 18, km: '1,245 km', time: '86h', rating: '4.9' },
  { id: 2, name: 'Ana Martínez',  trips: 15, km: '980 km',   time: '72h', rating: '4.8' },
  { id: 3, name: 'Carlos López',  trips: 12, km: '743 km',   time: '58h', rating: '4.5' },
])

const maxTrips = computed(() => Math.max(...driversReport.value.map(d => d.trips)))
</script>

<style scoped>
.reports {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Header */
.page-header {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 0.75rem;
}

.month-input {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  outline: none;
}

.export-btns { display: flex; gap: 0.5rem; }

.btn-export {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-export:hover { background: #f9fafb; }

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
.kpi-icon.purple { background: #f5f3ff; color: #7c3aed; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  margin-bottom: 0.25rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.green { color: #16a34a; }
.kpi-sub.gray  { color: #6b7280; }

/* Charts grid */
.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
}

/* Card */
.card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.card-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
  margin-bottom: 1.25rem;
}

.section-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 1rem;
}

/* Bar chart vertical */
.bar-chart {
  display: flex;
  align-items: flex-end;
  gap: 0.75rem;
  height: 120px;
  margin-bottom: 0.5rem;
}

.bar-col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
}

.bar-track {
  flex: 1;
  width: 100%;
  display: flex;
  align-items: flex-end;
}

.bar-fill {
  width: 100%;
  border-radius: 4px 4px 0 0;
  transition: height 0.3s ease;
  min-height: 4px;
}

.bar-label {
  font-size: 0.7rem;
  color: #9ca3af;
  margin-top: 0.4rem;
}

.bar-label.active { color: #2563eb; font-weight: 600; }

/* Valores debajo */
.bar-values {
  display: flex;
  gap: 0.75rem;
  border-top: 1px solid #f3f4f6;
  padding-top: 0.75rem;
  margin-top: 0.25rem;
}

.bar-value-col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
}

.bar-value {
  font-size: 0.95rem;
  font-weight: 700;
  color: #111827;
}

.bar-value.active { color: #2563eb; }

.bar-month-label {
  font-size: 0.65rem;
  color: #9ca3af;
}

/* Horizontal bars */
.hbar-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.hbar-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.hbar-label {
  width: 80px;
  font-size: 0.82rem;
  color: #374151;
  text-align: right;
  flex-shrink: 0;
}

.hbar-track {
  flex: 1;
  height: 10px;
  background: #f3f4f6;
  border-radius: 999px;
  overflow: hidden;
}

.hbar-fill {
  height: 100%;
  border-radius: 999px;
  transition: width 0.4s ease;
}

.hbar-value {
  width: 24px;
  font-size: 0.82rem;
  color: #374151;
  font-weight: 600;
  text-align: right;
  flex-shrink: 0;
}

/* Tabla */
.table-wrapper {
  border-radius: 8px;
  border: 1px solid #f3f4f6;
  overflow: hidden;
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
  font-size: 0.875rem;
  color: #374151;
  border-bottom: 1px solid #f9fafb;
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: #f9fafb; }

.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #6b7280; }

/* Trips cell con mini barra */
.trips-cell {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.mini-bar-track {
  width: 80px;
  height: 6px;
  background: #f3f4f6;
  border-radius: 999px;
  overflow: hidden;
}

.mini-bar-fill {
  height: 100%;
  background: #2563eb;
  border-radius: 999px;
}

/* Rating */
.rating {
  color: #f59e0b;
  font-weight: 600;
}
</style>