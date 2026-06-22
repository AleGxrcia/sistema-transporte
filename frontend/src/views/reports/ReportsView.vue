<template>
  <div class="reports">

    <!-- Header con filtros y exportación -->
    <div class="page-header">
      <input type="month" v-model="selectedMonth" class="month-input" />
      <div class="export-btns">
        <button class="btn-export" :disabled="reportsStore.isExporting" @click="handleExportExcel">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
            <polyline points="7 10 12 15 17 10"/>
            <line x1="12" y1="15" x2="12" y2="3"/>
          </svg>
          Excel
        </button>
        <button class="btn-export" :disabled="reportsStore.isExporting" @click="handleExportPdf">
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

    <div v-if="reportsStore.isLoading" class="empty-state">Cargando reporte…</div>

    <template v-else-if="summary">
      <!-- KPIs -->
      <div class="kpi-grid">
        <div class="kpi-card">
          <div class="kpi-top">
            <span class="kpi-label">VIAJES COMPLETADOS</span>
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
          <div class="kpi-value">{{ summary.tripsCompleted }}</div>
          <div class="kpi-sub" :class="tripsTrend.cls">{{ tripsTrend.label }}</div>
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
          <div class="kpi-value">{{ summary.completionRatePercent }}%</div>
          <div class="kpi-sub gray">{{ summary.cancelledCount }} cancelados</div>
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
          <div class="kpi-value">{{ formatCurrency(summary.fuelCost) }}</div>
          <div class="kpi-sub gray">{{ formatNumber(summary.fuelGallons) }} galones</div>
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
          <div class="kpi-value">{{ formatCurrency(summary.maintenanceCost) }}</div>
          <div class="kpi-sub gray">{{ summary.maintenanceServicesCount }} servicios</div>
        </div>
      </div>

      <!-- Gráficos -->
      <div class="charts-grid">

        <!-- Viajes por mes -->
        <div class="card">
          <div class="card-title">VIAJES POR MES</div>
          <div class="bar-chart">
            <div v-for="mes in summary.tripsByMonth" :key="`${mes.year}-${mes.month}`" class="bar-col">
              <div class="bar-track">
                <div
                  class="bar-fill"
                  :style="{
                    height: (mes.count / maxMonthly * 100) + '%',
                    background: isCurrentMonth(mes) ? 'var(--blue-hover)' : 'var(--blue-mid)'
                  }"
                ></div>
              </div>
              <span class="bar-label" :class="{ active: isCurrentMonth(mes) }">{{ mes.monthLabel }}</span>
            </div>
          </div>
          <div class="bar-values">
            <div v-for="mes in summary.tripsByMonth" :key="`v-${mes.year}-${mes.month}`" class="bar-value-col">
              <span class="bar-value" :class="{ active: isCurrentMonth(mes) }">{{ mes.count }}</span>
            </div>
          </div>
        </div>

        <!-- Solicitudes por área -->
        <div class="card">
          <div class="card-title">SOLICITUDES POR ÁREA</div>
          <div v-if="!summary.requestsByArea.length" class="empty-state">Sin solicitudes este mes.</div>
          <div v-else class="hbar-list">
            <div v-for="(area, i) in summary.requestsByArea" :key="area.area" class="hbar-item">
              <span class="hbar-label">{{ area.area }}</span>
              <div class="hbar-track">
                <div
                  class="hbar-fill"
                  :style="{
                    width: (area.count / maxArea * 100) + '%',
                    background: palette[i % palette.length]
                  }"
                ></div>
              </div>
              <span class="hbar-value">{{ area.count }}</span>
            </div>
          </div>
        </div>

        <!-- Vehículos más utilizados -->
        <div class="card">
          <div class="card-title">VEHÍCULOS MÁS UTILIZADOS</div>
          <div v-if="!summary.topVehicles.length" class="empty-state">Sin viajes completados este mes.</div>
          <div v-else class="hbar-list">
            <div v-for="(v, i) in summary.topVehicles" :key="v.vehicleId" class="hbar-item">
              <span class="hbar-label">{{ v.licensePlate }}</span>
              <div class="hbar-track">
                <div
                  class="hbar-fill"
                  :style="{
                    width: (v.tripsCompleted / maxVehicleUsage * 100) + '%',
                    background: palette[i % palette.length]
                  }"
                ></div>
              </div>
              <span class="hbar-value">{{ v.tripsCompleted }}</span>
            </div>
          </div>
        </div>

        <!-- Consumo de combustible -->
        <div class="card">
          <div class="card-title">CONSUMO DE COMBUSTIBLE (GALONES POR MES)</div>
          <div class="bar-chart">
            <div v-for="mes in summary.fuelByMonth" :key="`${mes.year}-${mes.month}`" class="bar-col">
              <div class="bar-track">
                <div
                  class="bar-fill"
                  :style="{
                    height: (mes.gallons / maxFuel * 100) + '%',
                    background: isCurrentMonth(mes) ? 'var(--amber)' : 'var(--amber-border)'
                  }"
                ></div>
              </div>
              <span class="bar-label" :class="{ active: isCurrentMonth(mes) }">{{ mes.monthLabel }}</span>
            </div>
          </div>
          <div class="bar-values">
            <div v-for="mes in summary.fuelByMonth" :key="`f-${mes.year}-${mes.month}`" class="bar-value-col">
              <span class="bar-value" :class="{ active: isCurrentMonth(mes) }">{{ formatNumber(mes.gallons) }}</span>
            </div>
          </div>
        </div>

      </div>

      <!-- Tabla conductores -->
      <div class="card">
        <h3 class="section-title">Conductores con más viajes</h3>
        <div v-if="!summary.topDrivers.length" class="empty-state">Sin viajes asignados este mes.</div>
        <div v-else class="table-wrapper">
          <table class="table">
            <thead>
              <tr>
                <th>CONDUCTOR</th>
                <th>VIAJES COMPLETADOS</th>
                <th>CANCELADOS</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="driver in summary.topDrivers" :key="driver.driverId">
                <td class="td-bold">{{ driver.driverName }}</td>
                <td>
                  <div class="trips-cell">
                    <span>{{ driver.tripsCompleted }}</span>
                    <div class="mini-bar-track">
                      <div class="mini-bar-fill"
                        :style="{ width: (driver.tripsCompleted / maxTrips * 100) + '%' }">
                      </div>
                    </div>
                  </div>
                </td>
                <td class="td-gray">{{ driver.tripsCancelled }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-else-if="reportsStore.error" class="empty-state">{{ reportsStore.error }}</div>

  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useReportsStore } from '@/stores/reports.store'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { formatCurrency, formatNumber } from '@/utils/formatters'

