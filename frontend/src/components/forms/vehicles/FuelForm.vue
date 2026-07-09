<script setup>
import { reactive, ref, computed } from 'vue'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'
import { formatCurrency } from '@/utils/formatters'

const props = defineProps({
  vehicleId: { type: String, required: true },
  record: { type: Object, default: null },
})
const emit = defineEmits(['saved', 'cancel'])

const isEditing = !!props.record

const form = reactive({
  recordDate: props.record?.recordDate
    ? new Date(props.record.recordDate).toISOString().split('T')[0]
    : new Date().toISOString().split('T')[0],
  gallons: props.record?.gallons ?? null,
  pricePerGallon: props.record?.pricePerGallon ?? null,
  mileageAtRefuel: props.record?.mileageAtRefuel ?? null,
  notes: props.record?.notes ?? '',
})

const isSubmitting = ref(false)
const errors = ref({})

const totalCost = computed(() => {
  if (form.gallons && form.pricePerGallon) {
    return form.gallons * form.pricePerGallon
  }
  return null
})

function validate() {
  const errs = {}
  if (!form.recordDate) errs.recordDate = 'La fecha es requerida'
  if (!form.gallons || form.gallons <= 0) errs.gallons = 'Ingresa la cantidad de galones'
  if (!form.pricePerGallon || form.pricePerGallon <= 0) errs.pricePerGallon = 'Ingresa el precio por galón'
  if (form.mileageAtRefuel !== null && form.mileageAtRefuel < 0) errs.mileageAtRefuel = 'No puede ser negativo'
  if (form.notes && form.notes.length > 300) errs.notes = 'Máximo 300 caracteres'
  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
  try {
    isSubmitting.value = true
    const payload = {
      recordDate: form.recordDate,
      gallons: form.gallons,
      pricePerGallon: form.pricePerGallon,
      mileageAtRefuel: form.mileageAtRefuel || 0,
      notes: form.notes || null,
    }
    if (isEditing) {
      await VehiclesService.updateFuel(props.vehicleId, props.record.id, payload)
    } else {
      await VehiclesService.registerFuel(props.vehicleId, payload)
    }
    emit('saved')
  } catch (err) {
    errors.value.general = getErrorMessage(err, 'Error al guardar el registro de combustible')
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
        <label>Fecha <span class="required">*</span></label>
        <input v-model="form.recordDate" type="date" :class="{ 'input-error': errors.recordDate }" />
        <span v-if="errors.recordDate" class="field-error">{{ errors.recordDate }}</span>
      </div>

      <div class="form-group">
        <label>Galones <span class="required">*</span></label>
        <input
          v-model.number="form.gallons"
          type="number"
          min="0"
          step="0.01"
          placeholder="0.00"
          :class="{ 'input-error': errors.gallons }"
        />
        <span v-if="errors.gallons" class="field-error">{{ errors.gallons }}</span>
      </div>

      <div class="form-group">
        <label>Precio por galón <span class="required">*</span></label>
        <input
          v-model.number="form.pricePerGallon"
          type="number"
          min="0"
          step="0.01"
          placeholder="0.00"
          :class="{ 'input-error': errors.pricePerGallon }"
        />
        <span v-if="errors.pricePerGallon" class="field-error">{{ errors.pricePerGallon }}</span>
      </div>

      <div class="form-group">
        <label>Kilometraje al cargar</label>
        <input v-model.number="form.mileageAtRefuel" type="number" min="0" placeholder="0" />
      </div>

      <!-- Resumen de costo -->
      <div v-if="totalCost !== null" class="form-group" style="grid-column: 1 / -1">
        <div class="cost-summary">
          <span>Costo total estimado</span>
          <strong>{{ formatCurrency(totalCost) }}</strong>
        </div>
      </div>

      <div class="form-group" style="grid-column: 1 / -1">
        <label>Notas</label>
        <textarea v-model="form.notes" rows="2" placeholder="Gasolinera, observaciones…" />
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn" @click="emit('cancel')">Cancelar</button>
      <button type="submit" class="btn primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Guardando…' : isEditing ? 'Actualizar carga' : 'Registrar carga' }}
      </button>
    </div>
  </form>
</template>

<style scoped>
.cost-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 14px;
  background: var(--blue-light, #eff6ff);
  border: 1px solid var(--sky-border, #bae6fd);
  border-radius: 8px;
  font-size: 14px;
  color: var(--text-2);
}
.cost-summary strong {
  font-size: 16px;
  color: var(--blue, #2563eb);
}
</style>