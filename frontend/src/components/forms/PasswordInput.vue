<template>
  <div class="password-field">
    <span class="field-icon">
      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
        viewBox="0 0 24 24" fill="none" stroke="currentColor"
        stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
        <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
      </svg>
    </span>

    <input
      :type="show ? 'text' : 'password'"
      :value="modelValue"
      :placeholder="placeholder"
      class="field-input"
      @input="$emit('update:modelValue', $event.target.value)"
    />

    <button type="button" class="field-toggle" @click="show = !show">
      <svg v-if="!show" xmlns="http://www.w3.org/2000/svg" width="16" height="16"
        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
        <circle cx="12" cy="12" r="3"/>
      </svg>
      <svg v-else xmlns="http://www.w3.org/2000/svg" width="16" height="16"
        viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
        <line x1="1" y1="1" x2="23" y2="23"/>
      </svg>
    </button>
  </div>
</template>

<script setup>
import { ref } from 'vue'

defineProps({
  modelValue: { type: String, default: '' },
  placeholder: { type: String, default: '••••••••••••' }
})

defineEmits(['update:modelValue'])

const show = ref(false)
</script>

<style scoped>
.password-field {
  display: flex;
  align-items: stretch;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  background: #fff;
  overflow: hidden;
  transition: border-color 0.2s;
}

.password-field:focus-within {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.field-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 42px;
  min-width: 42px;
  color: #6b7280;
  border-right: 1px solid #e5e7eb;
  padding: 0;
}

.field-input {
  flex: 1;
  border: none;
  outline: none;
  padding: 0.625rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: #111827;
  background: transparent;
  min-width: 0;
}

.field-input::placeholder { color: #6b7280; }

.field-toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 42px;
  min-width: 42px;
  border: none;
  background: none;
  cursor: pointer;
  color: #6b7280;
  padding: 0;
}

.field-toggle:hover { color: #6b7280; }
</style>