const reportsStore = useReportsStore()
const toast = useToast()

const palette = ['#3b82f6', '#22c55e', '#8b5cf6', '#f59e0b', '#ef4444', '#d1d5db']

const selectedMonth = ref(new Date().toISOString().slice(0, 7))

const selectedYear = computed(() => Number(selectedMonth.value.split('-')[0]))
const selectedMonthNumber = computed(() => Number(selectedMonth.value.split('-')[1]))

const summary = computed(() => reportsStore.summary)

function isCurrentMonth(m) {
  return m.year === selectedYear.value && m.month === selectedMonthNumber.value
}

const maxMonthly = computed(() =>
  Math.max(1, ...(summary.value?.tripsByMonth.map((m) => m.count) ?? [1])))

const maxArea = computed(() =>
  Math.max(1, ...(summary.value?.requestsByArea.map((a) => a.count) ?? [1])))

const maxVehicleUsage = computed(() =>
  Math.max(1, ...(summary.value?.topVehicles.map((v) => v.tripsCompleted) ?? [1])))

const maxFuel = computed(() =>
  Math.max(1, ...(summary.value?.fuelByMonth.map((m) => m.gallons) ?? [1])))

const maxTrips = computed(() =>
  Math.max(1, ...(summary.value?.topDrivers.map((d) => d.tripsCompleted) ?? [1])))

