<template>
  <div class="vehicles">

    <!-- Header -->
    <div class="page-header">
      <h2 class="page-title">Gestión de vehículos</h2>
      <button class="btn-primary" @click="showModal = true">
        + Nuevo vehículo
      </button>
    </div>

    <!-- KPIs -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">TOTAL</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="1" y="3" width="15" height="13" rx="2"/>
              <path d="M16 8h4l3 3v5h-7V8z"/>
              <circle cx="5.5" cy="18.5" r="2.5"/>
              <circle cx="18.5" cy="18.5" r="2.5"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">24</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">DISPONIBLES</span>
          <div class="kpi-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">16</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">MANTENIMIENTO</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">4</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">FUERA DE SERVICIO</span>
          <div class="kpi-icon red">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">2</div>
      </div>
    </div>

    <!-- Filtros -->
    <div class="filters">
      <div class="search-box">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
          viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/>
          <line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="search" type="text"
          placeholder="Buscar..." class="search-input" />
      </div>
      <select v-model="statusFilter" class="filter-select">
        <option value="">Todos los estados</option>
        <option value="Disponible">Disponible</option>
        <option value="En viaje">En viaje</option>
        <option value="En mantenimiento">En mantenimiento</option>
        <option value="Fuera de servicio">Fuera de servicio</option>
      </select>
      <select v-model="typeFilter" class="filter-select">
        <option value="">Todos los tipos</option>
        <option value="Van">Van</option>
        <option value="SUV">SUV</option>
        <option value="Pickup">Pickup</option>
        <option value="Sedan">Sedan</option>
        <option value="Minibus">Minibus</option>
      </select>
    </div>

    <!-- Tabla -->
    <div class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>MATRÍCULA</th>
            <th>MARCA / MODELO</th>
            <th>TIPO</th>
            <th>AÑO</th>
            <th>CAPACIDAD</th>
            <th>KILOMETRAJE</th>
            <th>ESTADO</th>
            <th>ACCIONES</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="vehicle in filteredVehicles" :key="vehicle.id">
            <td class="td-bold">{{ vehicle.plate }}</td>
            <td>{{ vehicle.brand }} {{ vehicle.model }}</td>
            <td><span class="type-badge">{{ vehicle.type }}</span></td>
            <td class="td-gray">{{ vehicle.year }}</td>
            <td class="td-gray">{{ vehicle.capacity }} pas.</td>
            <td class="td-gray">{{ vehicle.mileage.toLocaleString() }} km</td>
            <td>
              <span class="badge" :class="statusClass(vehicle.status)">
                • {{ vehicle.status }}
              </span>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn" title="Ver detalle"
                  @click="$router.push('/vehicles/' + vehicle.id)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                    <circle cx="12" cy="12" r="3"/>
                  </svg>
                </button>
                <button
                  v-if="vehicle.status !== 'En viaje' && vehicle.status !== 'En mantenimiento'"
                  class="action-btn" title="Editar"
                  @click="openEdit(vehicle)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                    <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                  </svg>
                </button>
                <button
                  v-if="vehicle.status === 'Disponible'"
                  class="action-btn danger" title="Eliminar">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="3 6 5 6 21 6"/>
                    <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/>
                    <path d="M10 11v6M14 11v6"/>
                    <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/>
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
      <span class="pagination-info">
        Mostrando 1-{{ filteredVehicles.length }} de {{ vehicles.length }} vehículos
      </span>
      <div class="pagination-btns">
        <button class="page-btn" disabled>← Anterior</button>
        <button class="page-btn">Siguiente →</button>
      </div>
    </div>

    <!-- Modal nuevo / editar vehículo -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">{{ editingVehicle ? 'Editar vehículo' : 'Nuevo vehículo' }}</h3>
          <button class="modal-close" @click="closeModal">✕</button>
        </div>

        <div class="modal-body">
          <div class="form-grid">

            <div class="form-group">
              <label class="form-label">Marca</label>
              <input v-model="form.brand" type="text"
                class="form-input" placeholder="ej: Toyota" />
            </div>

            <div class="form-group">
              <label class="form-label">Modelo</label>
              <input v-model="form.model" type="text"
                class="form-input" placeholder="ej: Hiace" />
            </div>

            <div class="form-group">
              <label class="form-label">Año</label>
              <input v-model="form.year" type="number"
                class="form-input" placeholder="ej: 2022" />
            </div>

            <div class="form-group">
              <label class="form-label">Matrícula</label>
              <input v-model="form.plate" type="text"
                class="form-input" placeholder="ej: ABC-123" />
            </div>

            <div class="form-group">
              <label class="form-label">Color</label>
              <input v-model="form.color" type="text"
                class="form-input" placeholder="ej: Blanco" />
            </div>

            <div class="form-group">
              <label class="form-label">Tipo</label>
              <select v-model="form.type" class="form-input">
                <option value="">Seleccionar tipo</option>
                <option value="Van">Van</option>
                <option value="SUV">SUV</option>
                <option value="Pickup">Pickup</option>
                <option value="Sedan">Sedan</option>
                <option value="Minibus">Minibus</option>
              </select>
            </div>

            <div class="form-group">
              <label class="form-label">Capacidad (pasajeros)</label>
              <input v-model="form.capacity" type="number"
                class="form-input" placeholder="ej: 12" />
            </div>

            <div class="form-group">
              <label class="form-label">Kilometraje actual</label>
              <input v-model="form.mileage" type="number"
                class="form-input" placeholder="ej: 45000" />
            </div>

            <div class="form-group full-width">
              <label class="form-label">Fecha de último mantenimiento</label>
              <input v-model="form.lastMaintenance" type="date" class="form-input" />
            </div>

          </div>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="closeModal">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : editingVehicle ? 'Guardar cambios' : 'Registrar vehículo' }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, reactive } from 'vue'

