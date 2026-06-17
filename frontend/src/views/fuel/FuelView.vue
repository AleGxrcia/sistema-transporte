<template>
  <div class="fuel">

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL GALONES — MAYO</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="13 17 18 12 13 7"/>
              <polyline points="6 17 11 12 6 7"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">842</div>
        <div class="kpi-sub green">↑ 6% vs abril</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">COSTO TOTAL — MAYO</span>
          <div class="kpi-icon purple">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="1" x2="12" y2="23"/>
              <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">RD$84k</div>
        <div class="kpi-sub gray">Promedio RD$99/gal</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">VEHÍCULOS REGISTRADOS</span>
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
        <div class="kpi-value">18</div>
        <div class="kpi-sub gray">De 24 en flota</div>
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
        <div class="kpi-value">ABC-123</div>
        <div class="kpi-sub gray">312 galones / mes</div>
      </div>
    </div>

    <!-- Grid principal -->
    <div class="main-grid">

      <!-- Consumo por vehículo -->
      <div class="card">
        <div class="card-header">
          <h3 class="card-title">Consumo por vehículo — Mayo 2026</h3>
        </div>
        <div class="bar-list">
          <div v-for="item in consumoVehiculos" :key="item.plate" class="bar-item">
            <span class="bar-label">{{ item.plate }}</span>
            <div class="bar-track">
              <div
                class="bar-fill"
                :style="{
                  width: (item.gallons / maxGallons * 100) + '%',
                  background: item.color
                }"
              ></div>
            </div>
            <span class="bar-value">{{ item.gallons }} gl</span>
          </div>
          <p class="bar-footer">
            + 13 vehículos más
            <span class="bar-total">Total: 842 gl — RD$ 84,158</span>
          </p>
        </div>
      </div>

      <!-- Formulario registro -->
      <div class="card">
        <div class="card-section-title">REGISTRAR CONSUMO</div>

        <div class="form-group">
          <label class="form-label">Vehículo</label>
          <select v-model="form.vehicle" class="form-input">
            <option value="">Seleccionar vehículo</option>
            <option value="ABC-123">ABC-123 — Toyota Hiace</option>
            <option value="DEF-456">DEF-456 — Honda CRV</option>
            <option value="GHI-789">GHI-789 — Toyota Hilux</option>
            <option value="JKL-012">JKL-012 — Nissan Frontier</option>
            <option value="MNO-345">MNO-345 — Hyundai H1</option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Fecha de carga</label>
            <input v-model="form.date" type="date" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">Galones cargados</label>
            <input v-model="form.gallons" type="number"
              class="form-input" placeholder="0.00"
              @input="calcTotal" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Precio por galón (RD$)</label>
            <input v-model="form.pricePerGallon" type="number"
              class="form-input" placeholder="99.50"
              @input="calcTotal" />
          </div>
          <div class="form-group">
            <label class="form-label">Kilometraje al cargar</label>
            <input v-model="form.kmAtLoad" type="number"
              class="form-input" placeholder="45,230" />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Costo total (calculado)</label>
          <div class="calculated-field">
            <span class="calculated-label">Se calcula automáticamente</span>
            <span class="calculated-value">RD$ {{ totalCost }}</span>
          </div>
        </div>

        <div class="warning-banner">
          ⚠️ El kilometraje ingresado debe ser
          <strong>mayor al último registrado</strong>
          para este vehículo (45,230 km).
        </div>

        <div class="form-actions">
          <button class="btn-cancel" @click="resetForm">Limpiar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : 'Guardar registro' }}
          </button>
        </div>
      </div>

    </div>

    <!-- Historial tabla -->
    <div class="card">
      <div class="card-header">
        <h3 class="card-title">Últimos registros de consumo</h3>
        <router-link to="/trips" class="card-link">Ver todos →</router-link>
      </div>
      <div class="table-wrapper">
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
              <th>ACCIONES</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in fuelHistory" :key="item.id">
              <td class="td-gray">{{ item.date }}</td>
              <td class="td-bold">{{ item.vehicle }}</td>
              <td>{{ item.gallons }} gl</td>
              <td class="td-gray">{{ item.pricePerGallon }}</td>
              <td class="td-bold">{{ item.totalCost }}</td>
              <td class="td-gray">{{ item.kmAtLoad }}</td>
              <td class="green">{{ item.kmDriven }}</td>
              <td class="green">{{ item.efficiency }}</td>
              <td>
                <div class="actions">
                  <button class="action-btn edit">
                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                      viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                      <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                    </svg>
                  </button>
                  <button class="action-btn delete">
                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13"
                      viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <polyline points="3 6 5 6 21 6"/>
                      <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/>
                      <path d="M10 11v6M14 11v6"/>
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'

const loading = ref(false)

const form = reactive({
  vehicle: 'ABC-123', date: '', gallons: '',
  pricePerGallon: '', kmAtLoad: ''
})

const totalCost = computed(() => {
  if (form.gallons && form.pricePerGallon) {
    return (parseFloat(form.gallons) * parseFloat(form.pricePerGallon)).toFixed(2)
  }
  return '0.00'
})

