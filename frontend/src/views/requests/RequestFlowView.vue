<template>
  <div class="request-flow">

    <!-- Stepper -->
    <div class="stepper">
      <div
        v-for="(step, index) in steps"
        :key="step.key"
        class="step-wrapper"
      >
        <div class="step" :class="getStepClass(index)">
          <div class="step-circle" :class="getStepClass(index)">
            <svg v-if="index < currentStep" xmlns="http://www.w3.org/2000/svg"
              width="14" height="14" viewBox="0 0 24 24" fill="none"
              stroke="white" stroke-width="3">
              <polyline points="20 6 9 17 4 12"/>
            </svg>
            <span v-else>{{ index + 1 }}</span>
          </div>
          <span class="step-label" :class="getStepClass(index)">{{ step.label }}</span>
        </div>
        <div v-if="index < steps.length - 1" class="step-line"
          :class="{ completed: index < currentStep }">
        </div>
      </div>
    </div>

    <!-- Paso 2: Revisión supervisor -->
    <div v-if="currentStep === 1" class="step-content">
      <div class="two-col">

        <!-- Detalles solicitud -->
        <div class="detail-card">
          <div class="card-section-title">DETALLES DE LA SOLICITUD #001</div>
          <div class="detail-grid">
            <div class="detail-item">
              <span class="detail-label">ÁREA SOLICITANTE</span>
              <span class="detail-value">Ventas</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">OPERADOR</span>
              <span class="detail-value">Pedro Operador</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">DESTINO</span>
              <span class="detail-value">Sto. Domingo Este</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">PASAJEROS</span>
              <span class="detail-value">6 personas</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">FECHA</span>
              <span class="detail-value blue">29/05/2026</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">HORARIO</span>
              <span class="detail-value">09:00 — 13:00</span>
            </div>
            <div class="detail-item full">
              <span class="detail-label">MOTIVO DEL VIAJE</span>
              <span class="detail-value">Visita a cliente corporativo para presentación de propuesta comercial trimestral.</span>
            </div>
            <div class="detail-item full">
              <span class="detail-label">ESTADO ACTUAL</span>
              <span class="badge pendiente">• Pendiente de aprobación</span>
            </div>
          </div>

          <!-- Botones aprobar / rechazar -->
          <div class="review-actions">
            <button class="btn-approve" @click="handleApprove">
              ✓ Aprobar solicitud
            </button>
            <button class="btn-reject" @click="showRejectModal = true">
              ✕ Rechazar
            </button>
          </div>
        </div>

        <!-- Otras solicitudes pendientes -->
        <div class="pending-col">
          <div class="pending-header">
            <span class="pending-title">Otras solicitudes pendientes</span>
            <router-link to="/requests" class="pending-link">Ver todas →</router-link>
          </div>
          <div class="pending-list">
            <div
              v-for="req in pendingRequests"
              :key="req.id"
              class="pending-item"
              :class="{ selected: req.id === 1 }"
            >
              <div class="pending-item-header">
                <span class="pending-id">{{ req.code }}</span>
                <span class="badge pendiente">• Pendiente</span>
                <span class="pending-time">{{ req.time }}</span>
              </div>
              <p class="pending-route">{{ req.route }}</p>
              <p class="pending-meta">{{ req.meta }}</p>
            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- Paso 3: Asignación de recursos -->
    <div v-if="currentStep === 2" class="step-content">

      <!-- Banner aprobación -->
      <div class="approved-banner">
        <span>✓ Solicitud #001 aprobada</span>
        <span class="approved-sub">Ahora asigna los recursos para el viaje.</span>
        <button class="btn-notify">Notificar operador</button>
      </div>

      <!-- Selección -->
      <div class="two-col">

        <!-- Vehículos -->
        <div>
          <div class="col-header">
            <div>
              <h3 class="col-title">Seleccionar vehículo</h3>
              <p class="col-sub">Disponibles para 09:00-13:00 del 29/05</p>
            </div>
            <span class="available-badge">4 libres</span>
          </div>
          <div class="options-list">
            <div
              v-for="vehicle in vehicles"
              :key="vehicle.id"
              class="option-card"
              :class="{ selected: selectedVehicle === vehicle.id, warning: vehicle.warning }"
              @click="selectedVehicle = vehicle.id"
            >
              <div class="option-left">
                <div class="option-icon" :class="{ active: selectedVehicle === vehicle.id }">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
                    viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <rect x="1" y="3" width="15" height="13" rx="2"/>
                    <path d="M16 8h4l3 3v5h-7V8z"/>
                    <circle cx="5.5" cy="18.5" r="2.5"/>
                    <circle cx="18.5" cy="18.5" r="2.5"/>
                  </svg>
                </div>
                <div>
                  <p class="option-name">{{ vehicle.plate }} — {{ vehicle.name }}</p>
                  <p class="option-detail">{{ vehicle.type }} · {{ vehicle.capacity }} pas. · {{ vehicle.status }}</p>
                  <p v-if="selectedVehicle === vehicle.id && !vehicle.warning" class="option-confirm">
                    ✓ Capacidad suficiente (6/{{ vehicle.capacity }})
                  </p>
                  <span v-if="vehicle.warning" class="warning-tag">⚠ Cap. insuf.</span>
                </div>
              </div>
              <div v-if="selectedVehicle === vehicle.id" class="check-circle">✓</div>
            </div>
          </div>
        </div>

        <!-- Conductores -->
        <div>
          <div class="col-header">
            <div>
              <h3 class="col-title">Seleccionar conductor</h3>
              <p class="col-sub">Con licencia vigente y sin conflicto</p>
            </div>
            <span class="available-badge">3 libres</span>
          </div>
          <div class="options-list">
            <div
              v-for="driver in drivers"
              :key="driver.id"
              class="option-card"
              :class="{ selected: selectedDriver === driver.id }"
              @click="selectedDriver = driver.id"
            >
              <div class="option-left">
                <div class="driver-avatar" :style="{ background: driver.avatarColor }">
                  {{ driver.initials }}
                </div>
                <div>
                  <p class="option-name">{{ driver.name }}</p>
                  <p class="option-detail">Lic. {{ driver.licenseType }} · {{ driver.licenseInfo }}</p>
                  <span v-if="driver.warning" class="warning-tag orange">⚠ Pronto</span>
                </div>
              </div>
              <div v-if="selectedDriver === driver.id" class="check-circle">✓</div>
            </div>
          </div>
        </div>

      </div>

      <!-- Botón confirmar -->
      <div class="assign-footer">
        <button
          class="btn-confirm"
          :disabled="!selectedVehicle || !selectedDriver"
          @click="showConfirmModal = true"
        >
          Confirmar asignación →
        </button>
      </div>

    </div>

    <!-- Paso 4: Confirmado -->
    <div v-if="currentStep === 3" class="step-content confirmed-step">
      <div class="confirmed-card">
        <div class="confirmed-icon">✅</div>
        <h2 class="confirmed-title">¡Asignación confirmada!</h2>
        <p class="confirmed-sub">La solicitud #001 ha sido procesada exitosamente.</p>
        <div class="confirmed-details">
          <div class="confirmed-item">
            <span class="confirmed-label">Vehículo</span>
            <span class="confirmed-value">ABC-123 — Toyota Hiace</span>
          </div>
          <div class="confirmed-item">
            <span class="confirmed-label">Conductor</span>
            <span class="confirmed-value">Juan Pérez</span>
          </div>
          <div class="confirmed-item">
            <span class="confirmed-label">Fecha y hora</span>
            <span class="confirmed-value">29/05/2026 · 09:00 — 13:00</span>
          </div>
        </div>
        <div class="confirmed-actions">
          <router-link to="/requests" class="btn-back">Ver solicitudes</router-link>
          <router-link to="/calendar" class="btn-calendar">Ver en calendario</router-link>
        </div>
      </div>
    </div>

    <!-- Modal rechazar -->
    <div v-if="showRejectModal" class="modal-overlay" @click.self="showRejectModal = false">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title">Rechazar solicitud</h3>
          <button class="modal-close" @click="showRejectModal = false">✕</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label class="form-label">Motivo del rechazo <span class="required">*</span></label>
            <textarea v-model="rejectReason" class="form-textarea"
              placeholder="Explica por qué se rechaza esta solicitud..."
              maxlength="300" />
            <span class="char-count">{{ rejectReason.length }}/300</span>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn-cancel" @click="showRejectModal = false">Cancelar</button>
          <button class="btn-reject-confirm" :disabled="!rejectReason.trim()"
            @click="handleReject">
            Rechazar solicitud
          </button>
        </div>
      </div>
    </div>

    <!-- Modal confirmar asignación -->
    <div v-if="showConfirmModal" class="modal-overlay" @click.self="showConfirmModal = false">
      <div class="modal">
        <div class="modal-header">
          <div class="modal-title-row">
            <div class="modal-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18"
                viewBox="0 0 24 24" fill="none" stroke="#2563eb" stroke-width="2">
                <polyline points="17 1 21 5 17 9"/>
                <path d="M3 11V9a4 4 0 0 1 4-4h14"/>
                <polyline points="7 23 3 19 7 15"/>
                <path d="M21 13v2a4 4 0 0 1-4 4H3"/>
              </svg>
            </div>
            <div>
              <h3 class="modal-title">Confirmar asignación</h3>
              <p class="modal-subtitle">Solicitud #001 — Ventas · 29 mayo 2026</p>
            </div>
          </div>
          <button class="modal-close" @click="showConfirmModal = false">✕</button>
        </div>

        <div class="modal-body">
          <p class="modal-desc">
            Revisa los detalles antes de confirmar. Una vez asignados, los recursos
            quedarán bloqueados para este horario.
          </p>

          <!-- Fecha y destino -->
          <div class="confirm-date-row">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14"
              viewBox="0 0 24 24" fill="none" stroke="#6b7280" stroke-width="2">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
              <line x1="16" y1="2" x2="16" y2="6"/>
              <line x1="8" y1="2" x2="8" y2="6"/>
              <line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
            <span class="confirm-date">29/05/2026</span>
            <span class="confirm-time">09:00 — 13:00 h</span>
            <span class="confirm-dest">Destino: Sto. Domingo Este</span>
          </div>

          <!-- Vehículo y conductor -->
          <div class="confirm-resources">
            <div class="confirm-resource">
              <div class="resource-label">VEHÍCULO ASIGNADO</div>
              <div class="resource-value">
                <div class="resource-icon blue">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                    viewBox="0 0 24 24" fill="none" stroke="#2563eb" stroke-width="1.5">
                    <rect x="1" y="3" width="15" height="13" rx="2"/>
                    <path d="M16 8h4l3 3v5h-7V8z"/>
                    <circle cx="5.5" cy="18.5" r="2.5"/>
                    <circle cx="18.5" cy="18.5" r="2.5"/>
                  </svg>
                </div>
                <div>
                  <p class="resource-name">ABC-123</p>
                  <p class="resource-detail">Toyota Hiace · 12 pas.</p>
                </div>
              </div>
            </div>
            <div class="confirm-resource">
              <div class="resource-label">CONDUCTOR ASIGNADO</div>
              <div class="resource-value">
                <div class="driver-avatar sm">JP</div>
                <div>
                  <p class="resource-name">Juan Pérez</p>
                  <p class="resource-detail">Lic. B · Vigente</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Validaciones -->
          <div class="validations">
            <p class="validation-item green">✓ Todas las validaciones pasaron</p>
            <p class="validation-item green">✓ Vehículo disponible en ese horario</p>
            <p class="validation-item green">✓ Conductor disponible y sin conflictos</p>
            <p class="validation-item green">✓ Licencia vigente (79 días restantes)</p>
            <p class="validation-item green">✓ Capacidad suficiente (6 de 12 asientos)</p>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="showConfirmModal = false">Atrás</button>
          <button class="btn-submit" @click="handleConfirm">Confirmar asignación</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const currentStep = ref(1)
