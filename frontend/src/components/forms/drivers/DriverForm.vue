<script setup>
import { reactive, ref, onMounted } from 'vue'
import { driverApi } from '@/services/drivers.service'
import { getErrorMessage } from '@/utils/apiError'
import { LICENSE_CATEGORIES } from '@/utils/enumLabels'

const props = defineProps({
  driver: { type: Object, default: null },
})
const emit = defineEmits(['saved', 'cancel'])

const isEditing = !!props.driver

const form = reactive(
  isEditing
    ? {
        firstName: props.driver.firstName || '',
        lastName: props.driver.lastName || '',
        phone: props.driver.phone || '',
        address: props.driver.address || '',
        supervisorId: props.driver.supervisorId || null,
      }
    : {
        firstName: '',
        lastName: '',
        nationalId: '',
        phone: '',
        address: '',
        licenseNumber: '',
        licenseCategory: null,
        licenseExpirationDate: '',
        supervisorId: null,
      }
)

const supervisors = ref([])
const isSubmitting = ref(false)
const errors = ref({})

onMounted(async () => {
  try {
    const { data } = await driverApi.list()
    supervisors.value = data.filter((d) => d.id !== props.driver?.id)
  } catch {
  }
})

function validate() {
  const errs = {}

  if (!form.firstName?.trim()) errs.firstName = 'El nombre es requerido'
  else if (form.firstName.length > 100) errs.firstName = 'Máximo 100 caracteres'

  if (!form.lastName?.trim()) errs.lastName = 'El apellido es requerido'
  else if (form.lastName.length > 100) errs.lastName = 'Máximo 100 caracteres'

  if (!isEditing) {
    if (!/^\d{11}$/.test(form.nationalId?.trim() || '')) {
      errs.nationalId = 'La cédula debe tener exactamente 11 dígitos'
    }
    if (!form.licenseNumber?.trim()) errs.licenseNumber = 'El número de licencia es requerido'
    if (form.licenseCategory === null || form.licenseCategory === undefined) {
      errs.licenseCategory = 'La categoría es requerida'
    }
    if (!form.licenseExpirationDate) {
      errs.licenseExpirationDate = 'La fecha de vencimiento es requerida'
    } else if (new Date(form.licenseExpirationDate) <= new Date()) {
      errs.licenseExpirationDate = 'La licencia debe estar vigente'
    }
  }
  if (!form.phone?.trim()) errs.phone = 'El teléfono es requerido'
  else if (form.phone.length > 20) errs.phone = 'Máximo 20 caracteres'

  errors.value = errs
  return Object.keys(errs).length === 0
}

async function handleSubmit() {
  if (!validate()) return
  try {
    isSubmitting.value = true
    if (isEditing) {
      await driverApi.update(props.driver.id, {
        firstName: form.firstName,
        lastName: form.lastName,
        phone: form.phone,
        address: form.address || null,
        supervisorId: form.supervisorId || null,
      })
    } else {
      await driverApi.create({
        firstName: form.firstName,
        lastName: form.lastName,
        nationalId: form.nationalId,
        licenseNumber: form.licenseNumber,
        licenseCategory: form.licenseCategory,
        licenseExpirationDate: form.licenseExpirationDate,
        phone: form.phone,
        address: form.address || null,
        supervisorId: form.supervisorId || null,
      })
    }
    emit('saved')
  } catch (error) {
    if (error.response?.status === 409) {
      errors.value.nationalId = 'Esta cédula ya está registrada'
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
    <div v-if="errors.general" class="alert red">{{ errors.general }}</div>

    <div class="form-grid">
      <div class="form-group">
        <label>Nombre <span class="required">*</span></label>
        <input v-model="form.firstName" placeholder="Juan" :class="{ 'input-error': errors.firstName }" />
        <span v-if="errors.firstName" class="field-error">{{ errors.firstName }}</span>
      </div>

      <div class="form-group">
        <label>Apellido <span class="required">*</span></label>
        <input v-model="form.lastName" placeholder="Pérez" :class="{ 'input-error': errors.lastName }" />
        <span v-if="errors.lastName" class="field-error">{{ errors.lastName }}</span>
      </div>

      <template v-if="!isEditing">
        <div class="form-group">
          <label>Cédula <span class="required">*</span></label>
          <input
            v-model="form.nationalId"
            placeholder="00112345678"
            maxlength="11"
            :class="{ 'input-error': errors.nationalId }"
          />
          <span v-if="errors.nationalId" class="field-error">{{ errors.nationalId }}</span>
        </div>

        <div class="form-group" style="grid-column: 1 / -1">
          <div class="section-label">Licencia de conducir</div>
        </div>

        <div class="form-group">
          <label>Número de licencia <span class="required">*</span></label>
          <input v-model="form.licenseNumber" placeholder="LIC-123456" :class="{ 'input-error': errors.licenseNumber }" />
          <span v-if="errors.licenseNumber" class="field-error">{{ errors.licenseNumber }}</span>
        </div>

        <div class="form-group">
          <label>Categoría <span class="required">*</span></label>
          <select v-model.number="form.licenseCategory" :class="{ 'input-error': errors.licenseCategory }">
            <option :value="null" disabled>Seleccionar categoría</option>
            <option v-for="c in LICENSE_CATEGORIES" :key="c.value" :value="c.value">{{ c.label }}</option>
          </select>
          <span v-if="errors.licenseCategory" class="field-error">{{ errors.licenseCategory }}</span>
        </div>

        <div class="form-group">
          <label>Vencimiento de licencia <span class="required">*</span></label>
          <input v-model="form.licenseExpirationDate" type="date" :class="{ 'input-error': errors.licenseExpirationDate }" />
          <span v-if="errors.licenseExpirationDate" class="field-error">{{ errors.licenseExpirationDate }}</span>
        </div>
      </template>

      <div class="form-group">
        <label>Teléfono <span class="required">*</span></label>
        <input v-model="form.phone" placeholder="809-555-0000" :class="{ 'input-error': errors.phone }" />
        <span v-if="errors.phone" class="field-error">{{ errors.phone }}</span>
      </div>

      <div class="form-group" :style="isEditing ? '' : 'grid-column: 1 / -1'">
        <label>Dirección</label>
        <input v-model="form.address" placeholder="Santo Domingo" />
      </div>

      <div v-if="supervisors.length > 0" class="form-group" style="grid-column: 1 / -1">
        <label>Supervisor asignado</label>
        <select v-model="form.supervisorId">
          <option :value="null">Sin supervisor</option>
          <option v-for="sup in supervisors" :key="sup.id" :value="sup.id">
            {{ sup.firstName }} {{ sup.lastName }}
          </option>
        </select>
      </div>
    </div>

    <div class="form-actions">
      <button type="button" class="btn" @click="emit('cancel')">Cancelar</button>
      <button type="submit" class="btn primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Guardando…' : isEditing ? 'Actualizar' : 'Registrar conductor' }}
      </button>
    </div>
  </form>
</template>

<style scoped>
.section-label {
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: .06em;
  color: var(--text-3);
  padding-bottom: 4px;
  border-bottom: 1px solid var(--border);
}
</style>
