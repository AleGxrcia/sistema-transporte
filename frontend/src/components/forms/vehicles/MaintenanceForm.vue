<script setup>
import { reactive, ref } from 'vue'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'
import { MAINTENANCE_TYPES, maintenanceTypeValueByName } from '@/utils/enumLabels'

const props = defineProps({
  vehicleId: { type: String, required: true },
  record: { type: Object, default: null },
})
const emit = defineEmits(['saved', 'cancel'])

const isEditing = !!props.record

const toDateInput = (d) => (d ? new Date(d).toISOString().split('T')[0] : '')

const form = reactive({
  type: props.record ? maintenanceTypeValueByName(props.record.type) : null,
  description: props.record?.description ?? '',
  entryDate: toDateInput(props.record?.entryDate),
  workshop: props.record?.workshop ?? '',
  estimatedExitDate: toDateInput(props.record?.estimatedExitDate),
  nextMaintenanceDateScheduled: toDateInput(props.record?.nextMaintenanceDateScheduled),
  nextMaintenanceKmScheduled: props.record?.nextMaintenanceKmScheduled ?? null,
})

const isSubmitting = ref(false)
const errors = ref({})

function validate() {
  const errs = {}
  if (form.type === null || form.type === undefined) errs.type = 'El tipo es requerido'
  if (!form.description.trim()) errs.description = 'La descripción es requerida'
  else if (form.description.length > 500) errs.description = 'Máximo 500 caracteres'
  if (!form.entryDate) errs.entryDate = 'La fecha de entrada es requerida'
  if (!form.workshop.trim()) errs.workshop = 'El taller es requerido'
  else if (form.workshop.length > 200) errs.workshop = 'Máximo 200 caracteres'
  if (form.nextMaintenanceKmScheduled !== null && form.nextMaintenanceKmScheduled <= 0) {
    errs.nextMaintenanceKmScheduled = 'Debe ser mayor a 0'
  }
  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
  try {
    isSubmitting.value = true
    const payload = {
      type: form.type,
      description: form.description,
      entryDate: form.entryDate,
      workshop: form.workshop || '',
      estimatedExitDate: form.estimatedExitDate || null,
      nextMaintenanceDateScheduled: form.nextMaintenanceDateScheduled || null,
      nextMaintenanceKmScheduled: form.nextMaintenanceKmScheduled || null,
    }
    if (isEditing) {
      await VehiclesService.updateMaintenance(props.vehicleId, props.record.id, payload)
    } else {
      await VehiclesService.registerMaintenance(props.vehicleId, payload)
    }
    emit('saved')
  } catch (err) {
    errors.value.general = getErrorMessage(err, 'Error al guardar el mantenimiento')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <div v-if="errors.general" class="alert red">{{ errors.general }}</div>

    <div class="form-grid">
      <div class="form-group">
        <label>Tipo <span class="required">*</span></label>
        <select v-model.number="form.type" :class="{ 'input-error': errors.type }">
          <option :value="null" disabled>Seleccionar tipo</option>
          <option v-for="t in MAINTENANCE_TYPES" :key="t.value" :value="t.value">{{ t.label }}</option>
        </select>
        <span v-if="errors.type" class="field-error">{{ errors.type }}</span>
      </div>

      <div class="form-group">
        <label>Fecha de entrada <span class="required">*</span></label>
        <input v-model="form.entryDate" type="date" :class="{ 'input-error': errors.entryDate }" />
        <span v-if="errors.entryDate" class="field-error">{{ errors.entryDate }}</span>
      </div>

      <div class="form-group" style="grid-column: 1 / -1">
        <label>Descripción <span class="required">*</span></label>
        <textarea
          v-model="form.description"
          rows="3"
          placeholder="Describe el trabajo a realizar…"
          :class="{ 'input-error': errors.description }"
        />
        <span v-if="errors.description" class="field-error">{{ errors.description }}</span>
      </div>

      <div class="form-group">
        <label>Taller <span class="required">*</span></label>
        <input v-model="form.workshop" placeholder="Nombre del taller" :class="{ 'input-error': errors.workshop }" />
        <span v-if="errors.workshop" class="field-error">{{ errors.workshop }}</span>
      </div>

      <div class="form-group">
        <label>Fecha estimada de salida</label>
        <input v-model="form.estimatedExitDate" type="date" />
      </div>

      <div class="form-group">
        <label>Próximo mantenimiento (fecha)</label>
        <input v-model="form.nextMaintenanceDateScheduled" type="date" />
      </div>

      <div class="form-group">
        <label>Próximo mantenimiento (km)</label>
        <input
          v-model.number="form.nextMaintenanceKmScheduled"
          type="number"
          min="0"
          placeholder="150000"
          :class="{ 'input-error': errors.nextMaintenanceKmScheduled }"
        />
        <span v-if="errors.nextMaintenanceKmScheduled" class="field-error">{{ errors.nextMaintenanceKmScheduled }}</span>
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn" @click="emit('cancel')">Cancelar</button>
      <button type="submit" class="btn primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Guardando…' : isEditing ? 'Actualizar mantenimiento' : 'Registrar mantenimiento' }}
      </button>
    </div>
  </form>
</template>