const tripsTrend = computed(() => {
  const months = summary.value?.tripsByMonth ?? []
  if (months.length < 2) return { label: '', cls: 'gray' }

  const current = months[months.length - 1].count
  const previous = months[months.length - 2].count

  if (previous === 0) return { label: '', cls: 'gray' }

  const change = Math.round(((current - previous) / previous) * 100)
  if (change === 0) return { label: 'Sin cambios vs mes anterior', cls: 'gray' }

  return change > 0
    ? { label: `↑ ${change}% vs mes anterior`, cls: 'green' }
    : { label: `↓ ${Math.abs(change)}% vs mes anterior`, cls: 'gray' }
})

function loadSummary() {
  reportsStore.fetchSummary(selectedYear.value, selectedMonthNumber.value)
}

onMounted(loadSummary)
watch(selectedMonth, loadSummary)

async function handleExportExcel() {
  try {
    await reportsStore.downloadExcel(selectedYear.value, selectedMonthNumber.value)
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al exportar a Excel'))
  }
}

async function handleExportPdf() {
  try {
    await reportsStore.downloadPdf(selectedYear.value, selectedMonthNumber.value)
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al exportar a PDF'))
  }
}
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
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  outline: none;
}

.export-btns { display: flex; gap: 0.5rem; }

.btn-export {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.5rem 1rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
  transition: background 0.2s;
}

.btn-export:hover:not(:disabled) { background: var(--surface-hover); }
.btn-export:disabled { opacity: 0.6; cursor: not-allowed; }

.empty-state {
  font-size: 0.85rem;
  color: var(--text-3);
  padding: 1.5rem 0;
  text-align: center;
}

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kpi-card {
  background: var(--white);
  border-radius: 10px;
  padding: 1.1rem;
  border: 1px solid var(--border);
  box-shadow: var(--shadow-xs);
  transition: box-shadow .15s, transform .15s;
}

.kpi-card:hover {
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
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
  color: var(--text-3);
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

.kpi-icon.blue   { background: var(--blue-light); color: var(--blue-hover); }
.kpi-icon.green  { background: var(--mint-bg); color: var(--mint-dark); }
.kpi-icon.orange { background: var(--amber-bg); color: var(--amber); }
.kpi-icon.purple { background: var(--purple-bg); color: var(--purple); }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: var(--text);
  margin-bottom: 0.25rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.green { color: var(--mint-dark); }
.kpi-sub.gray  { color: var(--text-2); }

/* Charts grid */
.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
}

/* Card */
.card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.25rem;
  box-shadow: var(--shadow-xs);
}

.card-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
  margin-bottom: 1.25rem;
}

.section-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text);
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
  color: var(--text-3);
  margin-top: 0.4rem;
}

.bar-label.active { color: var(--blue-hover); font-weight: 600; }

/* Valores debajo */
.bar-values {
  display: flex;
  gap: 0.75rem;
  border-top: 1px solid var(--border);
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
  color: var(--text);
}

.bar-value.active { color: var(--blue-hover); }

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
  color: var(--text-2);
  text-align: right;
  flex-shrink: 0;
}

.hbar-track {
  flex: 1;
  height: 10px;
  background: var(--border);
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
  color: var(--text-2);
  font-weight: 600;
  text-align: right;
  flex-shrink: 0;
}

/* Tabla */
.table-wrapper {
  border-radius: 8px;
  border: 1px solid var(--border);
  overflow: hidden;
}

.table { width: 100%; border-collapse: collapse; }

.table th {
  text-align: left;
  font-size: 0.68rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border);
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.875rem;
  color: var(--text-2);
  border-bottom: 1px solid var(--surface-hover);
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: var(--surface-hover); }

.td-bold { font-weight: 600; color: var(--text); }
.td-gray { color: var(--text-2); }

/* Trips cell con mini barra */
.trips-cell {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.mini-bar-track {
  width: 80px;
  height: 6px;
  background: var(--border);
  border-radius: 999px;
  overflow: hidden;
}

.mini-bar-fill {
  height: 100%;
  background: var(--blue-hover);
  border-radius: 999px;
}
</style>