const showRejectModal = ref(false)
const showConfirmModal = ref(false)
const rejectReason = ref('')
const selectedVehicle = ref(1)
const selectedDriver = ref(1)

const steps = [
  { key: 'created',    label: 'Solicitud creada'    },
  { key: 'review',     label: 'Revisión supervisor' },
  { key: 'assignment', label: 'Asignación recursos' },
  { key: 'confirmed',  label: 'Confirmado'          },
]

const pendingRequests = ref([
  { id: 1, code: '#001', route: 'Ventas → Sto. Domingo Este', meta: '29/05 · 09:00-13:00 · 6 personas', time: 'Hace 23 min' },
  { id: 2, code: '#002', route: 'Operaciones → La Romana',    meta: '30/05 · 07:00-18:00 · 3 personas', time: 'Hace 1h'     },
  { id: 4, code: '#004', route: 'Legal → Santiago',           meta: '31/05 · 06:00-20:00 · 5 personas', time: 'Hace 2h'     },
])

const vehicles = ref([
  { id: 1, plate: 'ABC-123', name: 'Toyota Hiace',   type: 'Van',    capacity: 12, status: 'Disponible', warning: false },
  { id: 2, plate: 'PQR-678', name: 'Hyundai H1',     type: 'Van',    capacity: 9,  status: 'Disponible', warning: false },
  { id: 3, plate: 'JKL-012', name: 'Nissan Frontier', type: 'Pickup', capacity: 5,  status: 'Disponible', warning: true  },
])

