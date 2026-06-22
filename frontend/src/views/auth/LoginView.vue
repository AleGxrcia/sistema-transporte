<template>
  <div class="auth-page">
    <div class="auth-card">

      <div class="auth-logo">
        <img src="/logo.png" alt="FleetPro" class="logo-img" />
      </div>

      <h1 class="auth-title">Bienvenido</h1>
      <p class="auth-subtitle">Inicia sesión con tus credenciales</p>

      <form @submit.prevent="handleLogin" class="auth-form">

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
            <input
              v-model="form.email"
              type="email"
              class="field-input"
              placeholder="ejemplo@aduanas.gob.do"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Contraseña</label>
          <PasswordInput v-model="form.password" />
          <div class="forgot-link">
            <router-link to="/forgot-password">Olvidé mi contraseña</router-link>
          </div>
        </div>

        <p v-if="error" class="error-msg">{{ error }}</p>

        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Cargando...' : 'Iniciar Sesión' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import PasswordInput from '@/components/forms/PasswordInput.vue'

const router = useRouter()
const { login } = useAuth()

const form = reactive({ email: '', password: '' })
const loading = ref(false)
const error = ref('')

async function handleLogin() {
  error.value = ''
  loading.value = true
  try {
    await login(form.email, form.password)
  } catch {
    error.value = 'Correo o contraseña incorrectos'
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
  background-color: var(--bg);
}

.auth-card {
  background: var(--white);
  border-radius: 16px;
  padding: 2rem 2rem 2rem;
  width: 100%;
  max-width: 380px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
}

.auth-logo {
  display: flex;
  justify-content: center;
  margin-bottom: 1.25rem;
}

.logo-img {
  height: 100px;
  width: auto;
  object-fit: contain;
  mix-blend-mode: multiply; /* elimina el fondo gris del logo */
}

.auth-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: var(--text);
  margin-bottom: 0.2rem;
}

.auth-subtitle {
  font-size: 0.82rem;
  color: var(--text-2);
  margin-bottom: 1.25rem;
}

.role-tabs {
  display: flex;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  overflow: hidden;
  margin-bottom: 1.25rem;
}

.role-tab {
  flex: 1;
  padding: 0.5rem 0.25rem;
  border: none;
  background: transparent;
  font-size: 0.82rem;
  font-family: 'Inter', sans-serif;
  color: var(--text-2);
  cursor: pointer;
  transition: all 0.2s;
}

.role-tab:not(:last-child) {
  border-right: 1px solid var(--border-strong);
}

.role-tab.active {
  background: #efecec;
  color: var(--blue-hover);
  font-weight: 600;
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
  color: var(--text-2);
}

.input-field {
  display: flex;
  align-items: center;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
  background: var(--white);
  overflow: hidden;
  transition: border-color 0.2s;
}

.input-field:focus-within {
  border-color: var(--blue-hover);
  box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
}

.field-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 42px;
  min-width: 42px;
  height: 42px;
  color: var(--text-3);
  border-right: 1px solid var(--border);
}

.field-input {
  flex: 1;
  border: none;
  outline: none;
  padding: 0.625rem 0.75rem;
  font-size: 0.875rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  background: transparent;
}

.field-input::placeholder { color: var(--text-3); }

.forgot-link {
  text-align: right;
}

.forgot-link a {
  font-size: 0.78rem;
  color: var(--blue-hover);
  text-decoration: none;
}

.error-msg {
  font-size: 0.78rem;
  color: var(--red);
  text-align: center;
}

.btn-primary {
  width: 50%;
  margin: 0 auto;
  padding: 0.75rem;
  background: var(--blue);
  color: var(--white);
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 600;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  transition: background 0.2s;
  margin-top: 0.25rem;
}

.btn-primary:hover:not(:disabled) { background: var(--blue-hover); }
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
</style>