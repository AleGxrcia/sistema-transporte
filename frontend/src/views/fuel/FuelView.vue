<template>
  <div class="fuel">

    <div class="page-header">
      <input type="month" v-model="selectedMonth" class="month-input" />
    </div>

    <div v-if="fuelStore.isLoadingSummary" class="empty-state">Cargando…</div>

    <template v-else-if="summary">
      <!-- KPIs -->
      <div class="kpi-grid">
        <div class="kpi-card">
          <div class="kpi-top">
            <span class="kpi-label">TOTAL GALONES</span>
            <div class="kpi-icon orange">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="13 17 18 12 13 7"/>
                <polyline points="6 17 11 12 6 7"/>
              </svg>
            </div>
          </div>
          <div class="kpi-value">{{ formatNumber(summary.totalGallons) }}</div>
          <div class="kpi-sub gray">galones cargados</div>
        </div>

        <div class="kpi-card">
          <div class="kpi-top">
            <span class="kpi-label">COSTO TOTAL</span>
            <div class="kpi-icon purple">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="12" y1="1" x2="12" y2="23"/>
                <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
              </svg>
            </div>
          </div>
          <div class="kpi-value">{{ formatCurrency(summary.totalCost) }}</div>
          <div class="kpi-sub gray">Promedio {{ formatCurrency(summary.averagePricePerGallon) }}/gal</div>
        </div>

        <div class="kpi-card">
          <div class="kpi-top">
            <span class="kpi-label">VEHÍCULOS CON CARGA</span>
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
          <div class="kpi-value">{{ summary.vehiclesWithRecords }}</div>
          <div class="kpi-sub gray">De {{ summary.totalVehicles }} en flota</div>
        </div>

        <div class="kpi-card">
          <div class="kpi-top">
            <span class="kpi-label">MAYOR CONSUMIDOR</span>
            <div class="kpi-icon teal">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
              </svg>
            </div>
          </div>
          <div class="kpi-value">{{ summary.topConsumer?.licensePlate ?? '—' }}</div>
          <div class="kpi-sub gray">
            {{ summary.topConsumer ? `${formatNumber(summary.topConsumer.gallons)} galones / mes` : 'Sin registros este mes' }}
          </div>
        </div>
      </div>

      <!-- Grid principal -->
      <div class="main-grid">

        <!-- Consumo por vehículo -->
        <div class="card">
          <div class="card-header">
            <h3 class="card-title">Consumo por vehículo — {{ monthLabel }}</h3>
          </div>
          <div v-if="!summary.consumptionByVehicle.length" class="empty-state">Sin registros este mes.</div>
          <div v-else class="bar-list">
            <div v-for="(item, i) in summary.consumptionByVehicle.slice(0, 8)" :key="item.vehicleId" class="bar-item">
              <span class="bar-label">{{ item.licensePlate }}</span>
              <div class="bar-track">
                <div
                  class="bar-fill"
                  :style="{
                    width: (item.gallons / maxGallons * 100) + '%',
                    background: palette[i % palette.length]
                  }"
                ></div>
              </div>
              <span class="bar-value">{{ formatNumber(item.gallons) }} gl</span>
            </div>
            <p class="bar-footer">
              <span v-if="summary.consumptionByVehicle.length > 8">
                + {{ summary.consumptionByVehicle.length - 8 }} vehículos más
              </span>
              <span v-else></span>
              <span class="bar-total">Total: {{ formatNumber(summary.totalGallons) }} gl — {{ formatCurrency(summary.totalCost) }}</span>
            </p>
          </div>
        </div>

        <!-- Formulario registro -->
        <div v-if="can('create', 'fuel')" class="card">
          <div class="card-section-title">REGISTRAR CONSUMO</div>

          <div class="form-group">
            <label class="form-label">Vehículo</label>
            <select v-model="form.vehicleId" class="form-input">
              <option value="">Seleccionar vehículo</option>
              <option v-for="v in vehicles" :key="v.id" :value="v.id">
                {{ v.licensePlate }} — {{ v.brand }} {{ v.model }}
              </option>
            </select>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Fecha de carga</label>
              <input v-model="form.date" type="date" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">Galones cargados</label>
              <input v-model="form.gallons" type="number" class="form-input" placeholder="0.00" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Precio por galón (RD$)</label>
              <input v-model="form.pricePerGallon" type="number" class="form-input" placeholder="99.50" />
            </div>
            <div class="form-group">
              <label class="form-label">Kilometraje al cargar</label>
              <input v-model="form.mileageAtRefuel" type="number" class="form-input" placeholder="45230" />
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Costo total (calculado)</label>
            <div class="calculated-field">
              <span class="calculated-label">Se calcula automáticamente</span>
              <span class="calculated-value">{{ formatCurrency(totalCostPreview) }}</span>
            </div>
          </div>

          <div v-if="selectedVehicle" class="warning-banner">
            ⚠️ El kilometraje ingresado debe ser
            <strong>mayor al último registrado</strong>
            para este vehículo ({{ formatKilometers(selectedVehicle.currentMileage) }}).
          </div>

          <div class="form-actions">
            <button class="btn-cancel" @click="resetForm">Limpiar</button>
            <button class="btn-submit" @click="handleSave" :disabled="saving">
              {{ saving ? 'Guardando...' : 'Guardar registro' }}
            </button>
          </div>
        </div>

      </div>

      <!-- Historial tabla -->
      <div class="card">
        <div class="card-header">
          <h3 class="card-title">Registros de consumo — {{ monthLabel }}</h3>
        </div>
        <div v-if="fuelStore.isLoadingHistory" class="empty-state">Cargando…</div>
        <div v-else-if="!history.length" class="empty-state">No hay registros de combustible este mes.</div>
        <div v-else class="table-wrapper">
          <table class="table">
            <thead>
              <tr>
                <th>FECHA</th>
                <th>VEHÍCULO</th>
                <th>GALONES</th>
                <th>PRECIO/GAL</th>
                <th>COSTO TOTAL</th>
                <th>KM AL CARGAR</th>
                <th>KM RECORRIDOS</th>
                <th>RENDIMIENTO</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in history" :key="item.id">
                <td class="td-gray">{{ formatDate(item.recordDate) }}</td>
                <td class="td-bold">{{ item.vehiclePlate }} — {{ item.vehicleLabel }}</td>
                <td>{{ formatNumber(item.gallons) }} gl</td>
                <td class="td-gray">{{ formatCurrency(item.pricePerGallon) }}</td>
                <td class="td-bold">{{ formatCurrency(item.totalCost) }}</td>
                <td class="td-gray">{{ formatKilometers(item.mileageAtRefuel) }}</td>
                <td class="green">{{ item.kmDriven != null ? `+${formatKilometers(item.kmDriven)}` : '—' }}</td>
                <td class="green">{{ item.efficiencyKmPerGallon != null ? `${item.efficiencyKmPerGallon} km/gl` : '—' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-else-if="fuelStore.summaryError" class="empty-state">{{ fuelStore.summaryError }}</div>

  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useFuelStore } from '@/stores/fuel.store'
import { useVehicleStore } from '@/stores/vehicles.store'
import { useAuth } from '@/composables/useAuth'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { formatCurrency, formatDate, formatKilometers, formatNumber } from '@/utils/formatters'

const fuelStore = useFuelStore()
const vehicleStore = useVehicleStore()
const { can } = useAuth()
const toast = useToast()

const palette = ['#3b82f6', '#22c55e', '#f59e0b', '#8b5cf6', '#ef4444', '#06b6d4', '#d1d5db', '#ec4899']

const selectedMonth = ref(new Date().toISOString().slice(0, 7))
const selectedYear = computed(() => Number(selectedMonth.value.split('-')[0]))
const selectedMonthNumber = computed(() => Number(selectedMonth.value.split('-')[1]))
const monthLabel = computed(() => selectedMonth.value)

const summary = computed(() => fuelStore.summary)
const history = computed(() => fuelStore.history)
const vehicles = computed(() => vehicleStore.vehicles)

const maxGallons = computed(() =>
  Math.max(1, ...(summary.value?.consumptionByVehicle.map((v) => v.gallons) ?? [1])))

function loadData() {
  fuelStore.refreshAll(selectedYear.value, selectedMonthNumber.value)
}

onMounted(() => {
  vehicleStore.fetchAll()
  loadData()
})

watch(selectedMonth, loadData)

const saving = ref(false)

const form = reactive({
  vehicleId: '', date: '', gallons: '', pricePerGallon: '', mileageAtRefuel: '',
})

const selectedVehicle = computed(() =>
  vehicles.value.find((v) => v.id === form.vehicleId) ?? null)

const totalCostPreview = computed(() => {
  const gallons = parseFloat(form.gallons)
  const price = parseFloat(form.pricePerGallon)
  return gallons > 0 && price > 0 ? gallons * price : 0
})

function resetForm() {
  Object.assign(form, { vehicleId: '', date: '', gallons: '', pricePerGallon: '', mileageAtRefuel: '' })
}

async function handleSave() {
  if (!form.vehicleId || !form.date || !form.gallons || !form.pricePerGallon || !form.mileageAtRefuel) {
    toast.error('Datos incompletos', 'Completa todos los campos requeridos.')
    return
  }

  if (selectedVehicle.value && Number(form.mileageAtRefuel) <= selectedVehicle.value.currentMileage) {
    toast.error('Kilometraje inválido', `Debe ser mayor a ${formatKilometers(selectedVehicle.value.currentMileage)}.`)
    return
  }

  saving.value = true
  try {
    await fuelStore.registerFuel(form.vehicleId, {
      recordDate: form.date,
      gallons: Number(form.gallons),
      pricePerGallon: Number(form.pricePerGallon),
      mileageAtRefuel: Number(form.mileageAtRefuel),
      notes: null,
    })
    toast.success('Combustible registrado', 'El consumo fue registrado correctamente.')
    resetForm()
    await Promise.all([vehicleStore.fetchAll(), loadData()])
  } catch (err) {
    toast.error('Error', getErrorMessage(err, 'Error al registrar el consumo de combustible'))
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.fuel {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

.page-header {
  display: flex;
  justify-content: flex-end;
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

.kpi-icon.orange { background: var(--amber-bg); color: var(--amber); }
.kpi-icon.purple { background: var(--purple-bg); color: var(--purple); }
.kpi-icon.blue   { background: var(--blue-light); color: var(--blue-hover); }
.kpi-icon.teal   { background: var(--sky-bg); color: var(--sky); }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: var(--text);
  margin-bottom: 0.25rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.gray  { color: var(--text-3); }

/* Main grid */
.main-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  align-items: stretch;
}

/* Card */
.card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.25rem;
  box-shadow: var(--shadow-xs);
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
  transition: box-shadow .15s, transform .15s;
}

.card:hover {
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text);
}

.card-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
}

/* Barras horizontales */
.bar-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.bar-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.bar-label {
  width: 70px;
  font-size: 0.82rem;
  font-weight: 500;
  color: var(--text-2);
  flex-shrink: 0;
}

.bar-track {
  flex: 1;
  height: 10px;
  background: var(--border);
  border-radius: 999px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 999px;
  transition: width 0.4s ease;
}

.bar-value {
  width: 60px;
  font-size: 0.78rem;
  color: var(--text-3);
  text-align: right;
  flex-shrink: 0;
}

.bar-footer {
  display: flex;
  justify-content: space-between;
  font-size: 0.78rem;
  color: var(--text-3);
  padding-top: 0.25rem;
  border-top: 1px solid var(--border);
}

.bar-total { color: var(--text-2); font-weight: 500; }

/* Formulario */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: var(--text-2);
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  outline: none;
  transition: border-color 0.2s;
  background: var(--white);
}

