<template>
  <div class="auth-page">
    <div class="auth-card">

      <div class="auth-logo">
        <img src="/logo.png" alt="FleetPro" class="logo-img" />
      </div>

      <h1 class="auth-title">Recuperar contraseña</h1>

      <form @submit.prevent="handleSubmit" class="auth-form">

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
              v-model="email"
              type="email"
              class="field-input"
              placeholder="ejemplo@aduanas.gob.do"
              required
            />
          </div>
        </div>

        <p v-if="enviado" class="success-msg">
          ✅ Revisa tu correo, te enviamos las instrucciones.
        </p>

        <p v-if="error" class="error-msg">{{ error }}</p>

        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Enviando...' : 'Enviar' }}
        </button>

        <button type="button" class="btn-link" @click="router.push('/login')">
          Volver atrás
        </button>

      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store.js'

const router = useRouter()
const authStore = useAuthStore()
const email = ref('')
const loading = ref(false)
const enviado = ref(false)
const error = ref('')

async function handleSubmit() {
  loading.value = true
  error.value = ''
  try {
    await authStore.forgotPassword(email.value)
    enviado.value = true
  } catch {
    error.value = 'No pudimos procesar tu solicitud. Intenta de nuevo.'
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
  padding: 2rem;
  width: 100%;
  max-width: 360px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.08);
}

.auth-logo {
  display: flex;
  justify-content: center;
  margin-bottom: 1.25rem;
}

.logo-img {
  height: 80px;
  width: auto;
  object-fit: contain;
  mix-blend-mode: multiply;
}

.auth-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text);
  text-align: center;
  margin-bottom: 1.5rem;
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
  font-size: 0.87rem;
  font-weight: 500;
  color: var(--text-2);
}

.input-field {
  display: flex;
  align-items: center;
  border: 1px solid var(--border-strong);
  border-radius: 8px;
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
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  color: var(--text);
  background: transparent;
}

.field-input::placeholder { color: var(--text-3); }

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
}

.btn-primary:hover:not(:disabled) { background: var(--blue-hover); }
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }

.btn-link {
  background: none;
  border: none;
  color: var(--blue-hover);
  font-size: 0.925rem;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
  text-align: center;
  text-decoration: underline;
}

.success-msg {
  font-size: 0.925rem;
  color: var(--mint-dark);
  text-align: center;
}

.error-msg {
  font-size: 0.83rem;
  color: var(--red);
  text-align: center;
}
</style>