const search = ref('')
const statusFilter = ref('')
const typeFilter = ref('')
const showModal = ref(false)
const editingVehicle = ref(null)
const loading = ref(false)

const form = reactive({
  brand: '', model: '', year: '', plate: '',
  color: '', type: '', capacity: '', mileage: '', lastMaintenance: ''
})

const vehicles = ref([
  { id: 1, plate: 'ABC-123', brand: 'Toyota',  model: 'Hiace',    type: 'Van',    year: 2022, capacity: 12, mileage: 45230, status: 'Disponible'       },
  { id: 2, plate: 'DEF-456', brand: 'Honda',   model: 'CRV',      type: 'SUV',    year: 2021, capacity: 5,  mileage: 62100, status: 'En viaje'         },
  { id: 3, plate: 'GHI-789', brand: 'Toyota',  model: 'Hilux',    type: 'Pickup', year: 2020, capacity: 5,  mileage: 88450, status: 'En mantenimiento' },
  { id: 4, plate: 'JKL-012', brand: 'Nissan',  model: 'Frontier', type: 'Pickup', year: 2023, capacity: 5,  mileage: 12780, status: 'Disponible'       },
  { id: 5, plate: 'MNO-345', brand: 'Hyundai', model: 'H1',       type: 'Van',    year: 2022, capacity: 9,  mileage: 35670, status: 'Fuera de servicio'},
])

const filteredVehicles = computed(() => {
  let result = vehicles.value
  if (search.value) {
    const q = search.value.toLowerCase()
    result = result.filter(v =>
      v.plate.toLowerCase().includes(q) ||
      v.brand.toLowerCase().includes(q) ||
      v.model.toLowerCase().includes(q)
    )
  }
  if (statusFilter.value) result = result.filter(v => v.status === statusFilter.value)
  if (typeFilter.value)   result = result.filter(v => v.type === typeFilter.value)
  return result
})

function statusClass(status) {
  const map = {
    'Disponible':        'disponible',
    'En viaje':          'en-viaje',
    'En mantenimiento':  'mantenimiento',
    'Fuera de servicio': 'fuera-servicio',
  }
  return map[status] || ''
}

function openEdit(vehicle) {
  editingVehicle.value = vehicle
  Object.assign(form, {
    brand: vehicle.brand, model: vehicle.model, year: vehicle.year,
    plate: vehicle.plate, color: vehicle.color || '', type: vehicle.type,
    capacity: vehicle.capacity, mileage: vehicle.mileage, lastMaintenance: ''
  })
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingVehicle.value = null
  Object.assign(form, {
    brand: '', model: '', year: '', plate: '',
    color: '', type: '', capacity: '', mileage: '', lastMaintenance: ''
  })
}

async function handleSave() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 800))
    closeModal()
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.vehicles {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
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
  transition: background 0.2s;
}

.btn-primary:hover { background: #1d4ed8; }

/* KPIs */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kpi-card {
  background: #fff;
  border-radius: 10px;
  padding: 1.25rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.kpi-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.75rem;
}

.kpi-label {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.kpi-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.kpi-icon.blue   { background: #eff6ff; color: #2563eb; }
.kpi-icon.green  { background: #f0fdf4; color: #16a34a; }
.kpi-icon.orange { background: #fff7ed; color: #d97706; }
.kpi-icon.red    { background: #fef2f2; color: #dc2626; }

.kpi-value {
  font-size: 2rem;
  font-weight: 700;
  color: #111827;
}

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
  width: 160px;
}

.filter-select {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  background: #fff;
  outline: none;
  cursor: pointer;
  min-width: 160px;
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
.td-gray { color: #6b7280; }

.type-badge {
  background: #f3f4f6;
  color: #374151;
  font-size: 0.75rem;
  padding: 2px 8px;
  border-radius: 4px;
  font-weight: 500;
}

/* Badges estado */
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

.badge.disponible     { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.en-viaje       { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.mantenimiento  { background: #fff7ed; color: #d97706; border-color: #fed7aa; }
.badge.fuera-servicio { background: #fef2f2; color: #dc2626; border-color: #fecaca; }

/* Acciones */
.actions { display: flex; gap: 0.4rem; }

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

.action-btn:hover { background: #eff6ff; border-color: #2563eb; color: #2563eb; }
.action-btn.danger:hover { background: #fef2f2; border-color: #dc2626; color: #dc2626; }

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

/* Modal */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 500;
}

.modal {
  background: #fff;
  border-radius: 12px;
  width: 100%;
  max-width: 560px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #f3f4f6;
}

.modal-title {
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1rem;
  color: #9ca3af;
  cursor: pointer;
}

.modal-body { padding: 1.5rem; }

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-group.full-width { grid-column: 1 / -1; }

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
}

.form-input:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #f3f4f6;
}

.btn-cancel {
  padding: 0.6rem 1.25rem;
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
  padding: 0.6rem 1.25rem;
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
</style>