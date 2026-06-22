<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { RequestsService } from '@/services/requests.service'
import { useToast } from '@/composables/useToast'
import { getErrorMessage } from '@/utils/apiError'
import { ArrowLeft } from '@lucide/vue'

const router = useRouter()
const toast = useToast()

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

function validate() {
  const errs = {}
  if (!form.area.trim()) errs.area = 'El área solicitante es requerida'
  else if (form.area.length > 150) errs.area = 'Máximo 150 caracteres'

  if (!form.passengers || form.passengers <= 0) errs.passengers = 'Debe ser al menos 1 colaborador'
  else if (form.passengers > 100) errs.passengers = 'No puede superar 100 personas'

  if (!form.date) errs.date = 'La fecha es requerida'

  if (!form.destination.trim()) errs.destination = 'El destino es requerido'
  else if (form.destination.length > 300) errs.destination = 'Máximo 300 caracteres'

  if (!form.reason.trim()) errs.reason = 'El motivo del viaje es requerido'
  else if (form.reason.length > 500) errs.reason = 'Máximo 500 caracteres'

  if (form.date && form.departureTime && form.returnTime) {
    const departure = new Date(`${form.date}T${form.departureTime}`)
    const returnAt = new Date(`${form.date}T${form.returnTime}`)
    if (departure <= new Date()) errs.date = 'La fecha y hora de salida debe ser en el futuro'
    if (returnAt <= departure) errs.returnTime = 'La hora de regreso debe ser posterior a la de salida'
  }

  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
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
  <div>
    <div class="page-header">
      <div style="display:flex;align-items:center;gap:10px">
        <button class="icon-btn" @click="router.push('/requests')">
          <ArrowLeft :size="16" />
        </button>
        <h1>Nueva solicitud de transporte</h1>
      </div>
    </div>

    <div class="card" style="max-width:680px">
      <div class="card-title">INFORMACIÓN DEL VIAJE</div>

      <form @submit.prevent="handleSubmit">
        <div v-if="errors.general" class="alert red">{{ errors.general }}</div>

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

          <div class="form-group full">
            <label>Destino <span class="required">*</span></label>
            <input
              v-model="form.destination"
              placeholder="Ciudad / dirección de destino"
              :class="{ 'input-error': errors.destination }"
            />
            <span v-if="errors.destination" class="field-error">{{ errors.destination }}</span>
          </div>

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
        </div>

        <div class="alert" style="background:var(--blue-light);border:1px solid var(--blue-mid);color:var(--blue-hover)">
          La solicitud quedará en estado <strong>Pendiente</strong> hasta ser aprobada por un supervisor.
        </div>

        <div class="form-actions">
          <button type="button" class="btn" @click="router.push('/requests')">Cancelar</button>
          <button type="submit" class="btn primary" :disabled="isSubmitting">
            {{ isSubmitting ? 'Enviando…' : 'Enviar solicitud' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
