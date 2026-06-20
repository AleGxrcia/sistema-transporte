<template>
  <div class="vehicle-detail">

    <!-- Breadcrumb + acciones -->
    <div class="page-header">
      <div class="breadcrumb">
        <router-link to="/vehicles" class="breadcrumb-link">← Vehículos</router-link>
        <span class="breadcrumb-sep">/</span>
        <span class="breadcrumb-current">{{ vehicle.brand }} {{ vehicle.model }} — {{ vehicle.plate }}</span>
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
      <div class="info-card-left">
        <div class="vehicle-icon">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32"
            viewBox="0 0 24 24" fill="none" stroke="#2563eb" stroke-width="1.5">
            <rect x="1" y="3" width="15" height="13" rx="2"/>
            <path d="M16 8h4l3 3v5h-7V8z"/>
            <circle cx="5.5" cy="18.5" r="2.5"/>
            <circle cx="18.5" cy="18.5" r="2.5"/>
          </svg>
        </div>
        <div>
          <div class="vehicle-name-row">
            <h2 class="vehicle-name">{{ vehicle.brand }} {{ vehicle.model }}</h2>
            <span class="badge" :class="statusClass(vehicle.status)">
              • {{ vehicle.status }}
            </span>
          </div>
          <div class="vehicle-meta">
            <div class="meta-item">
              <span class="meta-label">MATRÍCULA</span>
              <span class="meta-value">{{ vehicle.plate }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">MODELO</span>
              <span class="meta-value">{{ vehicle.model }} {{ vehicle.year }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">TIPO</span>
              <span class="type-badge">{{ vehicle.type }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">COLOR</span>
              <span class="meta-value">● {{ vehicle.color }}</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">CAPACIDAD</span>
              <span class="meta-value">{{ vehicle.capacity }} pasajeros</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">KILOMETRAJE</span>
              <span class="meta-value">{{ vehicle.mileage.toLocaleString() }} km</span>
            </div>
            <div class="meta-item">
              <span class="meta-label">ÚLT. MANTENIMIENTO</span>
              <span class="meta-value green">{{ vehicle.lastMaintenance }}</span>
            </div>
          </div>
        </div>
      </div>
      <button class="btn-status">Cambiar estado ▾</button>
    </div>

    <!-- KPIs del vehículo -->
    <div class="kpi-grid">
      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">VIAJES REALIZADOS</span>
          <div class="kpi-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">87</div>
        <div class="kpi-sub">Historial total</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">GALONES CONSUMIDOS</span>
          <div class="kpi-icon orange">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="13 17 18 12 13 7"/>
              <polyline points="6 17 11 12 6 7"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">312</div>
        <div class="kpi-sub">Mayo 2026</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">COSTO MANTENIMIENTO</span>
          <div class="kpi-icon purple">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="1" x2="12" y2="23"/>
              <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
            </svg>
          </div>
        </div>
        <div class="kpi-value">RD$27k</div>
        <div class="kpi-sub">Acumulado 2026</div>
      </div>

      <div class="kpi-card">
        <div class="kpi-top">
          <span class="kpi-label">PRÓX. MANTENIMIENTO</span>
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
        <div class="kpi-value green">10/08/26</div>
        <div class="kpi-sub">En 74 días</div>
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

    <!-- Tab: Información general -->
    <div v-if="activeTab === 'info'" class="tab-content">
      <div class="two-col">

        <!-- Datos del vehículo -->
        <div class="data-card">
          <div class="data-card-title">DATOS DEL VEHÍCULO</div>
          <div class="data-grid">
            <div class="data-item">
              <span class="data-label">Marca</span>
              <span class="data-value">{{ vehicle.brand }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Modelo</span>
              <span class="data-value">{{ vehicle.model }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Año</span>
              <span class="data-value">{{ vehicle.year }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Matrícula</span>
              <span class="data-value">{{ vehicle.plate }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Color</span>
              <span class="data-value">{{ vehicle.color }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Tipo</span>
              <span class="data-value">{{ vehicle.type }}</span>
            </div>
            <div class="data-item">
              <span class="data-label">Capacidad</span>
              <span class="data-value">{{ vehicle.capacity }} pasajeros</span>
            </div>
            <div class="data-item">
              <span class="data-label">Kilometraje actual</span>
              <span class="data-value">{{ vehicle.mileage.toLocaleString() }} km</span>
            </div>
            <div class="data-item full">
              <span class="data-label">Fecha último mantenimiento</span>
              <span class="data-value green">{{ vehicle.lastMaintenance }}</span>
            </div>
          </div>
        </div>

        <!-- Disponibilidad esta semana -->
        <div class="data-card">
          <div class="data-card-title">DISPONIBILIDAD — ESTA SEMANA</div>
          <div class="availability-list">
            <div v-for="day in availability" :key="day.day" class="avail-row">
              <span class="avail-day">{{ day.day }}</span>
              <div class="avail-bar" :class="day.type">
                {{ day.label }}
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- Tab: Historial de viajes -->
    <div v-if="activeTab === 'trips'" class="tab-content">
      <div class="filters">
        <div class="search-box">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="11" cy="11" r="8"/>
            <line x1="21" y1="21" x2="16.65" y2="16.65"/>
          </svg>
        </div>
        <input type="date" class="date-input" placeholder="Desde" />
        <input type="date" class="date-input" placeholder="Hasta" />
      </div>
      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>FECHA</th>
              <th>ÁREA</th>
              <th>DESTINO</th>
              <th>CONDUCTOR</th>
              <th>SALIDA</th>
              <th>REGRESO</th>
              <th>PASAJEROS</th>
              <th>ESTADO</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="trip in vehicleTrips" :key="trip.id">
              <td>{{ trip.date }}</td>
              <td class="td-bold">{{ trip.area }}</td>
              <td>{{ trip.destination }}</td>
              <td>{{ trip.driver }}</td>
              <td>{{ trip.departure }}</td>
              <td>{{ trip.return }}</td>
              <td>{{ trip.passengers }}</td>
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
        <span class="pagination-info">Mostrando 1-5 de 87 viajes</span>
        <div class="pagination-btns">
          <button class="page-btn" disabled>← Anterior</button>
          <button class="page-btn">Siguiente →</button>
        </div>
      </div>
    </div>

    <!-- Tab: Mantenimiento -->
    <div v-if="activeTab === 'maintenance'" class="tab-content">
      <div class="tab-header-action">
        <button class="btn-primary" @click="showMaintenanceModal = true">
          + Registrar mantenimiento
        </button>
      </div>
      <div class="timeline">
        <div v-for="item in maintenanceHistory" :key="item.id" class="timeline-item">
          <div class="timeline-dot" :class="item.type === 'Preventivo' ? 'green' : 'orange'"></div>
          <div class="timeline-content">
            <div class="timeline-header">
              <div>
                <p class="timeline-title">{{ item.title }}</p>
                <p class="timeline-date">{{ item.date }} · {{ item.shop }}</p>
              </div>
              <span class="badge" :class="item.type === 'Preventivo' ? 'preventivo' : 'correctivo'">
                • {{ item.type }}
              </span>
            </div>
            <p class="timeline-desc">{{ item.description }}</p>
            <div class="timeline-meta">
              <span><strong>COSTO</strong> {{ item.cost }}</span>
              <span><strong>TALLER</strong> {{ item.shop }}</span>
              <span><strong>PRÓX. SERVICIO</strong>
                <span class="green">{{ item.nextService }}</span>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tab: Consumo de combustible -->
    <div v-if="activeTab === 'fuel'" class="tab-content">
      <div class="tab-header-action">
        <div class="view-tabs">
          <button class="view-tab" :class="{ active: fuelView === 'lista' }"
            @click="fuelView = 'lista'">Lista</button>
          <button class="view-tab" :class="{ active: fuelView === 'grafico' }"
            @click="fuelView = 'grafico'">Gráfico</button>
        </div>
        <button class="btn-primary" @click="showFuelModal = true">
          + Registrar consumo
        </button>
      </div>

      <!-- Mini KPIs combustible -->
      <div class="fuel-kpis">
        <div class="fuel-kpi">
          <span class="kpi-label">TOTAL GALONES — MAYO</span>
          <div class="fuel-kpi-value">312 <span class="green text-sm">↑ 8% vs abril</span></div>
        </div>
        <div class="fuel-kpi">
          <span class="kpi-label">COSTO TOTAL — MAYO</span>
          <div class="fuel-kpi-value">RD$31k <span class="gray text-sm">Promedio RD$99/gal</span></div>
        </div>
        <div class="fuel-kpi">
          <span class="kpi-label">KM POR GALÓN (PROMEDIO)</span>
          <div class="fuel-kpi-value">14.5 <span class="gray text-sm">Rendimiento actual</span></div>
        </div>
      </div>

      <div class="table-wrapper">
        <table class="table">
          <thead>
            <tr>
              <th>FECHA</th>
              <th>GALONES</th>
              <th>PRECIO/GALÓN</th>
              <th>COSTO TOTAL</th>
              <th>KM AL CARGAR</th>
              <th>KM RECORRIDOS</th>
              <th>RENDIMIENTO</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="fuel in fuelHistory" :key="fuel.id">
              <td>{{ fuel.date }}</td>
              <td>{{ fuel.gallons }}</td>
              <td>{{ fuel.pricePerGallon }}</td>
              <td class="td-bold">{{ fuel.totalCost }}</td>
              <td class="td-gray">{{ fuel.kmAtLoad }}</td>
              <td class="green">{{ fuel.kmDriven }}</td>
              <td class="green">{{ fuel.efficiency }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="pagination">
        <span class="pagination-info">Mostrando 1-5 de 28 registros</span>
        <div class="pagination-btns">
          <button class="page-btn" disabled>← Anterior</button>
          <button class="page-btn">Siguiente →</button>
        </div>
      </div>
    </div>

    <!-- Modal registrar mantenimiento -->
    <div v-if="showMaintenanceModal" class="modal-overlay" @click.self="showMaintenanceModal = false">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">Registrar mantenimiento</h3>
          <button class="modal-close" @click="showMaintenanceModal = false">✕</button>
        </div>
        <div class="modal-body">
          <div class="form-grid">
            <div class="form-group full-width">
              <label class="form-label">Tipo de mantenimiento</label>
              <select v-model="maintenanceForm.type" class="form-input">
                <option value="">Seleccionar tipo</option>
                <option value="Preventivo">Preventivo</option>
                <option value="Correctivo">Correctivo</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Fecha</label>
              <input v-model="maintenanceForm.date" type="date" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">Taller</label>
              <input v-model="maintenanceForm.shop" type="text"
                class="form-input" placeholder="ej: Toyota Service Center" />
            </div>
            <div class="form-group">
              <label class="form-label">Costo (RD$)</label>
              <input v-model="maintenanceForm.cost" type="number"
                class="form-input" placeholder="ej: 5200" />
            </div>
            <div class="form-group">
              <label class="form-label">Próximo servicio</label>
              <input v-model="maintenanceForm.nextService" type="date" class="form-input" />
            </div>
            <div class="form-group full-width">
              <label class="form-label">Descripción</label>
              <textarea v-model="maintenanceForm.description" class="form-textarea"
                placeholder="Describe el trabajo realizado..." />
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-cancel" @click="showMaintenanceModal = false">Cancelar</button>
          <button class="btn-submit" @click="showMaintenanceModal = false">Guardar registro</button>
        </div>
      </div>
    </div>

    <!-- Modal registrar consumo -->
    <div v-if="showFuelModal" class="modal-overlay" @click.self="showFuelModal = false">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">Registrar consumo de combustible</h3>
          <button class="modal-close" @click="showFuelModal = false">✕</button>
        </div>
        <div class="modal-body">
          <div class="form-grid">
            <div class="form-group">
              <label class="form-label">Fecha de carga</label>
              <input v-model="fuelForm.date" type="date" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">Galones cargados</label>
              <input v-model="fuelForm.gallons" type="number"
                class="form-input" placeholder="0.00" />
            </div>
            <div class="form-group">
              <label class="form-label">Precio por galón (RD$)</label>
              <input v-model="fuelForm.pricePerGallon" type="number"
                class="form-input" placeholder="99.50" />
            </div>
            <div class="form-group">
              <label class="form-label">Kilometraje al cargar</label>
              <input v-model="fuelForm.kmAtLoad" type="number"
                class="form-input" placeholder="45,230" />
            </div>
            <div class="form-group full-width">
              <label class="form-label">Costo total (calculado)</label>
              <div class="calculated-field">
                <span class="calculated-label">Se calcula automáticamente</span>
                <span class="calculated-value">
                  RD$ {{ fuelForm.gallons && fuelForm.pricePerGallon
                    ? (fuelForm.gallons * fuelForm.pricePerGallon).toFixed(2)
                    : '0.00' }}
                </span>
              </div>
            </div>
          </div>
          <div class="warning-banner">
            ⚠️ El kilometraje ingresado debe ser
            <strong>mayor al último registrado</strong>
            para este vehículo ({{ vehicle.mileage.toLocaleString() }} km).
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-cancel" @click="showFuelModal = false">Limpiar</button>
          <button class="btn-submit" @click="showFuelModal = false">Guardar registro</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'

const activeTab = ref('info')
const fuelView = ref('lista')
const showMaintenanceModal = ref(false)
const showFuelModal = ref(false)

const tabs = [
  { key: 'info',        label: 'Información general' },
  { key: 'trips',       label: 'Historial de viajes' },
  { key: 'maintenance', label: 'Mantenimiento' },
  { key: 'fuel',        label: 'Consumo de combustible' },
]

const vehicle = ref({
  id: 1, plate: 'ABC-123', brand: 'Toyota', model: 'Hiace',
  type: 'Van', year: 2022, capacity: 12, mileage: 45230,
  color: 'Blanco', status: 'Disponible', lastMaintenance: '10/05/2026'
})

const availability = ref([
  { day: 'Lun', label: 'Disponible todo el día', type: 'available' },
  { day: 'Mar', label: 'Disponible todo el día', type: 'available' },
  { day: 'Mié', label: '07:00-10:30',            type: 'partial'   },
  { day: 'Jue', label: 'En viaje 07:00-17:00',   type: 'busy'      },
  { day: 'Vie', label: 'Disponible todo el día', type: 'available' },
  { day: 'Sáb', label: 'No operativo',           type: 'inactive'  },
])

const vehicleTrips = ref([
  { id: 1, date: '28/05/26', area: 'RRHH',     destination: 'Aeropuerto Las Américas',    driver: 'Juan Pérez',   departure: '07:30', return: '09:30', passengers: 8,  status: 'Finalizado' },
  { id: 2, date: '25/05/26', area: 'Legal',    destination: 'Santiago de los Caballeros', driver: 'Ana Martínez', departure: '06:00', return: '20:00', passengers: 5,  status: 'Finalizado' },
  { id: 3, date: '20/05/26', area: 'Ventas',   destination: 'Bávaro, La Altagracia',      driver: 'Juan Pérez',   departure: '08:00', return: '19:00', passengers: 10, status: 'Finalizado' },
  { id: 4, date: '15/05/26', area: 'IT',       destination: 'Zona Franca Industrial',     driver: 'Carlos López', departure: '10:30', return: '14:00', passengers: 4,  status: 'Cancelado'  },
  { id: 5, date: '10/05/26', area: 'Gerencia', destination: 'DGII — Centro de los Héroes',driver: 'Juan Pérez',  departure: '09:00', return: '12:00', passengers: 3,  status: 'Finalizado' },
])

const maintenanceHistory = ref([
  { id: 1, title: 'Revisión general 40,000 km', date: '10 de mayo, 2026',     shop: 'Toyota Service Center', type: 'Preventivo', cost: 'RD$ 5,200', nextService: '10/08/2026', description: 'Cambio de aceite y filtros, revisión de frenos, inspección general de suspensión y neumáticos.' },
  { id: 2, title: 'Reparación sistema de frenos', date: '12 de febrero, 2026', shop: 'AutoFix Santo Domingo', type: 'Correctivo', cost: 'RD$ 8,400', nextService: '10/05/2026', description: 'Reemplazo de pastillas y discos delanteros. Se detectó desgaste acelerado posiblemente por sobrecarga frecuente.' },
  { id: 3, title: 'Revisión general 20,000 km', date: '5 de agosto, 2025',    shop: 'Toyota Service Center', type: 'Preventivo', cost: 'RD$ 4,800', nextService: '10/11/2025', description: 'Mantenimiento de rutina. Cambio de aceite, filtros de aire y combustible. Sin novedades.' },
])

const fuelHistory = ref([
  { id: 1, date: '27/05/26', gallons: '45.5', pricePerGallon: 'RD$ 99.50', totalCost: 'RD$ 4,527', kmAtLoad: '45,230 km', kmDriven: '+620 km', efficiency: '13.6 km/gl' },
  { id: 2, date: '19/05/26', gallons: '48.0', pricePerGallon: 'RD$ 97.80', totalCost: 'RD$ 4,694', kmAtLoad: '44,610 km', kmDriven: '+710 km', efficiency: '14.8 km/gl' },
  { id: 3, date: '11/05/26', gallons: '50.0', pricePerGallon: 'RD$ 98.20', totalCost: 'RD$ 4,910', kmAtLoad: '43,900 km', kmDriven: '+680 km', efficiency: '13.6 km/gl' },
  { id: 4, date: '03/05/26', gallons: '42.0', pricePerGallon: 'RD$ 96.50', totalCost: 'RD$ 4,053', kmAtLoad: '43,220 km', kmDriven: '+640 km', efficiency: '15.2 km/gl' },
  { id: 5, date: '24/04/26', gallons: '46.5', pricePerGallon: 'RD$ 95.00', totalCost: 'RD$ 4,418', kmAtLoad: '42,580 km', kmDriven: '+600 km', efficiency: '12.9 km/gl' },
])

const maintenanceForm = reactive({
  type: '', date: '', shop: '', cost: '', nextService: '', description: ''
})

const fuelForm = reactive({
  date: '', gallons: '', pricePerGallon: '', kmAtLoad: ''
})

function statusClass(status) {
  const map = {
    'Disponible': 'disponible', 'En viaje': 'en-viaje',
    'En mantenimiento': 'mantenimiento', 'Fuera de servicio': 'fuera-servicio'
  }
  return map[status] || ''
}
</script>

<style scoped>
.vehicle-detail {
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

.info-card-left { display: flex; gap: 1.25rem; align-items: flex-start; }

.vehicle-icon {
  width: 64px;
  height: 64px;
  background: #eff6ff;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.vehicle-name-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.vehicle-name {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.vehicle-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
}

.meta-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

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
.kpi-icon.orange { background: #fff7ed; color: #d97706; }
.kpi-icon.purple { background: #f5f3ff; color: #7c3aed; }
.kpi-icon.teal   { background: #f0fdfa; color: #0d9488; }

.kpi-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  line-height: 1;
  margin-bottom: 0.25rem;
}

.kpi-value.green { color: #16a34a; }
.kpi-sub { font-size: 0.75rem; color: #6b7280; }

/* Tabs */
.tabs-bar {
  display: flex;
  gap: 0;
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

.tab-header-action {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* Two col layout */
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
}

.data-card-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
  margin-bottom: 1rem;
}

.data-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.data-item { display: flex; flex-direction: column; gap: 2px; }
.data-item.full { grid-column: 1 / -1; }

.data-label {
  font-size: 0.72rem;
  color: #9ca3af;
}

.data-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.data-value.green { color: #16a34a; }

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
.avail-bar.partial   { background: #dbeafe; color: #2563eb; }
.avail-bar.inactive  { background: #f3f4f6; color: #9ca3af; }

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

.badge.disponible     { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.en-viaje       { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.mantenimiento  { background: #fff7ed; color: #d97706; border-color: #fed7aa; }
.badge.fuera-servicio { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
.badge.finalizado     { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.cancelado      { background: #f9fafb; color: #6b7280; border-color: #e5e7eb; }
.badge.preventivo     { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.correctivo     { background: #fff7ed; color: #d97706; border-color: #fed7aa; }

/* Timeline mantenimiento */
.timeline { display: flex; flex-direction: column; gap: 0; }

.timeline-item {
  display: flex;
  gap: 1rem;
  padding-bottom: 1.5rem;
  position: relative;
}

.timeline-item::before {
  content: '';
  position: absolute;
  left: 7px;
  top: 16px;
  bottom: 0;
  width: 2px;
  background: #e5e7eb;
}

.timeline-item:last-child::before { display: none; }

.timeline-dot {
  width: 16px;
  height: 16px;
  min-width: 16px;
  border-radius: 50%;
  margin-top: 4px;
  border: 2px solid #fff;
  box-shadow: 0 0 0 2px currentColor;
}

.timeline-dot.green  { background: #16a34a; color: #16a34a; }
.timeline-dot.orange { background: #d97706; color: #d97706; }

.timeline-content {
  background: #fff;
  border: 1px solid #f3f4f6;
  border-radius: 10px;
  padding: 1rem;
  flex: 1;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.timeline-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

.timeline-date {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

.timeline-desc {
  font-size: 0.82rem;
  color: #6b7280;
  margin-bottom: 0.75rem;
  line-height: 1.5;
}

.timeline-meta {
  display: flex;
  gap: 1.5rem;
  font-size: 0.78rem;
  color: #6b7280;
}

.timeline-meta strong {
  display: block;
  font-size: 0.65rem;
  color: #9ca3af;
  letter-spacing: 0.05em;
  margin-bottom: 2px;
}

/* Fuel KPIs */
.fuel-kpis {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.fuel-kpi {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.fuel-kpi-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  margin-top: 0.4rem;
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
}

.text-sm { font-size: 0.75rem; font-weight: 400; }

/* View tabs */
.view-tabs { display: flex; }

.view-tab {
  padding: 0.4rem 0.875rem;
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
.view-tab.active      { background: #f9fafb; color: #111827; font-weight: 600; }

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
.gray    { color: #6b7280; }

/* Filtros */
.filters {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.search-box {
  display: flex;
  align-items: center;
  padding: 0.5rem 0.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  color: #9ca3af;
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
  max-width: 520px;
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

.form-group { display: flex; flex-direction: column; gap: 0.3rem; }
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

.form-textarea {
  padding: 0.6rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  outline: none;
  resize: vertical;
  min-height: 80px;
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
  margin-top: 1rem;
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
}

.btn-submit:hover { background: #1d4ed8; }

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
</style>