.form-input:focus {
  border-color: var(--blue-hover);
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.calculated-field {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.6rem 0.75rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--surface-hover);
}

.calculated-label { font-size: 0.82rem; color: var(--text-3); }
.calculated-value { font-size: 0.875rem; font-weight: 600; color: var(--blue-hover); }

.warning-banner {
  background: var(--amber-bg);
  border: 1px solid var(--amber-border);
  border-radius: 8px;
  padding: 0.75rem 1rem;
  font-size: 0.82rem;
  color: var(--amber-text);
  line-height: 1.5;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn-cancel {
  padding: 0.6rem 1rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--white);
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
}

.btn-cancel:hover { background: var(--surface-hover); }

.btn-submit {
  padding: 0.6rem 1rem;
  background: var(--blue-hover);
  color: var(--white);
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: var(--blue-hover); }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }

/* Tabla */
.table-wrapper {
  overflow: hidden;
  border-radius: 8px;
  border: 1px solid var(--border);
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  text-align: left;
  font-size: 0.68rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border);
  background: var(--white);
}

.table td {
  padding: 0.875rem 1rem;
  font-size: 0.85rem;
  color: var(--text-2);
  border-bottom: 1px solid var(--surface-hover);
}

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr:hover { background: var(--surface-hover); }

.td-bold { font-weight: 600; color: var(--text); }
.td-gray { color: var(--text-3); }
.green   { color: var(--mint-dark); font-weight: 500; }
</style>
