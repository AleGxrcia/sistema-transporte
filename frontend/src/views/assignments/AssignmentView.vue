<template>
  <div class="assignments">

    <!-- Info de la solicitud -->
    <div class="request-banner">
      <div class="request-info">
        <span class="request-dot"></span>
        <span class="request-id">Solicitud #001</span>
        <span class="badge aprobada">• Aprobada</span>
        <span class="request-meta"><strong>Área:</strong> Ventas</span>
        <span class="request-meta"><strong>Destino:</strong> Sto. Domingo Este</span>
        <span class="request-meta"><strong>Fecha:</strong> 29/05/2026</span>
        <span class="request-meta"><strong>Horario:</strong> 09:00 — 13:00</span>
        <span class="request-meta"><strong>Pasajeros:</strong> 6</span>
        <span class="request-meta"><strong>Motivo:</strong> Visita a cliente corporativo</span>
      </div>
    </div>

    <!-- Selección -->
    <div class="selection-grid">

      <!-- Vehículos -->
      <div class="selection-col">
        <div class="col-header">
          <div>
            <h3 class="col-title">Seleccionar vehículo</h3>
            <p class="col-sub">Vehículos disponibles para 09:00 — 13:00 del 29/05</p>
          </div>
          <span class="available-badge">{{ availableVehicles }} disponibles</span>
        </div>

        <div class="options-list">
          <div
            v-for="vehicle in vehicles"
            :key="vehicle.id"
            class="option-card"
            :class="{
              selected: selectedVehicle === vehicle.id,
              warning: vehicle.warning,
              disabled: vehicle.disabled
            }"
            @click="!vehicle.disabled && selectVehicle(vehicle.id)"
          >
            <div class="option-left">
              <div class="option-icon" :class="{ active: selectedVehicle === vehicle.id }">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20"
                  viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                  <rect x="1" y="3" width="15" height="13" rx="2"/>
                  <path d="M16 8h4l3 3v5h-7V8z"/>
                  <circle cx="5.5" cy="18.5" r="2.5"/>
                  <circle cx="18.5" cy="18.5" r="2.5"/>
                </svg>
              </div>
              <div>
                <p class="option-name">{{ vehicle.plate }} — {{ vehicle.name }}</p>
                <p class="option-detail">{{ vehicle.type }} · {{ vehicle.capacity }} pasajeros · {{ vehicle.mileage }} km</p>
                <p v-if="selectedVehicle === vehicle.id" class="option-confirm green">
                  ✓ Seleccionado — Capacidad suficiente (6/{{ vehicle.capacity }} pasajeros)
                </p>
                <span v-if="vehicle.warning" class="warning-tag">⚠ Cap. insuficiente</span>
              </div>
            </div>
            <div v-if="selectedVehicle === vehicle.id" class="check-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="var(--blue-hover)" stroke="var(--blue-hover)" stroke-width="2">
                <polyline points="20 6 9 17 4 12"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Conductores -->
      <div class="selection-col">
        <div class="col-header">
          <div>
            <h3 class="col-title">Seleccionar conductor</h3>
            <p class="col-sub">Conductores libres con licencia vigente</p>
          </div>
          <span class="available-badge">{{ availableDrivers }} disponibles</span>
        </div>

        <div class="options-list">
          <div
            v-for="driver in drivers"
            :key="driver.id"
            class="option-card"
            :class="{ selected: selectedDriver === driver.id }"
            @click="selectDriver(driver.id)"
          >
            <div class="option-left">
              <div class="driver-avatar" :style="{ background: driver.avatarColor }">
                {{ driver.initials }}
              </div>
              <div>
                <p class="option-name">{{ driver.name }}</p>
                <p class="option-detail">Lic. Clase {{ driver.licenseType }} · Vence {{ driver.licenseExpiry }}</p>
                <p v-if="selectedDriver === driver.id" class="option-confirm green">
                  ✓ Seleccionado — Licencia vigente · {{ driver.trips }} viajes este mes
                </p>
                <span v-if="driver.warning" class="warning-tag orange">⚠ Vence en {{ driver.daysToExpiry }} días</span>
              </div>
            </div>
            <div v-if="selectedDriver === driver.id" class="check-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="var(--blue-hover)" stroke="var(--blue-hover)" stroke-width="2">
                <polyline points="20 6 9 17 4 12"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- Resumen de asignación -->
    <div v-if="selectedVehicle && selectedDriver" class="summary-card">
      <div class="summary-title">RESUMEN DE ASIGNACIÓN</div>
      <div class="summary-content">

        <div class="summary-item">
          <div class="summary-icon blue">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <rect x="1" y="3" width="15" height="13" rx="2"/>
              <path d="M16 8h4l3 3v5h-7V8z"/>
              <circle cx="5.5" cy="18.5" r="2.5"/>
              <circle cx="18.5" cy="18.5" r="2.5"/>
            </svg>
          </div>
          <div>
            <span class="summary-label">VEHÍCULO</span>
            <span class="summary-value">{{ selectedVehicleData?.plate }} — {{ selectedVehicleData?.name }}</span>
          </div>
        </div>

        <div class="summary-item">
          <div class="driver-avatar sm" :style="{ background: selectedDriverData?.avatarColor }">
            {{ selectedDriverData?.initials }}
          </div>
          <div>
            <span class="summary-label">CONDUCTOR</span>
            <span class="summary-value">{{ selectedDriverData?.name }}</span>
          </div>
        </div>

        <div class="summary-item">
          <div class="summary-icon green">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
              <line x1="16" y1="2" x2="16" y2="6"/>
              <line x1="8" y1="2" x2="8" y2="6"/>
              <line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
          </div>
          <div>
            <span class="summary-label">HORARIO</span>
            <span class="summary-value">29/05/26 · 09:00 — 13:00</span>
          </div>
        </div>

      </div>
    </div>

    <!-- Acciones -->
    <div class="form-actions">
      <button class="btn-cancel" @click="$router.push('/requests')">Cancelar</button>
      <button
        class="btn-submit"
        :disabled="!selectedVehicle || !selectedDriver || loading"
        @click="handleConfirm"
      >
        {{ loading ? 'Confirmando...' : 'Confirmar asignación' }}
      </button>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)
