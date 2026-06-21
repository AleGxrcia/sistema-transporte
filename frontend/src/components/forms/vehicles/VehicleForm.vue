<script setup>
import { reactive, ref } from 'vue'
import { VehiclesService } from '@/services/vehicles.service'
import { getErrorMessage } from '@/utils/apiError'
import { VEHICLE_TYPES, vehicleTypeValueByName } from '@/utils/enumLabels'

const props = defineProps({
  vehicle: { type: Object, default: null },
})

const emit = defineEmits(['saved', 'cancel'])

const isEditing = !!props.vehicle
const currentYear = new Date().getFullYear()

const form = reactive({
  brand: props.vehicle?.brand || '',
  model: props.vehicle?.model || '',
  year: props.vehicle?.year || currentYear,
  licensePlate: props.vehicle?.licensePlate || '',
  color: props.vehicle?.color || '',
  type: isEditing ? vehicleTypeValueByName(props.vehicle?.type) : null,
  capacity: props.vehicle?.capacity || 1,
})

const isSubmitting = ref(false)
const errors = ref({})

function validate() {
  const errs = {}
  if (!form.brand.trim()) errs.brand = 'La marca es requerida'
  else if (form.brand.length > 100) errs.brand = 'Máximo 100 caracteres'

  if (!form.model.trim()) errs.model = 'El modelo es requerido'
  else if (form.model.length > 100) errs.model = 'Máximo 100 caracteres'

  if (!isEditing) {
    if (!form.licensePlate.trim()) errs.licensePlate = 'La matrícula es requerida'
    else if (form.licensePlate.length > 8) errs.licensePlate = 'Máximo 8 caracteres'

    if (form.year < 1990 || form.year > currentYear + 1) {
      errs.year = `El año debe estar entre 1990 y ${currentYear + 1}`
    }
    if (form.capacity < 1 || form.capacity > 100) {
      errs.capacity = 'La capacidad debe estar entre 1 y 100'
    }
  }

  if (form.color && form.color.length > 50) errs.color = 'Máximo 50 caracteres'
  if (form.type === null || form.type === undefined) errs.type = 'Selecciona un tipo'

  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
  try {
    isSubmitting.value = true
    if (isEditing) {
      await VehiclesService.update(props.vehicle.id, {
        brand: form.brand,
        model: form.model,
        color: form.color,
        type: form.type,
      })
    } else {
      await VehiclesService.create({
        licensePlate: form.licensePlate,
        capacity: form.capacity,
        brand: form.brand,
        model: form.model,
        year: form.year,
        color: form.color,
        type: form.type,
      })
    }
    emit('saved')
  } catch (error) {
    if (error.response?.status === 409) {
      errors.value.licensePlate = 'Esta matrícula ya está registrada'
    } else {
      errors.value.general = getErrorMessage(error, 'Error al guardar. Intenta de nuevo.')
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <div v-if="errors.general" class="alert red">
      {{ errors.general }}
    </div>

    <div class="form-grid">
      <div class="form-group">
        <label>Marca <span class="required">*</span></label>
        <input v-model="form.brand" placeholder="Toyota" :class="{ 'input-error': errors.brand }" />
        <span v-if="errors.brand" class="field-error">{{ errors.brand }}</span>
      </div>

      <div class="form-group">
        <label>Modelo <span class="required">*</span></label>
        <input v-model="form.model" placeholder="Hilux" :class="{ 'input-error': errors.model }" />
        <span v-if="errors.model" class="field-error">{{ errors.model }}</span>
      </div>

      <div class="form-group">
        <label>Año</label>
        <input
          v-model.number="form.year"
          type="number"
          :min="1990"
          :max="currentYear + 1"
          :disabled="isEditing"
          :class="{ 'input-error': errors.year }"
        />
        <span v-if="errors.year" class="field-error">{{ errors.year }}</span>
      </div>

      <div class="form-group">
        <label>Matrícula <span class="required">*</span></label>
        <input
          v-model="form.licensePlate"
          placeholder="ABC-123"
          maxlength="8"
          :disabled="isEditing"
          :class="{ 'input-error': errors.licensePlate }"
        />
        <span v-if="errors.licensePlate" class="field-error">{{ errors.licensePlate }}</span>
      </div>

      <div class="form-group">
        <label>Color</label>
        <input v-model="form.color" placeholder="Blanco" maxlength="50" :class="{ 'input-error': errors.color }" />
        <span v-if="errors.color" class="field-error">{{ errors.color }}</span>
      </div>

      <div class="form-group">
        <label>Tipo de vehículo <span class="required">*</span></label>
        <select v-model.number="form.type" :class="{ 'input-error': errors.type }">
          <option :value="null" disabled>Seleccionar tipo</option>
          <option v-for="t in VEHICLE_TYPES" :key="t.value" :value="t.value">
            {{ t.label }}
          </option>
        </select>
        <span v-if="errors.type" class="field-error">{{ errors.type }}</span>
      </div>

      <div class="form-group">
        <label>Capacidad (pasajeros)</label>
        <input
          v-model.number="form.capacity"
          type="number"
          min="1"
          max="100"
          :disabled="isEditing"
          :class="{ 'input-error': errors.capacity }"
        />
        <span v-if="errors.capacity" class="field-error">{{ errors.capacity }}</span>
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn" @click="emit('cancel')">Cancelar</button>
      <button type="submit" class="btn primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Guardando…' : isEditing ? 'Actualizar' : 'Registrar vehículo' }}
      </button>
    </div>
  </form>
</template>