const drivers = ref([
  { id: 1, name: 'Juan Pérez',      initials: 'JP', avatarColor: '#3b82f6', licenseType: 'B', licenseInfo: '79 días vigente',  warning: false },
  { id: 2, name: 'Ana Martínez',    initials: 'AM', avatarColor: '#8b5cf6', licenseType: 'B', licenseInfo: '238 días vigente', warning: false },
  { id: 3, name: 'Miguel Fernández',initials: 'MF', avatarColor: '#f59e0b', licenseType: 'C', licenseInfo: 'Vence en 12 días', warning: true  },
])

function getStepClass(index) {
  if (index < currentStep.value) return 'completed'
  if (index === currentStep.value) return 'active'
  return 'inactive'
}

function handleApprove() {
  currentStep.value = 2
}

function handleReject() {
  showRejectModal.value = false
  router.push('/requests')
}

function handleConfirm() {
  showConfirmModal.value = false
  currentStep.value = 3
}
</script>

<style scoped>
.request-flow {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  font-family: 'Inter', sans-serif;
}

/* Stepper */
.stepper {
  display: flex;
  align-items: center;
  gap: 0;
}

.step-wrapper {
  display: flex;
  align-items: center;
  flex: 1;
}

.step {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  white-space: nowrap;
}

.step-circle {
  width: 28px;
  height: 28px;
  min-width: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.78rem;
  font-weight: 700;
  border: 2px solid transparent;
}