const selectedVehicle = ref(1)
const selectedDriver = ref(1)

const vehicles = ref([
  { id: 1, plate: 'ABC-123', name: 'Toyota Hiace',   type: 'Van',    capacity: 12, mileage: '45,230', warning: false, disabled: false },
  { id: 2, plate: 'JKL-012', name: 'Nissan Frontier', type: 'Pickup', capacity: 5,  mileage: '12,780', warning: true,  disabled: false },
  { id: 3, plate: 'PQR-678', name: 'Hyundai H1',      type: 'Van',    capacity: 9,  mileage: '28,400', warning: false, disabled: false },
])

const drivers = ref([
  { id: 1, name: 'Juan Pérez',      initials: 'JP', avatarColor: 'var(--blue)', licenseType: 'B', licenseExpiry: '15/08/2026', trips: 18, warning: false },
  { id: 2, name: 'Ana Martínez',    initials: 'AM', avatarColor: 'var(--purple)', licenseType: 'B', licenseExpiry: '22/01/2027', trips: 12, warning: false },
  { id: 3, name: 'Miguel Fernández',initials: 'MF', avatarColor: 'var(--amber)', licenseType: 'C', licenseExpiry: '09/06/2026', trips: 8,  warning: true, daysToExpiry: 12 },
])

const availableVehicles = computed(() => vehicles.value.filter(v => !v.disabled).length)
const availableDrivers = computed(() => drivers.value.length)

const selectedVehicleData = computed(() => vehicles.value.find(v => v.id === selectedVehicle.value))
const selectedDriverData = computed(() => drivers.value.find(d => d.id === selectedDriver.value))

function selectVehicle(id) { selectedVehicle.value = id }
function selectDriver(id)  { selectedDriver.value = id  }

