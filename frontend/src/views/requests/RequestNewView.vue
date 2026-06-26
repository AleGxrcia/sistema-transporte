<script setup>
import { reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { RequestsService } from '@/services/requests.service'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { formatDate } from '@/utils/formatters'
import AppBreadcrumb from '@/components/ui/AppBreadcrumb.vue'
import { Check, Send } from '@lucide/vue'

const router = useRouter()
const toast = useToast()

const STEPS = ['Datos del viaje', 'Detalles', 'Confirmar']
const step = ref(1)

const form = reactive({
  area: '',
  passengers: '',
  date: '',
  departureTime: '08:00',
  returnTime: '17:00',
  destination: '',
  reason: '',
})

const errors = ref({})
const isSubmitting = ref(false)

function validateStep1() {
  const errs = {}
  if (!form.area.trim()) errs.area = 'El área solicitante es requerida'
  else if (form.area.length > 150) errs.area = 'Máximo 150 caracteres'

  if (!form.passengers || form.passengers <= 0) errs.passengers = 'Debe ser al menos 1 colaborador'
  else if (form.passengers > 100) errs.passengers = 'No puede superar 100 personas'

  if (!form.destination.trim()) errs.destination = 'El destino es requerido'
  else if (form.destination.length > 300) errs.destination = 'Máximo 300 caracteres'

  if (!form.date) errs.date = 'La fecha es requerida'

  if (form.date && form.departureTime && form.returnTime) {
    const departure = new Date(`${form.date}T${form.departureTime}`)
    const returnAt = new Date(`${form.date}T${form.returnTime}`)
    if (departure <= new Date()) errs.date = 'La fecha y hora de salida debe ser en el futuro'
    if (returnAt <= departure) errs.returnTime = 'La hora de regreso debe ser posterior a la de salida'
  }

  errors.value = errs
  return Object.keys(errs).length === 0
}

function validateStep2() {
  const errs = {}
  if (!form.reason.trim()) errs.reason = 'El motivo del viaje es requerido'
  else if (form.reason.length > 500) errs.reason = 'Máximo 500 caracteres'
  errors.value = errs
  return Object.keys(errs).length === 0
}

function next() {
  if (step.value === 1 && validateStep1()) step.value = 2
  else if (step.value === 2 && validateStep2()) step.value = 3
}

function back() {
  if (step.value > 1) step.value--
}

// Permite volver atrás libremente; avanzar solo si los pasos previos son válidos.
function goToStep(target) {
  if (target <= step.value) {
    step.value = target
    return
  }
  if (target >= 2 && !validateStep1()) {
    step.value = 1
    return
  }
  if (target >= 3 && !validateStep2()) {
    step.value = 2
    return
  }
  step.value = target
}

const summarySchedule = computed(() => {
  if (!form.date) return '—'
  return `${formatDate(form.date)} · ${form.departureTime}–${form.returnTime}`
})

async function handleSubmit() {
  if (!validateStep1()) { step.value = 1; return }
  if (!validateStep2()) { step.value = 2; return }
  try {
    isSubmitting.value = true
    await RequestsService.create({
      requestingArea: form.area,
      passengerCount: Number(form.passengers),
      destination: form.destination,
      departureDateTime: new Date(`${form.date}T${form.departureTime}`).toISOString(),
      returnDateTime: new Date(`${form.date}T${form.returnTime}`).toISOString(),
      tripPurpose: form.reason,
    })
    toast.success('Solicitud enviada', 'Quedará pendiente hasta ser revisada por un supervisor.')
    router.push('/requests')
  } catch (err) {
    errors.value.general = getErrorMessage(err, 'No se pudo enviar la solicitud')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div style="max-width:760px">
    <!-- Breadcrumb -->
    <AppBreadcrumb
      :items="[{ label: 'Solicitudes', to: '/requests' }, { label: 'Nueva solicitud' }]"
    />

    <!-- Stepper -->
    <div class="stepper">
      <template v-for="(label, i) in STEPS" :key="label">
        <div
          class="stepper-step"
          :class="{ active: step === i + 1, done: step > i + 1 }"
          @click="goToStep(i + 1)"
        >
          <div class="stepper-dot">
            <Check v-if="step > i + 1" :size="14" />
            <template v-else>{{ i + 1 }}</template>
          </div>
          <span class="stepper-label">{{ label }}</span>
        </div>
        <div v-if="i < STEPS.length - 1" class="stepper-line" :class="{ done: step > i + 1 }" />
      </template>
    </div>

    <div class="card">
      <div v-if="errors.general" class="alert red">{{ errors.general }}</div>

      <!-- PASO 1: Datos del viaje -->
      <form v-if="step === 1" @submit.prevent="next">
        <div class="step-title">Información del viaje</div>
        <div class="step-sub">La solicitud será revisada por un supervisor antes de aprobarse.</div>

        <div class="form-grid">
          <div class="form-group">
            <label>Área solicitante <span class="required">*</span></label>
            <input v-model="form.area" placeholder="Ej. Ventas" :class="{ 'input-error': errors.area }" />
            <span v-if="errors.area" class="field-error">{{ errors.area }}</span>
          </div>

          <div class="form-group">
            <label>Cantidad de colaboradores <span class="required">*</span></label>
            <input
              v-model.number="form.passengers"
              type="number"
              min="1"
              max="100"
              placeholder="0"
              :class="{ 'input-error': errors.passengers }"
            />
            <span v-if="errors.passengers" class="field-error">{{ errors.passengers }}</span>
          </div>

          <div class="form-group full">
            <label>Destino <span class="required">*</span></label>
            <input
              v-model="form.destination"
              placeholder="Ciudad / dirección de destino"
              :class="{ 'input-error': errors.destination }"
            />
            <span v-if="errors.destination" class="field-error">{{ errors.destination }}</span>
          </div>

          <div class="form-group">
            <label>Fecha del viaje <span class="required">*</span></label>
            <input v-model="form.date" type="date" :class="{ 'input-error': errors.date }" />
            <span v-if="errors.date" class="field-error">{{ errors.date }}</span>
          </div>

          <div class="form-group">
            <label>Hora de salida <span class="required">*</span></label>
            <input v-model="form.departureTime" type="time" />
          </div>

          <div class="form-group">
            <label>Hora estimada de regreso <span class="required">*</span></label>
            <input v-model="form.returnTime" type="time" :class="{ 'input-error': errors.returnTime }" />
            <span v-if="errors.returnTime" class="field-error">{{ errors.returnTime }}</span>
          </div>
        </div>

        <div class="form-actions">
          <button type="button" class="btn" @click="router.push('/requests')">Cancelar</button>
          <button type="submit" class="btn primary">Continuar →</button>
        </div>
      </form>

      <!-- PASO 2: Detalles -->
      <form v-else-if="step === 2" @submit.prevent="next">
        <div class="step-title">Detalles del viaje</div>
        <div class="step-sub">Describe el propósito para facilitar la aprobación.</div>

        <div class="form-group full">
          <label>Motivo del viaje <span class="required">*</span></label>
          <textarea
            v-model="form.reason"
            placeholder="Describe brevemente el propósito del viaje institucional…"
            maxlength="500"
            :class="{ 'input-error': errors.reason }"
          />
          <span v-if="errors.reason" class="field-error">{{ errors.reason }}</span>
        </div>

        <div class="form-actions" style="justify-content:space-between">
          <button type="button" class="btn" @click="back">← Atrás</button>
          <button type="submit" class="btn primary">Continuar →</button>
        </div>
      </form>

      <!-- PASO 3: Confirmar -->
      <div v-else>
        <div class="step-title">Confirmar solicitud</div>
        <div class="step-sub">Revisa los datos antes de enviar.</div>

        <div class="summary">
          <div class="summary-item">
            <div class="field-label">Área</div>
            <div class="field-value">{{ form.area }}</div>
          </div>
          <div class="summary-item">
            <div class="field-label">Colaboradores</div>
            <div class="field-value">{{ form.passengers }}</div>
          </div>
          <div class="summary-item">
            <div class="field-label">Destino</div>
            <div class="field-value">{{ form.destination }}</div>
          </div>
          <div class="summary-item">
            <div class="field-label">Fecha y horario</div>
            <div class="field-value">{{ summarySchedule }}</div>
          </div>
          <div class="summary-item" style="grid-column:1/-1">
            <div class="field-label">Motivo</div>
            <div class="field-value" style="font-weight:500;color:var(--text-2)">{{ form.reason }}</div>
          </div>
        </div>

        <div class="alert" style="background:var(--blue-light);border:1px solid var(--blue-mid);color:var(--blue-hover);margin-top:16px">
          Quedará en estado <strong>Pendiente</strong> hasta ser aprobada por un supervisor.
          Recibirás una notificación con el resultado.
        </div>

        <div class="form-actions" style="justify-content:space-between">
          <button type="button" class="btn" @click="back">← Atrás</button>
          <button type="button" class="btn primary" :disabled="isSubmitting" @click="handleSubmit">
            <Send :size="15" /> {{ isSubmitting ? 'Enviando…' : 'Enviar solicitud' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.stepper {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}
.stepper-step {
  display: flex;
  align-items: center;
  gap: 9px;
  cursor: pointer;
}
.stepper-dot {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 600;
  background: var(--white);
  border: 1.5px solid var(--border-strong);
  color: var(--text-3);
  flex-shrink: 0;
  transition: all 0.15s;
}
.stepper-step.active .stepper-dot {
  background: var(--blue);
  border-color: var(--blue);
  color: #fff;
}
.stepper-step.done .stepper-dot {
  background: var(--mint-dark);
  border-color: var(--mint-dark);
  color: #fff;
}
.stepper-label {
  font-size: 12.5px;
  font-weight: 500;
  color: var(--text-3);
  white-space: nowrap;
}
.stepper-step.active .stepper-label {
  color: var(--text);
  font-weight: 600;
}
.stepper-line {
  flex: 1;
  height: 2px;
  background: var(--border);
  margin: 0 12px;
}
.stepper-line.done {
  background: var(--mint-dark);
}

.step-title {
  font-size: 15px;
  font-weight: 600;
  color: var(--text);
  margin-bottom: 4px;
}
.step-sub {
  font-size: 12.5px;
  color: var(--text-3);
  margin-bottom: 20px;
}

.summary {
  background: var(--surface-hover);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 18px;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.field-label {
  font-size: 11px;
  color: var(--text-3);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 4px;
}
.field-value {
  font-size: 13.5px;
  font-weight: 600;
  color: var(--text);
}
@media (max-width: 560px) {
  .summary { grid-template-columns: 1fr; }
  .stepper-label { display: none; }
}
</style>
