<template>
  <div class="auth-page">
    <div class="auth-card">

      <div class="auth-logo">
        <img src="/logo.png" alt="FleetPro" class="logo-img" />
      </div>

      <h1 class="auth-title">Restablecer contraseña</h1>

      <form v-if="!invalidLink" @submit.prevent="handleSubmit" class="auth-form">

        <div class="form-group">
          <label class="form-label">Nueva contraseña</label>
          <PasswordInput v-model="newPassword" placeholder="Mín. 6 caracteres" />
        </div>

        <div class="form-group">
          <label class="form-label">Confirmar nueva contraseña</label>
          <PasswordInput v-model="confirmPassword" placeholder="Repite la nueva contraseña" />
          <p v-if="passwordMismatch" class="error-msg">Las contraseñas no coinciden</p>
        </div>

        <p v-if="reset" class="success-msg">✅ Contraseña actualizada. Ya puedes iniciar sesión.</p>
        <p v-if="error" class="error-msg">{{ error }}</p>

        <button v-if="!reset" type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Guardando...' : 'Restablecer' }}
        </button>

        <button v-else type="button" class="btn-primary" @click="router.push('/login')">
          Ir a iniciar sesión
        </button>
      </form>

      <p v-else class="error-msg">Enlace de restablecimiento inválido.</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store.js'
import PasswordInput from '@/components/forms/PasswordInput.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const email = route.query.email
const token = route.query.token
const invalidLink = !email || !token

const newPassword = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const reset = ref(false)
const error = ref('')

const passwordMismatch = computed(() =>
  confirmPassword.value.length > 0 && newPassword.value !== confirmPassword.value
)

async function handleSubmit() {
  if (passwordMismatch.value) return
  loading.value = true
  error.value = ''
  try {
    await authStore.resetPassword(email, token, newPassword.value)
    reset.value = true
  } catch {
    error.value = 'No se pudo restablecer la contraseña. El enlace puede haber expirado.'
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
  color: #111827;
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
  font-size: 0.82rem;
  font-weight: 500;
  color: #374151;
}

.success-msg {
  font-size: 0.875rem;
  color: #16a34a;
  text-align: center;
}

.error-msg {
  font-size: 0.78rem;
  color: #dc2626;
  text-align: center;
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
}

.btn-primary:hover:not(:disabled) { background: #1d4ed8; }
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
</style>
