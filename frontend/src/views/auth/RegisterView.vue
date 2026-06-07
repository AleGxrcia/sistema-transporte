<template>
  <div class="auth-page">
    <div class="auth-card">

      <div class="auth-logo">
        <img src="/logo.png" alt="FleetPro" class="logo-img" />
      </div>

      <h1 class="auth-title">Crear Cuenta</h1>

      <form @submit.prevent="handleRegister" class="auth-form">

        <div class="form-group">
          <label class="form-label">Nombre (s)</label>
          <div class="input-field">
            <span class="field-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                <circle cx="12" cy="7" r="4"/>
              </svg>
            </span>
            <input v-model="form.nombre" type="text"
              class="field-input" placeholder="ej: Juan Gabriel" required />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Apellido (s)</label>
          <div class="input-field">
            <span class="field-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                <circle cx="12" cy="7" r="4"/>
              </svg>
            </span>
            <input v-model="form.apellido" type="text"
              class="field-input" placeholder="ej: Pérez" required />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Correo Electronico</label>
          <div class="input-field">
            <span class="field-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/>
                <polyline points="22,6 12,12 2,6"/>
              </svg>
            </span>
            <input v-model="form.email" type="email"
              class="field-input" placeholder="ejemplo@aduanas.gob.do" required />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Contraseña</label>
          <PasswordInput v-model="form.password" />
        </div>

        <div class="form-group">
          <label class="form-label">Confirma contraseña</label>
          <PasswordInput v-model="form.confirmPassword" />
          <p v-if="passwordMismatch" class="error-msg">
            Las contraseñas no coinciden
          </p>
        </div>

        <p v-if="error" class="error-msg">{{ error }}</p>

        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Creando cuenta...' : 'Regístrame' }}
        </button>
      </form>

      <p class="auth-footer">
        ¿Ya tienes cuenta?
        <router-link to="/login">Ingresa aquí</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import PasswordInput from '@/components/forms/PasswordInput.vue'

const router = useRouter()

const form = reactive({
  nombre: '', apellido: '', email: '', password: '', confirmPassword: ''
})

const loading = ref(false)
const error = ref('')

const passwordMismatch = computed(() =>
  form.confirmPassword.length > 0 && form.password !== form.confirmPassword
)

async function handleRegister() {
  if (passwordMismatch.value) return
  loading.value = true
  error.value = ''
  try {
    await new Promise(r => setTimeout(r, 1000))
    router.push('/login')
  } catch {
    error.value = 'No se pudo crear la cuenta. Intenta de nuevo.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

* { font-family: 'Inter', sans-serif; }

.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f3f4f6;
  padding: 2rem 0;
}

.auth-card {
  background: #fff;
  border-radius: 16px;
  padding: 2rem;
  width: 100%;
  max-width: 380px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
}

.auth-logo {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.logo-img {
  height: 80px;
  width: auto;
  object-fit: contain;
  mix-blend-mode: multiply;
}

.auth-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #111827;
  text-align: center;
  margin-bottom: 1.25rem;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.form-label {
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
}

.input-field {
  display: flex;
  align-items: center;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  overflow: hidden;
  transition: border-color 0.2s;
}

.input-field:focus-within {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.field-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 42px;
  min-width: 42px;
  height: 42px;
  color: #9ca3af;
  border-right: 1px solid #e5e7eb;
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
}

.field-input::placeholder { color: #9ca3af; }

.error-msg {
  font-size: 0.78rem;
  color: #dc2626;
}

.btn-primary {
  width: 50%;
  margin: 0 auto;
  padding: 0.75rem;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
  margin-top: 0.25rem;
}

.btn-primary:hover:not(:disabled) { background: #1d4ed8; }
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }

.auth-footer {
  text-align: center;
  font-size: 0.82rem;
  color: #6b7280;
  margin-top: 1.1rem;
}

.auth-footer a {
  color: #2563eb;
  text-decoration: none;
  font-weight: 500;
}
</style>