const consumoVehiculos = ref([
  { plate: 'ABC-123', gallons: 312, color: '#3b82f6' },
  { plate: 'DEF-456', gallons: 240, color: '#22c55e' },
  { plate: 'GHI-789', gallons: 192, color: '#f59e0b' },
  { plate: 'JKL-012', gallons: 145, color: '#8b5cf6' },
  { plate: 'MNO-345', gallons: 98,  color: '#d1d5db' },
])

const maxGallons = computed(() =>
  Math.max(...consumoVehiculos.value.map(v => v.gallons))
)

const fuelHistory = ref([
  { id: 1, date: '27/05/26', vehicle: 'ABC-123', gallons: '45.5', pricePerGallon: 'RD$ 99.50', totalCost: 'RD$ 4,527', kmAtLoad: '45,230 km', kmDriven: '+620 km', efficiency: '13.6 km/gl' },
  { id: 2, date: '26/05/26', vehicle: 'DEF-456', gallons: '38.0', pricePerGallon: 'RD$ 99.50', totalCost: 'RD$ 3,781', kmAtLoad: '62,100 km', kmDriven: '+540 km', efficiency: '14.2 km/gl' },
  { id: 3, date: '22/05/26', vehicle: 'JKL-012', gallons: '30.0', pricePerGallon: 'RD$ 98.20', totalCost: 'RD$ 2,946', kmAtLoad: '12,780 km', kmDriven: '+450 km', efficiency: '15.0 km/gl' },
  { id: 4, date: '20/05/26', vehicle: 'GHI-789', gallons: '42.0', pricePerGallon: 'RD$ 97.50', totalCost: 'RD$ 4,095', kmAtLoad: '88,450 km', kmDriven: '+480 km', efficiency: '11.4 km/gl' },
  { id: 5, date: '18/05/26', vehicle: 'MNO-345', gallons: '35.5', pricePerGallon: 'RD$ 99.50', totalCost: 'RD$ 3,532', kmAtLoad: '35,670 km', kmDriven: '+510 km', efficiency: '14.4 km/gl' },
])

function calcTotal() {}

function resetForm() {
  Object.assign(form, {
    vehicle: '', date: '', gallons: '', pricePerGallon: '', kmAtLoad: ''
  })
}

async function handleSave() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
    resetForm()
  } finally {
    loading.value = false
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

.kpi-icon.orange { background: #fff7ed; color: #d97706; }
.kpi-icon.purple { background: #f5f3ff; color: #7c3aed; }
.kpi-icon.blue   { background: #eff6ff; color: #2563eb; }
.kpi-icon.teal   { background: #f0fdfa; color: #0d9488; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  margin-bottom: 0.25rem;
}

.kpi-sub { font-size: 0.75rem; }
.kpi-sub.green { color: #16a34a; }
.kpi-sub.gray  { color: #6b7280; }

/* Main grid */
.main-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  align-items: stretch;
}

/* Card */
.card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

.card-link {
  font-size: 0.8rem;
  color: #2563eb;
  text-decoration: none;
}

.card-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
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
  width: 60px;
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
  flex-shrink: 0;
}

.bar-track {
  flex: 1;
  height: 10px;
  background: #f3f4f6;
  border-radius: 999px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 999px;
  transition: width 0.4s ease;
}

.bar-value {
  width: 50px;
  font-size: 0.78rem;
  color: #6b7280;
  text-align: right;
  flex-shrink: 0;
}

.bar-footer {
  display: flex;
  justify-content: space-between;
  font-size: 0.78rem;
  color: #9ca3af;
  padding-top: 0.25rem;
  border-top: 1px solid #f3f4f6;
}

.bar-total { color: #374151; font-weight: 500; }

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
  color: #374151;
}

.form-input {
  padding: 0.6rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  outline: none;
  transition: border-color 0.2s;
  background: #fff;
}

.form-input:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.calculated-field {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.6rem 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #f9fafb;
}

.calculated-label { font-size: 0.82rem; color: #9ca3af; }
.calculated-value { font-size: 0.875rem; font-weight: 600; color: #2563eb; }

.warning-banner {
  background: #fffbeb;
  border: 1px solid #fde68a;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  font-size: 0.82rem;
  color: #92400e;
  line-height: 1.5;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn-cancel {
  padding: 0.6rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
}

.btn-cancel:hover { background: #f9fafb; }

.btn-submit {
  padding: 0.6rem 1rem;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: #1d4ed8; }
.btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }

/* Tabla */
.table-wrapper {
  overflow: hidden;
  border-radius: 8px;
  border: 1px solid #f3f4f6;
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

.td-bold { font-weight: 600; color: #111827; }
.td-gray { color: #9ca3af; }
.green   { color: #16a34a; font-weight: 500; }

/* Acciones */
.actions { display: flex; gap: 0.4rem; }

.action-btn {
  width: 26px;
  height: 26px;
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

.action-btn.edit:hover   { background: #eff6ff; border-color: #2563eb; color: #2563eb; }
.action-btn.delete:hover { background: #fef2f2; border-color: #dc2626; color: #dc2626; }
</style>