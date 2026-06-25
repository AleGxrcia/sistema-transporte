<template>
  <div class="fuel">

    <div class="page-header">
      <h1>Control de combustible</h1>
      <input type="month" v-model="selectedMonth" class="month-input" />
    </div>

    <div v-if="fuelStore.isLoadingSummary" class="loading-placeholder">Cargando…</div>

    <template v-else-if="summary">
      <!-- KPIs -->
      <div class="kpi-grid" style="grid-template-columns:repeat(4,1fr)">
        <div class="kpi">
          <div class="kpi-label">Total galones</div>
          <div class="kpi-val">{{ formatNumber(summary.totalGallons) }}</div>
          <div class="kpi-sub">galones cargados</div>
          <div class="kpi-icon amber"><Fuel :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Costo total</div>
          <div class="kpi-val">{{ formatCurrency(summary.totalCost) }}</div>
          <div class="kpi-sub">Promedio {{ formatCurrency(summary.averagePricePerGallon) }}/gal</div>
          <div class="kpi-icon purple"><DollarSign :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Vehículos con carga</div>
          <div class="kpi-val">{{ summary.vehiclesWithRecords }}</div>
          <div class="kpi-sub">De {{ summary.totalVehicles }} en flota</div>
          <div class="kpi-icon blue"><Truck :size="16" /></div>
        </div>

        <div class="kpi">
          <div class="kpi-label">Mayor consumidor</div>
          <div class="kpi-val">{{ summary.topConsumer?.licensePlate ?? '—' }}</div>
          <div class="kpi-sub">
            {{ summary.topConsumer ? `${formatNumber(summary.topConsumer.gallons)} galones / mes` : 'Sin registros este mes' }}
          </div>
          <div class="kpi-icon sky"><TrendingUp :size="16" /></div>
        </div>
      </div>

      <!-- Grid principal -->
      <div class="grid2">

        <!-- Consumo por vehículo -->
        <div class="card">
          <div class="card-header">
            <span class="card-title">Consumo por vehículo — {{ monthLabel }}</span>
          </div>
          <div v-if="!summary.consumptionByVehicle.length" class="empty-card">Sin registros este mes.</div>
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
          <div class="card-title">Registrar consumo</div>

          <div class="fuel-form">
            <div class="form-group">
              <label>Vehículo</label>
              <select v-model="form.vehicleId">
                <option value="">Seleccionar vehículo</option>
                <option v-for="v in vehicles" :key="v.id" :value="v.id">
                  {{ v.licensePlate }} — {{ v.brand }} {{ v.model }}
                </option>
              </select>
            </div>

            <div class="row2">
              <div class="form-group">
                <label>Fecha de carga</label>
                <input v-model="form.date" type="date" />
              </div>
              <div class="form-group">
                <label>Galones cargados</label>
                <input v-model="form.gallons" type="number" placeholder="0.00" />
              </div>
            </div>

            <div class="row2">
              <div class="form-group">
                <label>Precio por galón (RD$)</label>
                <input v-model="form.pricePerGallon" type="number" placeholder="99.50" />
              </div>
              <div class="form-group">
                <label>Kilometraje al cargar</label>
                <input v-model="form.mileageAtRefuel" type="number" placeholder="45230" />
              </div>
            </div>

            <div class="form-group">
              <label>Costo total (calculado)</label>
              <div class="calculated-field">
                <span class="calculated-label">Se calcula automáticamente</span>
                <span class="calculated-value">{{ formatCurrency(totalCostPreview) }}</span>
              </div>
            </div>

            <div v-if="selectedVehicle" class="alert amber" style="margin-bottom:0">
              El kilometraje ingresado debe ser <strong>mayor al último registrado</strong>
              para este vehículo ({{ formatKilometers(selectedVehicle.currentMileage) }}).
            </div>

            <div class="form-actions">
              <button class="btn" @click="resetForm">Limpiar</button>
              <button class="btn primary" @click="handleSave" :disabled="saving">
                {{ saving ? 'Guardando...' : 'Guardar registro' }}
              </button>
            </div>
          </div>
        </div>

      </div>

      <!-- Historial tabla -->
      <div>
        <div class="card-title" style="margin-bottom:12px">Registros de consumo — {{ monthLabel }}</div>
        <div v-if="fuelStore.isLoadingHistory" class="loading-placeholder">Cargando…</div>
        <div v-else-if="!history.length" class="empty-card">No hay registros de combustible este mes.</div>
        <div v-else class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Vehículo</th>
                <th>Galones</th>
                <th>Precio/gal</th>
                <th>Costo total</th>
                <th>Km al cargar</th>
                <th>Km recorridos</th>
                <th>Rendimiento</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in history" :key="item.id">
                <td class="muted">{{ formatDate(item.recordDate) }}</td>
                <td class="plate-cell">{{ item.vehiclePlate }} — {{ item.vehicleLabel }}</td>
                <td>{{ formatNumber(item.gallons) }} gl</td>
                <td class="muted">{{ formatCurrency(item.pricePerGallon) }}</td>
                <td style="font-weight:600">{{ formatCurrency(item.totalCost) }}</td>
                <td class="muted">{{ formatKilometers(item.mileageAtRefuel) }}</td>
                <td class="pos">{{ item.kmDriven != null ? `+${formatKilometers(item.kmDriven)}` : '—' }}</td>
                <td class="pos">{{ item.efficiencyKmPerGallon != null ? `${item.efficiencyKmPerGallon} km/gl` : '—' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </template>

    <div v-else-if="fuelStore.summaryError" class="alert red">{{ fuelStore.summaryError }}</div>

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
import { Fuel, DollarSign, Truck, TrendingUp } from '@lucide/vue'

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
  gap: 20px;
}

.month-input {
  width: auto;
  height: 34px;
}

/* Formulario */
.fuel-form {
  display: flex;
  flex-direction: column;
  gap: 14px;
}
.row2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
}

.calculated-field {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 11px;
  border: 1px solid var(--border);
  border-radius: 7px;
  background: var(--surface-hover);
}
.calculated-label { font-size: 13px; color: var(--text-3); }
.calculated-value { font-size: 14px; font-weight: 600; color: var(--blue); }

/* Barras horizontales */
.bar-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.bar-item {
  display: flex;
  align-items: center;
  gap: 12px;
}
.bar-label {
  width: 70px;
  font-size: 13.5px;
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
  font-size: 13px;
  color: var(--text-3);
  text-align: right;
  flex-shrink: 0;
}
.bar-footer {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  color: var(--text-3);
  padding-top: 8px;
  border-top: 1px solid var(--border);
}
.bar-total { color: var(--text-2); font-weight: 500; }

/* Celdas de rendimiento positivo */
.pos { color: var(--mint-dark); font-weight: 500; }
</style>
