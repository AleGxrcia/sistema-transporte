<template>
  <div class="drivers">

    <!-- Header -->
    <div class="page-header">
      <h2 class="page-title">Gestión de conductores</h2>
      <button class="btn-primary" @click="showModal = true">
        + Nuevo conductor
      </button>
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
        <option value="Suspendido">Suspendido</option>
        <option value="Inactivo">Inactivo</option>
      </select>
    </div>

    <!-- Grid de tarjetas -->
    <div class="drivers-grid">
      <div
        v-for="driver in filteredDrivers"
        :key="driver.id"
        class="driver-card"
        @click="$router.push('/drivers/' + driver.id)"
      >
        <div class="card-top">
          <div class="driver-avatar" :style="{ background: driver.avatarColor }">
            {{ driver.initials }}
          </div>
          <div class="driver-info">
            <h3 class="driver-name">{{ driver.name }}</h3>
            <p class="driver-cedula">Cédula: {{ driver.cedula }}</p>
            <p class="driver-phone">Tel: {{ driver.phone }}</p>
          </div>
          <button
            v-if="driver.status !== 'En viaje' && driver.status !== 'Suspendido'"
            class="edit-btn"
            @click.stop="openEdit(driver)"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
              viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
            </svg>
          </button>
        </div>

        <div class="card-bottom">
          <span class="badge" :class="statusClass(driver.status)">
            • {{ driver.status }}
          </span>
          <span
            class="license-badge"
            :class="driver.licenseExpired ? 'expired' : driver.licenseWarning ? 'warning' : 'normal'"
          >
            <span v-if="driver.licenseExpired">⚠️</span>
            Lic. {{ driver.licenseType }} —
            {{ driver.licenseExpired ? 'VENCIDA' : driver.licenseWarning ? 'Vence en ' + driver.daysToExpiry + ' días' : 'Vence ' + driver.licenseExpiry }}
          </span>
        </div>
      </div>

      <!-- Tarjeta agregar -->
      <div class="driver-card add-card" @click="showModal = true">
        <div class="add-icon">+</div>
        <p class="add-label">Agregar conductor</p>
      </div>
    </div>

    <!-- Modal nuevo / editar conductor -->
    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ editingDriver ? 'Editar conductor' : 'Nuevo conductor' }}
          </h3>
          <button class="modal-close" @click="closeModal">✕</button>
        </div>

        <div class="modal-body">
          <div class="form-grid">

            <div class="form-group">
              <label class="form-label">Nombre completo</label>
              <input v-model="form.name" type="text"
                class="form-input" placeholder="ej: Juan Pérez" />
            </div>

            <div class="form-group">
              <label class="form-label">Cédula</label>
              <input v-model="form.cedula" type="text"
                class="form-input" placeholder="ej: 001-2345678-9" />
            </div>

            <div class="form-group">
              <label class="form-label">Teléfono</label>
              <input v-model="form.phone" type="text"
                class="form-input" placeholder="ej: 809-555-0101" />
            </div>

            <div class="form-group">
              <label class="form-label">Número de licencia</label>
              <input v-model="form.license" type="text"
                class="form-input" placeholder="ej: LIC-009-2023" />
            </div>

            <div class="form-group">
              <label class="form-label">Tipo de licencia</label>
              <select v-model="form.licenseType" class="form-input">
                <option value="">Seleccionar tipo</option>
                <option value="A">Clase A</option>
                <option value="B">Clase B</option>
                <option value="C">Clase C</option>
                <option value="D">Clase D</option>
                <option value="E">Clase E</option>
              </select>
            </div>

            <div class="form-group">
              <label class="form-label">Fecha de vencimiento licencia</label>
              <input v-model="form.licenseExpiry" type="date" class="form-input" />
            </div>

            <div class="form-group full-width">
              <label class="form-label">Dirección</label>
              <input v-model="form.address" type="text"
                class="form-input"
                placeholder="ej: Av. Luperón 45, Los Jardines, Santo Domingo" />
            </div>

            <div class="form-group full-width">
              <label class="form-label">Supervisor asignado</label>
              <select v-model="form.supervisor" class="form-input">
                <option value="">Seleccionar supervisor</option>
                <option value="Carlos Méndez">Carlos Méndez</option>
                <option value="Ana Gómez">Ana Gómez</option>
                <option value="Pedro Ruiz">Pedro Ruiz</option>
              </select>
            </div>

          </div>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="closeModal">Cancelar</button>
          <button class="btn-submit" @click="handleSave" :disabled="loading">
            {{ loading ? 'Guardando...' : editingDriver ? 'Guardar cambios' : 'Registrar conductor' }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'

const search = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const editingDriver = ref(null)
const loading = ref(false)

const form = reactive({
  name: '', cedula: '', phone: '', license: '',
  licenseType: '', licenseExpiry: '', address: '', supervisor: ''
})

const drivers = ref([
  { id: 1, name: 'Juan Pérez',      initials: 'JP', avatarColor: '#3b82f6', cedula: '001-2345678-9', phone: '809-555-0101', status: 'Disponible', licenseType: 'B', licenseExpiry: '15/08/2026', licenseWarning: false, licenseExpired: false, daysToExpiry: 79  },
  { id: 2, name: 'Carlos López',    initials: 'CL', avatarColor: '#22c55e', cedula: '001-3456789-0', phone: '849-555-0202', status: 'En viaje',   licenseType: 'C', licenseExpiry: '30/11/2026', licenseWarning: false, licenseExpired: false, daysToExpiry: 170 },
  { id: 3, name: 'Roberto Díaz',    initials: 'RD', avatarColor: '#ef4444', cedula: '001-4567890-1', phone: '829-555-0303', status: 'Suspendido', licenseType: 'B', licenseExpiry: '10/03/2026', licenseWarning: false, licenseExpired: true,  daysToExpiry: 0   },
  { id: 4, name: 'Miguel Fernández',initials: 'MF', avatarColor: '#f59e0b', cedula: '001-5678901-2', phone: '809-555-0404', status: 'Disponible', licenseType: 'C', licenseExpiry: '15/06/2026', licenseWarning: true,  licenseExpired: false, daysToExpiry: 12  },
  { id: 5, name: 'Ana Martínez',    initials: 'AM', avatarColor: '#8b5cf6', cedula: '001-6789012-3', phone: '849-555-0505', status: 'Disponible', licenseType: 'B', licenseExpiry: '22/01/2027', licenseWarning: false, licenseExpired: false, daysToExpiry: 225 },
])

const filteredDrivers = computed(() => {
  let result = drivers.value
  if (search.value) {
    const q = search.value.toLowerCase()
    result = result.filter(d =>
      d.name.toLowerCase().includes(q) ||
      d.cedula.includes(q)
    )
  }
  if (statusFilter.value) result = result.filter(d => d.status === statusFilter.value)
  return result
})

function statusClass(status) {
  const map = {
    'Disponible': 'disponible',
    'En viaje':   'en-viaje',
    'Suspendido': 'suspendido',
    'Inactivo':   'inactivo',
  }
  return map[status] || ''
}

function openEdit(driver) {
  editingDriver.value = driver
  Object.assign(form, {
    name: driver.name, cedula: driver.cedula,
    phone: driver.phone, license: '', licenseType: driver.licenseType,
    licenseExpiry: '', address: '', supervisor: ''
  })
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingDriver.value = null
  Object.assign(form, {
    name: '', cedula: '', phone: '', license: '',
    licenseType: '', licenseExpiry: '', address: '', supervisor: ''
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
.drivers {
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
  width: 180px;
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
  min-width: 180px;
}

/* Grid de tarjetas */
.drivers-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.driver-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.25rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  cursor: pointer;
  transition: box-shadow 0.2s, border-color 0.2s;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.driver-card:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.08);
  border-color: #e5e7eb;
}

.card-top {
  display: flex;
  align-items: flex-start;
  gap: 0.875rem;
  position: relative;
}

.driver-avatar {
  width: 44px;
  height: 44px;
  min-width: 44px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 700;
  color: #fff;
}

.driver-info { flex: 1; }

.driver-name {
  font-size: 0.95rem;
  font-weight: 700;
  color: #111827;
  margin-bottom: 2px;
}

.driver-cedula,
.driver-phone {
  font-size: 0.78rem;
  color: #6b7280;
  margin-top: 1px;
}

.edit-btn {
  position: absolute;
  right: 0;
  top: 0;
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  background: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.15s;
}

.edit-btn:hover { background: #eff6ff; border-color: #2563eb; color: #2563eb; }

.card-bottom {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
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
  width: fit-content;
}

.badge.disponible { background: #f0fdf4; color: #16a34a; border-color: #bbf7d0; }
.badge.en-viaje   { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.badge.suspendido { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
.badge.inactivo   { background: #f9fafb; color: #6b7280; border-color: #e5e7eb; }

/* License badge */
.license-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 500;
  width: fit-content;
}

.license-badge.normal  { background: #f3f4f6; color: #374151; }
.license-badge.warning { background: #fffbeb; color: #d97706; border: 1px solid #fde68a; }
.license-badge.expired { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }

/* Tarjeta agregar */
.add-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border: 2px dashed #e5e7eb;
  background: #f9fafb;
  min-height: 140px;
}

.add-card:hover { border-color: #2563eb; background: #eff6ff; }

.add-icon {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 2px dashed #d1d5db;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  color: #9ca3af;
}

.add-card:hover .add-icon { border-color: #2563eb; color: #2563eb; }

.add-label {
  font-size: 0.82rem;
  color: #9ca3af;
}

.add-card:hover .add-label { color: #2563eb; }

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
  background: #fff;
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