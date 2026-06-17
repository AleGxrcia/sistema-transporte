<template>
  <div class="new-request">

    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <router-link to="/requests" class="breadcrumb-link">← Solicitudes</router-link>
      <span class="breadcrumb-sep">/</span>
      <span class="breadcrumb-current">Nueva solicitud</span>
    </div>

    <h2 class="page-title">Solicitud de transporte</h2>
    <p class="page-subtitle">Completa los datos del viaje. Será revisada por un supervisor antes de ser aprobada.</p>

    <div class="form-card">
      <div class="form-section-title">INFORMACIÓN DEL VIAJE</div>

      <div class="form-grid">

        <!-- Área solicitante -->
        <div class="form-group">
          <label class="form-label">Área solicitante</label>
          <select v-model="form.area" class="form-select">
            <option value="">Seleccionar área</option>
            <option v-for="area in areas" :key="area" :value="area">{{ area }}</option>
          </select>
        </div>

        <!-- Cantidad de colaboradores -->
        <div class="form-group">
          <label class="form-label">Cantidad de colaboradores</label>
          <input v-model="form.passengers" type="number" min="1"
            class="form-input" placeholder="0" />
        </div>

        <!-- Fecha del viaje -->
        <div class="form-group">
          <label class="form-label">Fecha del viaje</label>
          <input v-model="form.date" type="date" class="form-input" />
        </div>

        <!-- Vehículo asignado -->
        <div class="form-group">
          <label class="form-label">Vehículo asignado</label>
          <input type="text" class="form-input" placeholder="Pendiente de asignación" disabled />
        </div>

        <!-- Hora de salida -->
        <div class="form-group">
          <label class="form-label">Hora de salida</label>
          <input v-model="form.departureTime" type="time" class="form-input" />
        </div>

        <!-- Hora de regreso -->
        <div class="form-group">
          <label class="form-label">Hora estimada de regreso</label>
          <input v-model="form.returnTime" type="time" class="form-input" />
        </div>

        <!-- Destino -->
        <div class="form-group full-width">
          <label class="form-label">Destino</label>
          <input v-model="form.destination" type="text"
            class="form-input" placeholder="Ciudad / dirección de destino" />
        </div>

        <!-- Motivo -->
        <div class="form-group full-width">
          <label class="form-label">Motivo del viaje</label>
          <textarea v-model="form.reason" class="form-textarea"
            placeholder="Describe brevemente el propósito del viaje institucional..." />
        </div>

      </div>

      <!-- Info -->
      <div class="info-banner">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
          viewBox="0 0 24 24" fill="none" stroke="#2563eb" stroke-width="2">
          <circle cx="12" cy="12" r="10"/>
          <line x1="12" y1="8" x2="12" y2="12"/>
          <line x1="12" y1="16" x2="12.01" y2="16"/>
        </svg>
        <span>
          La solicitud quedará en estado <strong>Pendiente</strong> hasta ser aprobada por un supervisor.
          Recibirás una notificación con el resultado.
        </span>
      </div>

      <!-- Botones -->
      <div class="form-actions">
        <router-link to="/requests" class="btn-cancel">Cancelar</router-link>
        <button class="btn-submit" @click="handleSubmit" :disabled="loading">
          {{ loading ? 'Enviando...' : 'Enviar solicitud' }}
        </button>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)

const areas = [
  'Ventas', 'Operaciones', 'RRHH', 'Finanzas',
  'Legal', 'Marketing', 'Tecnología', 'Gerencia'
]

const form = reactive({
  area: '',
  passengers: '',
  date: '',
  departureTime: '08:00',
  returnTime: '17:00',
  destination: '',
  reason: ''
})

async function handleSubmit() {
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
.new-request {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  font-family: 'Inter', sans-serif;
  max-width: 760px;
}

/* Breadcrumb */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.82rem;
}

.breadcrumb-link {
  color: #2563eb;
  text-decoration: none;
}

.breadcrumb-link:hover { text-decoration: underline; }
.breadcrumb-sep { color: #d1d5db; }
.breadcrumb-current { color: #6b7280; }

.page-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
}

.page-subtitle {
  font-size: 0.82rem;
  color: #6b7280;
  margin-top: -0.5rem;
}

/* Card del formulario */
.form-card {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.form-section-title {
  font-size: 0.7rem;
  font-weight: 600;
  color: #9ca3af;
  letter-spacing: 0.08em;
  margin-bottom: 1.25rem;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.25rem;
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

.form-input,
.form-select {
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

.form-input:focus,
.form-select:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.form-input:disabled {
  background: #f9fafb;
  color: #9ca3af;
  cursor: not-allowed;
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
  min-height: 100px;
  transition: border-color 0.2s;
}

.form-textarea:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

/* Info banner */
.info-banner {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  background: #eff6ff;
  border: 1px solid #bfdbfe;
  border-radius: 8px;
  padding: 0.875rem 1rem;
  font-size: 0.82rem;
  color: #1e40af;
  margin-bottom: 1.25rem;
  line-height: 1.5;
}

/* Botones */
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
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
  text-decoration: none;
  display: flex;
  align-items: center;
  transition: background 0.2s;
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