.step-circle.completed { background: #16a34a; color: #fff; border-color: #16a34a; }
.step-circle.active    { background: #2563eb; color: #fff; border-color: #2563eb; }
.step-circle.inactive  { background: #fff; color: #9ca3af; border-color: #d1d5db; }

.step-label {
  font-size: 0.82rem;
  font-weight: 500;
}

.step-label.completed { color: #16a34a; }
.step-label.active    { color: #2563eb; font-weight: 600; }
.step-label.inactive  { color: #9ca3af; }

.step-line {
  flex: 1;
  height: 2px;
  background: #e5e7eb;
  margin: 0 0.75rem;
}

.step-line.completed { background: #16a34a; }

/* Contenido */
.step-content { display: flex; flex-direction: column; gap: 1rem; }

/* Two col */
.two-col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.25rem;
  align-items: flex-start;
}

/* Detail card */
.detail-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.card-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.detail-item { display: flex; flex-direction: column; gap: 2px; }
.detail-item.full { grid-column: 1 / -1; }

.detail-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.detail-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.detail-value.blue { color: #2563eb; }

/* Review actions */
.review-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.btn-approve {
  padding: 0.75rem;
  background: #f0fdf4;
  color: #16a34a;
  border: 1px solid #bbf7d0;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-approve:hover { background: #dcfce7; }

.btn-reject {
  padding: 0.75rem;
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-reject:hover { background: #fee2e2; }

/* Pending */
.pending-col { display: flex; flex-direction: column; gap: 0.75rem; }

.pending-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pending-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

.pending-link {
  font-size: 0.78rem;
  color: #2563eb;
  text-decoration: none;
}

.pending-list { display: flex; flex-direction: column; gap: 0.5rem; }

.pending-item {
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.pending-item:hover { border-color: #2563eb; }
.pending-item.selected { border-color: #2563eb; background: #eff6ff; }

.pending-item-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.4rem;
}

.pending-id {
  font-size: 0.78rem;
  font-weight: 600;
  color: #6b7280;
}

.pending-time {
  margin-left: auto;
  font-size: 0.72rem;
  color: #9ca3af;
}

.pending-route {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.pending-meta {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

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
  width: fit-content;
}

.badge.pendiente { background: #f5f3ff; color: #7c3aed; border-color: #ddd6fe; }

/* Approved banner */
.approved-banner {
  background: #f0fdf4;
  border: 1px solid #bbf7d0;
  border-radius: 8px;
  padding: 0.875rem 1.25rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #16a34a;
}

.approved-sub {
  font-weight: 400;
  color: #374151;
  flex: 1;
}

.btn-notify {
  padding: 0.5rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  cursor: pointer;
  margin-left: auto;
}

/* Col header */
.col-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.75rem;
}

.col-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
}

.col-sub {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 2px;
}

.available-badge {
  background: #f0fdf4;
  color: #16a34a;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 999px;
}

/* Options */
.options-list { display: flex; flex-direction: column; gap: 0.5rem; }

.option-card {
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 0.875rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  transition: all 0.2s;
}

.option-card:hover { border-color: #2563eb; }
.option-card.selected { border-color: #2563eb; background: #eff6ff; box-shadow: 0 0 0 1px #2563eb; }
.option-card.warning  { border-color: #fde68a; background: #fffbeb; }

.option-left {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.option-icon {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 8px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6b7280;
}

.option-icon.active { background: #dbeafe; color: #2563eb; }

.option-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.option-detail {
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 2px;
}

.option-confirm {
  font-size: 0.72rem;
  color: #16a34a;
  margin-top: 3px;
  font-weight: 500;
}

.warning-tag {
  display: inline-block;
  font-size: 0.7rem;
  font-weight: 500;
  color: #d97706;
  background: #fff7ed;
  border: 1px solid #fde68a;
  padding: 1px 6px;
  border-radius: 999px;
  margin-top: 3px;
}

.warning-tag.orange { color: #d97706; }

.check-circle {
  width: 24px;
  height: 24px;
  min-width: 24px;
  border-radius: 50%;
  background: #2563eb;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
}

.driver-avatar {
  width: 36px;
  height: 36px;
  min-width: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  color: #fff;
  background: #3b82f6;
}

.driver-avatar.sm {
  width: 36px;
  height: 36px;
  font-size: 0.72rem;
}

/* Assign footer */
.assign-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.5rem;
}

.btn-confirm {
  padding: 0.75rem 1.5rem;
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

.btn-confirm:hover:not(:disabled) { background: #1d4ed8; }
.btn-confirm:disabled { opacity: 0.5; cursor: not-allowed; }

/* Confirmed */
.confirmed-step {
  align-items: center;
  justify-content: center;
  min-height: 400px;
}

.confirmed-card {
  background: #fff;
  border-radius: 12px;
  border: 1px solid #f3f4f6;
  padding: 2.5rem;
  max-width: 480px;
  width: 100%;
  text-align: center;
  box-shadow: 0 4px 16px rgba(0,0,0,0.06);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.confirmed-icon { font-size: 3rem; }

.confirmed-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #111827;
}

.confirmed-sub {
  font-size: 0.875rem;
  color: #6b7280;
}

.confirmed-details {
  background: #f9fafb;
  border-radius: 8px;
  padding: 1rem;
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  text-align: left;
}

.confirmed-item {
  display: flex;
  justify-content: space-between;
  font-size: 0.875rem;
}

.confirmed-label { color: #9ca3af; }
.confirmed-value { font-weight: 600; color: #111827; }

.confirmed-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0.5rem;
}

.btn-back {
  padding: 0.6rem 1.25rem;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
  text-decoration: none;
}

.btn-calendar {
  padding: 0.6rem 1.25rem;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  text-decoration: none;
}

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
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #f3f4f6;
}

.modal-title-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.modal-icon {
  width: 36px;
  height: 36px;
  background: #eff6ff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-title {
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.modal-subtitle {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 1px;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1rem;
  color: #9ca3af;
  cursor: pointer;
}

.modal-body {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.modal-desc {
  font-size: 0.82rem;
  color: #6b7280;
  line-height: 1.5;
}

/* Confirm date row */
.confirm-date-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.82rem;
  color: #374151;
  background: #f9fafb;
  padding: 0.75rem;
  border-radius: 8px;
}

.confirm-date { font-weight: 700; color: #111827; }
.confirm-time { color: #6b7280; }
.confirm-dest { color: #6b7280; margin-left: auto; }

/* Confirm resources */
.confirm-resources {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  background: #f9fafb;
  border-radius: 8px;
  padding: 1rem;
}

.confirm-resource { display: flex; flex-direction: column; gap: 0.5rem; }

.resource-label {
  font-size: 0.65rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.05em;
}

.resource-value {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.resource-icon {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.resource-icon.blue { background: #dbeafe; }

.resource-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.resource-detail {
  font-size: 0.72rem;
  color: #9ca3af;
}

/* Validations */
.validations {
  background: #f0fdf4;
  border: 1px solid #bbf7d0;
  border-radius: 8px;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.validation-item {
  font-size: 0.82rem;
}

.validation-item.green { color: #16a34a; }

/* Form */
.form-group { display: flex; flex-direction: column; gap: 0.3rem; }

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
}

.required { color: #dc2626; }

.form-textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  outline: none;
  resize: vertical;
  min-height: 100px;
  transition: border-color 0.2s;
}

.form-textarea:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.char-count {
  font-size: 0.72rem;
  color: #9ca3af;
  text-align: right;
}

/* Modal footer */
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
}

.btn-submit:hover { background: #1d4ed8; }

.btn-reject-confirm {
  padding: 0.6rem 1.25rem;
  background: #dc2626;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
}

.btn-reject-confirm:hover:not(:disabled) { background: #b91c1c; }
.btn-reject-confirm:disabled { opacity: 0.5; cursor: not-allowed; }
</style>