async function handleConfirm() {
  loading.value = true
  try {
    await new Promise(r => setTimeout(r, 1000))
    router.push('/requests')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.assignments {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  font-family: 'Inter', sans-serif;
}

/* Banner solicitud */
.request-banner {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1rem 1.25rem;
  box-shadow: var(--shadow-xs);
}

.request-info {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.875rem;
}

.request-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--purple);
  flex-shrink: 0;
}

.request-id {
  font-weight: 700;
  color: var(--text);
}

.request-meta { color: var(--text-2); }
.request-meta strong { color: var(--text); }

/* Badge */
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

.badge.aprobada { background: var(--mint-bg); color: var(--mint-dark); border-color: var(--mint-border); }

/* Grid selección */
.selection-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
}

.selection-col {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.col-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.col-title {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text);
}

.col-sub {
  font-size: 0.78rem;
  color: var(--text-3);
  margin-top: 2px;
}

.available-badge {
  background: var(--mint-bg);
  color: var(--mint-dark);
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 999px;
  white-space: nowrap;
}

/* Opciones */
.options-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.option-card {
  background: var(--white);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  transition: all 0.2s;
}

.option-card:hover { border-color: var(--blue-hover); box-shadow: 0 0 0 1px var(--blue-hover); }
.option-card.selected { border-color: var(--blue-hover); background: var(--blue-light); box-shadow: 0 0 0 1px var(--blue-hover); }
.option-card.warning { border-color: var(--amber-border); background: var(--amber-bg); }
.option-card.disabled { opacity: 0.5; cursor: not-allowed; }

.option-left {
  display: flex;
  align-items: flex-start;
  gap: 0.875rem;
}

.option-icon {
  width: 40px;
  height: 40px;
  min-width: 40px;
  border-radius: 8px;
  background: var(--bg);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-2);
}

.option-icon.active { background: var(--blue-mid); color: var(--blue-hover); }

.option-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text);
}

.option-detail {
  font-size: 0.78rem;
  color: var(--text-2);
  margin-top: 2px;
}

.option-confirm {
  font-size: 0.75rem;
  margin-top: 4px;
  font-weight: 500;
}

.warning-tag {
  display: inline-block;
  font-size: 0.72rem;
  font-weight: 500;
  color: var(--amber-text);
  background: var(--amber-bg);
  border: 1px solid var(--amber-border);
  padding: 2px 8px;
  border-radius: 999px;
  margin-top: 4px;
}

.warning-tag.orange { color: var(--amber-text); }

.check-icon {
  width: 28px;
  height: 28px;
  min-width: 28px;
  border-radius: 50%;
  background: var(--blue-mid);
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Driver avatar */
.driver-avatar {
  width: 40px;
  height: 40px;
  min-width: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 700;
  color: var(--white);
}

.driver-avatar.sm {
  width: 36px;
  height: 36px;
  min-width: 36px;
  font-size: 0.75rem;
}

/* Summary */
.summary-card {
  background: var(--white);
  border-radius: 10px;
  border: 1px solid var(--border);
  padding: 1.25rem;
  box-shadow: var(--shadow-xs);
  transition: box-shadow 0.15s, transform 0.15s;
}

.summary-card:hover {
  box-shadow: var(--shadow-sm);
  transform: translateY(-1px);
}

.summary-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.08em;
  margin-bottom: 1rem;
}

.summary-content {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.summary-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.summary-icon {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.summary-icon.blue  { background: var(--blue-mid); color: var(--blue-hover); }
.summary-icon.green { background: var(--mint-bg); color: var(--mint-dark); }

.summary-label {
  display: block;
  font-size: 0.65rem;
  font-weight: 600;
  color: var(--text-3);
  letter-spacing: 0.05em;
  margin-bottom: 2px;
}

.summary-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text);
}

/* Acciones */
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn-cancel {
  padding: 0.6rem 1.25rem;
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
  padding: 0.6rem 1.25rem;
  background: var(--blue);
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

.green { color: var(--mint-dark); }
</style>