<script setup>
import { reactive, ref } from 'vue'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'
import { LICENSE_CATEGORIES, licenseCategoryValueByName } from '@/utils/enumLabels'

const props = defineProps({
  driverId: { type: String, required: true },
  currentLicense: { type: Object, default: null },
})
const emit = defineEmits(['saved', 'cancel'])

const form = reactive({
  licenseNumber: props.currentLicense?.licenseNumber || '',
  category: licenseCategoryValueByName(props.currentLicense?.licenseType),
  expirationDate: '',
})

const isSubmitting = ref(false)
const errors = ref({})

function validate() {
  const errs = {}
  if (!form.licenseNumber.trim()) errs.licenseNumber = 'El número de licencia es requerido'
  else if (form.licenseNumber.length > 50) errs.licenseNumber = 'Máximo 50 caracteres'
  if (form.category === null || form.category === undefined) errs.category = 'La categoría es requerida'
  if (!form.expirationDate) errs.expirationDate = 'La fecha de vencimiento es requerida'
  else if (new Date(form.expirationDate) <= new Date()) {
    errs.expirationDate = 'La fecha de vencimiento debe ser futura'
  }
  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
  try {
    isSubmitting.value = true
    await driverApi.renewLicense(props.driverId, {
      licenseNumber: form.licenseNumber,
      category: form.category,
      expirationDate: form.expirationDate,
    })
    emit('saved')
  } catch (err) {
    errors.value.general = getErrorMessage(err, 'Error al renovar la licencia')
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
        <label>Número de licencia <span class="required">*</span></label>
        <input v-model="form.licenseNumber" placeholder="LIC-123456" :class="{ 'input-error': errors.licenseNumber }" />
        <span v-if="errors.licenseNumber" class="field-error">{{ errors.licenseNumber }}</span>
      </div>

      <div class="form-group">
        <label>Categoría <span class="required">*</span></label>
        <select v-model.number="form.category" :class="{ 'input-error': errors.category }">
          <option :value="null" disabled>Seleccionar</option>
          <option v-for="c in LICENSE_CATEGORIES" :key="c.value" :value="c.value">{{ c.label }}</option>
        </select>
        <span v-if="errors.category" class="field-error">{{ errors.category }}</span>
      </div>

      <div class="form-group">
        <label>Nueva fecha de vencimiento <span class="required">*</span></label>
        <input v-model="form.expirationDate" type="date" :class="{ 'input-error': errors.expirationDate }" />
        <span v-if="errors.expirationDate" class="field-error">{{ errors.expirationDate }}</span>
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn" @click="emit('cancel')">Cancelar</button>
      <button type="submit" class="btn primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Guardando…' : 'Renovar licencia' }}
      </button>
    </div>
  </form>
</template>
