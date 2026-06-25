<template>
  <div class="reports">

    <!-- Header con filtros y exportación -->
    <div class="page-header">
      <h1>Reportes y estadísticas</h1>
      <div class="header-controls">
      <input type="month" v-model="selectedMonth" class="month-input" />
      <div class="export-btns">
        <button class="btn" :disabled="reportsStore.isExporting" @click="handleExportExcel">
          <Download :size="14" /> Excel
        </button>
        <button class="btn" :disabled="reportsStore.isExporting" @click="handleExportPdf">
          <Download :size="14" /> PDF
        </button>
      </div>
      </div>
    </div>

    <div v-if="reportsStore.isLoading" class="loading-placeholder">Cargando reporte…</div>

    <template v-else-if="summary">
      <!-- KPIs -->
      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Viajes completados</div>
          <div class="kpi-val">{{ summary.tripsCompleted }}</div>
          <div class="kpi-sub" :style="tripsTrend.cls === 'green' ? 'color:var(--mint-dark)' : ''">{{ tripsTrend.label }}</div>
          <div class="kpi-icon blue"><Truck :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Tasa de cumplimiento</div>
          <div class="kpi-val">{{ summary.completionRatePercent }}%</div>
          <div class="kpi-sub">{{ summary.cancelledCount }} cancelados</div>
          <div class="kpi-icon green"><CheckCircle2 :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Costo combustible</div>
          <div class="kpi-val">{{ formatCurrency(summary.fuelCost) }}</div>
          <div class="kpi-sub">{{ formatNumber(summary.fuelGallons) }} galones</div>
          <div class="kpi-icon amber"><Fuel :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Costo mantenimiento</div>
          <div class="kpi-val">{{ formatCurrency(summary.maintenanceCost) }}</div>
          <div class="kpi-sub">{{ summary.maintenanceServicesCount }} servicios</div>
          <div class="kpi-icon purple"><Wrench :size="16" /></div>
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
          <div v-if="!summary.requestsByArea.length" class="empty-card">Sin solicitudes este mes.</div>
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
          <div v-if="!summary.topVehicles.length" class="empty-card">Sin viajes completados este mes.</div>
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
        <div v-if="!summary.topDrivers.length" class="empty-card">Sin viajes asignados este mes.</div>
        <div v-else class="table-wrap" style="margin-bottom:0">
          <table>
            <thead>
              <tr>
                <th>Conductor</th>
                <th>Viajes completados</th>
                <th>Cancelados</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="driver in summary.topDrivers" :key="driver.driverId">
                <td style="font-weight:600">{{ driver.driverName }}</td>
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
                <td class="muted">{{ driver.tripsCancelled }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-else-if="reportsStore.error" class="alert red">{{ reportsStore.error }}</div>

  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useReportsStore } from '@/stores/reports.store'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { formatCurrency, formatNumber } from '@/utils/formatters'
import { Truck, CheckCircle2, Fuel, Wrench, Download } from '@lucide/vue'

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
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}
.page-header h1 {
  font-size: 21px;
  font-weight: 700;
  letter-spacing: -0.01em;
  color: var(--text);
}
.header-controls {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.month-input {
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  outline: none;
}

.export-btns { display: flex; gap: 0.5rem; }

/* Charts grid */
.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
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
  font-size: 0.75rem;
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
  font-size: 0.87rem;
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
  font-size: 0.87rem;
  color: var(--text-2);
  font-weight: 600;
  text-align: right;
  flex-shrink: 0;
}

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
